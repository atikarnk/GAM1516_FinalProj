using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//TODO clean up code

public enum eBooState : byte 
{
    Unknown,
    Chase,
    Idel,
    MAX = Idel

}
public class Boo : Enemy
{
    private eBooState m_state = eBooState.Unknown;

    public Sprite m_chanceSprite;
    public Sprite m_idelSprite;

    private Vector2 velocity = Vector2.zero;
    private Vector2 m_movementDirection = Vector2.zero;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Set the enemy type
        enemyType = EEnemyType.Boo;
        velocity = new Vector2(EnemyConstants.c_booSpeedx, EnemyConstants.c_booSpeedy);
        SetState(eBooState.Idel);
    }



    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;
        EMarioDirection direction = Game.Instance.MarioGameObject.GetComponent<MarioState>().Direction;

        if (direction == EMarioDirection.Right && marioLocation.x > transform.position.x)
        {
            SetState(eBooState.Chase);
        }
        if (direction == EMarioDirection.Right && marioLocation.x <= transform.position.x)
        {
            SetState(eBooState.Idel);
        }
        if (direction == EMarioDirection.Left && marioLocation.x >= transform.position.x)
        {
            SetState(eBooState.Idel);
        }
        if (direction == EMarioDirection.Left && marioLocation.x < transform.position.x)
        {
            SetState(eBooState.Chase);
        }

        if (m_state == eBooState.Chase)
        {
            Vector2 location = transform.position;
            location += m_movementDirection * velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            m_movementDirection = marioLocation - location;
            m_movementDirection.Normalize();
            transform.position = location;
        }
    }
    private void SetState(eBooState state)
    {
        m_state = state;
        if (m_state == eBooState.Idel)
        {
            spriteRenderer.sprite = m_idelSprite;
        }
        if (m_state == eBooState.Chase)
        {
            spriteRenderer.sprite = m_chanceSprite;
        }
    }
    public eBooState State
    {
        get { return m_state; }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Mario"))
        {
            // Get the Mario component from the GameObject
            Mario mario = collision.gameObject.GetComponent<Mario>();

            // Check if there's a contact object, in the contacts array
            if (collision.contacts.Length > 0)
            {
                // Get the normal from the first contact object
                Vector2 normal = collision.contacts[0].normal;
                mario.HandleDamage();
                   
                
            }
        }
    }

}
