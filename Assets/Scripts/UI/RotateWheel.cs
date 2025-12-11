using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWheel : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 25f;

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
    }

}
