using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoulderGennie : MonoBehaviour
{
    int count = 2;
    
    public bool spawnBoulders = false;
    [SerializeField] ParticleSystem boulderDust;
    [SerializeField] GameObject boulderPoint;
    [SerializeField] float radius = 5;
    public List<Vector3> positions = new List<Vector3>();

    private void Update()
    {
        if(spawnBoulders == true)
        {            
            for (int j = 0; j < positions.Count; j++)
            {
                var newPos = RandomNavmeshLocation(radius);
                positions.Add(newPos);
                SpawnBoulders();
            }
        }
    }

    void SpawnBoulders()
    {
        //loop through positions & spawn dust / boulder 
        //Clear positions list
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
}
