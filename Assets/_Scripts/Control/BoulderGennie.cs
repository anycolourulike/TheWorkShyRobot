using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Rambler.Movement;
using Rambler.Combat;
using Rambler.Control;

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
    public Vector3 newPos;
    
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
        if(nearestBoulder != null && this.gameObject.tag == "Enemy")
        {            
            mover.StartMoveAction(nearestBoulder.transform.position, 5);
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
        var player = transform.Find("/PlayerCore/Rambler");
        var mover = GetComponent<Mover>();
        mover.RotateTowards(player);
        anim.SetTrigger("meleeAttack");        
    }    

    //boulderGennie.FindNearestBoulder();
    //throw boulder at player
    //repeat until no boulders left
    //jump attack
    //Death
    //SFX
    //Intro
    //Enviroment

    public void RemoveFromBoulder(GameObject boulderToRemove)
    {
        boulders.Remove(boulderToRemove);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("boulder"))
        {
            mover.CancelNav();
            anim.SetTrigger("isPickingUp");
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
        var proj = Instantiate(boulderProj, this.transform);        
        var boulderScript = boulderProj.GetComponent<Boulder>();
        //Create boulder projectile script
        UnarmedRocker();
    }

    public void UnarmedRocker()
    {
        FindNearestBoulder();
        handBoulder.SetActive(false);
        aiCon.NoRocks();
    }
}
