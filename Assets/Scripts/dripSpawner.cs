using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dripSpawner : MonoBehaviour {

    public GameObject drip;
    public Vector2 pos;

	// Use this for initialization
	void Start () {
        StartCoroutine(Drip());
	}
	
    public IEnumerator Drip(){
        while (true)
        {
            Instantiate(drip, pos, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
    }
}
