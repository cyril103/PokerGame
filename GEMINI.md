# Analyse du Projet PokerGame

## Vue d'ensemble
Le projet est une application WPF implémentant un jeu de **Video Poker (Jacks or Better)**. Il est développé en C# (.NET 8.0) et suit une architecture séparant la logique métier de l'interface utilisateur.

## Architecture et Structure

### Logique Métier (Core)
La logique du jeu est encapsulée dans des classes C# pures, sans dépendance à WPF, ce qui est une excellente pratique pour la testabilité et la portabilité.
- **`Card.cs` & `Deck.cs`** : Modélisent les cartes et le paquet.
  - **Point fort** : Utilisation de `RandomNumberGenerator` (CSPRNG) pour le mélange, garantissant une distribution équitable et sécurisée, essentielle pour un jeu de hasard.
- **`HandEvaluator.cs`** : Contient la logique de détermination de la main de poker (Paire, Brelan, Suite, etc.).
  - **Point fort** : Couvre tous les rangs de main standard du poker.
- **`VideoPokerGame.cs`** : Gère l'état du jeu (Machine à états : `WaitingForBet` -> `Dealt` -> `GameOver`), les paris et le calcul des gains.
- **`Bankroll.cs`** : Gère les crédits du joueur.

### Interface Utilisateur (UI)
L'interface est réalisée en WPF (`MainWindow.xaml`).
- **Design** : Utilisation de dégradés et d'effets d'ombre pour un look "Casino".
- **`CardControl`** : Un UserControl personnalisé pour l'affichage des cartes.
  - **Rendu** : Les cartes sont dessinées vectoriellement (XAML) avec des animations de retournement (`FlipTo`) basées sur `Storyboard`. C'est léger et performant.
- **Interaction** : Gestion des clics pour "garder" (Hold) les cartes, avec mise à jour visuelle immédiate.

## Qualité du Code
- **Lisibilité** : Le code est propre, bien nommé et facile à suivre.
- **Séparation des responsabilités** : La logique de jeu ne connaît pas l'UI, et l'UI se contente d'afficher l'état du jeu.
- **Robustesse** : Gestion des exceptions présente pour les cas invalides (mises illégales, etc.).

## Fonctionnalités Actuelles
- [x] Jeu complet "Jacks or Better" (9/6 Paytable standard implémentée).
- [x] Gestion des paris (1 à 5 crédits) avec bonus pour la Quinte Flush Royale à mise max.
- [x] Distribution et tirage (Deal/Draw).
- [x] Système de "Hold" pour garder les cartes.
- [x] Animations de distribution et de retournement des cartes.
- [x] Gestion de la bankroll (crédits).
- [x] Double Up (Quitte ou Double) fonctionnel.
- [x] Effets sonores (SoundManager avec fallback).
- [x] Feedback visuel (Mise en évidence des cartes gagnantes).

## Recommandations d'Amélioration (Backlog)

### Priorité Haute
1.  **Sauvegarde et Persistance** :
    - Sauvegarder la bankroll et l'état du jeu (en cours de partie) dans un fichier local (JSON ou SQLite).
    - Charger automatiquement au démarrage pour ne pas perdre les crédits.

### Gameplay & Contenu
2.  **Système de Statistiques** :
    - Créer un écran dédié affichant : Nombre de mains jouées, Plus gros gain, Historique des mains (Carré, Quinte Flush...), Taux de retour (RTP).
3.  **Variantes de Jeu** :
    - Ajouter un menu pour choisir entre "Jacks or Better", "Deuces Wild" (2 = Joker), ou "Joker Poker".

### Interface / UX
4.  **Refonte Graphique** :
    - Remplacer le rendu vectoriel simple par des sprites de cartes (.png) ou un design XAML plus riche (texture papier, figures dessinées).
    - Ajouter des animations plus fluides pour la distribution.

### Technique
5.  **Tests Unitaires Robustes** :
    - Migrer `Tests.cs` vers un projet xUnit séparé.
    - Couvrir tous les cas limites de `HandEvaluator`.
6.  **Architecture MVVM** :
    - Refactoriser pour séparer la vue (`MainWindow`) du modèle de vue, facilitant l'ajout de nouvelles fonctionnalités complexes.

## Conclusion
Le projet est une base solide et bien conçue. L'implémentation actuelle est fonctionnelle et respecte les bonnes pratiques de développement (CSPRNG, séparation logique/UI). L'ajout récent des effets sonores et visuels a grandement amélioré l'expérience utilisateur ("Juice"). Les prochaines étapes pourraient inclure la persistance des données et des statistiques avancées.
