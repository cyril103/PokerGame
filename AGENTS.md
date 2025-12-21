# AI Agent Handover Document (AGENTS.md)

## Project Context
**Project Name**: Video Poker (Multi-Variant)
**Framework**: .NET 8.0 (C#)
**UI Framework**: WPF (Windows Presentation Foundation)
**Current State**: Functional Video Poker game supporting "Jacks or Better", "Deuces Wild" and "Double Double Bonus". Features include Game Selection screen, Double Up, bankroll persistence, sound effects, and a robust MVVM architecture with navigation.

## Architecture Overview
The project uses a clean MVVM architecture with a `ShellViewModel` managing navigation between the Game Selection screen and the Game View.

### Core Logic (No UI dependencies)
- **`IGameVariant.cs`**: Interface for game rules (EvaluateHand, CalculatePayout, GetWinningCards).
- **`JacksOrBetterVariant.cs`**: Implements standard 9/6 Jacks or Better rules.
- **`DeucesWildVariant.cs`**: Implements Deuces Wild rules (NSUD paytable, Wild logic).
- **`DoubleDoubleBonusVariant.cs`**: Implements Double Double Bonus rules (9/6 paytable, high quad payouts with kickers).
- **`VideoPokerGame.cs`**: Variant-agnostic game engine. Handles state machine, betting, and dealing.
- **`HandEvaluator.cs`**: Core evaluation logic (extended for Wilds).
- **`Card.cs` / `Deck.cs`**: Card model and CSPRNG shuffling.
- **`Bankroll.cs` / `PersistenceManager.cs`**: Credits management and JSON persistence.

### UI Layer (WPF & MVVM)
- **`ShellViewModel.cs`**: Main navigation controller.
- **`GameSelectionViewModel.cs`**: ViewModel for the start screen.
- **`VideoPokerViewModel.cs`**: ViewModel for the game screen (formerly MainViewModel).
- **`MainWindow.xaml`**: Uses `ContentControl` and `DataTemplate` to switch views based on `ShellViewModel.CurrentViewModel`.
- **`CardControl.xaml`**: Visual representation of cards with animations.

## Key Implementation Details
- **Polymorphism**: `VideoPokerGame` depends on `IGameVariant`, allowing easy addition of new game types.
- **Navigation**: `ShellViewModel` switches the `CurrentViewModel`. `MainWindow` renders the appropriate view using DataTemplates.
- **Deuces Wild**: Special logic for Wild Cards (2s), new hand ranks (Four Deuces, Wild Royal), and specific winning card highlighting.
- **Double Double Bonus**: Special evaluation for Four of a Kind hands based on rank and kicker (e.g., Four Aces with 2, 3, or 4 kicker).
- **UI Polish**: Hand rank names are formatted with spaces (e.g., "Four Aces With Kicker") in the status display.
- **Persistence**: Saves to `%LocalAppData%/PokerGame/savegame.json`. Auto-top-up if credits run out.
- **Double Up**: Outcome now compares card ranks only (suit no longer breaks ties), and a Double Up round cannot be restarted mid-round; UI hides the Double button during Double Up.
- **Testing**: xUnit suite covers all three variants plus Double Up win/push/lose and state guards.

## Current Backlog & Next Steps
1.  **Statistics**: Add a stats view (Win rate, Hands played, etc.).
2.  **Auto-Hold Strategy**: Implement a trainer mode suggesting optimal holds.
3.  **Visual Polish**: Better card assets (sprites), more varied sounds.
4.  **More Variants**: Joker Poker, Bonus Poker (Double Double Bonus already added).
5.  **CI/CD**: Automated build and test pipeline.

## How to Run
- `dotnet run` in the project directory.
- `dotnet test` to run the xUnit test suite.
