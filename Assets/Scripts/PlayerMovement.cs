using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;


public class PlayerMovement : MonoBehaviour
{
    private Vector2 joystickSize = new Vector2(200, 200);
    private Joystick joystick;

    private CharacterController controller;
    private Animator animator;

    private Finger movementFinger;
    private Vector2 movementAmount;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;

    private float turnSmoothVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        joystick = FindObjectOfType<Joystick>();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLostFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLostFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == movementFinger)
        {
            Vector2 knobPos;
            float maxMovement = joystickSize.x / 2;
            ETouch.Touch currentTouch = movedFinger.currentTouch;

            if (Vector2.Distance(
                currentTouch.screenPosition, 
                joystick.rectTransform.anchoredPosition
                ) > maxMovement)
            {
                knobPos = (
                    currentTouch.screenPosition - joystick.rectTransform.anchoredPosition
                    ).normalized * maxMovement;
            }
            else
            {
                knobPos = currentTouch.screenPosition - joystick.rectTransform.anchoredPosition;
            }

            joystick.knob.anchoredPosition = knobPos;
            movementAmount = knobPos / maxMovement;
        }
    }

    private void HandleLostFinger(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            movementFinger = null;
            joystick.knob.anchoredPosition = Vector2.zero;
            joystick.gameObject.SetActive(false);
            movementAmount = Vector2.zero;
        }
    }

    private void HandleFingerDown(Finger touchedFinger)
    {
        if (movementFinger == null)
        {
            movementFinger = touchedFinger;
            movementAmount = Vector2.zero;
            joystick.gameObject.SetActive(true);
            joystick.rectTransform.sizeDelta = joystickSize;
            joystick.rectTransform.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
        }
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if (startPosition.x < joystickSize.x)
        {
            startPosition.x = joystickSize.x / 2;
        }
        else if (startPosition.x > Screen.width - joystickSize.x / 2)
        {
            startPosition.x = Screen.width - joystickSize.x / 2;
        }

        if (startPosition.y < joystickSize.y)
        {
            startPosition.y = joystickSize.y / 2;
        }
        else if (startPosition.y > Screen.height - joystickSize.y / 2)
        {
            startPosition.y = Screen.height - joystickSize.y / 2;
        }

        return startPosition;
    }

    private void Update()
    {
        Vector3 scaledMovement = speed * Time.deltaTime * new Vector3(
            movementAmount.x, 
            0, 
            movementAmount.y
            );

        if (scaledMovement.magnitude >= 0.1f)
        {
            animator.SetBool("isMoving", true);
            float targetAngle = Mathf.Atan2(scaledMovement.x, scaledMovement.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(scaledMovement);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
