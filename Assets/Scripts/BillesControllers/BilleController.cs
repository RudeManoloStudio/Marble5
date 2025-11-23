using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilleController : MonoBehaviour
{

    [SerializeField] protected MeshRenderer mr;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minRotationSpeed = 0.09f;
    [SerializeField] private float maxRotationSpeed = 0.2f;
    [SerializeField] float impulseStrength = 10f;

    private Vector3 repulsionPoint;
    private Vector3 angularVelocity;
    private float speed;
    private bool rotate = true;
    private Vector2Int gridSize;

    private void OnEnable()
    {

        int x = Random.Range(-180, 180);
        int y = Random.Range(-180, 180);
        int z = Random.Range(-180, 180);

        angularVelocity = new Vector3(x, y, z);

        speed = Random.Range(minRotationSpeed, maxRotationSpeed);

        SetSpecificParameters();

        EventManager.AddListener("DropBilles", _OnDropBilles);
        EventManager.AddListener("ExplodeBilles", _OnExplodeBilles);

    }

        public void SetGridSize(Vector2Int size)
    {
        gridSize = size;
    }

    public virtual void SetSpecificParameters()
    {}

    public void DoRotate(bool rotate)
    {
        this.rotate = rotate;
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
        DoRotate(true);
        rb.isKinematic = false;
    }

    private void _OnExplodeBilles()
    {

        repulsionPoint = new Vector3(gridSize.x / 2, gridSize.y / 2, 0);

        rb.isKinematic = false;
        rb.useGravity = false;

        Vector3 direction = rb.position - repulsionPoint;
        float distance = direction.magnitude;

        // Option : plus proche = plus de force
        float strength = impulseStrength / Mathf.Max(distance, 0.1f);
        Vector3 impulse = direction.normalized * strength;

        rb.AddForce(impulse, ForceMode.Impulse);
    }
}
