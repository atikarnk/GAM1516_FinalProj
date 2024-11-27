using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStarEffect : MonoBehaviour
{
    public List<Sprite> m_starEffectList;
    // Start is called before the first frame update
    void Start()
    {
        m_starEffectList = new List<Sprite>();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject,1.0f);
        //TODO randomize star on timer
    }

    public void Spawn( Vector2 direction)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        int index = UnityEngine.Random.Range(0, 10) % m_starEffectList.Count;
        spriteRenderer.sprite = m_starEffectList[index];

        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0.0f;

        rigidbody.AddForce(direction * 1000, ForceMode2D.Force);
        //rigidbody.AddRelativeForce(direction, ForceMode2D.Force);
        //rigidbody.AddTorque(Mathf.PI * 2.0f, ForceMode2D.Impulse);

        //Vector2 location = rigidbody.position;
        //location += m_velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
        //rigidbody.position = location;
    }
}
