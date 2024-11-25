using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;

public enum eDonutLiftState : byte
{
    Unknown,
    Idel,
    SteppedOn,
    Falling,
    MAX = Falling
}
public class DonutLift : MonoBehaviour
{
    private eDonutLiftState m_state = eDonutLiftState.Unknown;

    protected new Rigidbody2D rigidbody;
    protected SpriteRenderer spriteRenderer;

    public Sprite m_IdelSprite;
    public Sprite m_SteppedOnSprite;
    public Vector3 m_OGPosition;
    private float m_SteppedMaxTime = 0.0f; 
    private Vector2 velocity = Vector2.zero;
    void Start()
    {
        m_OGPosition = transform.position;
        m_SteppedMaxTime =GameConstants.DonutLiftSteppedMaxTime;
        velocity.y -= GameConstants.DonutLiftFallingSpeed;
        rigidbody = GetComponentInParent<Rigidbody2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        SetState(eDonutLiftState.Idel);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state == eDonutLiftState.SteppedOn)
        {
            //shake logic
            {
                float xVal = UnityEngine.Random.Range(-0.03f, 0.03f) % 2;

                Vector2 location = transform.position;
                
                location.x += xVal;

                transform.position = location;
               
                if (Mathf.Repeat(m_SteppedMaxTime, 0.02f) <= 0.01)
                {
                    transform.position = m_OGPosition;
                }
            }

            m_SteppedMaxTime -= Time.deltaTime * Game.Instance.LocalTimeScale;
            if (m_SteppedMaxTime <= 0.0f)
            {
                m_SteppedMaxTime = 0.0f;
                SetState(eDonutLiftState.Falling);
            }
        }
        if (m_state == eDonutLiftState.Falling)
        {
           
            Vector2 location = transform.position;
            location += velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            transform.position = location;
           
        }
        if (transform.position.y < GameConstants.DestroyActorAtY)
        {
            Destroy(gameObject);
        }
    }
    private void SetState(eDonutLiftState newState)
    {
        if (m_state != newState)
        {
            m_state = newState;
            if (m_state == eDonutLiftState.Idel)
            {
                m_SteppedMaxTime = GameConstants.DonutLiftSteppedMaxTime;
                spriteRenderer.sprite = m_IdelSprite;
            }
            if (m_state == eDonutLiftState.SteppedOn)
            {
                spriteRenderer.sprite = m_SteppedOnSprite;
               
            }
            if (m_state == eDonutLiftState.Falling)
            {
                spriteRenderer.sprite = m_SteppedOnSprite;

            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y <= -0.8f)
            {
                SetState(eDonutLiftState.SteppedOn);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            if (m_state != eDonutLiftState.Falling)
            {
                SetState(eDonutLiftState.Idel);
            }
        }
    }
}
        
