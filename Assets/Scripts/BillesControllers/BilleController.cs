using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilleController : MonoBehaviour
{

    [SerializeField] protected MeshRenderer mr;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minRotationSpeed = 0.09f;
    [SerializeField] private float maxRotationSpeed = 0.2f;

    private Vector3 angularVelocity;
    private float speed;
    protected bool rotate = true;

    private void OnEnable()
    {

        int x = Random.Range(-180, 180);
        int y = Random.Range(-180, 180);
        int z = Random.Range(-180, 180);

        angularVelocity = new Vector3(x, y, z);

        speed = Random.Range(minRotationSpeed, maxRotationSpeed);

        SetSpecificParameters();

        EventManager.AddListener("DropBilles", _OnDropBilles);

    }

    public virtual void SetSpecificParameters()
    {}

    /*
    public void ActiveGravity()
    {
        rb.isKinematic = false;
    }
    */

    public void DoRotate(bool flagRotate)
    {
        rotate = flagRotate;
    }

    private void LateUpdate()
    {

        if (rotate)
        {
            transform.Rotate(angularVelocity * Time.deltaTime * speed, Space.Self);
        }

    }

    private void _OnDropBilles()
    {
        rb.isKinematic = false;
    }
}
