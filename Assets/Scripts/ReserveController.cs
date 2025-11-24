using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveController : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Camera reserveCam;

    [Header("Configuration")]
    [SerializeField] private int nombreBillesAffichees = 10;
    [SerializeField] private float espacement = 1.0f;
    [SerializeField] private float cameraSize = 1.4f;
    [SerializeField] private float graviteHorizontale = 5.0f;

    private GameObject billePrefab;
    private GameObject plombPrefab;
    private int frequencePlomb;
    private int compteurPourPlomb;

    private List<string> reserveQueue = new List<string>();
    private List<GameObject> billesAffichees = new List<GameObject>();

    public void Setup(GameObject bille, GameObject plomb, int totalBilles, int freqPlomb)
    {
        billePrefab = bille;
        plombPrefab = plomb;
        frequencePlomb = freqPlomb;
        compteurPourPlomb = 0;

        reserveQueue.Clear();
        for (int i = 0; i < totalBilles; i++)
        {
            reserveQueue.Add("bille");
        }

        RefreshDisplay();
        AdjustCamera();
    }

    private void FixedUpdate()
    {
        // Appliquer une force vers la droite à toutes les billes
        foreach (GameObject bille in billesAffichees)
        {
            if (bille != null)
            {
                Rigidbody rb = bille.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.right * graviteHorizontale, ForceMode.Acceleration);
                }
            }
        }
    }

    public void ConsommerBille()
    {
        if (reserveQueue.Count > 0)
        {
            reserveQueue.RemoveAt(reserveQueue.Count - 1);

            // Animer la disparition de la bille de droite
            if (billesAffichees.Count > 0)
            {
                GameObject billeASupprimer = billesAffichees[billesAffichees.Count - 1];
                billesAffichees.RemoveAt(billesAffichees.Count - 1);

                // Lancer l'animation shrink puis détruire
                StartCoroutine(ShrinkAnimation(billeASupprimer));
            }

            // Ajouter une nouvelle bille à gauche si nécessaire
            AjouterNouvelleBilleAGauche();

            compteurPourPlomb++;
            if (frequencePlomb > 0 && compteurPourPlomb >= frequencePlomb)
            {
                compteurPourPlomb = 0;
                EventManager.TriggerEvent("PoserPlomb");
            }
        }
    }

    private IEnumerator ShrinkAnimation(GameObject bille)
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 startScale = bille.transform.localScale;
        Vector3 endScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Désactiver la physique pendant l'animation
        Rigidbody rb = bille.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);

            if (bille != null)
            {
                bille.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }

            yield return null;
        }

        // Détruire la bille
        if (bille != null)
        {
            Destroy(bille);
        }
    }

    private void AjouterNouvelleBilleAGauche()
    {
        int nombreAAfficher = Mathf.Min(nombreBillesAffichees, reserveQueue.Count);

        if (billesAffichees.Count < nombreAAfficher)
        {
            int indexQueue = reserveQueue.Count - nombreAAfficher;
            if (indexQueue >= 0)
            {
                string type = reserveQueue[indexQueue];
                GameObject prefab = (type == "plomb") ? plombPrefab : billePrefab;

                if (prefab != null)
                {
                    GameObject nouvelleBille = Instantiate(prefab, container);
                    nouvelleBille.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    nouvelleBille.transform.localPosition = new Vector3(0, 0, 0); // Dans le champ de vision !
                    nouvelleBille.transform.localRotation = Quaternion.identity;

                    // Configurer le Rigidbody - KINEMATIC au début (pas de physique)
                    Rigidbody rb = nouvelleBille.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.useGravity = false;
                        rb.isKinematic = true; // Pas de physique pendant le scale
                        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                    }

                    BilleController bc = nouvelleBille.GetComponent<BilleController>();
                    if (bc != null)
                    {
                        bc.DoRotate(false);
                    }

                    billesAffichees.Insert(0, nouvelleBille);

                    // Lancer l'animation de scale, puis activer la physique
                    StartCoroutine(ScaleUpAnimation(nouvelleBille));
                }
            }
        }
    }

    private IEnumerator ScaleUpAnimation(GameObject bille)
    {
        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f);
        Vector3 endScale = new Vector3(1f, 1f, 1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);

            if (bille != null)
            {
                bille.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }

            yield return null;
        }

        // Finaliser le scale
        if (bille != null)
        {
            bille.transform.localScale = endScale;

            // Activer la physique maintenant !
            Rigidbody rb = bille.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

    public void AjouterBilles(int nombre)
    {
        for (int i = 0; i < nombre; i++)
        {
            reserveQueue.Insert(0, "bille");
        }

        // Ajouter visuellement les nouvelles billes
        for (int i = 0; i < nombre; i++)
        {
            if (billesAffichees.Count < nombreBillesAffichees)
            {
                AjouterNouvelleBilleAGauche();
            }
        }
    }

    public int GetNombreRestant()
    {
        return reserveQueue.Count;
    }

    private void RefreshDisplay()
    {
        foreach (GameObject billeObj in billesAffichees)
        {
            if (billeObj != null)
            {
                Destroy(billeObj);
            }
        }
        billesAffichees.Clear();

        int nombreAAfficher = Mathf.Min(nombreBillesAffichees, reserveQueue.Count);
        int startIndex = reserveQueue.Count - nombreAAfficher;

        for (int i = 0; i < nombreAAfficher; i++)
        {
            int indexQueue = startIndex + i;
            string type = reserveQueue[indexQueue];
            GameObject prefab = (type == "plomb") ? plombPrefab : billePrefab;

            if (prefab != null)
            {
                GameObject nouvelleBille = Instantiate(prefab, container);
                nouvelleBille.transform.localScale = new Vector3(1, 1, 1);
                nouvelleBille.transform.localPosition = new Vector3(i * espacement, 0, 0);
                nouvelleBille.transform.localRotation = Quaternion.identity;

                // Configurer le Rigidbody pour la physique horizontale
                Rigidbody rb = nouvelleBille.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = false;
                    rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                }

                BilleController bc = nouvelleBille.GetComponent<BilleController>();
                if (bc != null)
                {
                    bc.DoRotate(false);
                }

                billesAffichees.Add(nouvelleBille);
            }
        }
    }

    private void AdjustCamera()
    {
        float largeurTotale = (nombreBillesAffichees - 1) * espacement;
        float centerX = largeurTotale / 2.0f;

        //reserveCam.orthographicSize = cameraSize;
        reserveCam.transform.localPosition = new Vector3(centerX, 0, -1);
    }
}