using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMover : MonoBehaviour {

    private Vector3 initPos;
    bool ground = true;
    int mult = 1;

	// Use thisfor initialization
	void Start () {
        initPos = new Vector3(0, 0, -10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void StartMove(int dir){
        if (ground && dir < 0)
            return;
        ground = false;
        mult += dir;
        StartCoroutine(Move(dir));
    }

    public IEnumerator Move(int d){
        int i = 0;

        while(i<80){
            this.transform.position = new Vector3(initPos.x, this.transform.position.y + (.1f*d), initPos.z);
            i++;
            yield return new WaitForEndOfFrame();
        }
    }
}
