using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemBoxCoin : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private Vector2 start;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = PickupConstants.ItemBoxCoinGravityScale;
        rigidbody.AddForce(new Vector2(0.0f, PickupConstants.ItemBoxCoinImpulseY), ForceMode2D.Impulse);

        animator = GetComponent<Animator>();
        animator.Play("ItemBoxCoin");

        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < start.y)
        {
            Destroy(gameObject);
        }
    }
}
