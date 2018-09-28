using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class levelSelectScript : MonoBehaviour {

    private int mult = 0;
    public Text[] levels;
    public Button[] buttons;
    public Text down, up;
    public Button d,u;
    const int LEVELSELECTOR = 99;
    const int CURRENT_LEVEL = -1;
    const int SETTINGS = -2;
    public Color unlocked, locked;

    private void Start()
    {
        //first time creating level key
        if(!PlayerPrefs.HasKey("levelUnlocked")){
            PlayerPrefs.SetInt("levelUnlocked", 1);
        }
        for (int i = 0; i < levels.Length; i++){
            if (PlayerPrefs.GetInt("levelUnlocked") <= i + mult * 16)
                levels[i].color = locked;
            else
                levels[i].color = unlocked;
        }

    }


    public void loadLevel(int levelName){
        if (levelName == LEVELSELECTOR)
        {
            SceneManager.LoadScene("LevelSelect");
            return;
        }
        if(levelName == CURRENT_LEVEL){
            SceneManager.LoadScene(PlayerPrefs.GetInt("levelUnlocked")+"");
            return;
        }
        if(levelName == SETTINGS){
            SceneManager.LoadScene("settings");
            return;
        }
        if (levelName == 0 && mult == 0)
        {
            SceneManager.LoadScene("menu");
            return;
        }
        if (mult != 0 && levelName == 0)
        {
           Move(-1);
            return;
        }
        if(PlayerPrefs.GetInt("levelUnlocked") < levelName+mult){
            Debug.Log("LEVEL NOT YET UNLOCKED");
        }else
        SceneManager.LoadScene(levelName+mult+"");
    }

    public void Move(int dir){
        mult += dir;
        StartCoroutine(Mover());
    }

    public IEnumerator Mover()
    {
        
        d.interactable = false;
        //deactivate buttons and remove text
        for (int k = 0; k < buttons.Length; k++){
            buttons[k].interactable = false;
            levels[k].text = "";
        }

        u.interactable = false;
        down.text = "";
        up.text = "";
        int i=0;
        //wait
        while (i < 80)
        {
            i++;
            yield return new WaitForEndOfFrame();
        }
        if(mult == 0){
            down.text = "MAIN   MENU";
        }else
            down.text = "                  v";
        for (int k = 0; k < buttons.Length; k++){
            Debug.Log(k);
            buttons[k].interactable = true;
            levels[k].text = 1+k + (mult * 16) + "";
            if (PlayerPrefs.GetInt("levelUnlocked") <= k + mult * 16)
                levels[k].color = locked;
            else
                levels[k].color = unlocked;
        }
        up.text = "^";

        d.interactable = true;
        u.interactable = (mult <= 4);

    }


}
