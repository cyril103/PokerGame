# AI Agent Handover Document (AGENTS.md)

## Project Context
**Project Name**: Video Poker (Jacks or Better)
**Framework**: .NET 8.0 (C#)
**UI Framework**: WPF (Windows Presentation Foundation)
**Current State**: Functional "Jacks or Better" game with sound, visual effects, Double Up, bankroll persistence with auto top-up, xUnit test project, and a light MVVM layer (MainViewModel + RelayCommand) driving UI bindings.

## Architecture Overview
The project follows a separation of concerns, now with a light MVVM layer for the main window (bindings for status, bet, visibility, commands).

### Core Logic (No UI dependencies)
- **`Card.cs`**: Represents a playing card (Rank, Suit). Implements `IComparable`.
- **`Deck.cs`**: Manages the deck. Uses `RandomNumberGenerator.GetInt32` (CSPRNG) for unbiased Fisher-Yates shuffling.
- **`HandEvaluator.cs`**: Static class to evaluate poker hands (Royal Flush, Straight, etc.) and identify winning cards.
- **`VideoPokerGame.cs`**: Main game controller / State Machine (`WaitingForBet` -> `Dealt` -> `DoubleUp` -> `GameOver`).
- **`Bankroll.cs`**: Manages player credits.
- **`SoundManager.cs`**: Handles audio playback with fallback to system beeps.

### UI Layer (WPF)
- **`MainWindow.xaml` / `.cs`**: Main game window. Handles game loop interactions, animations, and binds to `MainViewModel`.
- **`MainViewModel.cs` / `RelayCommand.cs`**: MVVM glue exposing state (credits, bet, status, buttons visibility) and commands (Bet One/Max, Deal/Draw, Double, Collect).
- **`CardControl.xaml` / `.cs`**: UserControl for displaying cards. Handles animations (Flip, Win Glow) and "Held" status.

## Key Implementation Details
- **RNG**: We use `System.Security.Cryptography.RandomNumberGenerator.GetInt32` for the shuffle to ensure fairness. Do not replace with `System.Random`.
- **Animations**: Card flips and glows are done via WPF `Storyboard` in `CardControl`.
- **Audio**: `SoundManager` looks for .wav files in `Assets/Sounds`. If missing, it uses `Console.Beep` on a background thread.
- **Persistence**: `PersistenceManager` saves bankroll to `%LocalAppData%/PokerGame/savegame.json`; loads defaults if missing/corrupt or credits <= 0. `VideoPokerGame.Reset` auto-deposits 100 credits if bankroll is empty to avoid lockout. Path override supported for tests.
- **Tests**: `PokerGame.Tests` (xUnit, net8.0-windows) covers hand evaluation, payouts, persistence defaults, and auto top-up.
- **MVVM**: `MainViewModel` drives bindings for labels/visibilities and commands instead of direct button event handlers.

## Current Backlog & Next Steps
If you are picking up this project, here are the immediate priorities:

1.  **Statistics**:
    - Add a new Window/View to show stats: Hands played, Win rate, RTP, Best win.
2.  **UX/Gameplay**:
    - Improve Double Up UX (clear loop for double-again/collect, show amount in play) and enforce bet controls when credits are low.
3.  **Visual Polish**:
    - Replace vector card rendering with high-quality sprite assets.
4.  **Refactoring/Tests**:
    - Extend xUnit suite: Double Up deterministic deck injection, more paytable cases, UI-layer MVVM tests if feasible.
    - Continue MVVM refactor if UI logic grows.

## How to Run
- `dotnet run` in the project directory.
- Ensure `.wav` assets are in `bin/Debug/net8.0-windows/Assets/Sounds/` for full audio experience.
