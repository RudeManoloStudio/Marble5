using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilleController : MonoBehaviour
{



    [SerializeField] private MeshRenderer mr;

    private Vector3 angularVelocity;
    private float speed;

    private void OnEnable()
    {
        mr.material.SetFloat("_seed", Random.Range(int.MinValue, int.MaxValue));
        int x = Random.Range(0, 360);
        int y = Random.Range(0, 360);
        int z = Random.Range(0, 360);

        angularVelocity = new Vector3(x, y, z);

        speed = Random.Range(0.01f, .5f);

    }

    private void LateUpdate()
    {
           
 
        transform.Rotate(angularVelocity * Time.deltaTime * speed, Space.Self);
    }
}



