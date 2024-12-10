using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DeathStarEffect : MonoBehaviour
{
    public List<Sprite> m_starEffectList = new List<Sprite>();
    SpriteRenderer spriteRenderer;
    float randomStartTimer;
    int prevIndex;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        randomStartTimer = 0.1f;
        prevIndex = -1;
    }

// Update is called once per frame
    void Update()
    {
        randomStartTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;
        
        if (randomStartTimer <= 0.0f)
        {
            randomStartTimer = 0.1f;
            int index;
            do
            {
                index = UnityEngine.Random.Range(0, 10) % m_starEffectList.Count;
            }
            while (prevIndex == index);
            prevIndex = index;
            spriteRenderer.sprite = m_starEffectList[index];
        }  
    }

    public void Spawn( Vector2 direction)
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0.0f;

        rigidbody.AddForce(direction * 800, ForceMode2D.Force);
        Destroy(gameObject, 1.5f);
    }
}
