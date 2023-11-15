using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketShip : MonoBehaviour
{
    private const KeyCode FLY = KeyCode.Space;
    private const KeyCode TILT_LEFT = KeyCode.A;
    private const KeyCode TILT_RIGHT = KeyCode.D;

    private const float TRANSITION_DELAY = 2.5f;

    private const float LEFTOVER_DECREASE_COEFFICIENT = 0.99f;
    private const float LEFTOVER_DECREASE_THRESHOLD = 0.01f;

    [SerializeField] float rotationThrust;
    [SerializeField] float mainThrust;

    [SerializeField] AudioClip victory;
    [SerializeField] AudioClip destroy;

    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    private Vector3 up = Vector3.up;
    private Vector3 left = Vector3.forward;
    private Vector3 right = Vector3.back;

    private Vector3 thrustLeftover = Vector3.zero;
    private Vector3 leftLeftover = Vector3.zero;
    private Vector3 rightLeftover = Vector3.zero;

    private Rigidbody rigidBody;
    private AudioSource audioSource;
    private Light engineLight;

    enum State { Alive, Dying, Transcending };

    State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        engineLight = GetComponentsInChildren<Light>()
            .FirstOrDefault(x => x.name == "EngineLight");

        engineLight.enabled = false;
    }

    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        else if (state == State.Dying)
            StopEngine();
        else if (state == State.Transcending)
        {
            StopEngine();
            StopMoving();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                if (LandedOnFeetOfRocket(collision))
                    StartCoroutine(LoadNextScene());
                else if (state == State.Alive)
                    StartCoroutine(Die());
                break;
            default:
                StartCoroutine(Die());
                break;
        }
    }

    private void Thrust()
    {
        if (Pressed(FLY))
        {
            Fly(up);
            FireEngine();
        }
        else
            StopEngine();

        LeftoverThrust();
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
    private void Fly(Vector3 direction) => rigidBody.AddRelativeForce(direction * mainThrust * Time.deltaTime);
    private bool Pressed(KeyCode command) => Input.GetKey(command);
    private bool LandedOnFeetOfRocket(Collision collision) => collision.contacts.Any(contact => contact.thisCollider.ToString().Contains("Bottom"));
    private void FireEngine()
    {
        if (!this.audioSource.isPlaying)
            this.audioSource.Play();

        if (!this.leftThrustParticles.isPlaying)
            this.leftThrustParticles.Play();

        if (!this.rightThrustParticles.isPlaying)
            this.rightThrustParticles.Play();

        engineLight.enabled = true;
    }

    private void StopEngine()
    {
        if (this.audioSource.isPlaying)
            this.audioSource.Stop();

        if (this.leftThrustParticles.isPlaying)
            this.leftThrustParticles.Stop();

        if (this.rightThrustParticles.isPlaying)
            this.rightThrustParticles.Stop();

        this.engineLight.enabled = false;
    }

    private void StopMoving()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
    }

    private void LeftoverThrust()
    {
        if (thrustLeftover != Vector3.zero)
        {
            Fly(thrustLeftover);

            thrustLeftover *= LEFTOVER_DECREASE_COEFFICIENT;

            if (Math.Abs(thrustLeftover.y) <= LEFTOVER_DECREASE_THRESHOLD)
                thrustLeftover = Vector3.zero;
        }
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

    private IEnumerator LoadNextScene()
    {
        if (state == State.Alive)
        {
            state = State.Transcending;

            AudioSource.PlayClipAtPoint(victory, Camera.main.transform.position);

            successParticles.Play();

            yield return new WaitForSeconds(TRANSITION_DELAY);

            state = State.Alive;

            SceneManager.LoadScene(NextScene());
        }
    }

    private IEnumerator Die()
    {
        state = State.Dying;

        AudioSource.PlayClipAtPoint(destroy, Camera.main.transform.position);

        deathParticles.Play();

        yield return new WaitForSeconds(TRANSITION_DELAY);

        state = State.Alive;

        SceneManager.LoadScene(0);
    }

    private static int NextScene()
    {
        int nextScepe = SceneManager.GetActiveScene().buildIndex + 1;

        return nextScepe < SceneManager.sceneCountInBuildSettings ? nextScepe : 0;
    }
}