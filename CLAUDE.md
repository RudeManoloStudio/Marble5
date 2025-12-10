# Marble5 - Guide de Développement

## Description du Projet

Marble5 est un jeu de puzzle Unity où le joueur place des billes sur une grille pour former des "quintes" (alignements de 5 billes). Le jeu inclut un système de score, d'étoiles, de progression par niveaux et de bloqueurs pour augmenter la difficulté.

**Version** : Private beta live

---

## Structure du Projet

```
Assets/
├── Scripts/                    # 31 scripts C#
│   ├── BillesControllers/      # Contrôleurs visuels des billes
│   │   ├── BilleController.cs  # Base : rotation, physique explosion
│   │   ├── PlombController.cs  # Bloqueurs
│   │   ├── QuinteController.cs # Lignes de quinte
│   │   ├── StarController.cs   # Étoiles visuelles
│   │   └── EtoileController.cs
│   ├── Data/                   # ScriptableObjects
│   │   ├── LevelData.cs        # Configuration niveaux
│   │   ├── ScoreData.cs        # Valeurs de score
│   │   ├── RankingData.cs      # Rangs et seuils
│   │   ├── FXData.cs           # Références audio FX
│   │   ├── MusicData.cs        # Playlist musique
│   │   └── MotifData.cs        # Motifs initiaux
│   ├── UI/                     # Interface utilisateur
│   │   ├── ScoreBarController.cs
│   │   ├── StarRecapController.cs
│   │   ├── LevelSelector.cs
│   │   ├── UpdateScoreIncrement.cs
│   │   └── ReserveVisualController.cs
│   ├── GameManager.cs          # Singleton central
│   ├── UIManager.cs            # Gestion panneaux UI
│   ├── PlaceBille.cs           # Logique placement billes
│   ├── PlacePlomb.cs           # Placement bloqueurs
│   ├── ReserveController.cs    # File d'attente billes
│   ├── DisplayController.cs    # Rendu plateau
│   ├── EventManager.cs         # Système événements
│   ├── QuinteDetector.cs       # Détection alignements
│   ├── CameraController.cs     # Zoom/pan caméra
│   ├── CameraShake.cs          # Effet tremblement
│   ├── MusicManager.cs         # Musique de fond
│   ├── FXManager.cs            # Effets sonores
│   ├── DictionaryStorage.cs    # Persistance scores (SHA256)
│   ├── UserDataManager.cs      # Préférences utilisateur
│   └── PrototypeManager.cs     # Gestion expiration
├── Prefabs/                    # Prefabs du jeu
├── Sprites/                    # Assets graphiques
├── Sounds/                     # Assets audio
└── WanzyeeStudio/              # Librairie JSON tierce
```

---

## Architecture

### Patterns Utilisés

- **Singleton** : `GameManager.Instance`, `EventManager.instance`
- **Event System** : Communication découplée via `EventManager`
- **ScriptableObjects** : Configuration externalisée des niveaux/données
- **Component-Based** : Séparation Display / Logic / Physics

### Flux de Données Principal

```
GameManager (hub central)
    ├── UIManager (états UI)
    ├── DisplayController (rendu)
    ├── PlaceBille (logique jeu)
    ├── ReserveController (file d'attente)
    └── EventManager (événements)
```

### Événements Principaux

| Événement | Données | Description |
|-----------|---------|-------------|
| `PoseBille` | Vector3 | Bille placée |
| `QuinteFormee` | int | Quinte détectée (nombre) |
| `LevelSelected` | int | Niveau choisi |
| `ReturnToMainMenu` | - | Retour menu |
| `DropBilles` | - | Animation chute game-over |
| `ExplodeBilles` | - | Animation explosion |
| `MusicVolumeChanged` | float | Volume musique changé |
| `FxVolumeChanged` | float | Volume FX changé |
| `GameOver` | - | Fin de partie |
| `NoPoseBille` | - | Placement invalide |

---

## Conventions de Code

### Nommage

- **Classes** : PascalCase (`GameManager`, `PlaceBille`)
- **Méthodes publiques** : PascalCase (`Setup()`, `PrepareLevel()`)
- **Méthodes privées** : PascalCase ou camelCase (`_OnPoseBille()`, `ResetLiaisons()`)
- **Callbacks événements** : Préfixe `_On` (`_OnDropBilles`, `_OnExplodeBilles`)
- **Variables privées** : camelCase (`gridSize`, `compteurBilles`)
- **Constantes** : SCREAMING_SNAKE_CASE (`TOUCH_MOVE_THRESHOLD`)
- **SerializeField** : camelCase avec préfixe descriptif

### Structure des Classes

```csharp
public class ExempleController : MonoBehaviour
{
    // 1. SerializeField (configuration Inspector)
    [SerializeField] private Type nomVariable;

    [Header("Section")]
    [SerializeField] private Type autreVariable;

    // 2. Variables privées
    private Type variableInterne;

    // 3. Propriétés publiques (getter only)
    public Type Propriete { get { return variableInterne; } }

    // 4. Singleton (si applicable)
    public static ExempleController Instance { get; private set; }

    // 5. Unity Lifecycle (Awake, Start, Update...)
    private void Awake() { }
    private void Start() { }
    private void Update() { }

    // 6. Méthodes publiques
    public void Setup() { }

    // 7. Méthodes privées
    private void MethodeInterne() { }

    // 8. Callbacks événements
    private void _OnEvenement() { }
}
```

### Langue

- **Code** : Anglais (noms de classes, méthodes)
- **Variables métier** : Français accepté (`compteurBilles`, `difficulte`, `liaisonsUtilisées`)
- **Commentaires** : Français

### SerializeField

- Toujours `private` avec `[SerializeField]` (jamais `public`)
- Utiliser `[Header("Section")]` pour organiser l'Inspector
- Types références : managers, prefabs, données
- Types valeurs : durées, vitesses, seuils

---

## Systèmes Clés

### Détection de Quinte

Une quinte = 5 billes alignées. Algorithme dans `PlaceBille.cs` :

1. Vérifie 4 directions : →, ↑, ↗, ↘
2. Pour chaque direction : teste les 5 positions possibles
3. Valide que les 5 positions ont des billes ET les liaisons ne sont pas réutilisées
4. Double quinte possible (score bonus)

### Système de Bloqueurs (Plombs)

- Spawn après N billes placées (paramètre `Difficulte`)
- Apparaît dans une position adjacente à la dernière bille
- Bloque le placement mais compte pour remplir la grille

### Réserve (File d'attente)

- FIFO de billes et bloqueurs
- Affichage max 10 items
- Physique : gravité vers la droite
- Animations grow/shrink

### Scoring

- Score incrémente avec quintes consécutives
- 3 seuils étoiles par niveau (`FirstStarScore`, `SecondStarScore`, `ThirdStarScore`)
- Sauvegarde chiffrée avec hash SHA256

### Score Global

Le score global est calculé en sommant les scores de tous les niveaux, avec un multiplicateur selon le nombre d'étoiles obtenues :

| Étoiles | Multiplicateur | Bonus |
|---------|----------------|-------|
| 0 ★     | ×0             | 0%    |
| 1 ★     | ×1             | 0%    |
| 2 ★★    | ×1.5           | +50%  |
| 3 ★★★   | ×2             | +100% |

**Formule** : `Score Global = Σ (score_niveau × multiplicateur_étoiles)`

**Exemple** :
- Niveau 1 : 100 pts avec 2★ → 100 × 1.5 = 150
- Niveau 2 : 200 pts avec 3★ → 200 × 2.0 = 400
- Niveau 3 : 150 pts avec 1★ → 150 × 1.0 = 150
- **Score Global = 700**

Implémenté dans `GameManager.CalculateGlobalScore()` et affiché dans le menu principal via `UIManager.SetMainPanel()`.

---

## Configuration des Niveaux

Via `LevelData` ScriptableObject :

```csharp
public class Layer
{
    public Vector2Int GridSize;      // Taille grille
    public int FirstStarScore;       // Seuil 1 étoile
    public int SecondStarScore;      // Seuil 2 étoiles
    public int ThirdStarScore;       // Seuil 3 étoiles
    public GameObject Bille;         // Prefab bille
    public GameObject Plomb;         // Prefab bloqueur
    public GameObject Quinte;        // Prefab ligne quinte
    public Sprite BackgroundTexture; // Fond
    public MotifData Motif;          // Billes initiales
    public int Handicap;             // Billes initiales en moins
    public int Difficulte;           // Fréquence bloqueurs
}
```

---

## Commandes Utiles

### Tester le jeu

- **Mode développeur** : Débloque tous les niveaux (toggle dans préférences)
- **Coins infinis** : `infinisCoins = true` dans GameManager (Inspector)

### Persistance

- **Scores** : `Application.persistentDataPath/scores.json` (hashé)
- **Préférences** : `Application.persistentDataPath/UserPreferences.json`

---

## Plateformes Cibles

- Android (touch multi-point)
- Éditeur Unity (souris)

### Gestion Input

- **Touch** : Seuils anti-zoom accidentel (`TOUCH_MOVE_THRESHOLD`)
- **Souris** : Clic gauche place, clic droit pan, molette zoom

---

## Dépendances Externes

- **WanzyeeStudio** : Sérialisation JSON pour types complexes
- **Newtonsoft.Json** : Via WanzyeeStudio

---

## Notes Importantes

1. **Private beta live** : Le jeu est en bêta privée (voir `PrototypeManager`)
2. **Sécurité scores** : Hash SHA256 avec clé secrète pour éviter la triche
3. **EventManager requis** : Doit être présent dans la scène
4. **GameManager singleton** : `DontDestroyOnLoad`, un seul dans la hiérarchie
