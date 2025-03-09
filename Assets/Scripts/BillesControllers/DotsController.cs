using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsController : MonoBehaviour
{



    [SerializeField] private MeshRenderer mr;
    [SerializeField] private float minRotationSpeed = 0.09f;
    [SerializeField] private float maxRotationSpeed = 0.2f;

    private Vector3 angularVelocity;
    private float speed;

    private void OnEnable()
    {

        mr.material.SetFloat("_size", Random.Range(0.09f, 0.9f));

        int x = Random.Range(0, 360);
        int y = Random.Range(0, 360);
        int z = Random.Range(0, 360);

        angularVelocity = new Vector3(x, y, z);

        speed = Random.Range(minRotationSpeed, maxRotationSpeed);

    }

    private void LateUpdate()
    {
           
        transform.Rotate(angularVelocity * Time.deltaTime * speed, Space.Self);

    }
}



