# Analyse du Projet PokerGame

## Vue d'ensemble
Application WPF (.NET 8) de Video Poker "Jacks or Better". Logique de jeu isolée de l'UI; Double Up, sons, animations et persistance de bankroll sont en place.

## Architecture et Structure
### Logique métier (Core)
- `Card.cs` / `Deck.cs` : Modélisent les cartes et le paquet. Mélange via Fisher-Yates avec `RandomNumberGenerator.GetInt32` (CSPRNG).
- `HandEvaluator.cs` : Détermine le rang de main et retourne les cartes gagnantes.
- `VideoPokerGame.cs` : Machine d'états (`WaitingForBet`, `Dealt`, `DoubleUp`, `GameOver`), gestion des mises, calcul des gains, boucle de Double Up, auto top-up à 100 crédits si la bankroll tombe à 0.
- `Bankroll.cs` : Solde du joueur, validation des mises.
- `PersistenceManager.cs` : Sauvegarde JSON dans `%LocalAppData%/PokerGame/savegame.json` (retourne 100 crédits par défaut si fichier absent/corrompu ou crédits <= 0).
- `SoundManager.cs` : Lecture des WAV avec fallback `Console.Beep`.

### Interface utilisateur (WPF)
- `MainWindow.xaml` / `.cs` : Interactions Deal/Draw/Hold/Double/Collect, mise à jour de l'UI, animations séquentielles.
- `CardControl.xaml` / `.cs` : Rendu des cartes, flip animation, surbrillance des cartes gagnantes, overlay HELD.

## État actuel
- Jeu jouable avec paytable 9/6, bonus Royal Flush max bet (4000).
- Double Up fonctionnel (re-deal à chaque tentative) ; Collect ou double again supportés.
- Persistance de la bankroll entre sessions, remise à 100 crédits en cas de fichier invalide ou de bust.
- RNG non biaisé via `RandomNumberGenerator.GetInt32`.

## Points techniques récents
- Correction biais shuffle (rejet byte supprimé, usage GetInt32).
- Robustesse persistance (fallback si save null/<=0).
- Anti-lockout : auto-dépôt 100 crédits dans `Reset`.

## Backlog priorisé
1) Statistiques : fenêtre dédiée (mains jouées, win rate, RTP, plus gros gain) et éventuellement historique succinct.
2) UX/Gameplay : sécuriser les boutons de mise quand crédits insuffisants, clarifier la boucle Double Up (afficher montant en jeu, bouton "double again").
3) Habillage visuel : sprites haute qualité pour les cartes, polish animations.
4) Tests/qualité : migrer `Tests.cs` vers un projet xUnit, couvrir évaluation des mains, payouts, persistance, Double Up (win/lose/push), et ajouter CI.
5) Architecture : préparer une montée en MVVM si la fenêtre principale grossit (bindings pour l'état de jeu, commandes pour actions).

## Comment lancer
- `dotnet run` depuis la racine.
- Actifs audio attendus sous `bin/Debug/net8.0-windows/Assets/Sounds/`.***
