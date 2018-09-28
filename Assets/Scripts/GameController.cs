using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    

    public Text winText, loseText;
    public Text inst1, inst2;
    public Image winSplash;
    public Button menu, retry, next, resume;
    float score;
    public int initScore = 10000;
    public float scoreFactor = 200f;
    bool lowerScore = true;
    bool gameOver = false;
    public JustUrinal[] urinals;

    //float timeWhileUse;
    //float timeSinceUse;
    AudioSource urine, flush, zip, bgMusic;
    private bool playMusic, playSFX;
    private bool uplaying, fplaying, zplaying;
    private int urinalNumPlayer;
    PlayerController playerController;
	// Use this for initialization
	void Start () {
        //DEACTIVATE UI ON LOAD
        if(winSplash != null)
        winSplash.enabled = false;
        winText.text = "";
        loseText.text = "";
        menu.gameObject.SetActive(false);
        retry.gameObject.SetActive(false);
        next.gameObject.SetActive(false);
        resume.gameObject.SetActive(false);
        //END UI

        //GET REFERENCE TO AUDIO SOURCES
        urine = GameObject.FindGameObjectWithTag("UrineAudio").GetComponent<AudioSource>();
        flush = GameObject.FindGameObjectWithTag("FlushAudio").GetComponent<AudioSource>();
        zip = GameObject.FindGameObjectWithTag("ZipAudio").GetComponent<AudioSource>();
        bgMusic = GameObject.FindGameObjectWithTag("backgroundMusic").GetComponent<AudioSource>();
        //END AUDIO SOURCES

        //SETUP MUTE OR NOT. ONLY CALLED ONCE FOR EFFICIENCY
        playMusic = PlayerPrefs.GetInt("music") == 1;
        playSFX = PlayerPrefs.GetInt("sfx") == 1;
        //END MUTE

        score = initScore;
        //urinalControllers = new JustUrinal[urinals.Length];
        for (int i = 0; i < urinals.Length; i++)
        {
            //urinalControllers[i] = urinals[i].GetComponent<JustUrinal>();
            urinals[i].generateNextTimeSinceUse();
            urinals[i].generateNextTimeWhileInUse();
        }
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        urinalNumPlayer = -1;

        //start background music;
       // Debug.Log("Play music:"+playMusic);
        if (playMusic)
            bgMusic.Play();
        //timeWhileUse = UnityEngine.Random.Range(timeWhileUseMin, timeWhileUseMax);
        //timeSinceUse = UnityEngine.Random.Range(timeSinceUseMin, timeSinceUseMax);
    }
    public bool GetGameOver(){
        return gameOver;
    }

    public bool ShouldPlaySFX(){
        return playSFX;
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
                if (i != 0 && i != urinals.Length - 1)
                    urinals[i].makePlayerSlowDown();
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
        if (!gameOver && urinalNumPlayer >= 0 && urinalNumPlayer < urinals.Length - 1&& 
            urinals[urinalNumPlayer + 1].inUseByOther() &&
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
            
            if (j.timeWhileMin() >= 0 && (j.timeWhileInUse() >= j.nextTimeWhileUse()) && Time.timeScale > 0)
            {
                //Debug.Log("Inside");
                j.makeUserLeave();
                j.generateNextTimeWhileInUse();
            }
            else if (j.timeSinceMin() >= 0 && j.timeSinceLastUse() >= j.nextTimeSinceUse() && 
                Time.timeScale > 0 && !j.ifSwitching())
            {
                j.getNew(UnityEngine.Random.Range(0,3));
                j.generateNextTimeSinceUse();
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
        if(playMusic)
            flush.Play();
        gameOver = true;
        lowerScore = false;
        int fScore = (int)score;
        winText.text = "You win!\nYour score was " + fScore;
        if (winSplash != null)
         winSplash.enabled = true;
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
        if (int.Parse(sceneNum) >= 11)
        {
            SceneManager.LoadScene("menu"); //TODO: remove this check once more levels are built
            return;
        }
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
        zip.Pause();
        flush.Pause();
        urine.Pause();
        if (winSplash != null)
            winSplash.enabled = true;
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
       
        Time.timeScale = Time.timeScale < 0.1f ? 1f : 0f;
        if(Time.timeScale <=0.1f){
            zip.Pause();
            urine.Pause();
            flush.Pause();
            bgMusic.Pause();
        }else{
            zip.UnPause();
            urine.UnPause();
            flush.UnPause();
            bgMusic.UnPause();
        }

        menu.gameObject.SetActive(!menu.gameObject.activeSelf);
        retry.gameObject.SetActive(!retry.gameObject.activeSelf);
        resume.gameObject.SetActive(!resume.gameObject.activeSelf);
    }

}
