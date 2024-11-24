using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : Pickup
{
    // Start is called before the first frame update
    void Start()
    {
        pickupType = EPickupType.Coin;
        Animator animator = GetComponent<Animator>();
        animator.Play("Coin");
    }

    public void UseStaticCoin()
    {
        Animator animator = GetComponent<Animator>();
        animator.Play("CoinStatic");
    }
}
