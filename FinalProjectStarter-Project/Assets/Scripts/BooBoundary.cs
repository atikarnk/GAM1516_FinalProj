using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooBoundary : MonoBehaviour
{
    public List<Boo> m_booList;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Mario"))
        {
            foreach (Boo boo in m_booList)
            { 
                boo.IsActive =true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            foreach (Boo boo in m_booList)
            {
                boo.IsActive = false;
            }
        }
    }
}
