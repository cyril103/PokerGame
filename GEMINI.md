# Analyse du Projet PokerGame

## Vue d'ensemble
Application WPF (.NET 8) de Video Poker "Jacks or Better". Logique de jeu isolée de l'UI; Double Up, sons, animations, persistance de bankroll et MVVM léger (MainViewModel/RelayCommand) sont en place. Projet de tests xUnit ajouté.

## Architecture et Structure
### Logique métier (Core)
- `Card.cs` / `Deck.cs` : Modélisent les cartes et le paquet. Mélange via Fisher-Yates avec `RandomNumberGenerator.GetInt32` (CSPRNG).
- `HandEvaluator.cs` : Détermine le rang de main et retourne les cartes gagnantes.
- `VideoPokerGame.cs` : Machine d'états (`WaitingForBet`, `Dealt`, `DoubleUp`, `GameOver`), gestion des mises, calcul des gains, boucle de Double Up, auto top-up à 100 crédits si la bankroll tombe à 0.
- `Bankroll.cs` : Solde du joueur, validation des mises.
- `PersistenceManager.cs` : Sauvegarde JSON dans `%LocalAppData%/PokerGame/savegame.json` (retourne 100 crédits par défaut si fichier absent/corrompu ou crédits <= 0).
- `SoundManager.cs` : Lecture des WAV avec fallback `Console.Beep`.

- `MainWindow.xaml` / `.cs` : Interactions Deal/Draw/Hold/Double/Collect, animations séquentielles, bindings via MainViewModel.
- `MainViewModel.cs` / `RelayCommand.cs` : Expose l'état (crédits, mise, statut, visibilités) et les commandes (Bet One/Max, Deal/Draw, Double, Collect).
- `CardControl.xaml` / `.cs` : Rendu des cartes, flip animation, surbrillance des cartes gagnantes, overlay HELD.

## État actuel
- Jeu jouable avec paytable 9/6, bonus Royal Flush max bet (4000).
- Double Up fonctionnel (re-deal à chaque tentative) ; Collect ou double again supportés.
- Persistance de la bankroll entre sessions, remise à 100 crédits en cas de fichier invalide ou de bust. Chemin de sauvegarde surchargeable pour tests.
- RNG non biaisé via `RandomNumberGenerator.GetInt32`.
- MVVM léger pour la fenêtre principale (bindings sur libellés, visibilités, commandes).
- Projet xUnit `PokerGame.Tests` (net8.0-windows) couvrant évaluation des mains, cartes gagnantes, payouts, persistance et auto top-up.

## Points techniques récents
- Correction biais shuffle (rejet byte supprimé, usage GetInt32).
- Robustesse persistance (fallback si save null/<=0).
- Anti-lockout : auto-dépôt 100 crédits dans `Reset`.
- Couche MVVM ajoutée (MainViewModel/RelayCommand, bindings boutons/labels).
- Suite de tests xUnit créée (hand eval, payouts, persistance, top-up).

## Backlog priorisé
1) Statistiques : fenêtre dédiée (mains jouées, win rate, RTP, plus gros gain) et éventuellement historique succinct.
2) UX/Gameplay : sécuriser les boutons de mise quand crédits insuffisants, clarifier la boucle Double Up (afficher montant en jeu, bouton "double again").
3) Habillage visuel : sprites haute qualité pour les cartes, polish animations.
4) Tests/qualité : étendre xUnit (Double Up avec deck contrôlé, paytable complet, persistance atomique), ajouter CI.
5) Architecture : continuer la montée en MVVM (bindings complets pour l'état de jeu et les commandes, refactor des événements restants si nécessaire).

## Comment lancer
- `dotnet run` depuis la racine.
- `dotnet test PokerGame.Tests/PokerGame.Tests.csproj` pour lancer les tests.
- Actifs audio attendus sous `bin/Debug/net8.0-windows/Assets/Sounds/`.
