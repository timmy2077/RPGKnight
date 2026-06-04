using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevation_Exit : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] bounderColliders;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (Collider2D mountain in mountainColliders)
            {
                mountain.enabled = true;
            
            }
            foreach (Collider2D bounder in bounderColliders)
            {
                bounder.enabled = false;
            }
        collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder =0;
        }
    }
}
