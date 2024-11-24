using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimation : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("ChangeAnimation");
    }

    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}
