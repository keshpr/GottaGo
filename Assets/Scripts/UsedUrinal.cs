using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsedUrinal : MonoBehaviour {

    public GameObject user;
    public GameObject openLeft;
    public GameObject openRight;
    public Transform door;
    public GameObject[] prefabs;

    public bool hasBarriers;
    public float walkSpeed;

    private OpenUrinal left, right;
    GameObject player;
    PlayerController playerController;
    private GameObject gc;
    private GameController gameController;
    private bool isSwitching;
    


    private void Start()
    {
        isSwitching = false;
        gc = GameObject.FindWithTag("GameController");
        gameController = gc.GetComponent<GameController>();
        if(openLeft != null)
            left = openLeft.GetComponent<OpenUrinal>();
        if (openRight != null)
            right = openRight.GetComponent<OpenUrinal>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!gameController.GetGameOver() && !isSwitching)
        {
            if (left != null)
            {
                if (left.inUseByPlayer && playerController.isPeeing)
                {
                    if (GenerateProbability())
                    {
                        user.GetComponent<SpriteRenderer>().flipX = true;
                        user.GetComponent<lookScript>().Look();
                        if (user.tag == "boss")
                            left.MoveAway(1);
                        else if (user.tag == "coworker")
                            left.MoveAway(2);
                        else if (user.tag == "homeless")
                            left.MoveAway(3);
                    }
                }
            }
            if (right != null)
            {
                if (right.inUseByPlayer && playerController.isPeeing)
                {
                    if (GenerateProbability())
                    {
                        user.GetComponent<lookScript>().Look();
                        if (user.tag == "boss")
                            right.MoveAway(1);
                        else if (user.tag == "coworker")
                            right.MoveAway(2);
                        else if (user.tag == "homeless")
                            right.MoveAway(3);
                    }
                }
            }
        }
    }

    private bool GenerateProbability(){
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
        GameObject nextUser = Instantiate(prefabs[i], door.position, Quaternion.identity);
        StartCoroutine(leave(user, door.position, true));
        StartCoroutine(leave(nextUser,pos,false));
        user = nextUser;
    }
    IEnumerator leave(GameObject u, Vector2 endPos, bool destr)
    {
        Vector2 inter = u.transform.position;
        Vector2 startPos = inter;
        float startTime = Time.time;
        float distTravelled;
        float dist = Vector3.Distance(u.transform.position, endPos);
        if(destr)
            u.GetComponent<lookScript>().destroyBubble();

        while (inter != endPos)
        {
            
            distTravelled = (Time.time - startTime) * walkSpeed;
            float fracTravelled = distTravelled / dist;
            inter = Vector2.Lerp(startPos, endPos, fracTravelled);
            u.transform.position = inter;
            yield return null;
        }
        if (destr){            
            Destroy(u);
        }
        isSwitching = false;
        
    }

}
