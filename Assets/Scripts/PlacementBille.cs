using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlacementBille : MonoBehaviour
{

    [SerializeField] LayerMask billesLayer;
    [SerializeField] private Material ligneMat; // Matériau de la ligne
    private HashSet<(Vector3, Vector3)> liaisonsUtilisées = new HashSet<(Vector3, Vector3)>();
    private bool verificationEffectuee = false;

    private GameObject billePrefab;
    private bool gameOver = false;

    private void Start()
    {
        billePrefab = GameManager.Instance.BillePrefab;
        EventManager.AddListener("GameOver", _OnGameOver);
    }

    private void _OnGameOver()
    {
        gameOver = true;
    }

    void Update()
    {
        if (gameOver) { return; }

        if (Input.GetMouseButtonDown(0) && !verificationEffectuee)
        {
            verificationEffectuee = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"🎯 Raycast touche : {hit.collider.gameObject.name} à {hit.point}");

                if (hit.collider.gameObject.tag != "Bille") // Vérifie si l'emplacement est libre
                {
                    Vector3 nouvellePosition = new Vector3(
                        Mathf.FloorToInt(hit.point.x + 0.5f),
                        Mathf.FloorToInt(hit.point.y + 0.5f),
                        0.0f  // Fixe Z au bon niveau
                    );

                    GameObject nouvelleBille = Instantiate(billePrefab, nouvellePosition, Quaternion.identity);
                    nouvelleBille.transform.SetParent(this.transform);
                    Debug.Log("✅ Bille placée en : " + nouvellePosition);

                    EventManager.TriggerEvent("PoseBille");

                    // 📌 Vérification des quintes dans toutes les directions
                    bool quinteTrouvee = VerifierToutesLesQuintes(nouvellePosition);

                    if (quinteTrouvee)
                    {
                        Debug.Log("🎯 Une quinte a été détectée !");
                        //EventManager.TriggerEvent("QuinteFormee");
                    }
                }
            }
        }

        verificationEffectuee = false;
    }


    bool VerifierToutesLesQuintes(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        bool quinteTrouvee = false;

        // 1️⃣ Vérification des quintes horizontales
        for (int i = -4; i <= 0; i++)
        {
            var quinte = new List<Vector3>
        {
            new Vector3(x + i, y, 0),
            new Vector3(x + i + 1, y, 0),
            new Vector3(x + i + 2, y, 0),
            new Vector3(x + i + 3, y, 0),
            new Vector3(x + i + 4, y, 0)
        };

            if (VerifierQuinte(quinte[0], quinte[1], quinte[2], quinte[3], quinte[4]))
            {
                Debug.Log($"🎯 Quinte horizontale trouvée à partir de ({x + i}, {y})");
                TracerLigneQuinte(quinte); // 🔵 Tracé de la ligne
                quinteTrouvee = true;

                EventManager.TriggerEvent("UpdateScore");
            }
        }

        // 2️⃣ Vérification des quintes verticales
        for (int i = -4; i <= 0; i++)
        {
            var quinte = new List<Vector3>
        {
            new Vector3(x, y + i, 0),
            new Vector3(x, y + i + 1, 0),
            new Vector3(x, y + i + 2, 0),
            new Vector3(x, y + i + 3, 0),
            new Vector3(x, y + i + 4, 0)
        };

            if (VerifierQuinte(quinte[0], quinte[1], quinte[2], quinte[3], quinte[4]))
            {
                Debug.Log($"🎯 Quinte verticale trouvée à partir de ({x}, {y + i})");
                TracerLigneQuinte(quinte); // 🔵 Tracé de la ligne
                quinteTrouvee = true;

                EventManager.TriggerEvent("UpdateScore");
            }
        }

        // 3️⃣ Vérification des quintes diagonales ↘ (haut-gauche vers bas-droite)
        for (int i = -4; i <= 0; i++)
        {
            var quinte = new List<Vector3>
        {
            new Vector3(x + i, y + i, 0),
            new Vector3(x + i + 1, y + i + 1, 0),
            new Vector3(x + i + 2, y + i + 2, 0),
            new Vector3(x + i + 3, y + i + 3, 0),
            new Vector3(x + i + 4, y + i + 4, 0)
        };

            if (VerifierQuinte(quinte[0], quinte[1], quinte[2], quinte[3], quinte[4]))
            {
                Debug.Log($"🎯 Quinte diagonale ↘ trouvée à partir de ({x + i}, {y + i})");
                TracerLigneQuinte(quinte); // 🔵 Tracé de la ligne
                quinteTrouvee = true;

                EventManager.TriggerEvent("UpdateScore");
            }
        }

        // 4️⃣ Vérification des quintes diagonales ↙ (haut-droit vers bas-gauche)
        for (int i = -4; i <= 0; i++)
        {
            var quinte = new List<Vector3>
        {
            new Vector3(x + i, y - i, 0),
            new Vector3(x + i + 1, y - i - 1, 0),
            new Vector3(x + i + 2, y - i - 2, 0),
            new Vector3(x + i + 3, y - i - 3, 0),
            new Vector3(x + i + 4, y - i - 4, 0)
        };

            if (VerifierQuinte(quinte[0], quinte[1], quinte[2], quinte[3], quinte[4]))
            {
                Debug.Log($"🎯 Quinte diagonale ↙ trouvée à partir de ({x + i}, {y - i})");
                TracerLigneQuinte(quinte); // 🔵 Tracé de la ligne
                quinteTrouvee = true;

                EventManager.TriggerEvent("UpdateScore");
            }
        }

        return quinteTrouvee;
    }





    bool PositionContientBille(Vector3 position)
    {
        // Vérifie s'il y a une bille dans un petit rayon autour de la position
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);

        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag("Bille"))
            {
                return true;
            }
        }
        return false;
    }

    bool VerifierQuinte(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5)
    {
        // 1️⃣ Vérifier que chaque position contient bien une bille
        if (!PositionContientBille(p1) || !PositionContientBille(p2) ||
            !PositionContientBille(p3) || !PositionContientBille(p4) ||
            !PositionContientBille(p5))
        {
            return false; // Une des positions est vide
        }

        // 2️⃣ Vérifier que les liaisons entre les billes ne sont pas déjà utilisées
        var liaisons = new List<(Vector3, Vector3)>
    {
        (p1, p2), (p2, p3), (p3, p4), (p4, p5)
    };

        foreach (var liaison in liaisons)
        {
            var liaisonOrdonnee = liaison.Item1.x < liaison.Item2.x ? liaison : (liaison.Item2, liaison.Item1); // Toujours dans le même ordre
            if (liaisonsUtilisées.Contains(liaisonOrdonnee))
            {
                return false; // La quinte est invalide car une liaison est déjà utilisée
            }
        }

        return true; // Toutes les conditions sont remplies, la quinte est valide
    }



    void TracerLigneQuinte(List<Vector3> positions)
    {
        Debug.Log("🟢 Entrée dans TracerLigneQuinte()");

        GameObject nouvelleLigne = new GameObject("LigneQuinte");

        LineRenderer lr = nouvelleLigne.AddComponent<LineRenderer>();
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.useWorldSpace = true;

        lr.material = ligneMat;

        for (int i = 0; i < positions.Count - 1; i++)
        {
            var liaison = positions[i].x < positions[i + 1].x ? (positions[i], positions[i + 1]) : (positions[i + 1], positions[i]);
            liaisonsUtilisées.Add(liaison);
        }

        Debug.Log("📌 Liaisons mises à jour !");
        nouvelleLigne.transform.parent = transform;
    }
}