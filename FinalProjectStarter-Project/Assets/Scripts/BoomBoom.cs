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
    Death,
    Pause,
    MAX=Pause

}

public enum eBoomBoomInitialDirection : byte
{
    Unknown,
    Random,
    Left,
    Right,
    MAX=Right
}

public class BoomBoom : Enemy
{
    public BoxCollider2D m_primaryCollider;
    public BoxCollider2D m_frontTrigger;
    public BoxCollider2D m_backTrigger;

    protected Animator m_animator;

    public eBoomBoomInitialDirection m_initialDirection = eBoomBoomInitialDirection.Unknown;
    private eBoomBoomState m_state = eBoomBoomState.Unknown;

    private float m_stunnedDuration = 0.0f; //using same timer for stunned and dormant
    private Vector2 m_velocity = Vector2.zero;
    private float m_directionChangeInterval = 0.0f;
    private Int32 m_lives = 0;
    private Int32 m_nearDeathLives = 0;
    private Vector3 m_awakenRange = Vector3.zero;
    private bool m_allowRandomDirection = true; //prevent the random direction from happening if the OnTriggerEnter2D is still clipping an object and not continue that direction.
    private float m_boomBoomJumpInterval = 0.0f;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_animator = GetComponent<Animator>();
        setBoxColliderDementions();
        m_velocity.x = EnemyConstants.c_boomBoomSpeed;
        m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;
        m_lives = EnemyConstants.c_boomBoomLives;
        m_directionChangeInterval= EnemyConstants.c_boomBoomDirectionChangeInterval;
        m_nearDeathLives = EnemyConstants.c_boomBoomNearDeathLives;
        m_awakenRange = EnemyConstants.c_boomBoomAwakenRange;
        SetState(eBoomBoomState.Dormant);
        m_boomBoomJumpInterval = EnemyConstants.c_boomBoomJumpInterval;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
        if (m_state == eBoomBoomState.Stunned)
        {
            m_stunnedDuration -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (m_stunnedDuration <= 0.0f)
            {
                m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;
                SetState(eBoomBoomState.Dormant);
            }
        }
        if (m_state == eBoomBoomState.Dormant)
        {
            if (checkAwakenRange())
            {
                m_awakenRange.x = 100.0f;//setting range to 100 vs creating a new bool
                m_stunnedDuration -= Time.deltaTime * Game.Instance.LocalTimeScale;
            }

            
            if (m_stunnedDuration <= 0.0f)
            {
                m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;

                SetState((m_lives == m_nearDeathLives) ? eBoomBoomState.NearDeath : eBoomBoomState.Walking);

            }
        }
        if (m_state == eBoomBoomState.Walking || m_state == eBoomBoomState.NearDeath)
        {
            m_directionChangeInterval -= Time.deltaTime * Game.Instance.LocalTimeScale;
            m_boomBoomJumpInterval -= Time.deltaTime * Game.Instance.LocalTimeScale;
            if (m_directionChangeInterval <= 0.0f)
            {
                m_directionChangeInterval = EnemyConstants.c_boomBoomDirectionChangeInterval;
                RandomizeDirection();
            }
            if (m_boomBoomJumpInterval <= 0.0f)
            {
                m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;
                m_boomBoomJumpInterval = EnemyConstants.c_boomBoomJumpInterval;
                SetState(eBoomBoomState.Pause);
            }
            Vector2 location = rigidbody.position;
            location += m_velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            rigidbody.position = location;
        }
        if (m_state == eBoomBoomState.Pause)
        {
            m_stunnedDuration -= Time.deltaTime * Game.Instance.LocalTimeScale;
            if (m_stunnedDuration <= 0.0f)
            {
                m_stunnedDuration = EnemyConstants.c_boomBoomStunnedDuration;

                SetState(eBoomBoomState.Jump);

            }
        }
        if (m_state == eBoomBoomState.Jump)
        {
            //rigidbody.AddForce(new Vector2(0.0f, 3.0f), ForceMode2D.Impulse);
            c_boomBoomJumpForce


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
            setBoxColliderDementions();
            UpdateAnimator();
            if (m_state == eBoomBoomState.Walking)
            {
                
                RandomizeDirection();
               
            }
            else if (m_state == eBoomBoomState.NearDeath)
            {
                RandomizeDirection();
                
            }
            else if (m_state == eBoomBoomState.Stunned)
            {
                
            }
            else if (m_state == eBoomBoomState.Dormant)
            {
                
            }
            else if (m_state == eBoomBoomState.Jump)
            {
                
            }
            else if (m_state == eBoomBoomState.Death)
            {
               
            }
            else if (m_state == eBoomBoomState.Pause)
            {
                m_velocity.x = 0.0f;
            }
            
        }  
    }
    public eBoomBoomState State
    {
        get { return m_state; }
    }

    private void ApplyInitialVelocity()
    {
        if (m_initialDirection == eBoomBoomInitialDirection.Random)
        {
            RandomizeDirection();
        }
        else if (m_initialDirection == eBoomBoomInitialDirection.Right)
        {
            m_velocity.x = GetCurrentSpeed();
        }
        else if (m_initialDirection == eBoomBoomInitialDirection.Left)
        {
            m_velocity.x = -GetCurrentSpeed();
        }
    }

    private void RandomizeDirection()
    {
        if (m_allowRandomDirection)
        {
            int index = UnityEngine.Random.Range(0, 10) % 2;
            float[] speeds = { GetCurrentSpeed(), -GetCurrentSpeed() };
            m_velocity.x = speeds[index];
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_state == eBoomBoomState.Jump)
        {
            if (collision.gameObject.CompareTag("World"))
            { 
            SetState(eBoomBoomState.Walking);
            }
        }
        if (collision.gameObject.CompareTag("Mario"))
        {
            // Get the Mario component from the GameObject
            Mario mario = collision.gameObject.GetComponent<Mario>();

            // Check if there's a contact object, in the contacts array
            if (collision.contacts.Length > 0)
            {
                // Get the normal from the first contact object
                Vector2 normal = collision.contacts[0].normal;
                if (m_state == eBoomBoomState.Dormant)
                {
                    mario.HandleDamage();
                }

                if (m_state == eBoomBoomState.Walking || m_state == eBoomBoomState.NearDeath)
                {
                   
                    if (normal.x <= -0.8f || normal.x >= 0.8f || normal.y >= 0.7f)
                    {
                        //collided with Mario on the side
                        mario.HandleDamage();
                    }
                    else if (normal.y <= -0.7f)
                    {
                        collision.gameObject.GetComponent<MarioMovement>().ApplyBounce();

                        m_lives--;

                        if (m_lives >= m_nearDeathLives)
                        {

                            SetState(eBoomBoomState.Stunned);
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("World")|| other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Pickup"))
        {
            m_allowRandomDirection = false;
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            List<Collider2D> results = new List<Collider2D>();
            other.OverlapCollider(filter, results);

            if (results.Contains(m_frontTrigger))
            {
                m_velocity.x = GetCurrentSpeed();
            }
            else if (results.Contains(m_backTrigger))
            {
                m_velocity.x = -GetCurrentSpeed();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("World") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Pickup"))
        {
            m_allowRandomDirection = true;
        }
    }
    float GetCurrentSpeed()
    {
        return (m_state == eBoomBoomState.NearDeath) ? EnemyConstants.c_boomBoomNearDeathSpeed : EnemyConstants.c_boomBoomSpeed;
    }
    void setBoxColliderDementions()
    {
        //primaryCollider
        //Walking collision dimentions size: x=1.7 y = 1.6
        //Walking collision dimentions offset: x=0 y=0.8
        //Dormant collision dimentions size: Vector2(1.9,1.4)
        //Dormant collision dimentions offset:Vector2(0,0.7)
        if (m_state == eBoomBoomState.Walking || m_state == eBoomBoomState.NearDeath)
        {
            m_primaryCollider.size = new Vector2(1.7f, 1.6f);
            m_primaryCollider.offset = new Vector2(0, 0.8f);
        }
        else 
        {
            m_primaryCollider.size = new Vector2(1.9f, 1.4f);
            m_primaryCollider.offset = new Vector2(0, 0.7f);
        }

        //frontTrigger
        //Walking collision dimentions size= Vector2(0.8,1.5)
        //Walking collision dimentions offset= Vector2(-0.5,0.8)
        //Dormant collision dimentions size= Vector2(0.8,1.3)
        //Dormant collision dimentions offset=Vector2(-0.6,0.7)
        if (m_state == eBoomBoomState.Walking || m_state == eBoomBoomState.NearDeath)
        {
            m_frontTrigger.size = new Vector2(0.8f, 1.2f);
            m_frontTrigger.offset = new Vector2(-0.5f, 0.8f);
        }
        else
        {
            m_frontTrigger.size = new Vector2(0.8f, 1.1f);
            m_frontTrigger.offset = new Vector2(-0.6f, 0.7f);
        }

        //backTrigger
        //Walking collision dimentions size= Vector2(0.8,1.5)
        //Walking collision dimentions offset= Vector2(0.5,0.8)
        //Dormant collision dimentions size= Vector2(0.8,1.3)
        //Dormant collision dimentions offset= Vector2(0.6,0.7)
        if (m_state == eBoomBoomState.Walking || m_state == eBoomBoomState.NearDeath)
        {
            m_backTrigger.size = new Vector2(0.8f, 1.2f);
            m_backTrigger.offset = new Vector2(0.5f, 0.8f);
        }
        else
        {
            m_backTrigger.size = new Vector2(0.8f, 1.1f);
            m_backTrigger.offset = new Vector2(0.6f, 0.7f);
        }
    }
    bool checkAwakenRange()
    {
        return m_awakenRange.x >= Vector3.Distance(Game.Instance.MarioGameObject.transform.position, transform.position); 
    }
    private void UpdateAnimator()
    {
        
        if (m_state == eBoomBoomState.Walking || m_state == eBoomBoomState.NearDeath)
        {
            animator.Play("BoomBoomWalk");
        }
        else if (m_state == eBoomBoomState.Dormant)
        {
            animator.Play("BoomBoomDormant");
        }
        else if (m_state == eBoomBoomState.Stunned)
        {
            animator.Play("BoomBoomStunned");
        }
        else if (m_state == eBoomBoomState.Death)
        {
            animator.Play("BoomBoomDeath");
        }
    }
}
