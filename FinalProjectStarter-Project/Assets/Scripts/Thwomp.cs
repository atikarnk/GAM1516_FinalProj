using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EThwompState : byte
{
    Unknown,
    Idle,
    Active,
    AnimUp
}
public class Thwomp : Enemy
{
    private new Rigidbody2D rigidbody;
    private EThwompState state = EThwompState.Unknown;

    private Vector2 idleLocation = Vector2.zero;
    private Vector2 activeLocation = Vector2.zero;
    private float animationTimer = 0.0f;
    private bool isOnGround = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        enemyType = EEnemyType.Thwomp;

        idleLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EThwompState.Idle)
        {
            Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;
            if (marioLocation.x - idleLocation.x <= 1.0f && marioLocation.x - idleLocation.x >= -1.0f)
            {
                SetState(EThwompState.Active);
            }
        }
        else if (state == EThwompState.Active)
        {
            // check if hits ground and change active loc to transform pos
            SetState(EThwompState.AnimUp);
        }
        else if (state == EThwompState.AnimUp)
        {
            float pct = 1.0f - (animationTimer / EnemyConstants.ThwompAnimationDuration);
            float locationX = Mathf.Lerp(activeLocation.x, idleLocation.x, pct);
            float locationY = Mathf.Lerp(activeLocation.y, idleLocation.y, pct);
            transform.position = new Vector2(locationX, locationY);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EThwompState.Idle);
            }
        }
    }

    public void SetState(EThwompState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EThwompState.Idle)
            {
                transform.position = idleLocation;
                rigidbody.simulated = false;
            }
            else if (state == EThwompState.Active)
            {
                rigidbody.simulated = true;
            }
            else if (state == EThwompState.AnimUp)
            {
                rigidbody.simulated = false;
                animationTimer = EnemyConstants.ThwompAnimationDuration;

            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            // Get the Mario component from the GameObject
            Mario mario = collision.gameObject.GetComponent<Mario>();

            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y <= -0.8f)
            {
                mario.HandleDamage();
            }

            //// Check if there's a contact object, in the contacts array
            //if (collision.contacts.Length > 0)
            //{
            //    // Get the normal from the first contact object
            //    Vector2 normal = collision.contacts[0].normal;

            //    // Ensure the Goomba's state is walking
            //    if (state == EGoombaState.Walking)
            //    {
            //        if (normal.x <= -0.8f || normal.x >= 0.8f)
            //        {
            //            // Goomba collided with Mario on the side
            //            mario.HandleDamage();
            //        }
            //        else if (normal.y >= 0.7f)
            //        {
            //            // Goomba landed on Mario
            //            mario.HandleDamage();
            //        }
            //        else if (normal.y <= -0.7f)
            //        {
            //            // Mario landed on the Goomba, make Mario bounce off the enemy
            //            MarioMovement marioMovement = collision.gameObject.GetComponent<MarioMovement>();
            //            marioMovement.ApplyBounce();
            //        }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("World"))
        {
            isOnGround = true;
            activeLocation = transform.position;
            SetState(EThwompState.AnimUp);
        }
    }
}
