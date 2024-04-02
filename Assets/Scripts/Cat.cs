using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] private CatType catType;

    public CatType CatType => catType;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Cat cat))
        {
            if (cat.CatType == catType)
            {
               // Debug.Log("Cat " + catType + " collided with Cat " + cat.CatType);
                CatManager.Instance.QueueMergeCats(this, cat);
            }
            else
            {
              //  Debug.Log("Cat " + catType + " collided with Cat " + cat.CatType + ". They are not the same type.");
            }
        }
    }
}