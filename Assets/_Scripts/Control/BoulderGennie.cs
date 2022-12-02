using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoulderGennie : MonoBehaviour
{  
    [SerializeField] ParticleSystem boulderDust;
    [SerializeField] GameObject jumpLandFx;
    [SerializeField] GameObject boulderPoint;
    [SerializeField] GameObject boulder;
    [SerializeField] float radius = 30;

    public GameObject nearestBoulder;
    public bool spawnBoulders;
    public Vector3 newPos;
    
    int listSize = 3;
    public List<Vector3> positions = new List<Vector3>();
    public List<GameObject> boulders = new List<GameObject>();

    private void Update()
    {
        if(spawnBoulders == true)
        {           
            for (int j = 0; j < listSize; j++)
            {                
                newPos = RandomNavmeshLocation(radius);
                positions.Insert(0, newPos);                                
            }
            StartCoroutine(SpawnBoulders());
        }
        spawnBoulders = false;
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
        boulders.AddRange(collection: GameObject.FindGameObjectsWithTag("Boulder"));
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

    //Returns Ramdom Pos on Nav Mesh
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public void SpawnBouldersIsTrue()
    {
        spawnBoulders = true;
        jumpLandFx.SetActive(true);
    }
}
