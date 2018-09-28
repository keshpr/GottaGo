using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

    /*  bool sfx;
      bool music;*/
    public Toggle music;
    public Toggle sfx;

    // Use this for initialization
    void Start () {
        if (!PlayerPrefs.HasKey("sfx"))
            PlayerPrefs.SetInt("sfx", 1);
        if (!PlayerPrefs.HasKey("music"))
            PlayerPrefs.SetInt("music", 1);
        StartCoroutine(PrintValues());
        music.isOn = PlayerPrefs.GetInt("music") == 1;
        sfx.isOn = PlayerPrefs.GetInt("sfx") == 1;
    }
    
    // Update is called once per frame
    void Update () {
        
    }
    IEnumerator PrintValues(){
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log(PlayerPrefs.GetInt("music"));
        }
    }

    public void ResetLevelsUnlocked(){
        PlayerPrefs.SetInt("levelUnlocked", 1);
    }

    public void MainMenu(){
        SceneManager.LoadScene("menu");
    }

    public void toggleSFX(bool on){
        if (on)
            PlayerPrefs.SetInt("sfx", 1);
        else
            PlayerPrefs.SetInt("sfx", 0);
    }
    public void toggleMusic(bool on)
    {
        if (on)
            PlayerPrefs.SetInt("music", 1);
        else
            PlayerPrefs.SetInt("music", 0);
    }

    public void unlock(){ //TODO: delete this later
        PlayerPrefs.SetInt("levelUnlocked", 10);
    }


}