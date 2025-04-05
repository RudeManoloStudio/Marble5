using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveController : MonoBehaviour
{

    [SerializeField] private Transform reserveUI;
    [SerializeField] private Transform container;
    [SerializeField] private Camera reserveCam;

    //private Camera reserveCam;
    private GameObject bille;
    private GameObject plomb;

    private List<GameObject> list = new List<GameObject>();

    private RectTransform reserveUIRectTransform;

    private void Start()
    {
        reserveUIRectTransform = reserveUI.GetComponent<RectTransform>();
    }

    public void SetBilleAndPlomb(GameObject bille, GameObject plomb)
    {
        this.bille = bille;
        this.bille.transform.localScale = new Vector3(1, 1, 1);

        this.plomb = plomb;
        this.plomb.transform.localScale = new Vector3(1, 1, 1);
    }

    public void AddBille()
    {
        GameObject newBille = Instantiate(bille, container);
        newBille.transform.SetLocalPositionAndRotation(new Vector3(0, list.Count, 0), Quaternion.identity);
        BilleController bc = newBille.GetComponent<BilleController>();
        bc.DoRotate(false);

        list.Add(bille);

        AdjustCameraToContainer(list.Count);
        AdjustReserveUIHeight(list.Count);
    }

    void AdjustReserveUIHeight(int nbBilles)
    {
        Debug.Log(nbBilles);
        reserveUIRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, reserveUIRectTransform.rect.width * nbBilles);
        //reserveUIRectTransform.sizeDelta = new Vector2(reserveUIRectTransform.rect.x, reserveUIRectTransform.rect.x * nbBilles);
    }
    void AdjustCameraToContainer(int nbBilles)
    {

        float objectWidth = 1f; // a priori toujours 1
        float objectHeight = (float)nbBilles;

        // Calculer l'orthographicSize nécessaire pour cadrer l'objet
        //float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = objectHeight / 2.0f;
        float cameraWidth = objectWidth / 2.0f;

        //orthographicCamera.orthographicSize = Mathf.Max(cameraHeight, cameraWidth / screenAspect);
        reserveCam.orthographicSize = cameraHeight;
        reserveCam.aspect = 1f / (float)nbBilles;

       
    }
}
