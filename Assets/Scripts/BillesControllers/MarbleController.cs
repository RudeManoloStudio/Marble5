using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleController : BilleController
{

    public override void SetShaderParameters()
    {
        mr.material.SetFloat("_scale", Random.Range(50, 150));
    }
}



