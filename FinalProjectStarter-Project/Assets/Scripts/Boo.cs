using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum eBooState : byte 
{
    Unknown,
    Chase,
    Idel,
    MAX

}
public class Boo : Enemy
{
    private eBooState m_state = eBooState.Unknown;

    public Sprite m_chanceSprite;
    public Sprite m_idelSprite;

    private Vector2 velocity = Vector2.zero;
    private Vector2 m_movementDirection = Vector2.zero;
    private bool m_isActive = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Set the enemy type
        enemyType = EEnemyType.Boo;
        velocity = new Vector2(EnemyConstants.BooSpeedx, EnemyConstants.BooSpeedy);
        SetState(eBooState.Idel);
    }

    public bool IsActive
    {
        get { return m_isActive; }
        set { m_isActive = value; }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_isActive)
        {
            if (m_state != eBooState.Unknown)
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
                    if (transform.position.x > marioLocation.x)
                    {
                        Vector3 scale = transform.localScale;
                        scale.x = 1.0f;
                        transform.localScale = scale;
                    }
                    else
                    {
                        Vector3 scale = transform.localScale;
                        scale.x = -1.0f;
                        transform.localScale = scale;
                    }
                    Vector2 location = transform.position;
                    location += m_movementDirection * velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
                    m_movementDirection = marioLocation - location;
                    m_movementDirection.Normalize();
                    transform.position = location;
                }
            }
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
        // Is the Goomba colliding with the Mario GameObject?
        if (collision.gameObject.CompareTag("Mario"))
        {
            // Get the Mario component from the GameObject
            Mario mario = collision.gameObject.GetComponent<Mario>();

            // Check if there's a contact object, in the contacts array
            if (collision.contacts.Length > 0)
            {
                // Get the normal from the first contact object
                Vector2 normal = collision.contacts[0].normal;

                // Ensure the Goomba's state is walking
                //if (m_state == eBooState.Chase)
                {
                   
                    mario.HandleDamage();
                   
                }
            }
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("BooBoundary"))
    //    {
    //        SetState(eBooState.Unknown);
 
    //    }
    //}
}
