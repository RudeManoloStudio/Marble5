using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaController : BilleController
{

    public override void SetShaderParameters()
    {
        mr.material.SetFloat("_seed", Random.Range(0, 200));
    }
}



