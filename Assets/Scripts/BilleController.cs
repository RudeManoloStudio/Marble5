using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilleController : MonoBehaviour
{



    [SerializeField] private MeshRenderer mr;
    [SerializeField] private float minRotationSpeed = 0.09f;
    [SerializeField] private float maxRotationSpeed = 0.2f;

    private Vector3 angularVelocity;
    private float speed;

    private void OnEnable()
    {

        mr.material.SetFloat("_seed", Random.Range(int.MinValue, int.MaxValue));

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



