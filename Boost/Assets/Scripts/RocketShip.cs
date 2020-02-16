using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShip : MonoBehaviour
{
    private const KeyCode FLY = KeyCode.Space;
    private const KeyCode TILT_LEFT = KeyCode.A;
    private const KeyCode TILT_RIGHT = KeyCode.D;

    private Vector3 up = Vector3.up;
    private Vector3 left = Vector3.forward;
    private Vector3 right = Vector3.back;

    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Pressed(FLY))
            Fly();

        if (Pressed(TILT_RIGHT) && Pressed(TILT_LEFT))
            return;
        else if (Pressed(TILT_RIGHT))
            Tilt(right);
        else if (Pressed(TILT_LEFT))
            Tilt(left);
    }

    private void Fly() => rigidBody.AddRelativeForce(up);

    private void Tilt(Vector3 direction) => transform.Rotate(direction);

    private bool Pressed(KeyCode command) => Input.GetKey(command);
}