/*
 
 Boom Boom is the boss fight of the fortress. Boom Boom starts it a dormant state, when Mario approaches them, they awaken. 
Now awake, Boom Boom moves erratically side to side at a consistent speed, changing direction at a moments notice. 
Boom Boom can even jump, though they don't do that very ofter. Boom Boom's jump is preceded by a brief pause, then they will jump in either direction. 
If Mario lands on Boom Boom, then they are stunned. After a brief period, Boom Boom starts moving again. After Mario lands on him twice, Boom Boom will move even faster. 
If Mario lands on Boom Boom a third time, then he is dead, play the dead animation and after a short amount of time he 'explodes'. 
The poof animation is played along with the eight stars animating outward in a circle. The question mark circle is also spawned with an upward impulse. 
If side or bottom of Boom Boom collides with Mario, while Boom Boom is in its moving state (or jumping) then Mario is damaged.
 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBoomBoomState : byte
{
    Unknown,
    Walking,
    Stunned,
    Dormant,
    Jump,
    NearDeath,
    NearDeathStunned,
    Death,
    Pause,
    MAX=Pause

}

public enum eBoomBoomInitialDirection : byte
{
    Random,
    Left,
    Right,
    MAX=Right
}

public class BoomBoom : Enemy
{
    public BoxCollider2D primaryCollider;
    public BoxCollider2D frontTrigger;
    public BoxCollider2D backTrigger;

    private Int32 m_lives = 0;

    public Sprite m_dormantSprite;
    public Sprite m_walkingSprite;
    public Sprite m_stunnedSprite;
    public Sprite m_nearDeathStunned;
    public Sprite m_deathSprite;


    public EGoombaInitialDirection m_initialDirection = EGoombaInitialDirection.Random;
    private eBoomBoomState m_state = eBoomBoomState.Unknown;

    private float m_stunnedDuration = 0.0f;
    private Vector2 m_velocity = Vector2.zero;
    private float m_directionChangeInterval = 0.0f;
    private bool m_isInvincible = true;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_velocity.x = EnemyConstants.c_boomBoomSpeed;
        m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;
        SetState(eBoomBoomState.Dormant);
        m_lives = EnemyConstants.c_boomBoomLives;
        m_directionChangeInterval= EnemyConstants.c_boomBoomDirectionChangeInterval;
        
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }
    private void FixedUpdate()
    {
        if (m_state == eBoomBoomState.Walking)
        {
            m_directionChangeInterval -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (m_directionChangeInterval <= 0.0f)
            {
                m_directionChangeInterval = EnemyConstants.c_boomBoomDirectionChangeInterval;
                RandomizeDirection();
            }
            Vector2 location = rigidbody.position;
            location += m_velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            rigidbody.position = location;
        }
        if (m_state == eBoomBoomState.Dormant)
        {

            m_stunnedDuration -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (m_stunnedDuration <= 0.0f)
            {
                m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;
                SetState(eBoomBoomState.Walking);
            }
        }
        if (m_state == eBoomBoomState.Stunned|| m_state == eBoomBoomState.NearDeathStunned)
        {
            m_stunnedDuration -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (m_stunnedDuration <= 0.0f)
            {
                m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;
                SetState(eBoomBoomState.Dormant);
            }
        }
        if (m_state == eBoomBoomState.Death)
        {
            m_stunnedDuration -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (m_stunnedDuration <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }
    private void SetState(eBoomBoomState state)
    {
        if (m_state != state)
        {
            m_state = state;
            if (m_state == eBoomBoomState.Walking)
            {
                m_isInvincible = false;
                spriteRenderer.sprite = m_walkingSprite;
            }
            if (m_state == eBoomBoomState.Stunned)
            {
                m_isInvincible = true;
                spriteRenderer.sprite = m_stunnedSprite;
            }
            if (m_state == eBoomBoomState.Dormant)
            {
                m_isInvincible = true;
                spriteRenderer.sprite = m_dormantSprite;
            }
            if (m_state == eBoomBoomState.Jump)
            {
                m_isInvincible = false;
                spriteRenderer.sprite = m_walkingSprite;
            }
            if (m_state == eBoomBoomState.NearDeath)
            {
                m_isInvincible = false;
                m_velocity.x = EnemyConstants.c_boomBoomNearDeathSpeed;
                spriteRenderer.sprite = m_walkingSprite;
            }
            if (m_state == eBoomBoomState.Death)
            {
                m_isInvincible = true;
                spriteRenderer.sprite = m_deathSprite;
            }
            if (m_state == eBoomBoomState.Pause)
            {
                m_isInvincible = false;
                spriteRenderer.sprite = m_walkingSprite;
            }
            if (m_state == eBoomBoomState.NearDeathStunned)
            {
                m_isInvincible = true;
                spriteRenderer.sprite = m_nearDeathStunned;
            }
        }
        
    }
    public eBoomBoomState State
    {
        get { return m_state; }
    }

    private void ApplyInitialVelocity()
    {
        if (m_initialDirection == EGoombaInitialDirection.Random)
        {
            RandomizeDirection();
        }
        else if (m_initialDirection == EGoombaInitialDirection.Right)
        {
            m_velocity.x = EnemyConstants.c_boomBoomSpeed;
        }
        else if (m_initialDirection == EGoombaInitialDirection.Left)
        {
            m_velocity.x = -EnemyConstants.c_boomBoomSpeed;
        }
    }

    private void RandomizeDirection()
    {
        //TODO check near death
        int index = UnityEngine.Random.Range(0, 10) % 2;
        float[] speeds = { EnemyConstants.c_boomBoomSpeed, -EnemyConstants.c_boomBoomSpeed };
        m_velocity.x = speeds[index];
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
                if (m_state == eBoomBoomState.Walking)
                {
                    if (m_state == eBoomBoomState.Dormant)
                    {
                        mario.HandleDamage();
                    }
                    if (normal.x <= -0.8f || normal.x >= 0.8f)
                    {
                        // Goomba collided with Mario on the side
                        mario.HandleDamage();
                    }
                    else if (normal.y >= 0.7f)
                    {
                        // Goomba landed on Mario
                        mario.HandleDamage();
                    }
                    else if (normal.y <= -0.7f)
                    {
                        
                        if (!m_isInvincible) 
                        {
                            m_lives--;
                            // Mario landed on the Goomba, make Mario bounce off the enemy
                            MarioMovement marioMovement = collision.gameObject.GetComponent<MarioMovement>();
                            marioMovement.ApplyBounce();
                            if (m_lives > 1)
                            {

                                SetState(eBoomBoomState.Stunned);
                            }
                            else if (m_lives == 1)
                            {

                                SetState(eBoomBoomState.NearDeathStunned);
                            }
                            else
                            {
                                SetState(eBoomBoomState.Death);
                            }
                        }
                        
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        bool flipMovementDirection = false;

        if (other.gameObject.CompareTag("World"))
        {
            // The Goomba collided with the World
            flipMovementDirection = true;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            // The Goomba collided with another Enemy
            EEnemyType enemyType = other.gameObject.GetComponent<Enemy>().EnemyType;

            flipMovementDirection = true;
            
        }
        else if (other.gameObject.CompareTag("Pickup"))
        {
            flipMovementDirection = true;
        }

        // DO we need to flip the Goomba's direction
        if (flipMovementDirection)
        {
            // Get the Goomba's overlapping collider
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            List<Collider2D> results = new List<Collider2D>();
            other.OverlapCollider(filter, results);

            // Set the Goomba's velocity based on which collider was triggered 
            if (results.Contains(frontTrigger))
            {
                if (m_lives <= 1)
                {
                    m_velocity.x = -EnemyConstants.c_boomBoomNearDeathSpeed;
                }
                else
                {
                    m_velocity.x = -EnemyConstants.c_boomBoomSpeed;
                }

            }
            else if (results.Contains(backTrigger))
            {
                if (m_lives <= 1)
                {
                    m_velocity.x = EnemyConstants.c_boomBoomNearDeathSpeed;
                }
                else
                {
                    m_velocity.x = EnemyConstants.c_boomBoomSpeed;
                }
            }
        }
    }
}
