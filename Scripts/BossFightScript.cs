using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightScript : MonoBehaviour {

    public float killWidth = 3;
    public float bounceSpeed = 5;

    GameController gameController;
    GameObject player;
    PlayerController playerController;
    int numUrinals;
    float rightBound;
    float leftBound;
    int moveDirection;
	// Use this for initialization
	void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        numUrinals = gameController.urinals.Length;
        rightBound = gameController.urinals[numUrinals - 1].gameObject.transform.position.x;
        leftBound = gameController.urinals[0].gameObject.transform.position.x;
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        moveDirection = Random.Range(0,2);
        if (moveDirection == 0)
            moveDirection = -1;
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x <= leftBound)
            moveDirection = 1;
        else if (transform.position.x >= rightBound)
            moveDirection = -1;
        if(Time.timeScale > 0 && !gameController.GetGameOver())
            transform.position = new Vector3 (transform.position.x + moveDirection * bounceSpeed * Time.deltaTime, 
                transform.position.y);
        if (playerController.isPeeing && Mathf.Abs(player.transform.position.x - transform.position.x) <= killWidth
            && Time.timeScale > 0)
        {
            playerController.lostBossFight();
        }

	}
}
