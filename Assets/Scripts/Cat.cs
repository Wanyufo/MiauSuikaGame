using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Cat : MonoBehaviour
{
    [SerializeField] private CatType catType;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite angrySprite;
    [SerializeField] private Sprite otherSprite;
    [SerializeField] private float eyesOpenMaxDuration = 0.5f;
    [SerializeField] private float eyesClosedDuration = 0.5f;

    [SerializeField] private float angryDuration = 1f;

    [SerializeField] private float relativeVelocityToAngry = 10f;

    private SpriteRenderer _spriteRenderer;
    private bool _blinkingInProgress = false;

    public bool falling = false;

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

        if (collision.relativeVelocity.magnitude > relativeVelocityToAngry)
        {
            // Debug.Log("Cat " + catType + " collided with " + collision.relativeVelocity.magnitude + " force");

            LookAngry();
        }
    }

    private void Update()
    {
        if (!_blinkingInProgress)
        {
            float rand = Random.Range(eyesOpenMaxDuration / 3f, eyesOpenMaxDuration);
            ExclusiveInvoke(nameof(LookOther), rand);
            _blinkingInProgress = true;
        }
    }

    private void LookAngry()
    {
        _blinkingInProgress = true;
        _spriteRenderer.sprite = angrySprite;
        ExclusiveInvoke(nameof(LookNormal), angryDuration);
    }

    private void LookOther()
    {
        _spriteRenderer.sprite = otherSprite;
        ExclusiveInvoke(nameof(LookNormal), eyesClosedDuration);
    }

    private void LookNormal()
    {
        _spriteRenderer.sprite = defaultSprite;
        _blinkingInProgress = false;
    }

    private void ExclusiveInvoke(string methodName, float time)
    {
        CancelInvoke();
        Invoke(methodName, time);
    }
}