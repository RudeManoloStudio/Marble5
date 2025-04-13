using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveController : MonoBehaviour
{

    [SerializeField] private Transform reserveUI;
    [SerializeField] private Transform container;
    [SerializeField] private Camera reserveCam;

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
        this.plomb = plomb;

    }

    public void AddBille()
    {
        GameObject newBille = Instantiate(bille, container);
        newBille.transform.localScale = new Vector3(1, 1, 1);

        newBille.transform.SetLocalPositionAndRotation(new Vector3(0, list.Count * -1, 0), Quaternion.identity);
        
        BilleController bc = newBille.GetComponent<BilleController>();
        bc.DoRotate(false);
        bc.SetSpecificParameters();

        list.Add(newBille);

        AdjustCameraToContainer();
        AdjustReserveUIHeight(list.Count);
    }

    public void RemoveBille()
    {
        
        list.RemoveAt(0);
        Destroy(container.GetChild(0).gameObject);
        
        
        foreach (GameObject billeToMove in list)
        {
            billeToMove.transform.Translate(new Vector3(0, 1, 0), Space.World);
        }
        
        

        AdjustCameraToContainer();
        AdjustReserveUIHeight(list.Count);
    }

    public void AddPlomb()
    {
        GameObject newPlomb = Instantiate(plomb, container);
        newPlomb.transform.localScale = new Vector3(1, 1, 1);

        newPlomb.transform.SetLocalPositionAndRotation(new Vector3(0, list.Count * -1, 0), Quaternion.identity);

        BilleController bc = newPlomb.GetComponent<BilleController>();
        bc.DoRotate(false);
        bc.SetSpecificParameters();

        list.Add(newPlomb);

        AdjustCameraToContainer();
        AdjustReserveUIHeight(list.Count);
    }

    /*
    public void RemovePlomb()
    {

        list.RemoveAt(0);
        Destroy(container.GetChild(0).gameObject);

        foreach (GameObject billeToMove in list)
        {
            billeToMove.transform.Translate(new Vector3(0, -1, 0), Space.World);
        }


        AdjustCameraToContainer();
        AdjustReserveUIHeight(list.Count);
    }
    */

    void AdjustReserveUIHeight(int nbBilles)
    {

        reserveUIRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, reserveUIRectTransform.rect.width * nbBilles);

    }
    void AdjustCameraToContainer()
    {

        float nbBilles = list.Count;
        float objectHeight = (float)nbBilles;
        float cameraHeight = objectHeight / 2.0f;

        reserveCam.orthographicSize = cameraHeight;
        reserveCam.aspect = 1f / (float)nbBilles;

        reserveCam.transform.SetLocalPositionAndRotation(new Vector3(0, -(nbBilles / 2f - 0.5f), 0), Quaternion.identity);

    }
}
