using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBoomAwakenBoundry : MonoBehaviour
{
    public BoomBoom m_boomboom;
    public Trapwall m_trapWall;
    //public TrapWall trapWall;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Mario"))
        {
            m_boomboom.IsAwake = true;
            m_trapWall.IsActive = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            m_boomboom.Reset();
            m_trapWall.Reset();
        }
    }
   
}
