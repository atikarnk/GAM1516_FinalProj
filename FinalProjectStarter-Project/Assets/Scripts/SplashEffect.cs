using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEffect : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("SplashEffect");
    }

    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}

