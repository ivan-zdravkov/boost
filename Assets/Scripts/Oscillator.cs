using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    const float TAU = Mathf.PI * 2;

    [SerializeField] Vector3 movementVector = new Vector3(10f, 0f, 0f);
    [SerializeField] float period = 2f;

    private Vector3 startingPosition;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        if (period <= Mathf.Epsilon)
            return;

        float cycles = Time.time / period;
        float rawSinWave = Mathf.Sin(cycles * TAU);
        float factor = rawSinWave / 2f + 0.5f;

        transform.position = startingPosition + movementVector * factor;
    }
}
