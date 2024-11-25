using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneUpPickup : Pickup
{
    public BoxCollider2D frontTrigger;
    public BoxCollider2D backTrigger;
    protected new Rigidbody2D rigidbody;
    protected Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        pickupType = EPickupType.OneUp;
    }

    private void FixedUpdate()
    {
        if (pickupState == EPickupState.Active)
        {
            rigidbody.position += velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
        }
    }

    protected override void OnPickupActive()
    {
        base.OnPickupActive();

        velocity.x = (UnityEngine.Random.Range(0, 10) % 2 == 0) ? (PickupConstants.MushroomSpeed) : (-PickupConstants.MushroomSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickupState == EPickupState.Active)
        {
            if (other.gameObject.CompareTag("World") || other.gameObject.CompareTag("Enemy"))
            {
                ContactFilter2D filter = new ContactFilter2D().NoFilter();
                List<Collider2D> results = new List<Collider2D>();
                other.OverlapCollider(filter, results);

                if (results.Contains(frontTrigger))
                {
                    velocity.x = -PickupConstants.MushroomSpeed;
                }
                else if (results.Contains(backTrigger))
                {
                    velocity.x = PickupConstants.MushroomSpeed;
                }
            }
        }
    }
}
