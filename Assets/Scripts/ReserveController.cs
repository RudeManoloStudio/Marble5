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
    [SerializeField] private float horizontalGravity = 15.0f;

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
    /// <summary>
    /// Construit la queue initiale avec les billes et plombs.
    /// Ordre : Élément le plus à droite (consommé en premier) -> Élément le plus à gauche.
    /// </summary>
    private void BuildInitialQueue(int totalMarbles)
    {
        queue.Clear();
        compteurPlombReserve = 0; // Réinitialisation du compteur pour l'initialisation

        if (blockerFrequency > 0)
        {
            int billesInserees = 0;
            // On ajoute les éléments dans l'ordre de consommation (de droite à gauche)
            for (int i = 0; i < totalMarbles; i++)
            {
                // 1. Ajouter la bille
                queue.Add(ReserveItemType.Marble);
                billesInserees++;

                // 2. Vérifier si un plomb doit suivre
                if (billesInserees % blockerFrequency == 0)
                {
                    // Insère le plomb qui sera consommé JUSTE APRÈS les 'blockerFrequency' billes.
                    queue.Add(ReserveItemType.Blocker);
                }
            }

            // Correction : Inverser la liste pour que l'index 0 soit le plus neuf
            // et l'index Count-1 le plus ancien (le premier à consommer).
            queue.Reverse();

            // S'assurer que le compteur de plomb qui gère les NOUVEAUX ajouts
            // commence à partir de là où on s'est arrêté.
            // Le nombre de billes depuis le dernier plomb inséré (dans l'ordre inversé)
            compteurPlombReserve = billesInserees % blockerFrequency;
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

                    // Empêcher le mouvement vers la gauche (anti-rebond)
                    if (rb.velocity.x < 0)
                    {
                        rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                    }
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
    /// <summary>
    /// Gère le compteur de plombs et retire le plomb de la queue si nécessaire
    /// </summary>
    /// <summary>
    /// Gère le compteur de plombs et retire le plomb de la queue si nécessaire
    /// </summary>
    private void HandleBlockerCounter()
    {
        if (blockerFrequency <= 0) return;

        compteurPlombGrille++; // La bille qui vient d'être posée

        if (compteurPlombGrille >= blockerFrequency)
        {
            // Vérifier si l'élément le plus à droite est bien un Plomb qui doit être consommé
            if (queue.Count > 0 && queue[queue.Count - 1] == ReserveItemType.Blocker)
            {
                // 1. Retirer le plomb de la queue de données
                queue.RemoveAt(queue.Count - 1);

                // 2. Supprimer visuellement l'élément le plus à droite (c'est le plomb !)
                RemoveRightmostDisplayedItem();

                // 3. Ajouter un nouvel élément à gauche si nécessaire
                if (!isAnimating)
                {
                    TryAddItemOnLeft();
                }

                // Réinitialiser le compteur seulement APRÈS avoir consommé le plomb
                compteurPlombGrille = 0;
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
            // 1. Ajouter la bille
            queue.Insert(0, ReserveItemType.Marble); // Ajout au début de la queue
            itemsToAdd.Add(ReserveItemType.Marble);

            if (blockerFrequency > 0)
            {
                compteurPlombReserve++; // Une bille a été ajoutée

                if (compteurPlombReserve >= blockerFrequency)
                {
                    // 2. Ajouter le plomb immédiatement APRES la bille, donc AVANT dans la liste (index 0)
                    queue.Insert(0, ReserveItemType.Blocker);
                    itemsToAdd.Add(ReserveItemType.Blocker);
                    compteurPlombReserve = 0; // Le cycle recommence
                }
            }
        }

        // ... (Le reste de la méthode AjouterBilles reste inchangé) ...
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
    /// <summary>
    /// Ajoute les éléments visuellement avec animation séquentielle
    /// </summary>
    private IEnumerator AddItemsVisuallyAnimated(List<ReserveItemType> items)
    {
        isAnimating = true;

        // Démarrer bien à gauche, hors de la file
        float safeOffset = -15f * spacing;

        foreach (ReserveItemType itemType in items)
        {
            // Vérifier la limite max avant l'ajout
            if (displayedItems.Count < maxDisplayedItems)
            {
                AddItemOnLeft(itemType, safeOffset);
                yield return new WaitForSeconds(0.4f);
            }
            else
            {
                // Si la file est pleine, on arrête l'animation pour les éléments restants, 
                // car ils sont déjà dans la queue de données.
                break;
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
    /// <summary>
    /// Tente d'ajouter un élément à gauche si la limite n'est pas atteinte
    /// </summary>
    private void TryAddItemOnLeft()
    {
        // 1. Vérifier si on doit afficher un nouvel élément.
        // On affiche un élément si on est sous la limite max ET s'il y a encore des éléments dans la queue de données.
        if (displayedItems.Count < maxDisplayedItems && displayedItems.Count < queue.Count)
        {
            // L'index de l'élément à ajouter est le premier élément NON affiché
            // qui se trouve à l'avant (index 0) de la sous-liste de la queue *NON* affichée.
            // L'index dans la queue de données est donc : queue.Count - displayedItems.Count - 1
            // (La liste visuelle affichée a une taille 'displayedItems.Count', et l'index 0 de la queue est le plus à gauche.)
            int queueIndex = queue.Count - (displayedItems.Count + 1);

            // Sanity check, mais avec la condition ci-dessus, il devrait toujours être >= 0
            if (queueIndex >= 0)
            {
                ReserveItemType itemType = queue[queueIndex];
                // L'offset est utilisé pour que la bille/bloc démarre hors-champ
                AddItemOnLeft(itemType, -15f * spacing);
            }
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

        // Désactiver le rebond sur le collider
        Collider col = item.GetComponent<Collider>();
        if (col != null)
        {
            PhysicMaterial noBounceMat = new PhysicMaterial();
            noBounceMat.bounciness = 0f;
            noBounceMat.bounceCombine = PhysicMaterialCombine.Minimum;
            col.material = noBounceMat;
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
                // Déplacer vers la droite pendant l'animation (kinematic)
                item.transform.localPosition += Vector3.right * horizontalGravity * Time.deltaTime;
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
                rb.velocity = Vector3.right * horizontalGravity;
            }
        }
    }
}