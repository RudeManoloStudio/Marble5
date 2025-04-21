using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuinteController : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;
    [SerializeField] float impulseStrength = 10f;

    private Vector3 repulsionPoint;

    private void OnEnable()
    {

        EventManager.AddListener("DropBilles", _OnDropQuintes);
        EventManager.AddListener("ExplodeBilles", _OnExplodeBilles);

    }

    private void _OnDropQuintes()
    {

        rb.isKinematic = false;
        
    }

    private void _OnExplodeBilles()
    {

        repulsionPoint = new Vector3(GameManager.Instance.GridSize.x / 2, GameManager.Instance.GridSize.y / 2, 0);

        rb.isKinematic = false;
        rb.useGravity = false;

        Vector3 direction = rb.position - repulsionPoint;
        float distance = direction.magnitude;

        float strength = impulseStrength / Mathf.Max(distance, 0.1f);
        Vector3 impulse = direction.normalized * strength;

        rb.AddForce(impulse, ForceMode.Impulse);
    }
}
