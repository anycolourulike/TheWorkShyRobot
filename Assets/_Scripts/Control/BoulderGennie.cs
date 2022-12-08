using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Rambler.Movement;
using Rambler.Combat;
using Rambler.Control;
using System.Linq;

public class BoulderGennie : MonoBehaviour
{  
    [SerializeField] ParticleSystem boulderDust;
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject jumpLandFx;
    [SerializeField] GameObject boulderPoint;
    [SerializeField] GameObject boulder;
    [SerializeField] GameObject boulderProj;
    [SerializeField] GameObject handBoulder;
    [SerializeField] Mover mover;
    [SerializeField] Fighter fighter;
    [SerializeField] Animator anim;
    [SerializeField] float radius = 30;
    public GameObject player;

    public delegate void FindRocks();
    public static FindRocks findRocks;

    public GameObject nearestBoulder;
    public bool spawnBoulders;
    public Vector3 newPos;
    Projectile boulderScript;
    bool rocksOnGround;
    bool hasStoodUp;
    bool firstCaveIn;
    
    int listSize = 3;
    AIController aiCon;
    public List<Vector3> positions = new List<Vector3>();
    public List<GameObject> boulders = new List<GameObject>();

   void Start()
    {
        aiCon = GetComponent<AIController>();
    }

    void OnEnable()
    {
        findRocks += FindNearestBoulder;
    } 

    private void OnDisable()
    {
        findRocks -= FindNearestBoulder;        
    }

    private void Update()
    {
        if(spawnBoulders == true)
        {               
            for (int j = 0; j < listSize; j++)
            {                
                newPos = RandomNavmeshLocation(radius, this.transform);
                positions.Insert(0, newPos);                                
            }
            StartCoroutine(SpawnBoulders());
        }
        spawnBoulders = false;
        
        if (hasStoodUp == false)
        { 
            return; 
        } 
        else 
        {
            //if(firstCaveIn == false) { return; }
            //int i = boulders.Count;
            //if(i <= 0)
            //{
            //    StartFlex();
            //}
        }
    }

    IEnumerator SpawnBoulders()
    {        
        //Shake screen & phone
        foreach(Vector3 newPos in positions)
        {
            Instantiate(boulderPoint, newPos + new Vector3(0,15,0), boulderPoint.transform.rotation);
            //SoundFX
        }

        yield return new WaitForSeconds(1f);
        foreach(Vector3 newPos in positions)
        {            
            Instantiate(boulderDust, newPos + new Vector3(0, 15, 0), boulderPoint.transform.rotation);
            //SoundFX
        }

        yield return new WaitForSeconds(4f);
        foreach(Vector3 newPos in positions)
        {
            int height = Random.Range(15, 35);
            Instantiate(boulder, newPos + new Vector3(0, height, 0), boulderPoint.transform.rotation);
            //SoundFX
        }
        yield return new WaitForSeconds(3f);
        FindNearestBoulder();
        aiCon.IsRocks();
        aiCon.CaveInFalse();
        firstCaveIn = true;
        mover.StartMoveAction(nearestBoulder.transform.position, 5);
    }

    public void FindNearestBoulder()
    {       
        boulders.AddRange(GameObject.FindGameObjectsWithTag("boulder"));         
        float distToClosestTarget = Mathf.Infinity;
        nearestBoulder = null;

        foreach (GameObject boulder in boulders)
        {
            float distanceToTarget = (boulder.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToTarget < distToClosestTarget)
            {
                distToClosestTarget = distanceToTarget;
                nearestBoulder = boulder;
            }
        }
    }
        
    public Vector3 RandomNavmeshLocation(float radius, Transform thisTransform)
    {        
        Vector3 randomDirection = new Vector3(3,3,3) + Random.onUnitSphere.normalized * radius;
        randomDirection += thisTransform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }    

    public void ThrowRocks()
    {
        player = GameObject.Find("/PlayerCore/Rambler");
        anim.SetTrigger("meleeAttack");
        aiCon.EmptyHand();
    }


    public void SetTarget()
    {
        boulderScript.SetTarget(player.transform.position);
    }

    public void StandUp()
    {
        aiCon.StandingUp();
        aiCon.FacePlayer();
        aiCon.CaveInFalse();
        hasStoodUp = true;
    }

    public void StartFlex()
    {
        anim.SetTrigger("isFlexing");
        aiCon.NoRocksOnGround();
        FindNearestBoulder();
    }

   
    //Death
    //SFX
    //Intro
    //Enviroment

    public void RemoveFromBoulder(GameObject boulderToRemove)
    {
        boulders.Remove(boulderToRemove);
        positions.Remove(boulderToRemove.transform.position);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("boulder"))
        {
            mover.CancelNav();
            var nav = GetComponent<NavMeshAgent>();
            nav.velocity = Vector3.zero;
            anim.SetTrigger("isPickingUp");
        }
    }

    public void AttachBoulderToHand()
    {
        mover.CancelNav();
        var boulder = nearestBoulder;
        if (boulder != null)
        {
            var firstBoulder = boulder.GetComponentInChildren<Boulder>();
            ActivateBoulderProj();
            RemoveFromBoulder(boulder);
            firstBoulder.DestroyThisObj();
        }
    }

    public void SpawnBouldersIsTrue()
    {
        spawnBoulders = true;
        jumpLandFx.SetActive(true);
    }

    public void ActivateBoulderProj()
    {
        handBoulder.SetActive(true);
        aiCon.HasRock();
    }

    public void ThrowBoulder()
    {        
        var mover = GetComponent<Mover>();
        //mover.CancelNav();
        mover.RotateTowards(player.transform);
        var proj = Instantiate(boulderProj, this.transform);        
        var boulderScript = boulderProj.GetComponent<Projectile>();
        UnarmedRocker();
    }

    public void UnarmedRocker()
    {
        aiCon.EmptyHand();
        FindNearestBoulder();
        handBoulder.SetActive(false);
    }
}
