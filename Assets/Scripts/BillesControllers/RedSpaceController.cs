using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpaceController : BilleController
{

    public override void SetShaderParameters()
    {
        int x = Random.Range(0, 360);
        int y = Random.Range(0, 360);
        int z = Random.Range(0, 360);

        Vector3 angularPosition = new Vector3(x, y, z);

        mr.material.SetFloat("_seed", Random.Range(0, 200));
        mr.gameObject.transform.localRotation = Quaternion.Euler(angularPosition);
    }
}



