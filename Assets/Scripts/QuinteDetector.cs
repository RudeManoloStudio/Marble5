using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe responsable de la détection des quintes (alignements de 5 billes).
/// Logique pure, indépendante du rendu.
/// </summary>
public class QuinteDetector
{
    // Liaisons déjà utilisées (une liaison ne peut servir qu'une fois)
    private HashSet<(Vector3, Vector3)> usedLinks = new HashSet<(Vector3, Vector3)>();

    // Les 4 directions possibles pour former une quinte
    private static readonly Vector3[] quintetDirections = new Vector3[]
    {
        new Vector3(1, 0, 0),   // Horizontal →
        new Vector3(0, 1, 0),   // Vertical ↑
        new Vector3(1, 1, 0),   // Diagonale ↗
        new Vector3(1, -1, 0)   // Diagonale ↘
    };

    /// <summary>
    /// Résultat d'une détection de quinte
    /// </summary>
    public struct QuintetResult
    {
        public List<Vector3> Positions;
        public int Index; // 1, 2, 3... pour les quintes multiples

        public QuintetResult(List<Vector3> positions, int index)
        {
            Positions = positions;
            Index = index;
        }
    }

    /// <summary>
    /// Réinitialise les liaisons utilisées (nouveau niveau)
    /// </summary>
    public void Reset()
    {
        usedLinks.Clear();
    }

    /// <summary>
    /// Détecte toutes les quintes formées par le placement d'une bille à la position donnée.
    /// </summary>
    /// <param name="position">Position de la bille nouvellement placée</param>
    /// <returns>Liste des quintes détectées</returns>
    public List<QuintetResult> DetectQuintets(Vector3 position)
    {
        List<QuintetResult> results = new List<QuintetResult>();

        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        int quintetIndex = 0;

        foreach (Vector3 dir in quintetDirections)
        {
            // Vérifier d'abord si c'est une double quinte
            var doubleQuintet = CheckDoubleQuintet(x, y, dir);
            if (doubleQuintet != null)
            {
                quintetIndex++;
                results.Add(new QuintetResult(doubleQuintet.Value.Item1, quintetIndex));
                RegisterLinks(doubleQuintet.Value.Item1);

                quintetIndex++;
                results.Add(new QuintetResult(doubleQuintet.Value.Item2, quintetIndex));
                RegisterLinks(doubleQuintet.Value.Item2);
            }
            else
            {
                // Chercher une quinte simple
                List<List<Vector3>> validQuintets = new List<List<Vector3>>();

                for (int pos = 0; pos < 5; pos++)
                {
                    List<Vector3> quintet = GenerateQuintet(x, y, dir, pos);
                    if (IsQuintetValid(quintet))
                    {
                        validQuintets.Add(quintet);
                    }
                }

                if (validQuintets.Count > 0)
                {
                    // Choisir la meilleure quinte
                    List<Vector3> bestQuintet = SelectBestQuintet(validQuintets);

                    quintetIndex++;
                    results.Add(new QuintetResult(bestQuintet, quintetIndex));
                    RegisterLinks(bestQuintet);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Vérifie si la position (x,y) est le centre d'une double quinte dans la direction donnée
    /// </summary>
    private (List<Vector3>, List<Vector3>)? CheckDoubleQuintet(int x, int y, Vector3 dir)
    {
        // Quinte "avant" : de -4 à 0 par rapport à la position
        List<Vector3> quintetBefore = new List<Vector3>();
        for (int i = -4; i <= 0; i++)
        {
            quintetBefore.Add(new Vector3(x + dir.x * i, y + dir.y * i, 0));
        }

        // Quinte "après" : de 0 à +4 par rapport à la position
        List<Vector3> quintetAfter = new List<Vector3>();
        for (int i = 0; i <= 4; i++)
        {
            quintetAfter.Add(new Vector3(x + dir.x * i, y + dir.y * i, 0));
        }

        // Les deux quintes doivent être valides pour une double quinte
        bool beforeValid = IsQuintetValid(quintetBefore);
        bool afterValid = IsQuintetValid(quintetAfter);

        if (beforeValid && afterValid)
        {
            return (quintetBefore, quintetAfter);
        }

        return null;
    }

    /// <summary>
    /// Génère les 5 positions d'une quinte où la bille posée est à la position donnée (0 à 4)
    /// </summary>
    private List<Vector3> GenerateQuintet(int x, int y, Vector3 dir, int positionInQuintet)
    {
        List<Vector3> quintet = new List<Vector3>();
        int startOffset = -positionInQuintet;

        for (int i = 0; i < 5; i++)
        {
            int offset = startOffset + i;
            quintet.Add(new Vector3(x + dir.x * offset, y + dir.y * offset, 0));
        }

        return quintet;
    }

    /// <summary>
    /// Vérifie si une quinte est valide (5 billes présentes + liaisons non utilisées)
    /// </summary>
    private bool IsQuintetValid(List<Vector3> positions)
    {
        // 1. Vérifier que chaque position contient une bille
        foreach (Vector3 pos in positions)
        {
            if (!HasMarbleAt(pos))
            {
                return false;
            }
        }

        // 2. Vérifier que les liaisons ne sont pas déjà utilisées
        for (int i = 0; i < positions.Count - 1; i++)
        {
            var link = NormalizeLink(positions[i], positions[i + 1]);
            if (usedLinks.Contains(link))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Vérifie s'il y a une bille à la position donnée
    /// </summary>
    private bool HasMarbleAt(Vector3 position)
    {
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

    /// <summary>
    /// Sélectionne la meilleure quinte parmi les candidates
    /// </summary>
    private List<Vector3> SelectBestQuintet(List<List<Vector3>> candidates)
    {
        List<Vector3> best = candidates[0];
        int bestScore = CalculateQuintetScore(best);

        for (int i = 1; i < candidates.Count; i++)
        {
            int score = CalculateQuintetScore(candidates[i]);
            if (score > bestScore)
            {
                bestScore = score;
                best = candidates[i];
            }
        }

        return best;
    }

    /// <summary>
    /// Calcule un score de qualité pour une quinte potentielle.
    /// Plus le score est élevé, meilleure est la quinte (moins de trous créés).
    /// </summary>
    private int CalculateQuintetScore(List<Vector3> quintet)
    {
        int score = 0;
        Vector3[] directions = { Vector3.right, Vector3.up, new Vector3(1, 1, 0), new Vector3(1, -1, 0) };

        foreach (Vector3 pos in quintet)
        {
            foreach (Vector3 dir in directions)
            {
                Vector3 prev = pos - dir;
                Vector3 next = pos + dir;

                var linkBefore = NormalizeLink(pos, prev);
                var linkAfter = NormalizeLink(pos, next);

                if (usedLinks.Contains(linkBefore) || usedLinks.Contains(linkAfter))
                {
                    score += 100;
                }
            }
        }

        // Bonus pour les liaisons consécutives non utilisées
        int consecutiveLinks = 0;
        for (int i = 0; i < quintet.Count - 1; i++)
        {
            var link = NormalizeLink(quintet[i], quintet[i + 1]);
            if (!usedLinks.Contains(link))
            {
                consecutiveLinks++;
            }
        }
        score += consecutiveLinks * 10;

        return score;
    }

    /// <summary>
    /// Enregistre les liaisons d'une quinte comme utilisées
    /// </summary>
    private void RegisterLinks(List<Vector3> positions)
    {
        for (int i = 0; i < positions.Count - 1; i++)
        {
            var link = NormalizeLink(positions[i], positions[i + 1]);
            usedLinks.Add(link);
        }
    }

    /// <summary>
    /// Normalise une liaison pour qu'elle soit toujours dans le même ordre
    /// </summary>
    private (Vector3, Vector3) NormalizeLink(Vector3 pos1, Vector3 pos2)
    {
        if (pos1.x < pos2.x)
            return (pos1, pos2);
        if (pos1.x > pos2.x)
            return (pos2, pos1);
        if (pos1.y < pos2.y)
            return (pos1, pos2);
        return (pos2, pos1);
    }
}