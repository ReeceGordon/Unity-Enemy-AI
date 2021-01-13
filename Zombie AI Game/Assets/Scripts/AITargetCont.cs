using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class AITargetCont : MonoBehaviour
{
    private enum AIState { WANDERING, CHASING, DEAD, FLEE };

    private AICharacterControl aicharacterController;
    private GameObject[] allWaypoints;
    private GameObject[] fleePoints;

    Vector3 deathSpot;
    List<Vector3> deathSpots;
    Vector3 averageDeathLocation;
    int deathSpotsIter = 0;

    private int currentWaypoint = 0;
    private int currentFleepoint = 0;
    private ThirdPersonCharacter tpCharacter;
    private AIState state = AIState.WANDERING;
    private float deathTimeout = 5.0f;
    private int decoyEffectiveness = 5;
    public int zombieHealth = 2;
    Animator playerAnimator;
    Vector3 playerPosition;
    Vector3 decoyPosition;

    GameObject cube;


    protected bool CanSeePlayer()
    {
        //function to determine whether the AI character can see the player

        playerPosition = GameObject.FindGameObjectWithTag("player").transform.position; // get player position

        //we only want to look ahead so we check if the player in a 90 degree arc. (Vec3.angle returns an absolute value, so 45 degrees either way means <=45)
        if (Vector3.Angle(transform.forward, playerPosition - transform.position) <= 45f){
            LayerMask layerMask = LayerMask.NameToLayer("zombie");// a mask used for the raycast to ignore any zombies

            RaycastHit hit;// variable to store the hit so we can check it
            //We now check if a ray cast from us (the zombie) to the player hits anything (except zombies) also move it up a little to avoid the floor
            if (Physics.Raycast(transform.position + new Vector3(0f, 0.5f, 0f), playerPosition - transform.position, out hit, Mathf.Infinity, layerMask)){
                //Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), playerPosition - transform.position, Color.red);
                return (hit.collider.tag.Equals("player"));//return whether or not the thing we hit is the player
            }
        }//or if any of these tests failed, i can't see them.
        return false;
    }
    public void kill()
    {
        zombieHealth -= 1;
        if (playerAnimator.GetBool("isBoxing"))
        {
            this.GetComponentInChildren<Renderer>().material.color = Color.red;
        }

        if (zombieHealth <= 0)
        {
            this.state = AIState.DEAD;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        aicharacterController = GetComponent<AICharacterControl>();
        tpCharacter = GetComponent<ThirdPersonCharacter>();
        playerAnimator = GameObject.FindGameObjectWithTag("player").GetComponent<Animator>();
        deathSpots = new List<Vector3>();

        allWaypoints = GameObject.FindGameObjectsWithTag("waypoint");
        fleePoints = GameObject.FindGameObjectsWithTag("fleepoint");

        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
        allWaypoints = allWaypoints.OrderBy(x => rnd.Next()).ToArray();
        fleePoints = fleePoints.OrderBy(x => rnd.Next()).ToArray();
    }

    private void OnDestroy()
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = deathSpot;
        cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        cube.AddComponent<NavMeshObstacle>();
        cube.GetComponent<NavMeshObstacle>().carving = true;
        cube.GetComponent<BoxCollider>().enabled = false;
        cube.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnApplicationQuit()
    {
        Destroy(cube);
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = GameObject.FindGameObjectWithTag("player").transform.position;
        if (GameObject.FindGameObjectWithTag("decoy") != null)
        {
            decoyPosition = GameObject.FindGameObjectWithTag("decoy").transform.position;
        }
        if (Input.GetKeyDown(KeyCode.T)){
            decoyEffectiveness -= 1;
        }

        if (playerAnimator.GetBool("isBoxing") == false && zombieHealth > 1)
        {
            this.GetComponentInChildren<Renderer>().material.color = Color.white;
        }
        if(zombieHealth <= 1)
        {
            this.GetComponentInChildren<Renderer>().material.color = Color.blue;
        }

        switch (state)
        {
            case AIState.WANDERING:
                aicharacterController.SetTarget(allWaypoints[currentWaypoint].transform);
                Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), allWaypoints[currentWaypoint].transform.position - transform.position, Color.red);
                // Debug.Log("Current Waypoint: " + currentWaypoint);
                if ((Vector3.Distance(aicharacterController.target.transform.position, transform.position) < 2.0f))
                {
                    //...make me target the next one
                    currentWaypoint++;
                    //make sure that we don't fall off the end of the array but lop back round
                    currentWaypoint %= allWaypoints.Length;
                }
                //can I see the player? if so, the chase is on!
                if (CanSeePlayer() && GameObject.FindGameObjectWithTag("decoy") == null)
                {
                    state = AIState.CHASING;
                }
                if (zombieHealth == 1)
                {
                    state = AIState.FLEE;
                }
                break;
            case AIState.CHASING:
                if (GameObject.FindGameObjectWithTag("decoy") != null)
                {
                    if(decoyEffectiveness <= 0)
                    {
                        //If decoy is used 5 times, it will become inaffective.
                        aicharacterController.SetTarget(GameObject.FindGameObjectWithTag("player").transform);
                        Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), playerPosition - transform.position, Color.red);
                    }
                    else
                    {
                        aicharacterController.SetTarget(GameObject.FindGameObjectWithTag("decoy").transform);
                        Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), decoyPosition - transform.position, Color.red);
                    }
                }
                else
                {
                    aicharacterController.SetTarget(GameObject.FindGameObjectWithTag("player").transform);
                    Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), playerPosition - transform.position, Color.red);
                }
                if (!CanSeePlayer())
                {
                    //i can't see him, so back to wandering...
                    if (GameObject.FindGameObjectWithTag("decoy") == null)
                    {
                        state = AIState.WANDERING;
                    }
                }
                if(zombieHealth == 1)
                {
                    state = AIState.FLEE;
                }
                break;
            case AIState.DEAD:
                this.GetComponent<Animator>().enabled = false; //stop trying to animate myself
                this.GetComponent<NavMeshAgent>().enabled = false; //stop trying to navigate
                this.GetComponent<AICharacterControl>().enabled = false; // stop the AI

                this.GetComponent<Rigidbody>().isKinematic = true; //Make myself kinematic (ragdoll)

                foreach (Rigidbody rbody in GetComponentsInChildren<Rigidbody>())
                {
                    rbody.isKinematic = false;
                }

                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    col.enabled = true;
                }
                deathSpot = transform.position;
                Destroy(gameObject, deathTimeout); // remove myself from the game after timeout
                break;
            case AIState.FLEE:
                aicharacterController.SetTarget(fleePoints[currentFleepoint].transform);
                Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), fleePoints[currentFleepoint].transform.position - transform.position, Color.red);
                // Debug.Log("Current Waypoint: " + currentWaypoint);
                if ((Vector3.Distance(aicharacterController.target.transform.position, transform.position) < 2.0f))
                {
                    //...make me target the next one
                    currentFleepoint++;
                    //make sure that we don't fall off the end of the array but lop back round
                    currentFleepoint %= fleePoints.Length;
                }
                break;
        }
        Debug.Log("DeathSPots: " + deathSpots.Count);
    }
}

//Red task is simply an informative piece.