using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GameOverCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Cat cat))
        {
            if (cat.falling)
            {
                CatManager.Instance.GameOver();
            }
            else
            {
                Destroy(cat.gameObject);
               // TODO Add small congrats message for finding the bug
            }
        }
    }
}