using System;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Both Cameras")]
    public Transform focusPoint;
    public bool _3DMode = false;
    [Tooltip("Lerp time in seconds")]
    public float lerpTime = 2f;

    [Header("3D Camera")]
    public float radius = 8f;
    public float turnSpeed = 5f;
    public float _3DHeightOffset = 5f;

    [Header("2D Camera")]
    public float _2DHeightOffset = 5f;

    private float angle = 0f;
    private bool lerping = false;
    private bool previousMode = false;

    private float lerpTimer = 0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Detect mode change and then start lerping
        if (previousMode != _3DMode)
        {
            lerping = true;
        }

        handleInput();

        // Turn of camera movement when lerping
        if (!lerping)
        {
            if (_3DMode)
            {
                _3dCamera();
            }
            else
            {
                _2dCamera();
            }
        }
        else
        {
            lerpBetweenModes();
        }

        // Avoid rotation issues when in 2d camera
        if (!_3DMode && !lerping)
        {
            transform.eulerAngles = new Vector3(90f, angle - 180f, 0f);
        }
        else
        {
            transform.LookAt(focusPoint.position);
        }


        previousMode = _3DMode;
    }

    private void lerpBetweenModes()
    {
        Vector3 startPosition;
        Vector3 endPosition;

        Vector3 _2dPosition = focusPoint.position + new Vector3(0f, _2DHeightOffset, 0f);
        Vector3 _3dPosition = focusPoint.position + calculatePositionOnCircle(angle, radius, _3DHeightOffset);
        if (_3DMode)
        {
            startPosition = _2dPosition;
            endPosition = _3dPosition;
        }
        else
        {
            startPosition = _3dPosition;
            endPosition = _2dPosition;
        }
        if (lerpTimer < 1f)
        {
            lerpTimer += Time.deltaTime / lerpTime;
            Vector3 test = Vector3.Lerp(startPosition, endPosition, lerpTimer);
            transform.position = test;
        }
        else
        {
            lerpTimer = 0f;
            lerping = false;
        }
    }

    private Vector3 calculatePositionOnCircle(float angle, float radius, float _3DHeightOffset)
    {
        double radians = angle * Mathf.Deg2Rad;
        return new Vector3((float)Math.Sin(radians), 0f, (float)Math.Cos(radians)) * radius + new Vector3(0f, _3DHeightOffset, 0f);
    }

    public static float Clamp0360(float eulerAngle)
    {
        float result = eulerAngle - Mathf.CeilToInt(eulerAngle / 360f) * 360f;
        if (result < 0f)
        {
            result += 360f;
        }
        return result;
    }

    private void _2dCamera()
    {
        transform.position = focusPoint.position + new Vector3(0f, _2DHeightOffset, 0f);
    }

    private void _3dCamera()
    {
        angle = Clamp0360(angle);
        transform.position = focusPoint.position + calculatePositionOnCircle(angle, radius, _3DHeightOffset);
    }

    private void handleInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            angle += turnSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            angle -= turnSpeed * Time.deltaTime;
        }
    }
}
