using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.RuleTile.TilingRuleOutput;
public enum ePodobooState : byte
{
    Unknown,
    MovingOut,
    MovingIn,
    Hiding,
    MAX

}
public enum ePodobooDirection : int
{
    Unknown,
    Up=1,
    Down=-1,
    Left=-2,
    Right=2,
    MAX

}
public class Podoboo : Enemy
{
    private ePodobooState m_state = ePodobooState.Unknown;
    public ePodobooDirection m_direction = ePodobooDirection.Unknown;
    public float m_apexDistance = 0.0f;
    private Vector2 m_apexLocation = Vector2.zero;
    private Vector2 m_hidingLocation = Vector2.zero;
    private float m_hidingTimer = 0.0f;
    private float animationTimer = 0.0f;
    private Vector2 m_exitLavaLocation = Vector2.zero;
    private Vector3 m_splashDirection = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        // Set the enemy type
        enemyType = EEnemyType.Podoboo;
        Vector2 apexDistance = Vector2.zero;
        // Capture the starting location (hiding) and from that calculate the active (on-screen) location
        m_hidingLocation = transform.position;
        if (m_direction == ePodobooDirection.Left || m_direction == ePodobooDirection.Right)
        {
            transform.Rotate(0, 0, -90* (2 / (int)m_direction));
            apexDistance = new Vector2((m_apexDistance* (2/(int)m_direction)), 0.0f);
        }
        else
        {
            apexDistance = new Vector2(0.0f, (m_apexDistance * (int)m_direction));
        }
        m_apexLocation = m_hidingLocation + apexDistance;

        // Set the state to hiding
        SetState(ePodobooState.Hiding);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state == ePodobooState.Hiding)
        {
            m_hidingTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (m_hidingTimer <= 0.0f)
            {
                m_hidingTimer = 0.0f;
                SetState(ePodobooState.MovingOut);
            }
        }
        else if (m_state == ePodobooState.MovingOut)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / EnemyConstants.c_podobooAnimationDuration);
            float locationX = Mathf.Lerp(m_hidingLocation.x, m_apexLocation.x, pct);
            float locationY = Mathf.Lerp(m_hidingLocation.y, m_apexLocation.y, pct);
            transform.position = new Vector2(locationX, locationY);

            if (Vector2.Distance(m_apexLocation, transform.position) <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(ePodobooState.MovingIn);
            }
        }
        else if (m_state == ePodobooState.MovingIn)
        {


            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;
            //fliping the image makes it shift over, this will shift it back
            float x = m_apexLocation.x - ((m_direction == ePodobooDirection.Left || m_direction == ePodobooDirection.Right)? 1 : 0 );
            float y = m_apexLocation.y - ((m_direction == ePodobooDirection.Up || m_direction == ePodobooDirection.Down) ? 1 : 0); ;

            float pct = 1.0f - (animationTimer / EnemyConstants.c_podobooAnimationDuration);
            float locationX = Mathf.Lerp(x, m_hidingLocation.x, pct);
            float locationY = Mathf.Lerp(y, m_hidingLocation.y, pct);
            transform.position = new Vector2(locationX, locationY);

            if (Vector2.Distance(m_hidingLocation, transform.position) <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(ePodobooState.Hiding);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            // Get the Mario component from the GameObject
            Mario mario = collision.gameObject.GetComponent<Mario>();

            if (collision.contacts.Length > 0)
            {
                mario.HandleDamage(true);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            //set splash zone once
            if (m_exitLavaLocation==Vector2.zero)
            { 
                m_exitLavaLocation = collision.gameObject.transform.position;

                if (m_direction == ePodobooDirection.Left)
                {
                    m_exitLavaLocation.x += 0.5f;
                    m_exitLavaLocation.y += 0.5f;

                    m_splashDirection = new Vector3(0, 0, 90 * (2 / (int)m_direction));
                }
                else if (m_direction == ePodobooDirection.Right)
                {
                    m_exitLavaLocation.x -= 0.5f;
                    m_exitLavaLocation.y += 0.5f;
                    m_splashDirection = new Vector3(0, 0, 90 * (2 / (int)m_direction));
                }
                else if (m_direction == ePodobooDirection.Up)
                {
                    m_splashDirection = new Vector3(0, 0, 180);
                }
                else if (m_direction == ePodobooDirection.Down)
                {
                    m_exitLavaLocation.y += 1.0f;
                    m_splashDirection = new Vector3(0, 0, 0);
                }
            }

            Game.Instance.SpawnSplash(m_exitLavaLocation, m_splashDirection);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        

    }
    void ChangeDirection(ePodobooDirection direction)
    {
        if (m_direction == direction)
        {
            Vector3 scale = transform.localScale;
            m_direction = (ePodobooDirection)((int)m_direction * -1);
            if (direction == ePodobooDirection.Left || direction == ePodobooDirection.Right)
            {
                scale.y = (float)(2 / (int)m_direction);
                scale.x = 1.0f;
            }
            else if (direction == ePodobooDirection.Up || direction == ePodobooDirection.Down)
            {
                scale.y = (float)direction;
                scale.x = 1.0f;
            }
            transform.localScale = scale;

        }
    }

    private void SetState(ePodobooState newState)
    {
        if (m_state != newState)
        {
            m_state = newState;
            if (m_state == ePodobooState.Hiding)
            {
                m_hidingTimer = Random.Range(EnemyConstants.c_podobooHiddenDurationMin, EnemyConstants.c_podobooHiddenDurationMax);
            }
            if (m_state == ePodobooState.MovingIn)
            {
                ChangeDirection(m_direction);
                animationTimer = EnemyConstants.c_podobooAnimationDuration;
            }
            
            if (m_state == ePodobooState.MovingOut)
            {
                ChangeDirection(m_direction);
                animationTimer = EnemyConstants.c_podobooAnimationDuration;
            }
        }
    }
}
