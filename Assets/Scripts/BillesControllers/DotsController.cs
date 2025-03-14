using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsController : BilleController
{

    private override void SetShaderParameters()
    {
        mr.material.SetFloat("_size", Random.Range(0.09f, 0.9f));
    }

}



