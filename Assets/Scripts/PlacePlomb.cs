using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacePlomb : MonoBehaviour
{

    [SerializeField] private Transform container;
    private GameObject plomb;

    void Start()
    {
        //plomb = GameManager.Instance.Plomb;
        //container = GameManager.Instance.Container;

        //EventManager.AddListener("PosePlomb", _OnPosePlomb);
    }

    public void Setup(Vector2Int gridSize, GameObject plomb)
    {

        //this.gridSize = gridSize;
        this.plomb = plomb;

    }

    public void PlacePlombAt(Vector3 position)
    {

        // Récupération de la position de la dernière bille
        Vector3 positionDerniereBille = position;
        Debug.Log("Dernière bille placée en : " + positionDerniereBille);

        // --- 1) Recherche au niveau 1 ---
        List<Vector3> positionsLibres = GetFreePositions(positionDerniereBille, directions);

        // --- 2) Si aucune position libre au niveau 1, on tente le niveau 2 ---
        if (positionsLibres.Count == 0)
        {
            Debug.LogWarning("Aucune position libre autour de la bille (niveau 1). Recherche au niveau 2...");
            positionsLibres = GetFreePositions(positionDerniereBille, directionsLevel2);

            // Si toujours aucune position, on abandonne
            if (positionsLibres.Count == 0)
            {
                Debug.LogWarning("Aucune position libre autour de la bille (niveau 2) non plus !");
                return;
            }
        }

        // On a au moins une position disponible
        Debug.Log("Nombre d'emplacements libres : " + positionsLibres.Count);

        // On choisit une position aléatoire
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

    // Tableau des offsets autour d'une position (niveau 1) : 8 directions
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

    // Tableau des offsets pour le niveau 2 (14 directions)
    private static readonly Vector3[] directionsLevel2 = new Vector3[]
    {
        new Vector3(0,  2, 0),
        new Vector3(0, -2, 0),

        new Vector3(2,  1, 0),
        new Vector3(2, -1, 0),
        new Vector3(-2, 1, 0),
        new Vector3(-2, -1, 0),

        new Vector3(1,  2, 0),
        new Vector3(-1,  2, 0),
        new Vector3(1, -2, 0),
        new Vector3(-1, -2, 0),

        new Vector3(2,  2, 0),
        new Vector3(-2, 2, 0),
        new Vector3(2, -2, 0),
        new Vector3(-2, -2, 0)
    };

    // Cette fonction récupère la position de la bille et essaie de placer un plomb
    // autour de la bille, d'abord au niveau 1, sinon au niveau 2.

    /*
    void _OnPosePlomb(object data)
    {
        // Récupération de la position de la dernière bille
        Vector3 positionDerniereBille = (Vector3)data;
        Debug.Log("Dernière bille placée en : " + positionDerniereBille);

        // --- 1) Recherche au niveau 1 ---
        List<Vector3> positionsLibres = GetFreePositions(positionDerniereBille, directions);

        // --- 2) Si aucune position libre au niveau 1, on tente le niveau 2 ---
        if (positionsLibres.Count == 0)
        {
            Debug.LogWarning("Aucune position libre autour de la bille (niveau 1). Recherche au niveau 2...");
            positionsLibres = GetFreePositions(positionDerniereBille, directionsLevel2);

            // Si toujours aucune position, on abandonne
            if (positionsLibres.Count == 0)
            {
                Debug.LogWarning("Aucune position libre autour de la bille (niveau 2) non plus !");
                return;
            }
        }

        // On a au moins une position disponible
        Debug.Log("Nombre d'emplacements libres : " + positionsLibres.Count);

        // On choisit une position aléatoire
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
    */

    // Fonction utilitaire pour récupérer la liste des emplacements libres
    // en fonction d'une position de base et d'un tableau de directions.
    private List<Vector3> GetFreePositions(Vector3 basePosition, Vector3[] dirs)
    {
        List<Vector3> positionsLibres = new List<Vector3>();

        foreach (Vector3 dir in dirs)
        {
            Vector3 positionATester = basePosition + dir;

            // Vérification de la présence d'une bille ou d'un plomb
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

            // Ajout si libre
            if (EmplacementLibre)
            {
                positionsLibres.Add(positionATester);
            }
        }

        return positionsLibres;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
