using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSwitch : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private new BoxCollider2D collider;

    public Sprite usedSprite;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y <= -0.8f)
            {
                spriteRenderer.sprite = usedSprite;
                collider.isTrigger = true;

                Game.Instance.OnCoinSwitch();
            }
        }
    }
}
