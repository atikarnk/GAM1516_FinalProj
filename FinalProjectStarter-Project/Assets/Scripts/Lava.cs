using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lava : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
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
}