using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DictionaryStorage
{
    // Méthode pour sauvegarder un dictionnaire dans un fichier JSON
    public static void SaveDictionaryToFile<T, U>(Dictionary<T, U> dictionary, string fileName)
    {
        try
        {
            // Chemin où on veut sauvegarder le fichier (dans le dossier persistant d'Unity)
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            // Sérialisation du dictionnaire en JSON
            string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);

            // Écriture du JSON dans un fichier local
            File.WriteAllText(filePath, json);

            Debug.Log($"Le dictionnaire a été sauvegardé avec succès dans {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors de la sauvegarde du dictionnaire : {ex.Message}");
        }
    }

    // Méthode pour charger un dictionnaire depuis un fichier JSON
    public static Dictionary<T, U> LoadDictionaryFromFile<T, U>(string fileName)
    {
        try
        {
            // Chemin du fichier
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            // Vérifie si le fichier existe
            if (File.Exists(filePath))
            {
                // Lecture du contenu du fichier JSON
                string json = File.ReadAllText(filePath);

                // Désérialisation du JSON en dictionnaire
                Dictionary<T, U> dictionary = JsonConvert.DeserializeObject<Dictionary<T, U>>(json);

                Debug.Log("Le dictionnaire a été chargé avec succès.");
                return dictionary;
            }
            else
            {
                Debug.LogWarning("Le fichier n'existe pas.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors du chargement du dictionnaire : {ex.Message}");
            return null;
        }
    }
}