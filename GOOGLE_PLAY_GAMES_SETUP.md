# Google Play Games Services - Guide d'Intégration

**Objectif** : Intégrer les leaderboards (classements) Google Play Games pour permettre aux joueurs de voir leur position globale.

**Status** : 🟢 Fonctionnel

---

## 📋 Vue d'ensemble

### Ce qu'on va mettre en place

1. **Leaderboard principal** : Score global (tous niveaux confondus avec multiplicateurs d'étoiles)
2. **Affichage in-game** :
   - Top 3 (podium)
   - Score du joueur avec contexte (position -1, sa position, position +1)
3. **Mise à jour automatique** : À chaque fin de niveau

---

## 🎯 Étape 1 : Configuration Google Play Console

### Informations du projet

**Projet Google Cloud** : Marble5 Game Services
- **ID du projet** : `marble5-game-services`
- **Numéro du projet / App ID** : `844807100158`
- **Lien direct** : https://console.cloud.google.com/home/dashboard?project=marble5-game-services

**Application Android** :
- **Package Name** : `com.OneMoreMoveStudio.Marble5`
- **SHA-1 (release keystore)** : `0B:F7:4B:E7:F1:44:57:04:C9:B3:D1:30:D7:CC:7E:AE:FA:9A:0A:FB`
- **Keystore** : `D:\Unity Manu\Marble5\Keystores\marble5-release.keystore` (alias: `marble5`)

**OAuth Client ID** : `844807100158-fu2ljmmjtftbgoae9fvsq4d0csns40kv.apps.googleusercontent.com`

**Leaderboard "Global Score"** :
- **ID** : `CgkI_q21k8sYEAIQAQ`
- **Ordre** : Score le plus élevé en premier
- **Protection contre la falsification** : Activée

### 1.1 - Accéder à Play Games Services

1. Va sur [Google Play Console](https://play.google.com/console)
2. Sélectionne ton application **Marble5**
3. Dans le menu latéral, cherche **"Play Games Services"** ou **"Services de jeu Play"**
   - Si tu ne le vois pas : **Développer > Services et API > Play Games Services**

### 1.2 - Activer Play Games Services

1. Clique sur **"Configurer Play Games Services"**
2. Active le service pour ton application
3. Note bien ton **Application ID** (tu en auras besoin dans Unity)

### 1.3 - Créer les Leaderboards

#### Leaderboard 1 : Score Global

1. Dans Play Games Services, va dans **"Classements"** (Leaderboards)
2. Clique sur **"Créer un classement"**
3. Remplis les informations :

| Champ | Valeur suggérée |
|-------|-----------------|
| **Nom** | Global Score |
| **ID** | `CgkI_q21k8sYEAIQAQ` |
| **Description** | Classement par score total tous niveaux |
| **Ordre** | Plus grand est mieux (Larger is better) |
| **Format du score** | Numérique (Numeric) |
| **Icône** | Icône de ton jeu ou une étoile |

4. **Sauvegarde** le leaderboard
5. **Note l'ID** généré (ex: `CgkI...` ou tu peux utiliser le nom personnalisé)

#### (Optionnel) Leaderboards par niveau

Si tu veux aussi des classements par niveau (recommandé pour plus tard) :
- `leaderboard_level_1`, `leaderboard_level_2`, etc.
- Même configuration que le global

### 1.4 - Créer les Réussites (Achievements) - À FAIRE À TERME

**Note** : À implémenter après les leaderboards

1. Dans Play Games Services, va dans **"Réussites"** (Achievements)
2. Crée des achievements pour récompenser les joueurs :

**Exemples d'achievements** :
- "Première Quinte" : Former sa première quinte
- "Maître des Doubles" : Former 10 doubles quintes
- "Perfection" : Obtenir 3 étoiles sur un niveau
- "Collectionneur" : Obtenir 3 étoiles sur tous les niveaux
- "Expert" : Atteindre 10 000 points de score global
- "Légende" : Atteindre le top 10 du classement

Pour chaque achievement :
- **Nom** : Nom visible par le joueur
- **Description** : Comment le débloquer
- **Icône** : Image de l'achievement (verrouillée/déverrouillée)
- **Points XP** : Récompense en points d'expérience Google Play
- **Type** : Instantané (déblocage unique) ou Progressif (avec paliers)

### 1.5 - Obtenir les identifiants OAuth 2.0

1. Dans Google Play Console, va dans **"Identifiants OAuth"** ou **"Credentials"**
2. Tu dois avoir un **Client OAuth Android**
3. Si ce n'est pas le cas :
   - Va dans [Google Cloud Console](https://console.cloud.google.com/)
   - Sélectionne ton projet lié à Marble5
   - **APIs & Services > Credentials**
   - Crée un **OAuth 2.0 Client ID** de type **Android**
   - Entre ton **Package Name** : `com.OneMoreMoveStudio.Marble5`
   - Entre ton **SHA-1** du keystore de signature (voir ci-dessous)

#### Obtenir le SHA-1 de ton keystore

```bash
# Si tu utilises le keystore de debug Unity
keytool -list -v -keystore "%USERPROFILE%\.android\debug.keystore" -alias androiddebugkey -storepass android -keypass android

# Si tu as ton propre keystore de release
keytool -list -v -keystore "CHEMIN_VERS_TON_KEYSTORE.keystore" -alias TON_ALIAS
```

Copie le **SHA-1** et ajoute-le dans les credentials OAuth.

---

## 🎮 Étape 2 : Installation dans Unity

### 2.1 - Installer le plugin Google Play Games

1. Ouvre Unity (ton projet Marble5)
2. Va dans **Window > Package Manager**
3. Change le filtre en haut à gauche sur **"Unity Registry"**
4. Cherche **"Google Play Games"** ou **"Play Games Services"**
5. **Installe** le package `com.google.play.games`

**Alternative si pas disponible** :
- Télécharge le plugin depuis [GitHub](https://github.com/playgameservices/play-games-plugin-for-unity)
- Importe le `.unitypackage` dans ton projet

### 2.2 - Configurer le plugin

1. Une fois installé, va dans **Window > Google Play Games > Setup > Android Setup**
2. Entre les informations :
   - **Application ID** : `844807100158`
   - **Package Name** : `com.OneMoreMoveStudio.Marble5`
3. Clique sur **"Setup"**
4. Le plugin va générer un fichier `GooglePlayGamesPluginConfiguration.xml` dans `Assets/Plugins/Android/`

### 2.3 - Vérifier les dépendances

Le plugin doit ajouter automatiquement les dépendances dans `mainTemplate.gradle`. Vérifie que tu as :
```gradle
dependencies {
    implementation 'com.google.android.gms:play-services-games:23.1.0'
    implementation 'com.google.android.gms:play-services-auth:20.7.0'
}
```

---

## 💻 Étape 3 : Implémentation dans le code

### 3.1 - Créer le LeaderboardManager

Je vais créer un script `LeaderboardManager.cs` qui gérera :
- L'authentification Google Play Games
- La soumission des scores
- La récupération des scores pour l'affichage

### 3.2 - Modifier GameManager

Ajouter l'appel au `LeaderboardManager` pour :
- Se connecter au démarrage
- Soumettre le score global après chaque niveau

### 3.3 - Créer l'UI du classement

Créer un panel dans `UIManager` pour afficher :
- Top 3 (podium)
- Position du joueur avec contexte

---

## 🧪 Étape 4 : Tests

### 4.1 - Test en développement

1. **Build Android** avec le keystore de debug
2. Installe sur un **vrai device Android** (pas l'émulateur)
3. Le joueur doit être connecté à un compte Google
4. Vérifie que :
   - L'authentification fonctionne
   - Les scores sont soumis
   - Le leaderboard s'affiche

### 4.2 - Test en bêta

1. Ajoute des testeurs dans ta **bêta fermée** Play Console
2. Publie une nouvelle version avec Play Games Services
3. Les testeurs doivent pouvoir :
   - Se connecter
   - Voir les scores des autres testeurs
   - Comparer leurs positions

---

## 📊 Architecture proposée

```
GameManager
    └── LeaderboardManager (nouveau singleton)
            ├── Authenticate() : Connexion au démarrage
            ├── SubmitScore(int score) : Envoie le score global
            ├── LoadLeaderboard() : Charge les données
            └── GetPlayerContext() : Récupère position -1, joueur, +1

UIManager
    └── LeaderboardPanel (nouveau panel)
            ├── Top3Display : Affiche le podium
            └── PlayerContextDisplay : Affiche le contexte du joueur
```

---

## ✅ Checklist de progression

### Configuration Google Play Console
- [x] Play Games Services activé
- [x] Leaderboard "Global Score" créé (ID: `CgkI_q21k8sYEAIQAQ`)
- [x] OAuth 2.0 configuré avec SHA-1
- [x] Identifiants liés dans Play Console
- [x] Propriétés requises remplies (description, catégorie, icône, image)
- [x] Play Games Services **publié** (plus en brouillon)

### Installation Unity
- [x] Plugin Google Play Games v2.1.0 installé
- [x] Configuration Android Setup complétée
- [x] Dépendances Gradle vérifiées (`com.google.games:gpgs-plugin-support:2.1.0`)

### Implémentation Code
- [x] `LeaderboardManager.cs` créé (`Assets/Scripts/LeaderboardManager.cs`)
- [x] `GameManager.cs` modifié pour soumettre les scores au game over
- [x] `UIManager.cs` avec méthode `ShowLeaderboard()`
- [x] Bouton "Classement" créé dans le menu principal
- [x] Bouton masqué pendant le jeu (visible uniquement au menu)

### Tests
- [x] Build Android (APK 0.4.2)
- [x] Authentification testée sur device ✅ (popup "Bonjour [compte]" au lancement)
- [x] Bouton leaderboard ouvre l'interface Google Play Games ✅
- [x] Soumission de score ✅
- [ ] Tests avec plusieurs comptes

---

## 🔗 Ressources utiles

- [Documentation officielle Google Play Games](https://developers.google.com/games/services)
- [Unity Plugin GitHub](https://github.com/playgameservices/play-games-plugin-for-unity)
- [Tutoriel leaderboards](https://developers.google.com/games/services/android/leaderboards)

---

## 🐛 Problèmes fréquents

### "Failed to authenticate"
- Vérifie que le SHA-1 correspond à ton keystore
- Vérifie que le Package Name est identique partout
- Attends 24h après configuration OAuth (propagation Google)

### "Leaderboard not found"
- Vérifie l'ID du leaderboard
- Assure-toi que le leaderboard est publié dans Play Console

### Scores ne s'affichent pas
- En bêta fermée, seuls les testeurs inscrits voient les scores
- Vérifie que le joueur est bien authentifié

---

## ✅ Problème résolu : Profil privé

### Cause
Le profil Google Play Games de l'utilisateur était en mode **privé**, ce qui empêchait l'affichage des scores dans le leaderboard public.

### Solution
Dans l'écran du leaderboard, cliquer sur "Afficher votre score" pour rendre le profil public.

### Note importante
C'est un paramètre personnel de chaque joueur. La plupart des joueurs ont un profil public par défaut. Seuls ceux qui ont choisi de masquer leur activité de jeu devront activer cette option.

---

## 📝 Notes

- **Délai de propagation** : Les changements dans Play Console peuvent prendre jusqu'à 24h
- **Bêta fermée** : Les leaderboards fonctionnent, mais uniquement entre testeurs
- **Production** : Une fois publié, les leaderboards seront publics
- **Hors ligne** : Prévoir un système de cache si le joueur n'est pas connecté

---

## 🎯 Prochaines étapes

Une fois cette checklist complétée, on pourra :
1. Tester avec tes beta-testeurs
2. Ajuster l'UI selon les retours
3. Ajouter des leaderboards par niveau si souhaité
4. Intégrer des achievements (succès) Google Play Games
