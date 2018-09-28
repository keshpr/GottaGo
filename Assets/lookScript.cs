using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lookScript : MonoBehaviour {

    public float timeWhileLooking = 2;
    public float dialogYOffset;
    public float bubbleWaitTime = 2;

    public GameObject dialogBox;

    private string userTag;
    private string[] coworkerPhrases ={
        "Did you \nsee the game\n last week?",
        "Check out \nthis pic of\n my kids!"
    };
    private string[] bossPhrases ={
        "I've been \nthinking about \nsome layoffs",
        "How would \nyou feel about \na demotion?"
    };

    Animator anim;
    Camera cam;
    TextMesh dialog;
    GameObject box;
	// Use this for initialization
	void Start () {
        
        userTag = this.gameObject.tag;
        anim = this.gameObject.GetComponent<Animator>();
        cam = Camera.main;
        box = null;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Look(){
        if(userTag == "boss"){
            Vector3 pos = this.gameObject.GetComponentInChildren<Transform>().position + 
                new Vector3(0,dialogYOffset,-1);
            if (box != null)
                Destroy(box);
            box = Instantiate(dialogBox,pos,Quaternion.identity);
            int ind = Random.Range(0, 2);
            dialog = box.GetComponentInChildren<TextMesh>();
            dialog.text = bossPhrases[ind];
            StartCoroutine(breakBubble());
            //Vector3 pos2 = cam.ScreenToWorldPoint();
        }


        if(userTag == "homeless"){
            anim.SetTrigger("isLooking");
            StartCoroutine(wait(timeWhileLooking));
        }


        if(userTag == "coworker"){
            Vector3 pos = this.gameObject.GetComponentInChildren<Transform>().position +
                new Vector3(0, dialogYOffset, -1);
            box = Instantiate(dialogBox, pos, Quaternion.identity);
            int ind = Random.Range(0, 2);
            dialog = box.GetComponentInChildren<TextMesh>();
            dialog.text = coworkerPhrases[ind];
            StartCoroutine(breakBubble());
        }
        Debug.Log("LOOKING");
    }
    IEnumerator wait(float waitamt)
    {
        yield return new WaitForSeconds(waitamt);
        
        if (userTag == "homeless")
        {
            anim.SetTrigger("isIdle");
        }
        

    }
    IEnumerator breakBubble()
    {
        yield return new WaitForSeconds(bubbleWaitTime);
        Destroy(box);
    }
    public void destroyBubble()
    {
        if(box != null)
            Destroy(box);
    }
    private void OnDestroy()
    {
        Destroy(box);
    }
}
