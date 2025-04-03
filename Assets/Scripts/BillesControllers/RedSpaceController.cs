using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpaceController : BilleController
{

    public override void SetSpecificParameters()
    {

        int x = Random.Range(-180, 180);
        int y = Random.Range(-180, 180);
        int z = Random.Range(-180, 180);

        Vector3 angularPosition = new Vector3(x, y, z);

        mr.gameObject.transform.localRotation = Quaternion.Euler(angularPosition);

    }
}



