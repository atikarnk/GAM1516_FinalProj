using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum EThwompState : byte
{
    Unknown,
    Idle,
    AnimDown,
    Active,
    AnimUp
}
public class Thwomp : Enemy
{
    private new Rigidbody2D rigidbody;
    private EThwompState state = EThwompState.Unknown;

    private Vector2 idleLocation = Vector2.zero;
    private Vector2 activeLocation = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    private float animationTimer = 0.0f;
    private float holdTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        enemyType = EEnemyType.Thwomp;

        idleLocation = transform.position;
        velocity.y = -EnemyConstants.ThwompFallingSpeed;
        rigidbody.gravityScale = 0.0f;

        SetState(EThwompState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EThwompState.Idle)
        {
            Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;
            if (marioLocation.x - idleLocation.x <= 1.5f && marioLocation.x - idleLocation.x >= -1.5f)
            {
                SetState(EThwompState.AnimDown);
            }
        }
        else if (state == EThwompState.AnimDown)
        {
            activeLocation = transform.position;
            activeLocation += velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            transform.position = activeLocation;
        }
        else if (state == EThwompState.Active)
        {
            MarioCamera camera = Game.Instance.marioCamera;
            camera.CheckShaking = true;

            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0.0f)
            {
                camera.CheckShaking = false;
                holdTimer = 0.0f;
                SetState(EThwompState.AnimUp);
            }
        }
        else if (state == EThwompState.AnimUp)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

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
            }
            else if (state == EThwompState.AnimDown)
            {

            }
            else if (state == EThwompState.Active)
            {
                holdTimer = EnemyConstants.ThwompHoldTimer;
            }
            else if (state == EThwompState.AnimUp)
            {
                activeLocation = transform.position;
                animationTimer = EnemyConstants.ThwompAnimationDuration;

            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == EThwompState.AnimDown)
        {
            if (collision.gameObject.CompareTag("World"))
            {
                if (collision.contacts.Length > 0)
                {
                    SetState(EThwompState.Active);
                }
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
                if (normal.x <= -0.8f || normal.x >= 0.8f)
                {
                    // collided with Mario on the side
                    mario.HandleDamage();
                }
                else if (normal.y >= 0.7f)
                {
                    // landed on Mario
                    mario.HandleDamage();
                }
                else if (normal.y <= -0.7f)
                {
                    // Mario landed on
                    mario.HandleDamage();
                }
            }
        }
    }
}
