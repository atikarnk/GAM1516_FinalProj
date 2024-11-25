using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBooState : byte 
{
    Unknown,
    Chase,
    Idel,
    MAX

}
public class Boo : Enemy
{
    private eBooState m_state = eBooState.Unknown;

    public Sprite m_chanceSprite;
    public Sprite m_idelSprite;

    private Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Set the enemy type
        enemyType = EEnemyType.Boo;
        velocity = new Vector2(EnemyConstants.BooSpeedx, EnemyConstants.BooSpeedy);
        SetState(eBooState.Idel);
    }



    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;

        

        if (m_state == eBooState.Chase)
        {
            Vector2 location = transform.position;
            location += velocity * Time.deltaTime * Game.Instance.LocalTimeScale+ marioLocation;
            transform.position = location;
        }
    }
    private void SetState(eBooState state)
    {
        m_state = state;
        if (m_state == eBooState.Idel)
        {
            spriteRenderer.sprite = m_idelSprite;
        }
        if (m_state == eBooState.Chase)
        {
            spriteRenderer.sprite = m_chanceSprite;
        }
    }
    public eBooState State
    {
        get { return m_state; }
    }

    
}
