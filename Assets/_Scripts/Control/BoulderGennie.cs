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

    public delegate void FindRocks();
    public static FindRocks findRocks;

    public GameObject nearestBoulder;
    public bool spawnBoulders;
    public GameObject player;
    public Vector3 newPos;
    
    public bool hasStoodUp;
    public bool firstCaveIn;
    public bool canCauseCaveIn;
    
    public int listSize = 3;    
    public List<Vector3> positions = new List<Vector3>();
    public List<GameObject> boulders = new List<GameObject>();

    AIController aiCon;
    Projectile boulderScript;

    void OnEnable()
    {
        findRocks += FindNearestBoulder;
    }

    void OnDisable()
    {
        findRocks -= FindNearestBoulder;
    }

    void Start()
    {
        aiCon = GetComponent<AIController>();
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
            if (firstCaveIn == false || canCauseCaveIn == true) { return; }            
            int i = boulders.Count;
            if (i <= 0)
            {
                positions.Clear();
                StartFlex();
            }
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

        yield return new WaitForSeconds(5f);
        foreach(Vector3 newPos in positions)
        {
            int height = Random.Range(15, 35);
            Instantiate(boulder, newPos + new Vector3(0, height, 0), boulderPoint.transform.rotation);
            //SoundFX
        }          
        aiCon.RocksOnGround(); //changes state
        canCauseCaveIn = false;
        BoulderGennie.findRocks.Invoke();
    }

    public void FindNearestBoulder()
    {
        boulders.Clear();
        boulders.AddRange(GameObject.FindGameObjectsWithTag("boulder"));         
        float distToClosestTarget = Mathf.Infinity;
        nearestBoulder = null;

        foreach (GameObject boulder in boulders)
        {
            if (boulder != null)
            {
                float distanceToTarget = (boulder.transform.position - this.transform.position).sqrMagnitude;
                if (distanceToTarget < distToClosestTarget)
                {
                    distToClosestTarget = distanceToTarget;
                    nearestBoulder = boulder;
                }
            }
        }
        aiCon.readyToThrow = false;
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
    }

    public void InstantiateRock()
    {
        aiCon.EmptyHand();
        var proj = Instantiate(boulderProj, rightHand.transform.position, Quaternion.identity);
        boulderScript = proj.GetComponent<Projectile>();
        SetTarget();
        FindNearestBoulder();
        UnarmedRocker();
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
        canCauseCaveIn = true;
        anim.SetTrigger("isFlexing");
        aiCon.NoRocksOnGround(); //ChangesState
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
            firstBoulder.DestroyThisObj();
        }
    }

    public void MoveToNearestRock()
    {  
        aiCon.RocksOnGround();
        aiCon.CaveInFalse();
        firstCaveIn = true;
        if (nearestBoulder != null)
        {
            StartCoroutine("MoveDelay");
        }
    }

    IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(1.9f);
        mover.StartMoveAction(nearestBoulder.transform.position, 5);
    }

    public void SpawnBouldersIsTrue()
    {        
        spawnBoulders = true;
        jumpLandFx.SetActive(true);
    }

    public void ActivateBoulderProj()
    {
        handBoulder.SetActive(true);
        aiCon.RockInHand();
    }

    public void UnarmedRocker()
    {
        aiCon.EmptyHand();
        handBoulder.SetActive(false);
    }
}
