using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    private Transform target;
    public Vector3 offset;

    public float smoothSpeed = 0.125f;

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        if (smoothedPos.z < -37)
        {
            smoothedPos.z = -37;
        }
        else if (smoothedPos.z > -30)
        {
            smoothedPos.z = -30;
        }

        if (smoothedPos.y != 23)
        {
            smoothedPos.y = 23;
        }
        smoothedPos.x = 0;
        transform.position = smoothedPos;
    }
}
