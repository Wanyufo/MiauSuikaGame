using System;
using UnityEngine;


public class AspectKeeper : MonoBehaviour
{
    // Set your desired aspect ratio here
    [SerializeField] private Vector2 aspectRatio = new Vector2(16, 9);
    [SerializeField] private Color backgroundColor = Color.gray;


    private float _targetAspect = 16f / 9f;

    private Camera _mainCamera;

    private void Start()
    {
        _targetAspect = aspectRatio.x / aspectRatio.y;
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("No main camera found!");
            Destroy(this);
            return;
        }

        Camera cam = gameObject.AddComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Failed to add camera component!");
            Destroy(this);
            return;
        }


        cam.depth = _mainCamera.depth - 1;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.cullingMask = 0;
        cam.backgroundColor = backgroundColor;
    }

    // Use this for initialization
    void Update()
    {
        // determine the game window's current aspect ratio
        float windowAspect = (float) Screen.width / (float) Screen.height;

        // current viewport height should be scaled by this amount
        float scaleHeight = windowAspect / _targetAspect;

        if (Math.Abs(scaleHeight - 1) < 0.01f) return;

        // if scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f)
        {
            Rect rect = _mainCamera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            _mainCamera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleHeight;

            Rect rect = _mainCamera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            _mainCamera.rect = rect;
        }
    }
}