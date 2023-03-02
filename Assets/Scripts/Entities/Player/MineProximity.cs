using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineProximity : MonoBehaviour
{
    public Mine mine;
    // Start is called before the first frame update


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            mine.Explode();
        }    
    }
}
