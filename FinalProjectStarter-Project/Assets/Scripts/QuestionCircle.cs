using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionCircle : Pickup
{
    // Start is called before the first frame update
    void Start()
    {
        pickupType = EPickupType.QuestionCircle;
        
    }
    public new void Spawn()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = PickupConstants.QuestionCircleGravityScale;
        rigidbody.AddForce(new Vector2(0.0f, PickupConstants.QuestionCircleImpulseY), ForceMode2D.Impulse);

    }

}

