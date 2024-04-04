using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cat : MonoBehaviour
{
    [SerializeField] private CatType catType;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite eyesClosedSprite;
    [SerializeField] private float eyesOpenMaxDuration = 0.5f;
    [SerializeField] private float eyesClosedDuration = 0.5f;
    [SerializeField] private float invincibilityDuration = 0.1f;

    private SpriteRenderer _spriteRenderer;
    private bool _blinkingInProgress = false;

    private void Start()
    {
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>().First();
    }


    public CatType CatType => catType;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Cat cat))
        {
            if (cat.CatType == catType)
            {
                // Debug.Log("Cat " + catType + " collided with Cat " + cat.CatType);
                CatManager.Instance.QueueMergeRequest(this, cat);
            }
            else
            {
                //  Debug.Log("Cat " + catType + " collided with Cat " + cat.CatType + ". They are not the same type.");
            }
        }
    }

    private void Update()
    {
        if (!_blinkingInProgress)
        {
            float rand = Random.Range(eyesOpenMaxDuration / 3f, eyesOpenMaxDuration);
            Invoke(nameof(BlinkClose), rand);
            _blinkingInProgress = true;
        }
    }

    private void BlinkClose()
    {
        _spriteRenderer.sprite = eyesClosedSprite;
        Invoke(nameof(BlinkOpen), eyesClosedDuration);
    }

    private void BlinkOpen()
    {
        _spriteRenderer.sprite = defaultSprite;
        _blinkingInProgress = false;
    }
}