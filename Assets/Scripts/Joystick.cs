using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rectTransform;
    public RectTransform knob;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
