using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Animator animator;
    GameObject player;
    GameObject urinalInUse;
    GameObject urineMeter;
    GameObject gc;
    GameController gameController;
    public UrineMeterController urineMeterController;
    public Sprite normalSprite;
    public Sprite selectedSprite;
    //public AudioClip urine, flush, zip;

    public float initUrineLevel = 25;
    public float speed = 5;
    public float waitTime = 2;
    public float waitWhileSeen = 1;
    public float yOffset = 1f;
    public float zipTime = 0.3f;
    public int urineIncreaseFactor = 7;
    public int urineDecreaseFactor = 5;
    public int urineDecreaseFactorOnTap = 20;
    public float bossSpike = 20;
    Vector2 startLoc;
    AudioSource urineAudio,flushAudio,zipAudio;
    
    public bool isPeeing;
    public bool canMoveToUrinal;
    float urineLevel;
    bool gameIsOver;

    private int urinalSlowDown;
    private bool isSelected;
    private bool m_isDragging;
    public void getUrinalSlowDown(int s){
        urinalSlowDown = s;
    }
    
    SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {

        m_isDragging = false;
        isSelected = false;
        startLoc = this.transform.position;
        gc = GameObject.FindGameObjectWithTag("GameController");
        gameController = gc.GetComponent<GameController>();
        gameIsOver = false;

        isPeeing = false;
        //urineLevel = 10f;
        canMoveToUrinal = true;
        animator = this.GetComponent<Animator>();
        player = this.gameObject;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        urineMeter = GameObject.FindGameObjectWithTag("UrineMeter");
        urineMeterController = urineMeter.GetComponent<UrineMeterController>();
        urineLevel = initUrineLevel;

        urineAudio = GameObject.FindGameObjectWithTag("UrineAudio").GetComponent<AudioSource>();
        flushAudio = GameObject.FindGameObjectWithTag("FlushAudio").GetComponent<AudioSource>();
        zipAudio = GameObject.FindGameObjectWithTag("ZipAudio").GetComponent<AudioSource>();

        spriteRenderer.sprite = normalSprite;
    }
	
	// Update is called once per frame
	void Update () {

        if (urineLevel >= 100f)
        {
            gameIsOver = true;
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
            //player peeing
            urineLevel -= Time.deltaTime * (urineDecreaseFactor -urinalSlowDown);
            urineMeterController.SetLevel(urineLevel);
        }
        if (urineLevel <= 0 && !gameOver())
        {
            urineAudio.Stop();
            gameController.Win();
            gameIsOver = true;
        }
        
	}
    public bool ifSelected()
    {
        return isSelected;
    }
    public void selectPlayer()
    {
        //Debug.Log("Inside select");
        isSelected = true;
        spriteRenderer.sprite = selectedSprite;
    }
    public void deselectPlayer()
    {
        isSelected = false;
        spriteRenderer.sprite = normalSprite;
    }
    private void OnMouseDown()
    {
        //Debug.Log("Mouse Down");
        if(canMoveToUrinal)
        {
            //Debug.Log("Inside");
            GetComponent<Animator>().enabled = false;
            if (ifSelected())
                deselectPlayer();
            else
                selectPlayer();
        }
    }
    private void OnMouseDrag()
    {
        m_isDragging = true;
        //Debug.Log(m_isDragging);
    }
    private void OnMouseUp()
    {        
        m_isDragging = false;
        //Debug.Log(m_isDragging);
    }
    public bool isDragging()
    {
        return m_isDragging;
    }
    public void decreaseUrine()
    {
        urineLevel -= Time.deltaTime * urineDecreaseFactorOnTap;
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
    public void stopPeeing(int byPlayer)
    {
        Vector3 endV3 = startLoc;
        bool isLeft = (Vector2.Dot(endV3 - player.transform.position, Vector2.right) <= 0);
        spriteRenderer.flipX = !isLeft;        
        StartCoroutine(moveAwayFromUrinal(byPlayer));
    }
    public void lostBossFight()
    {
        wetHimself();
    }
    private void wetHimself()
    {
        Physics.queriesHitTriggers = false;
        animator.SetTrigger("isWetting");
        StartCoroutine(waitForWetting());
    }
    IEnumerator movePlayerToUrinal(Vector2 endPos)
    {
        deselectPlayer();
        GetComponent<Animator>().enabled = true;
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
        if(!gameIsOver)
            StartCoroutine(playZipAndUrine());
    }
    IEnumerator playZipAndUrine()
    {
        zipAudio.time = 1.5f;
        zipAudio.time = 0f;
        if(gameController.ShouldPlaySFX())
            zipAudio.Play();
        yield return new WaitForSeconds(zipTime);
        zipAudio.Stop();
        urineAudio.loop = true;
        if (isPeeing && !gameController.GetGameOver())
        {
            urineAudio.time = 0f;
            if (gameController.ShouldPlaySFX())
                urineAudio.Play();
        }

    }
    IEnumerator moveAwayFromUrinal(int byPlayer)
    {
        urinalSlowDown = 0;
        Vector2 inter = player.transform.position;
        Vector2 endPos = startLoc;
        Vector2 startPos = inter;
        float startTime = Time.time;
        float distTravelled;
        float dist = Vector3.Distance(player.transform.position, endPos);


        if (byPlayer != 0)
        {
            if (byPlayer == 1)
            {
                urineLevel += bossSpike;
                yield break;
            }
            else if (byPlayer == 2)
            {
                isPeeing = false;
                urineAudio.Stop();
                StartCoroutine(zipUp());
            }
            else if (byPlayer == 3)
            {
                urineLevel += 20;
                isPeeing = false;
                urineAudio.Stop();
                StartCoroutine(zipUp());
            }
        }
        else {
            isPeeing = false;
            urineAudio.Stop();
            StartCoroutine(zipUp());
        }
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
    IEnumerator zipUp()
    {
        zipAudio.time = 0f;
        if(gameController.ShouldPlaySFX())
            zipAudio.Play();
        yield return new WaitForSeconds(zipTime);
        zipAudio.Stop();
    }
    IEnumerator waitForWetting()
    {
        gameController.Lose();

        yield return new WaitForSeconds(waitTime);
        Debug.Log("Game Over");
    }
    
    public bool zipAudioEnabled()
    {
        return zipAudio.isPlaying;
    }
    public bool gameOver()
    {
        return gameIsOver;
    }
}
