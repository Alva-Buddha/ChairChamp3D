using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public static AudioManager instance;

    Animator animator; // Reference to the Animator component

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            animator.SetBool("IsMoving", true);
        }
        if (Input.GetKey("a"))
        {
            animator.SetBool("IsSitting", true);
        }
        if (Input.GetKey("s"))
        {
            animator.SetBool("IsStunned", true);
        }
        if (Input.GetKey("d"))
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsStunned", false);
        }
    }
}
