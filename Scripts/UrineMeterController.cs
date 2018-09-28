using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrineMeterController : MonoBehaviour {

    public GameObject level;
    public GameObject waves;

    private GameObject levelObject;
    private GameObject waveObject;
    private bool done = false;
    public float urineLevel=10f;

    private void Start()
    {
        levelObject = Instantiate(level, this.transform.localPosition, Quaternion.identity);
        waveObject = Instantiate(waves, this.transform.localPosition, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        //max urine level
        if(urineLevel > 100){
            urineLevel = 100;
        }
        if (urineLevel <= 0)
        {
            done = true;
            Destroy(waveObject);
            Destroy(levelObject);
        }
        if(!done){
            waveObject.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.01f + urineLevel / 48.0f);
            levelObject.transform.localScale = new Vector3(levelObject.transform.localScale.x, urineLevel / 5f);
            levelObject.transform.localPosition = new Vector3(this.transform.localPosition.x - 0.05f, this.transform.localPosition.y - 0.48f + urineLevel / 90.0f);
        }
    }

    public void SetLevel(float level){
        urineLevel = level;
    }
}
