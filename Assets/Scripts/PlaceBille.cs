using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.VisualScripting;

public class PlaceBille : MonoBehaviour
{
    //
    [SerializeField] private Transform container;

    private HashSet<(Vector3, Vector3)> liaisonsUtilisées = new HashSet<(Vector3, Vector3)>();
    private bool verificationEffectuee = false;
    private GameObject bille;
    private GameObject quinte;
    private bool pause = false;
    private Vector2Int gridSize;

    private static readonly Vector3[] adjacentDirections = new Vector3[]
   {
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(1, 1, 0),
        new Vector3(-1, 1, 0),
        new Vector3(1, -1, 0),
        new Vector3(-1, -1, 0)
   };

    private void Start()
    {
        EventManager.AddListener("GameOver", _OnPause);
    }

    public void Pause()
    {
        pause = true;
    }

    public void Unpause()
    {
        pause = false;
    }

    public void Setup(Vector2Int gridSize, GameObject bille, GameObject quinte)
    {

        this.gridSize = gridSize;
        this.bille = bille;
        this.quinte = quinte;

        ResetLiaisons();
    }

    public void ResetLiaisons()
    {
        liaisonsUtilisées.Clear();
    }
    public void Replay()
    {
        
        liaisonsUtilisées.Clear();
        pause = false;
        
    }

    private void _OnPause()
    {
        pause = true;
    }

    void Update()
    {
        if (pause) { return; }

        bool interaction = false;
        Vector3 interactionPosition = Vector3.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            interaction = true;
            interactionPosition = Input.mousePosition;
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                interaction = true;
                interactionPosition = Input.GetTouch(0).position;
            }
        }
#endif

        if (interaction && !verificationEffectuee)
        {
            verificationEffectuee = true;

            Ray ray = Camera.main.ScreenPointToRay(interactionPosition);
            RaycastHit hit;

            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag != "Bille" && hit.collider.gameObject.tag != "Plomb")
                {
                    Vector3 nouvellePosition = new Vector3(
                        Mathf.FloorToInt(hit.point.x + 0.5f),
                        Mathf.FloorToInt(hit.point.y + 0.5f),
                        0.0f
                    );

                    if (nouvellePosition.x < 0 || nouvellePosition.x > gridSize.x || nouvellePosition.y < 0 || nouvellePosition.y > gridSize.y || EventSystem.current.IsPointerOverGameObject())
                    {
                        EventManager.TriggerEvent("NoPoseBille");
                        return;
                    }

                    float rayonDeCollision = 0.4f;
                    Collider[] collisions = Physics.OverlapSphere(nouvellePosition, rayonDeCollision);
                    bool estOccupe = collisions.Any(col => col.CompareTag("Bille") || col.CompareTag("Plomb"));

                    if (estOccupe)
                    {
                        EventManager.TriggerEvent("NoPoseBille");
                    }
                    else
                    {
                        if (nouvellePosition.x < 0 || nouvellePosition.x > gridSize.x || nouvellePosition.y < 0 || nouvellePosition.y > gridSize.y)
                        {
                            EventManager.TriggerEvent("NoPoseBille");
                            verificationEffectuee = false;
                            return;
                        }

                        bool auMoinsUnVoisin = false;
                        foreach (Vector3 dir in adjacentDirections)
                        {
                            Vector3 voisin = nouvellePosition + dir;
                            Collider[] voisins = Physics.OverlapSphere(voisin, 0.4f);
                            if (voisins.Any(col => col.CompareTag("Bille") || col.CompareTag("Plomb")))
                            {
                                auMoinsUnVoisin = true;
                                break;
                            }
                        }

                        if (!auMoinsUnVoisin)
                        {
                            EventManager.TriggerEvent("NoPoseBille");
                            verificationEffectuee = false;
                            return;
                        }

                        GameObject nouvelleBille = Instantiate(bille, nouvellePosition, Quaternion.identity);
                        nouvelleBille.transform.SetParent(container);
                        nouvelleBille.tag = "Bille";

                        int quinteTrouvees = VerifierToutesLesQuintes(nouvellePosition);
                        if (quinteTrouvees > 0)
                        {
                            // Ancienne méthode avant refacto : GameManager.Instance.UpdateScoreAndCoins(quinteTrouvees);
                            EventManager.TriggerEvent("QuinteFormee", quinteTrouvees);
                        }

                        EventManager.TriggerEvent("PoseBille", nouvellePosition);
                    }
                }
                else
                {
                    EventManager.TriggerEvent("NoPoseBille");
                }
            }
            else
            {
                EventManager.TriggerEvent("NoPoseBille");
            }
        }
    


    verificationEffectuee = false;

    }


    // ============================================================
    // MÉTHODE REFACTORISÉE - VerifierToutesLesQuintes
    // ============================================================

    // Les 4 directions possibles pour former une quinte
    private static readonly Vector3[] quinteDirections = new Vector3[]
    {
    new Vector3(1, 0, 0),   // Horizontal →
    new Vector3(0, 1, 0),   // Vertical ↑
    new Vector3(1, 1, 0),   // Diagonale ↗
    new Vector3(1, -1, 0)   // Diagonale ↘
    };

    int VerifierToutesLesQuintes(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        int quintesTrouvees = 0;

        // Pour chaque direction possible
        foreach (Vector3 dir in quinteDirections)
        {
            // On vérifie d'abord la double quinte (la bille est au centre de 2 quintes)
            if (VerifierDoubleQuinte(x, y, dir))
            {
                quintesTrouvees += 2;
            }
            else
            {
                // Sinon on vérifie les quintes simples
                // La bille posée peut être en position 1, 2, 3, 4 ou 5 de la quinte
                for (int pos = 0; pos < 5; pos++)
                {
                    List<Vector3> quinte = GenererQuinte(x, y, dir, pos);
                    if (VerifierQuinte(quinte[0], quinte[1], quinte[2], quinte[3], quinte[4]))
                    {
                        TracerLigneQuinte(quinte);
                        quintesTrouvees++;
                        break; // Une seule quinte par direction (évite les doublons)
                    }
                }
            }
        }

        return quintesTrouvees;
    }

    /// <summary>
    /// Vérifie si la position (x,y) est le centre d'une double quinte dans la direction donnée
    /// </summary>
    private bool VerifierDoubleQuinte(int x, int y, Vector3 dir)
    {
        // Quinte "avant" : de -4 à 0 par rapport à la position
        List<Vector3> quinteAvant = new List<Vector3>();
        for (int i = -4; i <= 0; i++)
        {
            quinteAvant.Add(new Vector3(x + dir.x * i, y + dir.y * i, 0));
        }

        // Quinte "après" : de 0 à +4 par rapport à la position
        List<Vector3> quinteApres = new List<Vector3>();
        for (int i = 0; i <= 4; i++)
        {
            quinteApres.Add(new Vector3(x + dir.x * i, y + dir.y * i, 0));
        }

        // Les deux quintes doivent être valides pour une double quinte
        bool avantValide = VerifierQuinte(quinteAvant[0], quinteAvant[1], quinteAvant[2], quinteAvant[3], quinteAvant[4]);
        bool apresValide = VerifierQuinte(quinteApres[0], quinteApres[1], quinteApres[2], quinteApres[3], quinteApres[4]);

        if (avantValide && apresValide)
        {
            TracerLigneQuinte(quinteAvant);
            TracerLigneQuinte(quinteApres);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Génère les 5 positions d'une quinte où la bille posée est à la position 'positionDansBille' (0 à 4)
    /// </summary>
    private List<Vector3> GenererQuinte(int x, int y, Vector3 dir, int positionDansQuinte)
    {
        List<Vector3> quinte = new List<Vector3>();

        // La bille posée est à l'index 'positionDansQuinte'
        // Donc on commence à (positionDansQuinte) cases "avant" dans la direction opposée
        int startOffset = -positionDansQuinte;

        for (int i = 0; i < 5; i++)
        {
            int offset = startOffset + i;
            quinte.Add(new Vector3(x + dir.x * offset, y + dir.y * offset, 0));
        }

        return quinte;
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
        //Debug.Log("🟢 Entrée dans TracerLigneQuinte()");

        /* abandon du lr
        GameObject nouvelleLigne = new GameObject("LigneQuinte");

        LineRenderer lr = nouvelleLigne.AddComponent<LineRenderer>();
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.useWorldSpace = true;

        lr.material = ligneMat;
        fin abandon du lr */

        GameObject newQuinte = Instantiate(quinte, positions[2], Quaternion.identity);
        newQuinte.transform.SetParent(container);

        // calcul de l'orientation de la quinte
        Vector3 direction = positions[1] - positions[0];

        // Calculer la rotation nécessaire pour orienter le cylindre
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        newQuinte.transform.rotation = rotation * Quaternion.Euler(90, 0, 0);

        //pour ensuite etirer
        float distance = Vector3.Distance(positions[0], positions[4]);
        newQuinte.transform.localScale = new Vector3(newQuinte.transform.localScale.x, distance / 2, newQuinte.transform.localScale.z);

        //stop rotation
        for (int i = 0; i < positions.Count; i++)
        {

            Collider[] colliders = Physics.OverlapSphere(positions[i], 0.1f);

            foreach (Collider col in colliders)
            {
                if (col.gameObject.CompareTag("Bille"))
                {
                    BilleController bc = col.gameObject.GetComponent<BilleController>();
                    bc.DoRotate(false); // = false;
                }
            }
        }

        for (int i = 0; i < positions.Count - 1; i++)
        {

            var liaison = positions[i].x < positions[i + 1].x ? (positions[i], positions[i + 1]) : (positions[i + 1], positions[i]);
            liaisonsUtilisées.Add(liaison);

        }

        //Debug.Log("📌 Liaisons mises à jour !");
        // abandon lr nouvelleLigne.transform.SetParent(container);
    }
}