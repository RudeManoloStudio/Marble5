using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveCameraController : MonoBehaviour
{

    private Camera rc;

    [SerializeField] private float aspectRatio;

    private void Start()
    {
        rc = transform.GetComponent<Camera>();

        AdjustCameraToObject();

        //Debug.Log("before : " + rc.aspect);
        //Debug.Log(rc.orthographicSize);

        //rc.aspect = 0.5f;

        //Debug.Log("after : " + rc.aspect);
    }

    void AdjustCameraToObject()
    {

        // pour l'instant il y a 5 billes
        int nbBilles = 6;

        // Obtenir les limites de l'objet
        //Bounds bounds = targetObject.GetComponent<Renderer>().bounds;

        // Calculer la taille nécessaire pour la caméra
        //float objectWidth = bounds.size.x + padding;
        float objectWidth = 1f; // a priori toujours 1
        //float objectHeight = bounds.size.y + padding;
        float objectHeight = (float)nbBilles;

        // Calculer l'orthographicSize nécessaire pour cadrer l'objet
        //float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = objectHeight / 2.0f;
        float cameraWidth = objectWidth / 2.0f;

        //orthographicCamera.orthographicSize = Mathf.Max(cameraHeight, cameraWidth / screenAspect);
        rc.orthographicSize = cameraHeight;
        rc.aspect = 1f / (float)nbBilles;

       
    }

    private void Update()
    {
        //rc.aspect = aspectRatio;
    }

}
