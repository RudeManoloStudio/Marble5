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
    private bool coroutineEnCours = false;

    private List<string> reserveQueue = new List<string>();
    private List<GameObject> billesAffichees = new List<GameObject>();

    public void Setup(GameObject bille, GameObject plomb, int totalBilles, int freqPlomb)
    {
        billePrefab = bille;
        plombPrefab = plomb;
        frequencePlomb = freqPlomb;
        compteurPourPlomb = 0;

        reserveQueue.Clear();

        if (freqPlomb > 0)
        {
            int billesRestantes = totalBilles;
            int compteur = 0;

            while (billesRestantes > 0)
            {
                int billesAvantPlomb = Mathf.Min(freqPlomb - compteur, billesRestantes);

                for (int i = 0; i < billesAvantPlomb; i++)
                {
                    reserveQueue.Insert(0, "bille");
                    billesRestantes--;
                    compteur++;
                }

                if (compteur >= freqPlomb && billesRestantes > 0)
                {
                    reserveQueue.Insert(0, "plomb");
                    compteur = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < totalBilles; i++)
            {
                reserveQueue.Add("bille");
            }
        }

        RefreshDisplay();
        AdjustCamera();
    }

    private void FixedUpdate()
    {
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

            if (billesAffichees.Count > 0)
            {
                // Trouver la bille la plus à droite (position X max)
                GameObject billeASupprimer = billesAffichees[0];
                int indexASupprimer = 0;

                for (int i = 1; i < billesAffichees.Count; i++)
                {
                    if (billesAffichees[i].transform.localPosition.x > billeASupprimer.transform.localPosition.x)
                    {
                        billeASupprimer = billesAffichees[i];
                        indexASupprimer = i;
                    }
                }

                billesAffichees.RemoveAt(indexASupprimer);
                StartCoroutine(ShrinkAnimation(billeASupprimer));
            }

            if (!coroutineEnCours)
            {
                AjouterNouvelleBilleAGauche();
            }

            compteurPourPlomb++;
            if (frequencePlomb > 0 && compteurPourPlomb >= frequencePlomb)
            {
                compteurPourPlomb = 0;
                EventManager.TriggerEvent("PoserPlomb");

                if (reserveQueue.Count > 0 && reserveQueue[reserveQueue.Count - 1] == "plomb")
                {
                    reserveQueue.RemoveAt(reserveQueue.Count - 1);

                    if (billesAffichees.Count > 0)
                    {
                        // Trouver le plomb le plus à droite (position X max)
                        GameObject plombASupprimer = billesAffichees[0];
                        int indexASupprimer = 0;

                        for (int i = 1; i < billesAffichees.Count; i++)
                        {
                            if (billesAffichees[i].transform.localPosition.x > plombASupprimer.transform.localPosition.x)
                            {
                                plombASupprimer = billesAffichees[i];
                                indexASupprimer = i;
                            }
                        }

                        billesAffichees.RemoveAt(indexASupprimer);
                        StartCoroutine(ShrinkAnimation(plombASupprimer));
                    }

                    if (!coroutineEnCours)
                    {
                        AjouterNouvelleBilleAGauche();
                    }
                }
            }
        }
    }

    private IEnumerator ShrinkAnimation(GameObject bille)
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 startScale = bille.transform.localScale;
        Vector3 endScale = new Vector3(0.1f, 0.1f, 0.1f);

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

        if (bille != null)
        {
            Destroy(bille);
        }
    }

    private void AjouterNouvelleBilleAGauche(string typeForce = null, float decalageX = 0f)
    {
        int nombreAAfficher = Mathf.Min(nombreBillesAffichees, reserveQueue.Count);

        bool doitAjouter = (typeForce != null)
            ? (billesAffichees.Count < nombreBillesAffichees)
            : (billesAffichees.Count < nombreAAfficher);

        if (doitAjouter)
        {
            string type;
            if (typeForce != null)
            {
                type = typeForce;
            }
            else
            {
                int indexQueue = reserveQueue.Count - nombreAAfficher;
                type = (indexQueue >= 0) ? reserveQueue[indexQueue] : "bille";
            }

            GameObject prefab = (type == "plomb") ? plombPrefab : billePrefab;

            if (prefab != null)
            {
                GameObject nouvelleBille = Instantiate(prefab, container);
                nouvelleBille.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                nouvelleBille.transform.localPosition = new Vector3(decalageX, 0, 0);
                nouvelleBille.transform.localRotation = Quaternion.identity;

                Rigidbody rb = nouvelleBille.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                    rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                }

                BilleController bc = nouvelleBille.GetComponent<BilleController>();
                if (bc != null)
                {
                    bc.DoRotate(false);
                }

                billesAffichees.Insert(0, nouvelleBille);

                StartCoroutine(ScaleUpAnimation(nouvelleBille));
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

        if (bille != null)
        {
            bille.transform.localScale = endScale;

            Rigidbody rb = bille.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

    public void AjouterBilles(int nombre)
    {
        if (frequencePlomb > 0)
        {
            List<string> pourAffichage = new List<string>();

            for (int i = 0; i < nombre; i++)
            {
                // 1. Compter les billes consécutives à GAUCHE (début de la queue)
                int billesAGauche = 0;
                for (int j = 0; j < reserveQueue.Count; j++)
                {
                    if (reserveQueue[j] == "plomb")
                        break;
                    billesAGauche++;
                }

                // 2. Ajouter la bille à gauche
                reserveQueue.Insert(0, "bille");
                pourAffichage.Add("bille");

                // 3. Si (billes à gauche + notre bille) >= fréquence → ajouter plomb
                if ((billesAGauche + 1) >= frequencePlomb)
                {
                    reserveQueue.Insert(0, "plomb");
                    pourAffichage.Add("plomb");
                }
            }

            // 4. Afficher visuellement
            StartCoroutine(AjouterBillesVisuellement(pourAffichage));
        }
        else
        {
            // Sans plomb : que des billes
            for (int i = 0; i < nombre; i++)
            {
                reserveQueue.Insert(0, "bille");
            }

            for (int i = 0; i < nombre; i++)
            {
                if (billesAffichees.Count < nombreBillesAffichees)
                {
                    AjouterNouvelleBilleAGauche();
                }
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
        reserveCam.transform.localPosition = new Vector3(centerX, 0, -1);
    }

    private IEnumerator AjouterBillesVisuellement(List<string> elements)
    {
        coroutineEnCours = true;

        float decalage = 0f;

        foreach (string type in elements)
        {
            if (billesAffichees.Count < nombreBillesAffichees)
            {
                AjouterNouvelleBilleAGauche(type, decalage);
                decalage -= espacement;
                yield return new WaitForSeconds(0.1f);
            }
        }

        coroutineEnCours = false;
    }
}