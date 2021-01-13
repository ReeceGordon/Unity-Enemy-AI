using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decoy : MonoBehaviour
{

    Vector3 playerDirection;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = GameObject.Find("decoyPos").transform.position;
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.gameObject.tag = "decoy";
            cube.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            cube.AddComponent<Rigidbody>();
            cube.GetComponent<Rigidbody>().mass = 100;
            cube.transform.position = playerDirection;
            Destroy(cube, 5);
        }
        
    }
}
