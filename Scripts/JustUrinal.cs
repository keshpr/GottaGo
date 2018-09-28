using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustUrinal : MonoBehaviour {

    // Use this for initialization
    public GameObject user;
    
    public Transform door;
    public GameObject[] prefabs;
    public Sprite highlightedSprite;
    public Sprite normalSprite;

    public bool hasBarriers;
    public float walkSpeed;
    public float timeSinceUseMin;
    public float timeSinceUseMax;
    public float timeWhileUseMin;
    public float timeWhileUseMax;
    public int slowDownFactor;
    public float dragLength;
    public float minMouseDragTime;

    GameObject player;
    PlayerController playerController;
    private GameObject gc;
    private GameController gameController;
    private bool isSwitching;
    private bool m_inUseByPlayer;
    private bool m_inUseByOther;
    bool isHighlighted;
    SpriteRenderer spriteRenderer;
    private float m_timeSinceLastUse;
    private float m_timeWhileInUse;

    private float m_nextTimeSinceLastUse;
    private float m_nextTimeWhileInUse;
    private bool hasTouchedUrinal;
    private bool hasStoppedTouch;
    private bool canDecreaseUrine;
    private Vector3 mouseMoveDir;
    private float dragTime;
    private bool hasStartedChecking;

    Camera cam;

    private void Start()
    {
        dragTime = 0;
        hasStartedChecking = false;
        hasTouchedUrinal = false;
        hasStoppedTouch = true;
        canDecreaseUrine = false;
        m_timeSinceLastUse = 0.0f;
        gc = GameObject.FindWithTag("GameController");
        gameController = gc.GetComponent<GameController>();
        
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        isHighlighted = false;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        m_inUseByPlayer = false;
        Physics.queriesHitTriggers = true;
        isSwitching = false;
        if (user == null)
        {
            m_inUseByOther = false;
            m_inUseByPlayer = false;
        }
        else
            m_inUseByOther = true;
        cam = Camera.main;
        StartCoroutine(incrementTime());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && inUseByPlayer() &&
            playerController.isPeeing &&/* !playerController.zipAudioEnabled() &&*/
            !playerController.gameOver() && Time.timeScale > 0)
        {
            //Debug.Log("Inside fired");
            if (!touchedPlayerUrinal())
            {
                //Debug.Log("Inside touched");
                //playerController.stopPeeing(0);
                //m_inUseByPlayer = false;
                hasTouchedUrinal = false;
                canDecreaseUrine = false;
                return;
            }
            else
            {
                Debug.Log("Inside decrease");
                hasTouchedUrinal = true;
                canDecreaseUrine = true;
                StartCoroutine(checkMouseDrag());
                //playerController.decreaseUrine();
            }
            //Debug.Log("Inside fired after");
        }
    }
    
    private bool touchedPlayerUrinal()
    {
        RaycastHit2D hit;
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            //Debug.Log("checking");
            hit = Physics2D.Raycast(new Vector2(cam.ScreenToWorldPoint(Input.mousePosition).x,
                cam.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero,0f);
            if (hit.collider != null && ((hit.collider.tag == "Urinal" && 
                hit.collider.gameObject.GetComponent<JustUrinal>().inUseByPlayer()) 
                || hit.collider.tag == "Player"))
                return true;
        }
        else {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero, 0f);
                if (hit.collider != null && (hit.collider.tag == "Urinal" || hit.collider.tag == "Player") &&
                    hit.collider.gameObject.GetComponent<JustUrinal>().inUseByPlayer())
                    return true;
            }
        }
        return false;
    }
    private void OnMouseDrag()
    {
        /*Debug.Log("Dragging");
        dragTime += Time.deltaTime;
        if (shouldCheckDrag())
        {
            checkDrag();
        }*/
    }
    public bool shouldCheckDrag()
    {
        return hasTouchedUrinal && !hasStartedChecking;
    }
    public void checkDrag()
    {
        StartCoroutine(checkMouseDrag());
        hasStartedChecking = true;
    }
    private void OnMouseUp()
    {
        /*hasStoppedTouch = true;
        if (canDecreaseUrine && dragTime < minMouseDragTime)
        {
            playerController.decreaseUrine();
        }*/
    }
    public float timeSinceMin()
    {
        return timeSinceUseMin;
    }
    public float timeWhileMin()
    {
        return timeWhileUseMin;
    }
    public void generateNextTimeSinceUse()
    {        
        m_nextTimeSinceLastUse = UnityEngine.Random.Range(timeSinceUseMin, timeSinceUseMax);
    }
    public float nextTimeSinceUse()
    {
        return m_nextTimeSinceLastUse;
    }
    public void generateNextTimeWhileInUse()
    {
        m_nextTimeWhileInUse = UnityEngine.Random.Range(timeWhileUseMin, timeWhileUseMax);
    }
    public float nextTimeWhileUse()
    {
        return m_nextTimeWhileInUse;
    }
    public int makeUserLook(bool flip)
    { 
        user.GetComponent<SpriteRenderer>().flipX = flip;
        user.GetComponent<lookScript>().Look();
        if (user.tag == "boss")
            return (1);
        else if (user.tag == "coworker")
            return (2);
        else if (user.tag == "homeless")
            return (3);
        else
            return 0;
    }
    public bool inUseByPlayer()
    {
        return m_inUseByPlayer;
    }
    public bool inUseByOther()
    {
        return m_inUseByOther;
    }
    public float timeSinceLastUse()
    {
        return m_timeSinceLastUse;
    }
    public float timeWhileInUse()
    {
        return m_timeWhileInUse;
    }
    public void makePlayerSlowDown()
    {
        playerController.getUrinalSlowDown(slowDownFactor);
    }
    public void MoveAway(int i)
    {
        if (!inUseByPlayer())
            return;
        if (i != 1)
            m_inUseByPlayer = false;
        playerController.stopPeeing(i);
    }
    public bool GenerateProbability()
    {
        if (!inUseByOther())
            return false;
        float num;
        if (hasBarriers)
        {
            num = Random.Range(0, 0.992f);
        }
        else
            num = Random.Range(0, 1f);

        return num > 0.99f;
    }
    public void getNew(int i)
    {
        
        isSwitching = true;
        Vector3 pos = transform.position - new Vector3(0, 1f, 0);

        //TODO:remove this check once boss walk animation is working
        if (i == 2)
            i = Random.Range(0, 2);

        GameObject nextUser = Instantiate(prefabs[i], door.position, Quaternion.identity);
        
        StartCoroutine(moveOther(nextUser, pos, false));
        user = nextUser;
    }
    public void makeUserLeave()
    {
        if (user != null)
        {
            user.GetComponent<lookScript>().destroyBubble();
            StartCoroutine(moveOther(user, door.position, true));            
        }
    }
    public bool ifSwitching()
    {
        return isSwitching;
    }
    private IEnumerator checkMouseDrag()
    {
        Debug.Log("started routine");
        Debug.Log(playerController.isDragging());
        dragTime = 0;
        Vector3 mousePos = Vector3.zero;
        mouseMoveDir = Vector3.zero;
        while (!Input.GetMouseButtonUp(0))
        {

            Debug.Log("Drag time:" + dragTime);
            dragTime += Time.deltaTime;
            mouseMoveDir += cam.ScreenToWorldPoint(Input.mousePosition) - mousePos;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            yield return null;
        }
        Debug.Log(mouseMoveDir.x + "  " + mouseMoveDir.y);
        if ( mouseMoveDir.magnitude > dragLength && Vector3.Dot(mouseMoveDir, Vector3.down) > 0)
        {
            playerController.stopPeeing(0);
            m_inUseByPlayer = false;
            dragTime = 0;
        }
        else if (dragTime < minMouseDragTime)
        {
            playerController.decreaseUrine();
        }

    }
    public IEnumerator moveOther(GameObject u, Vector2 endPos, bool leaving)
    {
        Vector2 inter = u.transform.position;
        Vector2 startPos = inter;
        float startTime = Time.time;
        float distTravelled;
        float dist = Vector3.Distance(u.transform.position, endPos);
        if (leaving)
        {
            u.GetComponent<lookScript>().destroyBubble();
            m_inUseByOther = false;
            user = null;
            
        }
        u.GetComponent<SpriteRenderer>().flipX = leaving;
        if(u.tag == "homeless" || u.tag == "coworker")
        u.GetComponent<Animator>().SetTrigger("isWalking");
        while (inter != endPos)
        {
            if (m_inUseByPlayer && !leaving)
            {
                leaving = true;
                u.GetComponent<SpriteRenderer>().flipX = leaving;
                inter = u.transform.position;
                endPos = door.transform.position;
                startPos = u.transform.position;
                startTime = Time.time;
                dist = Vector3.Distance(u.transform.position, endPos);
            }
            distTravelled = (Time.time - startTime) * walkSpeed;
            float fracTravelled = distTravelled / dist;
            inter = Vector2.Lerp(startPos, endPos, fracTravelled);
            u.transform.position = inter;
            yield return null;
        }
        if (leaving)
        {
            Destroy(u);
        }
        else {
            m_inUseByOther = true;
            if(u.tag == "homeless" || u.tag == "coworker")
            u.GetComponent<Animator>().SetTrigger("isIdle");
        }
        isSwitching = false;

    }
    private void OnMouseExit()
    {
        isHighlighted = false;
        spriteRenderer.sprite = normalSprite;
    }
    private void OnMouseOver()
    {
        //if (inUseByPlayer)
        //    return;
        if (!isHighlighted && !inUseByPlayer() && !inUseByOther())
        {
            isHighlighted = true;
            spriteRenderer.sprite = highlightedSprite;
        }
        else if (isHighlighted && inUseByOther())
        {
            isHighlighted = false;
            spriteRenderer.sprite = normalSprite;
        }
    }
    private void OnMouseDown()
    {
        /*if (inUseByPlayer() && playerController.isPeeing && !playerController.zipAudioEnabled() &&
            !playerController.gameOver() && Time.timeScale > 0)
        {
            playerController.stopPeeing(0);
            m_inUseByPlayer = false;
            return;
        }*/
        if (!playerController.gameOver() && !inUseByOther() && Time.timeScale > 0 && playerController.canMoveToUrinal
            && playerController.ifSelected())
        {
            playerController.moveToUrinal(this.transform.position);
            m_inUseByPlayer = true;
        }
    }
    IEnumerator incrementTime()
    {
        while (true)
        {
            if (!inUseByOther() && !inUseByPlayer())
            {
                m_timeSinceLastUse += Time.deltaTime;
                m_timeWhileInUse = 0.0f;
                //Debug.Log(timeSinceLastUse);
            }
            else
            {
                //Debug.Log("in use");
                m_timeSinceLastUse = 0.0f;
                m_timeWhileInUse += Time.deltaTime;
            }
            yield return null;
        }
    }
}
