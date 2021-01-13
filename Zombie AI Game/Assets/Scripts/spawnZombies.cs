using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnZombies : MonoBehaviour
{
    public int numberOfZombies = 1;

    private GameObject[] allWaypoints;
    private GameObject zombiePrefab;
    // Start is called before the first frame update
    void Start()
    {
        allWaypoints = GameObject.FindGameObjectsWithTag("waypoint");
        zombiePrefab = GameObject.FindGameObjectWithTag("zombie");
        zombiePrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //using a counter of the amount of zombies we have at the moment. We want to avoid having to do GameObject.find too many times
        int nrZombiesExist = GameObject.FindGameObjectsWithTag("zombie").Length;
        while (nrZombiesExist < numberOfZombies)
        {

            //instatiates a copy of zombiePrefab. I places it at the position of one of the waypoints (randomly picks an index in the waypoint array). It gives it a new rotation and adds the object that this script is attached to as parent
            Instantiate(zombiePrefab, allWaypoints[Random.Range(0, allWaypoints.Length - 1)].transform.position, new Quaternion(), transform).SetActive(true);
            //count them manually instead of calling FindGameObjectsWithTag every time
            nrZombiesExist++;
        }
        //Spawn Zombie test key
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(zombiePrefab, new Vector3(1.9f, -15.8f, -7.5f), new Quaternion(), transform).SetActive(true);
        }
    }
}
