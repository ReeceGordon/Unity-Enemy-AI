using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitEnemy : MonoBehaviour
{
    private Animator animator;
    private hitEnemy enemyScript;
    // Start is called before the first frame update

    public bool isBoxing = false;

    void OnTriggerEnter(Collider other)
    {
        if (isBoxing && other.tag == "zombie")
        {
            other.GetComponent<AITargetCont>().kill();
        }
    }

    void Start()
    {
       // animator = GetComponent<Animator>();
        animator = GetComponentInParent<Animator>();
        enemyScript = GetComponentInChildren<hitEnemy>();
        if (enemyScript != null)
        {
            enemyScript.isBoxing = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool boxing = Input.GetKey(KeyCode.B);
        animator.SetBool("isBoxing", boxing);
        if (enemyScript != null)
        {
            enemyScript.isBoxing = boxing;
        }
    }
}
