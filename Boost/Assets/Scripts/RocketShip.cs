using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShip : MonoBehaviour
{
    private const KeyCode FLY = KeyCode.Space;
    private const KeyCode TILT_LEFT = KeyCode.A;
    private const KeyCode TILT_RIGHT = KeyCode.D;

    private const float LEFTOVER_DECREASE_COEFFICIENT = 0.99f;
    private const float LEFTOVER_DECREASE_THRESHOLD = 0.01f;

    [SerializeField] float rotationThrust;

    private Vector3 up = Vector3.up;
    private Vector3 left = Vector3.forward;
    private Vector3 right = Vector3.back;

    private Vector3 leftLeftover = Vector3.zero;
    private Vector3 rightLeftover = Vector3.zero;

    private Rigidbody rigidBody;
    private AudioSource audioSource;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Thrust();
        Rotate();
    }

    private void Thrust()
    {
        if (Pressed(FLY))
        {
            Fly();
            FireEngine();
        }
        else
            StopEngine();
    }

    private void Rotate()
    {
        GainManualControl();

        if (Pressed(TILT_RIGHT) && Pressed(TILT_LEFT)) { }
        else if (Pressed(TILT_RIGHT))
            TiltRight();
        else if (Pressed(TILT_LEFT))
            TiltLeft();

        LeftoverRight();
        LeftoverLeft();

        ReleaseManualControl();
    }

    private void GainManualControl() => rigidBody.freezeRotation = true;

    private void ReleaseManualControl() => rigidBody.freezeRotation = false;

    private void Tilt(Vector3 direction) => transform.Rotate(direction * rotationThrust * Time.deltaTime);

    private void Fly() => rigidBody.AddRelativeForce(up);

    private bool Pressed(KeyCode command) => Input.GetKey(command);

    private void FireEngine()
    {
        if (!this.audioSource.isPlaying)
            this.audioSource.Play();
    }

    private void StopEngine()
    {
        if (this.audioSource.isPlaying)
            this.audioSource.Stop();
    }

    private void LeftoverRight()
    {
        if (leftLeftover != Vector3.zero)
        {
            Tilt(leftLeftover);

            leftLeftover *= LEFTOVER_DECREASE_COEFFICIENT;

            if (Math.Abs(leftLeftover.z) <= LEFTOVER_DECREASE_THRESHOLD)
                leftLeftover = Vector3.zero;
        }
    }

    private void LeftoverLeft()
    {
        if (rightLeftover != Vector3.zero)
        {
            Tilt(rightLeftover);

            rightLeftover *= LEFTOVER_DECREASE_COEFFICIENT;

            if (Math.Abs(rightLeftover.z) <= LEFTOVER_DECREASE_THRESHOLD)
                rightLeftover = Vector3.zero;
        }
    }

    private void TiltRight()
    {
        this.rightLeftover = right;

        this.Tilt(right);
    }

    private void TiltLeft()
    {
        this.leftLeftover = left;

        this.Tilt(left);
    }
}