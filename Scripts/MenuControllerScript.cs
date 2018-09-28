using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControllerScript : MonoBehaviour {
    
    public AudioSource music;

    void Start()
    {
        Debug.Log("the result of the music settings is "+PlayerPrefs.GetInt("music"));
        if (PlayerPrefs.GetInt("music") == 1)
            music.Play();
        else
            music.Stop();
    }

    public void loadScene(){
        SceneManager.LoadScene(PlayerPrefs.GetInt("levelUnlocked")+"");
    }
    public void loadLevelSelect(){
        SceneManager.LoadScene("levelSelect");
    }
    public void loadSettings(){
        SceneManager.LoadScene("settings");
    }
	
}
