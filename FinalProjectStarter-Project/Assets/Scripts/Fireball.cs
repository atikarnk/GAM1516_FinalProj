using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("Fireball");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime * Game.Instance.LocalTimeScale);
    }

    public void Spawn(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }
}
