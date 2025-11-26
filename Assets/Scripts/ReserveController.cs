using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type d'élément dans la réserve
/// </summary>
public enum ReserveItemType
{
    Marble,
    Blocker
}

/// <summary>
/// Gère la réserve de billes du joueur (données + affichage visuel)
/// </summary>
public class ReserveController : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Camera reserveCam;

    [Header("Configuration")]
    [SerializeField] private int maxDisplayedItems = 10;
    [SerializeField] private float spacing = 1.0f;
    [SerializeField] private float horizontalGravity = 5.0f;

    [Header("Animation")]
    [SerializeField] private float shrinkDuration = 0.2f;
    [SerializeField] private float growDuration = 0.3f;

    // Prefabs
    private GameObject marblePrefab;
    private GameObject blockerPrefab;

    // Configuration du niveau
    private int blockerFrequency;
    private int compteurPlombGrille;
    private int compteurPlombReserve;
    private int marblesSinceLastBlocker;

    // File d'attente des éléments (données)
    private List<ReserveItemType> queue = new List<ReserveItemType>();

    // Éléments affichés (visuels)
    private List<GameObject> displayedItems = new List<GameObject>();

    // État des animations
    private bool isAnimating = false;

    // ============================================================
    // INITIALISATION
    // ============================================================

    public void Setup(GameObject marble, GameObject blocker, int totalMarbles, int blockerFreq)
    {
        marblePrefab = marble;
        blockerPrefab = blocker;
        blockerFrequency = blockerFreq;
        compteurPlombGrille = 0;
        compteurPlombReserve = 0;

        queue.Clear();
        BuildInitialQueue(totalMarbles);

        RefreshDisplay();
        AdjustCamera();
    }

    /// <summary>
    /// Construit la queue initiale avec les billes et plombs
    /// </summary>
    private void BuildInitialQueue(int totalMarbles)
    {
        if (blockerFrequency > 0)
        {
            int blockerCounter = 0;
            for (int i = 0; i < totalMarbles; i++)
            {
                queue.Insert(0, ReserveItemType.Marble);
                blockerCounter++;

                if (blockerCounter >= blockerFrequency)
                {
                    queue.Insert(0, ReserveItemType.Blocker);
                    blockerCounter = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < totalMarbles; i++)
            {
                queue.Add(ReserveItemType.Marble);
            }
        }
    }

    // ============================================================
    // MISE À JOUR (PHYSIQUE)
    // ============================================================

    private void FixedUpdate()
    {
        ApplyHorizontalForce();
    }

    /// <summary>
    /// Applique une force horizontale aux éléments affichés
    /// </summary>
    private void ApplyHorizontalForce()
    {
        foreach (GameObject item in displayedItems)
        {
            if (item != null)
            {
                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.right * horizontalGravity, ForceMode.Acceleration);
                }
            }
        }
    }

    // ============================================================
    // CONSOMMATION (quand le joueur pose une bille)
    // ============================================================

    public void ConsommerBille()
    {
        if (queue.Count == 0) return;

        // 1. Retirer de la queue
        queue.RemoveAt(queue.Count - 1);

        // 2. Supprimer visuellement l'élément le plus à droite
        RemoveRightmostDisplayedItem();

        // 3. Ajouter un nouvel élément à gauche si nécessaire
        if (!isAnimating)
        {
            TryAddItemOnLeft();
        }

        // 4. Gérer le compteur de plombs
        HandleBlockerCounter();
    }

    /// <summary>
    /// Gère le compteur de plombs et retire le plomb de la queue si nécessaire
    /// </summary>
    private void HandleBlockerCounter()
    {
        if (blockerFrequency <= 0) return;

        compteurPlombGrille++;

        if (compteurPlombGrille >= blockerFrequency)
        {
            compteurPlombGrille = 0;

            if (queue.Count > 0 && queue[queue.Count - 1] == ReserveItemType.Blocker)
            {
                queue.RemoveAt(queue.Count - 1);
                RemoveRightmostDisplayedItem();

                if (!isAnimating)
                {
                    TryAddItemOnLeft();
                }
            }
        }
    }

    // ============================================================
    // AJOUT DE BILLES (quand le joueur forme une quinte)
    // ============================================================

    public void AjouterBilles(int count)
    {
        List<ReserveItemType> itemsToAdd = new List<ReserveItemType>();

        for (int i = 0; i < count; i++)
        {
            queue.Insert(0, ReserveItemType.Marble);
            itemsToAdd.Add(ReserveItemType.Marble);

            if (blockerFrequency > 0)
            {
                compteurPlombReserve++;

                if (compteurPlombReserve >= blockerFrequency)
                {
                    queue.Insert(0, ReserveItemType.Blocker);
                    itemsToAdd.Add(ReserveItemType.Blocker);
                    compteurPlombReserve = 0;
                }
            }
        }

        if (blockerFrequency > 0)
        {
            StartCoroutine(AddItemsVisuallyAnimated(itemsToAdd));
        }
        else
        {
            AddItemsVisuallyImmediate(count);
        }
    }

    /// <summary>
    /// Ajoute les éléments visuellement sans animation
    /// </summary>
    private void AddItemsVisuallyImmediate(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (displayedItems.Count < maxDisplayedItems)
            {
                TryAddItemOnLeft();
            }
        }
    }


    /// <summary>
    /// Ajoute les éléments visuellement avec animation séquentielle
    /// </summary>
    private IEnumerator AddItemsVisuallyAnimated(List<ReserveItemType> items)
    {
        isAnimating = true;

        // Démarrer bien à gauche, hors champ de la caméra
        float safeOffset = -maxDisplayedItems * spacing;

        foreach (ReserveItemType itemType in items)
        {
            if (displayedItems.Count < maxDisplayedItems)
            {
                AddItemOnLeft(itemType, safeOffset);
                yield return new WaitForSeconds(0.4f); // Délai augmenté pour laisser partir la bille
            }
        }

        isAnimating = false;
    }

    // ============================================================
    // GESTION DE L'AFFICHAGE
    // ============================================================

    /// <summary>
    /// Trouve et supprime l'élément affiché le plus à droite
    /// </summary>
    private void RemoveRightmostDisplayedItem()
    {
        if (displayedItems.Count == 0) return;

        int rightmostIndex = FindRightmostIndex();
        GameObject itemToRemove = displayedItems[rightmostIndex];

        displayedItems.RemoveAt(rightmostIndex);
        StartCoroutine(ShrinkAndDestroy(itemToRemove));
    }

    /// <summary>
    /// Trouve l'index de l'élément le plus à droite
    /// </summary>
    private int FindRightmostIndex()
    {
        int rightmostIndex = 0;
        float maxX = displayedItems[0].transform.localPosition.x;

        for (int i = 1; i < displayedItems.Count; i++)
        {
            float x = displayedItems[i].transform.localPosition.x;
            if (x > maxX)
            {
                maxX = x;
                rightmostIndex = i;
            }
        }

        return rightmostIndex;
    }

    /// <summary>
    /// Tente d'ajouter un élément à gauche si la limite n'est pas atteinte
    /// </summary>
    private void TryAddItemOnLeft()
    {
        int itemsToDisplay = Mathf.Min(maxDisplayedItems, queue.Count);

        if (displayedItems.Count < itemsToDisplay)
        {
            int queueIndex = queue.Count - itemsToDisplay;
            ReserveItemType itemType = (queueIndex >= 0) ? queue[queueIndex] : ReserveItemType.Marble;
            AddItemOnLeft(itemType, 0f);
        }
    }

    /// <summary>
    /// Ajoute un élément visuel à gauche
    /// </summary>
    private void AddItemOnLeft(ReserveItemType itemType, float offsetX)
    {
        GameObject prefab = (itemType == ReserveItemType.Blocker) ? blockerPrefab : marblePrefab;
        if (prefab == null) return;

        GameObject newItem = Instantiate(prefab, container);
        ConfigureDisplayedItem(newItem, new Vector3(offsetX, 0, 0), isKinematic: true);

        displayedItems.Insert(0, newItem);
        StartCoroutine(GrowAnimation(newItem));
    }

    /// <summary>
    /// Rafraîchit complètement l'affichage
    /// </summary>
    private void RefreshDisplay()
    {
        // Détruire les éléments existants
        foreach (GameObject item in displayedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        displayedItems.Clear();

        // Recréer les éléments
        int itemsToDisplay = Mathf.Min(maxDisplayedItems, queue.Count);
        int startIndex = queue.Count - itemsToDisplay;

        for (int i = 0; i < itemsToDisplay; i++)
        {
            int queueIndex = startIndex + i;
            ReserveItemType itemType = queue[queueIndex];
            GameObject prefab = (itemType == ReserveItemType.Blocker) ? blockerPrefab : marblePrefab;

            if (prefab != null)
            {
                GameObject newItem = Instantiate(prefab, container);
                Vector3 position = new Vector3(i * spacing, 0, 0);
                ConfigureDisplayedItem(newItem, position, isKinematic: false);
                displayedItems.Add(newItem);
            }
        }
    }

    /// <summary>
    /// Configure un élément affiché (position, Rigidbody, rotation)
    /// </summary>
    private void ConfigureDisplayedItem(GameObject item, Vector3 localPosition, bool isKinematic)
    {
        item.transform.localPosition = localPosition;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = isKinematic ? new Vector3(0.1f, 0.1f, 0.1f) : Vector3.one;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = isKinematic;
            rb.constraints = RigidbodyConstraints.FreezePositionY
                           | RigidbodyConstraints.FreezePositionZ
                           | RigidbodyConstraints.FreezeRotation;
        }

        BilleController bc = item.GetComponent<BilleController>();
        if (bc != null)
        {
            bc.DoRotate(false);
        }
    }

    /// <summary>
    /// Ajuste la position de la caméra
    /// </summary>
    private void AdjustCamera()
    {
        float totalWidth = (maxDisplayedItems - 1) * spacing;
        float centerX = totalWidth / 2.0f;
        reserveCam.transform.localPosition = new Vector3(centerX, 0, -1);
    }

    // ============================================================
    // ANIMATIONS
    // ============================================================

    /// <summary>
    /// Animation de rétrécissement puis destruction
    /// </summary>
    private IEnumerator ShrinkAndDestroy(GameObject item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Vector3 startScale = item.transform.localScale;
        Vector3 endScale = new Vector3(0.1f, 0.1f, 0.1f);
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / shrinkDuration);

            if (item != null)
            {
                item.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }

            yield return null;
        }

        if (item != null)
        {
            Destroy(item);
        }
    }

    /// <summary>
    /// Animation de grossissement
    /// </summary>
    private IEnumerator GrowAnimation(GameObject item)
    {
        Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f);
        Vector3 endScale = Vector3.one;
        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / growDuration);

            if (item != null)
            {
                item.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }

            yield return null;
        }

        if (item != null)
        {
            item.transform.localScale = endScale;

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }
}