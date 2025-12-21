# Video Poker Game
A feature-rich Video Poker application built with C# and WPF (.NET 8.0).

## Features
- **Multiple Game Variants**:
  - **Jacks or Better**: Classic 9/6 paytable.
  - **Deuces Wild**: "Not So Ugly Ducks" (NSUD) paytable with Wild Twos.
  - **Double Double Bonus**: 9/6 paytable with high payouts for specific Four of a Kind hands and kickers.
- **Double Up**: Risk your winnings in a "High Card" mini-game.
- **Persistent Bankroll**: Credits are saved between sessions.
- **Sound & Visuals**: Card animations, winning hand highlights, and sound effects.
- **Modern Architecture**: MVVM pattern with navigation and polymorphic game logic.

## Getting Started

### Prerequisites
- .NET 8.0 SDK

### Running the Game
1. Clone the repository.
2. Navigate to the project directory:
   ```bash
   cd PokerGame
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

### Running Tests
To run the unit tests (xUnit):
```bash
dotnet test
```

## Controls
- **Select Game**: Choose a variant from the start screen.
- **Bet One / Bet Max**: Adjust your wager (1-5 credits).
- **Deal**: Start a new hand.
- **Click Cards**: Hold/Unhold cards before the Draw.
- **Draw**: Replace unheld cards.
- **Double / Collect**: After a win, choose to risk it or take the credits.
- **Menu**: Return to the game selection screen.

## Architecture
The project uses a clean separation of concerns:
- **Core**: `VideoPokerGame`, `IGameVariant`, `HandEvaluator`, `Deck`, `Card`.
- **UI**: WPF with `ShellViewModel` for navigation and `DataTemplate` for views.
- **Persistence**: JSON-based save system in `%LocalAppData%`.

## License
Open Source.
