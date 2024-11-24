using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnemyType : byte
{
    Unknown,
    PiranhaPlant,
    Goomba,
    FirePiranha

        //TODO: Add additional EnemyType enumerators here
}

public class Enemy : MonoBehaviour
{
    protected new Rigidbody2D rigidbody;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected EEnemyType enemyType = EEnemyType.Unknown;

    public EEnemyType EnemyType 
    { 
        get { return enemyType; } 
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rigidbody = GetComponentInParent<Rigidbody2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        animator = GetComponentInParent<Animator>();
    }
}
