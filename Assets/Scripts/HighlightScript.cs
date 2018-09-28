using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightScript : MonoBehaviour {

    GameObject gOb;
    bool isHighlighted;
    SpriteRenderer spriteRenderer;

    public Sprite highlightedSprite;
    public Sprite normalSprite;
	// Use this for initialization
	void Start () {
        Physics.queriesHitTriggers = true;
        gOb = this.gameObject;
        isHighlighted = false;
        spriteRenderer = this.GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnMouseOver()
    {
        if (!isHighlighted && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            isHighlighted = true;
            spriteRenderer.sprite = highlightedSprite;
        }
    }
    private void OnMouseExit()
    {
        isHighlighted = false;
        spriteRenderer.sprite = normalSprite;
    }
}
