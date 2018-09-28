using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Animator animator;
    GameObject player;
    GameObject urinalInUse;
    GameObject urineMeter;
    public UrineMeterController urineMeterController;

    public float speed = 5;
    public float waitTime = 2;
    public float yOffset = 1f;
    public int urineIncreaseFactor = 5;
    public int urineDecreaseFactor = 10;
    Vector2 startLoc;

    public bool isPeeing;
    bool canMoveToUrinal;
    float urineLevel;
    
    SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {

        startLoc = this.transform.position;

        isPeeing = false;
        urineLevel = 0f;
        canMoveToUrinal = true;
        animator = this.GetComponent<Animator>();
        player = this.gameObject;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        urineMeter = GameObject.FindGameObjectWithTag("UrineMeter");
        urineMeterController = urineMeter.GetComponent<UrineMeterController>();
        urineLevel = 10f;
    }
	
	// Update is called once per frame
	void Update () {

        if (urineLevel >= 100f)
        {
            wetHimself();
        }
        if (!isPeeing)
        {
            urineLevel += Time.deltaTime * urineIncreaseFactor;
            urineMeterController.SetLevel(urineLevel);
            //Debug.Log(urineLevel);
        }
        else
        {
            urineLevel -= Time.deltaTime * urineDecreaseFactor;
            urineMeterController.SetLevel(urineLevel);
        }
	}
    public void moveToUrinal(Vector2 endPos)
    {
        if (!canMoveToUrinal)
            return;
        Vector3 endV3 = endPos;
        bool isLeft = (Vector2.Dot(endV3 - player.transform.position, Vector2.right) <= 0);
        animator.SetTrigger("isWalking");
        spriteRenderer.flipX = !isLeft;        
        canMoveToUrinal = false;
        endPos += new Vector2(0,-yOffset);
        StartCoroutine(movePlayerToUrinal(endPos));
    }
    public void stopPeeing(bool byPlayer)
    {
        Vector3 endV3 = startLoc;
        bool isLeft = (Vector2.Dot(endV3 - player.transform.position, Vector2.right) <= 0);
        spriteRenderer.flipX = !isLeft;        
        StartCoroutine(moveAwayFromUrinal(byPlayer));
    }
    private void wetHimself()
    {
        Physics.queriesHitTriggers = false;
        animator.SetTrigger("isWetting");
        StartCoroutine(waitForWetting());
    }
    IEnumerator movePlayerToUrinal(Vector2 endPos)
    {
        Vector2 inter = player.transform.position;
        Vector2 startPos = inter;
        float startTime = Time.time;
        float distTravelled;
        float dist = Vector3.Distance(player.transform.position,endPos);

        while (inter != endPos)
        {
            if (urineLevel >= 100f)
            {
                wetHimself();
                break;
            }
            distTravelled = (Time.time - startTime) * speed;
            float fracTravelled = distTravelled / dist;
            inter = Vector2.Lerp(startPos,endPos,fracTravelled);
            player.transform.position = inter;
            yield return null;
        }
        isPeeing = true;
        animator.SetTrigger("isPeeing");        
    }
    IEnumerator moveAwayFromUrinal(bool byPlayer)
    {
        Vector2 inter = player.transform.position;
        Vector2 endPos = startLoc;
        Vector2 startPos = inter;
        float startTime = Time.time;
        float distTravelled;
        float dist = Vector3.Distance(player.transform.position, endPos);
        
        

        isPeeing = false;
        animator.SetTrigger("isWalking");
        while (inter != endPos)
        {
            if (urineLevel >= 100f)
            {
                wetHimself();
                break;
            }
            distTravelled = (Time.time - startTime) * speed;
            float fracTravelled = distTravelled / dist;
            inter = Vector2.Lerp(startPos, endPos, fracTravelled);
            player.transform.position = inter;
            yield return null;
        }
        animator.SetTrigger("isIdle");
        canMoveToUrinal = true;
    }
    IEnumerator waitForWetting()
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Game Over");
    }
    private void OnMouseDown()
    {
        if (isPeeing)
        {
            isPeeing = false;

        }
    }
}
