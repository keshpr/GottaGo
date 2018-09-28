using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    

    public Text winText, loseText;
    public Text inst1, inst2;
    public Button menu, retry, next;
    float score;
    public int initScore = 10000;
    public float scoreFactor = 200f;
    bool lowerScore = true;
    bool gameOver = false;
    public JustUrinal[] urinals;
    public float timeSinceUse;
    public float timeWhileUse;
    public float epsilon;
    AudioSource urine, flush, zip;

    private bool uplaying, fplaying, zplaying;
    private int urinalNumPlayer;
    PlayerController playerController;
	// Use this for initialization
	void Start () {
        //DEACTIVATE UI ON LOAD
        winText.text = "";
        loseText.text = "";
        menu.gameObject.SetActive(false);
        retry.gameObject.SetActive(false);
        next.gameObject.SetActive(false);
        //END UI
        //GET REFERENCE TO AUDIO SOURCES
        urine = GameObject.FindGameObjectWithTag("UrineAudio").GetComponent<AudioSource>();
        flush = GameObject.FindGameObjectWithTag("FlushAudio").GetComponent<AudioSource>();
        zip = GameObject.FindGameObjectWithTag("ZipAudio").GetComponent<AudioSource>();
        //END AUDIO SOURCES
        score = initScore;
        //urinalControllers = new JustUrinal[urinals.Length];
        /*for (int i = 0; i < urinals.Length; i++)
        {
            //urinalControllers[i] = urinals[i].GetComponent<JustUrinal>();
            urinals[i].setUrinalNumber(i);
        }*/
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        urinalNumPlayer = -1;
	}
    public bool GetGameOver(){
        return gameOver;
    }
	
	// Update is called once per frame
	void Update () {
        if (lowerScore)
        {
            score -= (scoreFactor * Time.deltaTime);
            if (score < 0)
                score = 0;
        }
        for (int i = 0; i < urinals.Length; i++)
        {
            if (urinals[i].inUseByPlayer())
            {
                urinalNumPlayer = i;
                //Debug.Log("User using " + i);
            }
        }
        if (!gameOver && urinalNumPlayer > 0 && urinals[urinalNumPlayer - 1].inUseByOther() && 
            playerController.isPeeing && Time.timeScale > 0)
        {
            //Debug.Log("User using right");
            if (urinals[urinalNumPlayer - 1].GenerateProbability())
            {
                int otherNum = urinals[urinalNumPlayer - 1].makeUserLook(false);
                urinals[urinalNumPlayer].MoveAway(otherNum);
            }
        }
        if (!gameOver && urinalNumPlayer < 4 && urinals[urinalNumPlayer + 1].inUseByOther() &&
            playerController.isPeeing && Time.timeScale > 0)
        {
            //Debug.Log("User using left");
            if (urinals[urinalNumPlayer + 1].GenerateProbability())
            {
                int otherNum = urinals[urinalNumPlayer + 1].makeUserLook(true);
                urinals[urinalNumPlayer].MoveAway(otherNum);
            }
        }
        for (int i = 0; i < urinals.Length; i++)
        {
           JustUrinal j = urinals[i].GetComponent<JustUrinal>();
            if ((j.timeSinceLastUse() >= timeSinceUse) && Time.timeScale > 0)
            {
                //Debug.Log("Inside");
                j.makeUserLeave();
            }
            else if (j.timeWhileInUse() >= timeWhileUse && Time.timeScale > 0)
            {
                j.getNew(UnityEngine.Random.Range(0,3));
            }

        }
	}
    public JustUrinal getUrinal(int i)
    {
        if (i < 0 || i >= urinals.Length)
            return null;
        return urinals[i];
    }
    public void Win(){
        flush.Play();
        gameOver = true;
        lowerScore = false;
        int fScore = (int)score;
        winText.text = "You win!\nYour score was " + fScore;
        if (inst1 != null)
        {
            inst1.text = "";
        }
        if(inst2 != null){
            inst2.text = "";
        }
        next.gameObject.SetActive(true);
        menu.gameObject.SetActive(true);
        StartCoroutine(W());
    }
    public IEnumerator W(){
        string sceneNum = SceneManager.GetActiveScene().name;
        string nextScene = int.Parse(sceneNum) + 1 + "";
        PlayerPrefs.SetInt("levelUnlocked", int.Parse(nextScene));
        yield return new WaitForSeconds(8);
        if (int.Parse(nextScene) > 8)
            SceneManager.LoadScene("menu");
        else
            SceneManager.LoadScene(nextScene);

    }

    public void NextLevel(){
        string sceneNum = SceneManager.GetActiveScene().name;
        string nextScene = int.Parse(sceneNum) + 1 + "";
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
    }
    public void MainMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("menu");
    }
    public void RetryLevel(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void Lose(){
        gameOver = true;
        loseText.text = "You wet your pants!";
        if (inst1 != null)
        {
            inst1.text = "";
        }
        if (inst2 != null)
        {
            inst2.text = "";
        }
        menu.gameObject.SetActive(true);
        retry.gameObject.SetActive(true);
        StartCoroutine(L());
    }

    public IEnumerator L(){
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene("menu");
    }

    public void Pause(){
        if (gameOver)
            return;
        //play
        Debug.Log("uplaying:" + uplaying);
        if (uplaying)
        {
            urine.Play();
            uplaying = false;
        }
        if (fplaying)
        {
            flush.Play();
            fplaying = false;
        }
        if (zplaying)
        {
            zip.Play();
            zplaying = false;
        }       

        //pause
        if (urine.isPlaying)
        {
            uplaying = true;
            urine.Pause();
        }
        if (zip.isPlaying)
        {
            zplaying = true;
            zip.Pause();
        }
        if (flush.isPlaying){
            fplaying = true;
            flush.Pause();
        }
        Time.timeScale = Time.timeScale < 0.1f ? 1f : 0f;
        menu.gameObject.SetActive(!menu.gameObject.activeSelf);
        retry.gameObject.SetActive(!retry.gameObject.activeSelf);
    }

}
