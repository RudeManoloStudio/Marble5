using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementPlomb : MonoBehaviour
{

    private GameObject plomb;
    private Transform container;

    void Start()
    {

        plomb = GameManager.Instance.PlombPrefab;
        container = GameManager.Instance.Container;

        EventManager.AddListener("PosePlomb", _OnPosePlomb);

    }

    // Tableau des offsets autour d'une position : 8 directions
    private static readonly Vector3[] directions = new Vector3[]
{
    new Vector3(1,  0, 0),  // Droite
    new Vector3(-1, 0, 0),  // Gauche
    new Vector3(0,  1, 0),  // Avant (y+)
    new Vector3(0, -1, 0),  // Arrière (y-)
    new Vector3(1,  1, 0),  // Diagonale avant-droite
    new Vector3(-1, 1, 0),  // Diagonale avant-gauche
    new Vector3(1, -1, 0),  // Diagonale arrière-droite
    new Vector3(-1, -1, 0)  // Diagonale arrière-gauche
};


    // Cette fonction récupère la position de la bille et essaie de placer un plomb
    // autour de la bille, dans une position aléatoire qui est libre.
    void _OnPosePlomb(object data)
    {
        // Récupération de la position de la dernière bille
        Vector3 positionDerniereBille = (Vector3)data;

        Debug.Log("Dernière bille placée en : " + positionDerniereBille);

        // Liste pour stocker les positions libres
        List<Vector3> positionsLibres = new List<Vector3>();

        // On teste toutes les directions
        foreach (Vector3 dir in directions)
        {
            Vector3 positionATester = positionDerniereBille + dir;

            // Vérification de la présence d'une bille via un OverlapSphere
            Collider[] colliders = Physics.OverlapSphere(positionATester, 0.1f);

            bool EmplacementLibre = true;
            foreach (Collider col in colliders)
            {
                if (col.gameObject.CompareTag("Bille") || col.gameObject.CompareTag("Plomb"))
                {
                    EmplacementLibre = false;
                    break;
                }
            }

            // Si aucune bille n'a été trouvée, on ajoute cette position à la liste
            if (EmplacementLibre)
            {
                positionsLibres.Add(positionATester);
            }
        }

        // Si aucune position libre, on sort (rien à faire)
        if (positionsLibres.Count == 0)
        {
            Debug.LogWarning("Aucune position libre autour de la bille !");
            return;
        }

        Debug.Log("Nombre d'emplacements libres : " + positionsLibres.Count);


        // On choisit une position aléatoire parmi les positions libres
        Vector3 positionChoisie = positionsLibres[Random.Range(0, positionsLibres.Count)];

        // On instancie le plomb à la position choisie
        GameObject nouveauPlomb = Instantiate(plomb, positionChoisie, Quaternion.identity);
        nouveauPlomb.transform.SetParent(container);
        nouveauPlomb.tag = "Plomb";

        // Gestion du composant de la bille (rotation éventuelle, etc.)
        BilleController bc = nouveauPlomb.GetComponent<BilleController>();
        bc.DoRotate(false);

        Debug.Log("Plomb placée en : " + positionChoisie);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
