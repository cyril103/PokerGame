# Analyse du Projet PokerGame

## Vue d'ensemble
Application WPF (.NET 8) de Video Poker multi-variantes ("Jacks or Better", "Deuces Wild" et "Double Double Bonus"). Architecture MVVM avec navigation, logique de jeu polymorphique, Double Up, sons, animations et persistance de bankroll.

## Architecture et Structure
### Logique métier (Core)
- `IGameVariant.cs` : Interface définissant les règles spécifiques (évaluation, payouts, cartes gagnantes).
- `JacksOrBetterVariant.cs` / `DeucesWildVariant.cs` / `DoubleDoubleBonusVariant.cs` : Implémentations des variantes.
    - Jacks or Better : 9/6 standard.
    - Deuces Wild : "Not So Ugly Ducks" (NSUD).
    - Double Double Bonus : 9/6 avec bonus kickers.
- `VideoPokerGame.cs` : Moteur de jeu agnostique, délègue la logique spécifique à `IGameVariant`.
- `HandEvaluator.cs` : Logique d'évaluation de base (étendue pour supporter les jokers/wilds).
- `Card.cs` / `Deck.cs` : Modèle de cartes et RNG (CSPRNG).
- `Bankroll.cs` / `PersistenceManager.cs` : Gestion et sauvegarde des crédits (`%LocalAppData%/PokerGame/savegame.json`).

### UI / MVVM
- `ShellViewModel.cs` : ViewModel principal gérant la navigation (CurrentViewModel).
- `GameSelectionViewModel.cs` : Écran de choix du jeu.
- `VideoPokerViewModel.cs` : ViewModel du jeu (anciennement MainViewModel), gère l'état de la partie et les commandes.
- `ViewModelBase.cs` : Classe de base implémentant `INotifyPropertyChanged`.
- `MainWindow.xaml` : Fenêtre unique utilisant un `ContentControl` et des `DataTemplate` pour changer de vue.
- `CardControl.xaml` : Affichage des cartes avec animations (Flip) et états (Held, Winning).

## État actuel
- **Variantes** : Jacks or Better (9/6), Deuces Wild (NSUD) et Double Double Bonus (9/6).
- **Navigation** : Écran d'accueil pour choisir le jeu, bouton "MENU" en jeu pour revenir.
- **Gameplay** : Double Up fiabilisé (comparaison sur le rang uniquement, tie = push, reroll bloqué pendant la manche), Auto-Hold (à implémenter plus intelligemment), mise 1-5 crédits.
- **Visuel** : Animations de distribution, retournement des cartes, surbrillance précise des cartes gagnantes.
- **Tests** : Couverture xUnit pour les deux variantes et pour les scénarios Double Up (win/push/lose, garde anti-reroll).

## Points techniques récents
- **Refactoring Polymorphique** : Introduction de `IGameVariant` pour supporter plusieurs jeux sans dupliquer le moteur.
- **Navigation MVVM** : Mise en place de `ShellViewModel` pour passer du menu au jeu sans redémarrer.
- **Deuces Wild** : Implémentation complète avec gestion des 2 comme Wilds, nouvelles mains (Four Deuces, Wild Royal) et paytable ajustée.
- **Double Double Bonus** : Ajout de la variante avec gestion spécifique des carrés (Four of a Kind) et des kickers pour les bonus.
- **Correctifs** : Affichage initial des dos de cartes, correction du bug de redémarrage (réinitialisation des contrôles), Double Up sécurisé (pas de reroll, tie correctement gérée), formatage des noms de mains dans l'UI.

## Backlog priorisé
1) **Statistiques** : Fenêtre dédiée (mains jouées, win rate, RTP par variante).
2) **Stratégie Auto-Hold** : Suggérer les meilleures cartes à garder (Trainer mode).
3) **UX/Polish** : Améliorer les assets graphiques (sprites cartes), sons plus variés.
4) **Plus de variantes** : Joker Poker, Bonus Poker, etc. (Double Double Bonus déjà ajouté).
5) **CI/CD** : Pipeline de build et tests automatisés.

## Comment lancer
- `dotnet run` depuis la racine.
- `dotnet test` pour lancer la suite de tests (incluant les règles Deuces Wild).
