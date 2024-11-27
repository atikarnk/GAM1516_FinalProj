using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBoomAwakenBoundry : MonoBehaviour
{
    public BoomBoom m_boomboom;
    //public TrapWall trapWall;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Mario"))
        {
            m_boomboom.IsAwake = true;
            //TODO trigger trap wall
        }
    }
}
