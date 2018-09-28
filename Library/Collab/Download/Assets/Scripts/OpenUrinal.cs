using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUrinal : MonoBehaviour {

    public GameObject left;
    public GameObject right;
    public Sprite highlightedSprite;
    public Sprite normalSprite;
    public GameObject user;

    public bool inUseByPlayer;
    public bool hasDividers;
    public int bonus;

    GameObject player;
    PlayerController playerController;
    Camera cam;
    GameObject gOb;
    bool isHighlighted;
    SpriteRenderer spriteRenderer;    

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        cam = Camera.main;
        Physics.queriesHitTriggers = true;
        gOb = this.gameObject;
        isHighlighted = false;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        inUseByPlayer = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && inUseByPlayer)
        {
            playerController.stopPeeing(true);
            inUseByPlayer = false;
        } 
	}
    
    private void OnMouseDown()
    {
       // if (inUseByOther || inUseByPlayer)
          //  return;
        
        playerController.moveToUrinal(this.transform.position);
        inUseByPlayer = true;
    }
    private void OnMouseOver()
    {
      //  if (inUseByOther)
        //    return;
        if (!isHighlighted && !inUseByPlayer)
        {
            isHighlighted = true;
            spriteRenderer.sprite = highlightedSprite;
        }
        else if (isHighlighted && inUseByPlayer)
        {
            isHighlighted = false;
            spriteRenderer.sprite = normalSprite;
        }
    }
    private void OnMouseExit()
    {
        isHighlighted = false;
        spriteRenderer.sprite = normalSprite;
    }
    //
    public void MoveAway(){
        inUseByPlayer = false;
        playerController.stopPeeing(false);
    }



}
