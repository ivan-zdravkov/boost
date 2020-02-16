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
    private AudioSource audioSource;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Pressed(FLY))
        {
            Fly();
            FireEngine();
        }
        else
            StopEngine();

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
}