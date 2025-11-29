using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image star1;
    [SerializeField] private Image star2;
    [SerializeField] private Image star3;
    [SerializeField] private Transform vault;

    [Header("Couleurs des étoiles")]
    [SerializeField] private Color star1Color = new Color(1f, 1f, 0.6f, 1f);      // Jaune clair
    [SerializeField] private Color star2Color = new Color(1f, 0.9f, 0.2f, 1f);    // Jaune
    [SerializeField] private Color star3Color = new Color(1f, 0.75f, 0f, 1f);     // Or
    [SerializeField] private Color starNotObtainedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Gris semi-transparent

    public void SetLevelParameters(LevelStruct level)
    {
        text.text = (level.ID + 1).ToString();

        // Réinitialiser toutes les étoiles en gris
        star1.color = starNotObtainedColor;
        star2.color = starNotObtainedColor;
        star3.color = starNotObtainedColor;

        if (!level.available)
        {
            vault.gameObject.SetActive(true);
        }
        else
        {
            vault.gameObject.SetActive(false);

            // Appliquer les couleurs selon le nombre d'étoiles obtenues
            if (level.stars >= 1) star1.color = star1Color;
            if (level.stars >= 2) star2.color = star2Color;
            if (level.stars >= 3) star3.color = star3Color;
        }
    }
}