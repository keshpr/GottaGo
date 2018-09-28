using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScript : MonoBehaviour {


    public float timePeeing = 7;
    public GameObject bossPrefab;
    public GameObject homelessPrefab;
    public GameObject coworkerPrefab;

    float t;
    GameController gameController;
    JustUrinal ju;
	// Use this for initialization
	void Start () {
        t = 0;
        //gameController = GameObject.FindGameObjectWithTag("GameObject").GetComponent<GameController>();
        ju = this.gameObject.GetComponent<JustUrinal>();
	}
	
	// Update is called once per frame
	void Update () {
        if(GetComponent<JustUrinal>().inUseByOther())
            t += Time.deltaTime;
        if (t >= timePeeing)
        {
            Debug.Log(t);
            t = 0;
            //int i = Random.Range(0, 3);
            ju.makeUserLeave();
            
        }
	}
}
