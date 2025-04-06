using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuinteController : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    private void OnEnable()
    {

        EventManager.AddListener("DropBilles", _OnDropQuintes);

    }

    private void _OnDropQuintes()
    {

        StartCoroutine("WaitForBilles");

        
    }

    private IEnumerator WaitForBilles()
    {

  
        yield return new WaitForSeconds(2f);

        rb.isKinematic = false;

    }
}
