using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementPlomb : MonoBehaviour
{
    private GameObject plomb;
    [SerializeField] private Transform containerPlomb;

    // Start is called before the first frame update
    void Start()
    {
       plomb=GameManager.Instance.PlombPrefab;

        EventManager.AddListener("PosePlomb", _OnPosePlomb);
    }

    void _OnPosePlomb(object data)
    {
        Vector3 positionDerniereBille = (Vector3)data;

        // Vérification position droite
        Vector3 positionATester = positionDerniereBille + Vector3.right;

                        
        // Vérifie s'il y a une bille dans un petit rayon autour de la position
        Collider[] colliders = Physics.OverlapSphere(positionATester, 0.1f);

        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag("Bille"))
            {
                return;
            }
        }

        GameObject nouveauPlomb = Instantiate(plomb, positionATester, Quaternion.identity);
        nouveauPlomb.transform.SetParent(containerPlomb);
        Debug.Log("Plomb placée en : " + positionATester);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
