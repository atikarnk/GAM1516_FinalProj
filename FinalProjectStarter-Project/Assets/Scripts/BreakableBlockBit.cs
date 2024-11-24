using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBreakableBlockBitType : byte
{
    Unknown,
    Left,
    Right
}

public class BreakableBlockBit : MonoBehaviour
{
    public Sprite leftSprite;
    public Sprite rightSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < GameConstants.DestroyActorAtY)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(EBreakableBlockBitType type, Vector2 impulse)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (type == EBreakableBlockBitType.Left)
        {
            spriteRenderer.sprite = leftSprite;
        }
        else if (type == EBreakableBlockBitType.Right)
        {
            spriteRenderer.sprite = rightSprite;
        }

        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 2.5f;
        rigidbody.AddForce(impulse, ForceMode2D.Impulse);
        rigidbody.AddTorque(Mathf.PI * 2.0f, ForceMode2D.Impulse);
    }
}
