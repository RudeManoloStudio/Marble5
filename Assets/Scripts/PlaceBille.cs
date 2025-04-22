using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

public class PlaceBille : MonoBehaviour
{

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
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
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
                            GameManager.Instance.UpdateScoreAndCoins(quinteTrouvees);
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


    //bool VerifierToutesLesQuintes(Vector3 position)
    int VerifierToutesLesQuintes(Vector3 position)
    {

        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        //bool quinteTrouvee = false;
        int quintestrouvees = 0;

        //**************************************************
        // 1️⃣ Vérification des quintes horizontales
        //**************************************************

        // Recherche de la double quinte horizontale
        var quinteHorizontaleGauche = new List<Vector3> {
        new Vector3(x -4, y, 0),
        new Vector3(x -3, y, 0),
        new Vector3(x -2, y, 0),
        new Vector3(x -1, y, 0),
        new Vector3(x , y, 0)
            };
        var quinteHorizontaleDroite = new List<Vector3> {
        new Vector3(x , y, 0),
        new Vector3(x +1, y, 0),
        new Vector3(x +2, y, 0),
        new Vector3(x +3 , y, 0),
        new Vector3(x +4, y, 0)
            };

        if (VerifierQuinte(quinteHorizontaleGauche[0], quinteHorizontaleGauche[1], quinteHorizontaleGauche[2], quinteHorizontaleGauche[3], quinteHorizontaleGauche[4])
            && VerifierQuinte(quinteHorizontaleDroite[0], quinteHorizontaleDroite[1], quinteHorizontaleDroite[2], quinteHorizontaleDroite[3], quinteHorizontaleDroite[4]))
        {
            //Debug.Log($"🎯 Double quinte horizontale trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteHorizontaleGauche); // 🔵 Tracé de la quinte gauche-droite
            TracerLigneQuinte(quinteHorizontaleDroite); // 🔵 Tracé de la quinte droite-gauche
            //quinteTrouvee = true;
            quintestrouvees = quintestrouvees + 2;

            //EventManager.TriggerEvent("UpdateScore"); // Score quinte gauche-droite
            //EventManager.TriggerEvent("UpdateScore"); // Score quinte droite-gauche

        }

        // Recherche de la quinte horizontale centrale
        var quinteCentraleHorizontale = new List<Vector3> {
        new Vector3(x -2, y, 0),
        new Vector3(x -1, y, 0),
        new Vector3(x , y, 0),
        new Vector3(x +1, y, 0),
        new Vector3(x +2, y, 0)
            };

        if (VerifierQuinte(quinteCentraleHorizontale[0], quinteCentraleHorizontale[1], quinteCentraleHorizontale[2], quinteCentraleHorizontale[3], quinteCentraleHorizontale[4]))
        {
            //Debug.Log($"🎯 Quinte horizontale centrale trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteCentraleHorizontale); // 🔵 Tracé de la quinte centrale horizontale
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); // Score quinte centrale horizontale

        }

        // Recherche de la quinte horizontale avec bille en position 2
        var quinteHorizontalePos2 = new List<Vector3> {
        new Vector3(x -1, y, 0),
        new Vector3(x , y, 0),
        new Vector3(x +1, y, 0),
        new Vector3(x +2, y, 0),
        new Vector3(x +3, y, 0)
            };

        if (VerifierQuinte(quinteHorizontalePos2[0], quinteHorizontalePos2[1], quinteHorizontalePos2[2], quinteHorizontalePos2[3], quinteHorizontalePos2[4]))
        {
            //Debug.Log($"🎯 Quinte horizontale trouvée avec bille en position 2 sur ({x}, {y})");
            TracerLigneQuinte(quinteHorizontalePos2); // 🔵 Tracé de la quinte horizontale position 2
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); // Score quinte position 2

        }

        // Recherche de la quinte avec bille en position 4
        var quinteHorizontalePos4 = new List<Vector3> {
        new Vector3(x -3, y, 0),
        new Vector3(x -2, y, 0),
        new Vector3(x -1, y, 0),
        new Vector3(x , y, 0),
        new Vector3(x +1, y, 0)
            };

        if (VerifierQuinte(quinteHorizontalePos4[0], quinteHorizontalePos4[1], quinteHorizontalePos4[2], quinteHorizontalePos4[3], quinteHorizontalePos4[4]))
        {
            //Debug.Log($"🎯 Quinte horizontale trouvée avec bille en position 4 sur ({x}, {y})");
            TracerLigneQuinte(quinteHorizontalePos4); // 🔵 Tracé de la quinte horizontale position 4
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); // Score quinte position 4

        }

        // Recherche de la quinte avec bille en position 1
        var quinteHorizontalePos1 = new List<Vector3> {
        new Vector3(x , y, 0),
        new Vector3(x +1, y, 0),
        new Vector3(x +2, y, 0),
        new Vector3(x +3, y, 0),
        new Vector3(x +4, y, 0)
            };

        if (VerifierQuinte(quinteHorizontalePos1[0], quinteHorizontalePos1[1], quinteHorizontalePos1[2], quinteHorizontalePos1[3], quinteHorizontalePos1[4]))
        {
            //Debug.Log($"🎯 Quinte horizontale trouvée avec bille en position 1 sur ({x}, {y})");
            TracerLigneQuinte(quinteHorizontalePos1); // 🔵 Tracé de la quinte horizontale position 1
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); // Score quinte position 1

        }

        // Recherche de la quinte avec bille en position 5
        var quinteHorizontalePos5 = new List<Vector3> {
        new Vector3(x-4 , y, 0),
        new Vector3(x-3, y, 0),
        new Vector3(x-2, y, 0),
        new Vector3(x-1, y, 0),
        new Vector3(x, y, 0)
            };

        if (VerifierQuinte(quinteHorizontalePos5[0], quinteHorizontalePos5[1], quinteHorizontalePos5[2], quinteHorizontalePos5[3], quinteHorizontalePos5[4]))
        {
            //Debug.Log($"🎯 Quinte trouvée avec bille en position 5 sur ({x}, {y})");
            TracerLigneQuinte(quinteHorizontalePos5); // 🔵 Tracé de la quinte horizontale position 5
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); // Score quinte position 5

        }


        //**************************************************
        // 2️⃣ Vérification des quintes verticales
        //**************************************************

        // Recherche de la double quinte verticale
        var quinteBas = new List<Vector3> {
        new Vector3(x, y -4, 0),
        new Vector3(x, y -3, 0),
        new Vector3(x, y -2, 0),
        new Vector3(x, y -1, 0),
        new Vector3(x, y, 0)
            };
        var quinteHaut = new List<Vector3> {
        new Vector3(x, y, 0),
        new Vector3(x, y +1, 0),
        new Vector3(x, y +2, 0),
        new Vector3(x, y +3, 0),
        new Vector3(x, y +4, 0)
            };

        if (VerifierQuinte(quinteBas[0], quinteBas[1], quinteBas[2], quinteBas[3], quinteBas[4])
            && VerifierQuinte(quinteHaut[0], quinteHaut[1], quinteHaut[2], quinteHaut[3], quinteHaut[4]))
        {
            //Debug.Log($"🎯 Double quinte verticale trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteBas); // 🔵 Tracé de la quinte bas
            TracerLigneQuinte(quinteHaut); // 🔵 Tracé de la quinte haut
            //quinteTrouvee = true;
            quintestrouvees = quintestrouvees + 2;

            //EventManager.TriggerEvent("UpdateScore"); //Score quinte bas
            //EventManager.TriggerEvent("UpdateScore"); //Score quinte haut

        }

        // Recherche de la quinte verticale centrale
        var quinteCentraleVerticale = new List<Vector3> {
        new Vector3(x, y -2, 0),
        new Vector3(x, y -1, 0),
        new Vector3(x, y, 0),
        new Vector3(x, y +1, 0),
        new Vector3(x, y +2, 0)
            };

        if (VerifierQuinte(quinteCentraleVerticale[0], quinteCentraleVerticale[1], quinteCentraleVerticale[2], quinteCentraleVerticale[3], quinteCentraleVerticale[4]))
        {
            //Debug.Log($"🎯 Quinte verticale centrale trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteCentraleVerticale); // 🔵 Tracé de la quinte centrale
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); //Score quinte centrale

        }

        // Recherche de la quinte verticale avec bille position 2
        var quinteVerticalePos2 = new List<Vector3> {
        new Vector3(x, y -1, 0),
        new Vector3(x, y, 0),
        new Vector3(x, y +1, 0),
        new Vector3(x, y +2, 0),
        new Vector3(x, y +3, 0)
            };

        if (VerifierQuinte(quinteVerticalePos2[0], quinteVerticalePos2[1], quinteVerticalePos2[2], quinteVerticalePos2[3], quinteVerticalePos2[4]))
        {
            //Debug.Log($"🎯 Quinte verticale trouvée avec bille en position 2 sur ({x}, {y})");
            TracerLigneQuinte(quinteVerticalePos2); // 🔵 Tracé de la quinte en position 2
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); //Score quinte position 2

        }

        // Recherche de la quinte avec bille position 4
        var quinteVerticalePos4 = new List<Vector3> {
        new Vector3(x, y -3, 0),
        new Vector3(x, y -2, 0),
        new Vector3(x, y -1, 0),
        new Vector3(x, y, 0),
        new Vector3(x, y +1, 0)
            };

        if (VerifierQuinte(quinteVerticalePos4[0], quinteVerticalePos4[1], quinteVerticalePos4[2], quinteVerticalePos4[3], quinteVerticalePos4[4]))
        {
            //Debug.Log($"🎯 Quinte verticale trouvée avec bille en position 4 sur ({x}, {y})");
            TracerLigneQuinte(quinteVerticalePos4); // 🔵 Tracé de la quinte en position 4
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); //Score quinte position 4

        }

        // Recherche de la quinte avec bille position 1
        var quinteVerticalePos1 = new List<Vector3> {
        new Vector3(x, y, 0),
        new Vector3(x, y +1, 0),
        new Vector3(x, y +2, 0),
        new Vector3(x, y +3, 0),
        new Vector3(x, y +4, 0)
            };

        if (VerifierQuinte(quinteVerticalePos1[0], quinteVerticalePos1[1], quinteVerticalePos1[2], quinteVerticalePos1[3], quinteVerticalePos1[4]))
        {
            //Debug.Log($"🎯 Quinte verticale trouvée avec bille en position 1 sur ({x}, {y})");
            TracerLigneQuinte(quinteVerticalePos1); // 🔵 Tracé de la quinte en position 1
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); //Score quinte position 1

        }

        // Recherche de la quinte avec bille position 5
        var quinteVerticalePos5 = new List<Vector3> {
        new Vector3(x, y -4, 0),
        new Vector3(x, y -3, 0),
        new Vector3(x, y -2, 0),
        new Vector3(x, y -1, 0),
        new Vector3(x, y, 0)
            };

        if (VerifierQuinte(quinteVerticalePos5[0], quinteVerticalePos5[1], quinteVerticalePos5[2], quinteVerticalePos5[3], quinteVerticalePos5[4]))
        {
            //Debug.Log($"🎯 Quinte trouvée avec bille en position 5 sur ({x}, {y})");
            TracerLigneQuinte(quinteVerticalePos5); // 🔵 Tracé de la quinte en position 5
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); //Score quinte position 5

        }

        //**************************************************
        // 3 Vérification des quintes diagonales ↘
        //**************************************************

        // Recherche de la double quinte diagonale ↘
        var quinteDiagonaleHG = new List<Vector3> {
        new Vector3(x -4, y +4, 0),
        new Vector3(x -3, y +3, 0),
        new Vector3(x -2, y +2, 0),
        new Vector3(x -1, y +1, 0),
        new Vector3(x , y, 0)
            };
        var quinteDiagonaleBD = new List<Vector3> {
        new Vector3(x , y, 0),
        new Vector3(x +1, y -1, 0),
        new Vector3(x +2, y -2, 0),
        new Vector3(x +3, y -3, 0),
        new Vector3(x +4, y -4, 0)
            };

        if (VerifierQuinte(quinteDiagonaleHG[0], quinteDiagonaleHG[1], quinteDiagonaleHG[2], quinteDiagonaleHG[3], quinteDiagonaleHG[4])
            && VerifierQuinte(quinteDiagonaleBD[0], quinteDiagonaleBD[1], quinteDiagonaleBD[2], quinteDiagonaleBD[3], quinteDiagonaleBD[4]))
        {
            //Debug.Log($"🎯 Double quinte diagonale ↘ trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteDiagonaleHG); // 🔵 Tracé de la quinte haut-gauche
            TracerLigneQuinte(quinteDiagonaleBD); // 🔵 Tracé de la quinte bas-droite
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore"); // Score quinte haut-gauche
            //EventManager.TriggerEvent("UpdateScore"); // Score quinte bas-droite

        }

        // Recherche de la quinte diagonale ↘ centrale
        var quinteCentraleDiagonale = new List<Vector3> {
            new Vector3(x -2, y +2, 0),
            new Vector3(x -1, y +1, 0),
            new Vector3(x , y, 0),
            new Vector3(x +1, y -1, 0),
            new Vector3(x +2, y -2, 0)
            };

        if (VerifierQuinte(quinteCentraleDiagonale[0], quinteCentraleDiagonale[1], quinteCentraleDiagonale[2], quinteCentraleDiagonale[3], quinteCentraleDiagonale[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↘ centrale trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteCentraleDiagonale);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↘ avec bille en position 2
        var quinteDiagonalePos2 = new List<Vector3> {
    new Vector3(x -1, y +1, 0),
    new Vector3(x , y, 0),
    new Vector3(x +1, y -1, 0),
    new Vector3(x +2, y -2, 0),
    new Vector3(x +3, y -3, 0)
};

        if (VerifierQuinte(quinteDiagonalePos2[0], quinteDiagonalePos2[1], quinteDiagonalePos2[2], quinteDiagonalePos2[3], quinteDiagonalePos2[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↘ trouvée avec bille en position 2 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonalePos2);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↘ avec bille en position 4
        var quinteDiagonalePos4 = new List<Vector3> {
    new Vector3(x -3, y +3, 0),
    new Vector3(x -2, y +2, 0),
    new Vector3(x -1, y +1, 0),
    new Vector3(x , y, 0),
    new Vector3(x +1, y -1, 0)
};

        if (VerifierQuinte(quinteDiagonalePos4[0], quinteDiagonalePos4[1], quinteDiagonalePos4[2], quinteDiagonalePos4[3], quinteDiagonalePos4[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↘ trouvée avec bille en position 4 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonalePos4);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↘ avec bille en position 1
        var quinteDiagonalePos1 = new List<Vector3> {
    new Vector3(x , y, 0),
    new Vector3(x +1, y -1, 0),
    new Vector3(x +2, y -2, 0),
    new Vector3(x +3, y -3, 0),
    new Vector3(x +4, y -4, 0)
};

        if (VerifierQuinte(quinteDiagonalePos1[0], quinteDiagonalePos1[1], quinteDiagonalePos1[2], quinteDiagonalePos1[3], quinteDiagonalePos1[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↘ trouvée avec bille en position 1 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonalePos1);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↘ avec bille en position 5
        var quinteDiagonalePos5 = new List<Vector3> {
    new Vector3(x-4 , y +4, 0),
    new Vector3(x-3, y +3, 0),
    new Vector3(x-2, y +2, 0),
    new Vector3(x-1, y +1, 0),
    new Vector3(x, y, 0)
};

        if (VerifierQuinte(quinteDiagonalePos5[0], quinteDiagonalePos5[1], quinteDiagonalePos5[2], quinteDiagonalePos5[3], quinteDiagonalePos5[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↘ trouvée avec bille en position 5 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonalePos5);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        //**************************************************
        // 4 Vérification des quintes diagonales ↙
        //**************************************************
        // Recherche de la double quinte diagonale ↙
        var quinteDiagonaleHautDroit = new List<Vector3> {
    new Vector3(x +4, y +4, 0),
    new Vector3(x +3, y +3, 0),
    new Vector3(x +2, y +2, 0),
    new Vector3(x +1, y +1, 0),
    new Vector3(x , y, 0)
};
        var quinteDiagonaleBasGauche = new List<Vector3> {
    new Vector3(x , y, 0),
    new Vector3(x -1, y -1, 0),
    new Vector3(x -2, y -2, 0),
    new Vector3(x -3, y -3, 0),
    new Vector3(x -4, y -4, 0)
};

        if (VerifierQuinte(quinteDiagonaleHautDroit[0], quinteDiagonaleHautDroit[1], quinteDiagonaleHautDroit[2], quinteDiagonaleHautDroit[3], quinteDiagonaleHautDroit[4])
            && VerifierQuinte(quinteDiagonaleBasGauche[0], quinteDiagonaleBasGauche[1], quinteDiagonaleBasGauche[2], quinteDiagonaleBasGauche[3], quinteDiagonaleBasGauche[4]))
        {
            //Debug.Log($"🎯 Double quinte diagonale ↙ trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteDiagonaleHautDroit);
            TracerLigneQuinte(quinteDiagonaleBasGauche);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");
            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↙ centrale
        var quinteCentraleDiagonaleBG = new List<Vector3> {
    new Vector3(x +2, y +2, 0),
    new Vector3(x +1, y +1, 0),
    new Vector3(x , y, 0),
    new Vector3(x -1, y -1, 0),
    new Vector3(x -2, y -2, 0)
};

        if (VerifierQuinte(quinteCentraleDiagonaleBG[0], quinteCentraleDiagonaleBG[1], quinteCentraleDiagonaleBG[2], quinteCentraleDiagonaleBG[3], quinteCentraleDiagonaleBG[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↙ centrale trouvée à partir de ({x}, {y})");
            TracerLigneQuinte(quinteCentraleDiagonaleBG);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↙ avec bille en position 2
        var quinteDiagonaleBGPos2 = new List<Vector3> {
    new Vector3(x +1, y +1, 0),
    new Vector3(x , y, 0),
    new Vector3(x -1, y -1, 0),
    new Vector3(x -2, y -2, 0),
    new Vector3(x -3, y -3, 0)
};

        if (VerifierQuinte(quinteDiagonaleBGPos2[0], quinteDiagonaleBGPos2[1], quinteDiagonaleBGPos2[2], quinteDiagonaleBGPos2[3], quinteDiagonaleBGPos2[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↙ trouvée avec bille en position 2 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonaleBGPos2);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↙ avec bille en position 4
        var quinteDiagonaleBGPos4 = new List<Vector3> {
    new Vector3(x +3, y +3, 0),
    new Vector3(x +2, y +2, 0),
    new Vector3(x +1, y +1, 0),
    new Vector3(x , y, 0),
    new Vector3(x -1, y -1, 0)
};

        if (VerifierQuinte(quinteDiagonaleBGPos4[0], quinteDiagonaleBGPos4[1], quinteDiagonaleBGPos4[2], quinteDiagonaleBGPos4[3], quinteDiagonaleBGPos4[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↙ trouvée avec bille en position 4 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonaleBGPos4);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↙ avec bille en position 1
        var quinteDiagonaleBGPos1 = new List<Vector3> {
    new Vector3(x , y, 0),
    new Vector3(x -1, y -1, 0),
    new Vector3(x -2, y -2, 0),
    new Vector3(x -3, y -3, 0),
    new Vector3(x -4, y -4, 0)
};

        if (VerifierQuinte(quinteDiagonaleBGPos1[0], quinteDiagonaleBGPos1[1], quinteDiagonaleBGPos1[2], quinteDiagonaleBGPos1[3], quinteDiagonaleBGPos1[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↙ trouvée avec bille en position 1 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonaleBGPos1);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        // Recherche de la quinte diagonale ↙ avec bille en position 5
        var quinteDiagonaleBGPos5 = new List<Vector3> {
    new Vector3(x+4 , y +4, 0),
    new Vector3(x+3, y +3, 0),
    new Vector3(x+2, y +2, 0),
    new Vector3(x+1, y +1, 0),
    new Vector3(x, y, 0)
};

        if (VerifierQuinte(quinteDiagonaleBGPos5[0], quinteDiagonaleBGPos5[1], quinteDiagonaleBGPos5[2], quinteDiagonaleBGPos5[3], quinteDiagonaleBGPos5[4]))
        {
            //Debug.Log($"🎯 Quinte diagonale ↙ trouvée avec bille en position 5 sur ({x}, {y})");
            TracerLigneQuinte(quinteDiagonaleBGPos5);
            //quinteTrouvee = true;
            quintestrouvees++;

            //EventManager.TriggerEvent("UpdateScore");

        }

        return quintestrouvees;

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