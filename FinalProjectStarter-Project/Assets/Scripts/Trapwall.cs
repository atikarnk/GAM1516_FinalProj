using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapwall : MonoBehaviour
{
    public BoxCollider2D primaryCollider;
    public BoxCollider2D topTrigger;

    private new Rigidbody2D rigidbody;
    private Vector2 velocity = Vector2.zero;
    private Vector2 location = Vector2.zero;
    private bool isActive = false;
    private bool isGrounded = false;
    private float shakeTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        primaryCollider.enabled = false;
        rigidbody = GetComponent<Rigidbody2D>();
        //rigidbody.bodyType = RigidbodyType2D.Static;
        rigidbody.gravityScale = 0.0f;
        velocity.y = -EnemyConstants.ThwompFallingSpeed;
        location = transform.position;
        shakeTimer = GameConstants.CameraShakeMaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            rigidbody.gravityScale = 0.0f;
            location = transform.position;

            MarioCamera camera = Game.Instance.marioCamera;
            camera.CheckShaking = true;

            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0.0f)
            {
                camera.CheckShaking = false;
                shakeTimer = 0.0f;
            }
        }
        else if (isActive)
        {
            rigidbody.gravityScale = 1.0f;
            //rigidbody.bodyType = RigidbodyType2D.Dynamic;
            location = transform.position;
            location += velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            transform.position = location;
            //Vector2 vlocation = rigidbody.position;
            //vlocation += velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            //rigidbody.position = vlocation;
        }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("World"))
        {
            if (collision.contacts.Length > 0)
            {
                //&& collision.contacts[0].normal.y >= 0.7f
                isGrounded = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();
        collision.OverlapCollider(filter, results);

        primaryCollider.enabled = true;
    }
}
