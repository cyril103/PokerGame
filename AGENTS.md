# AI Agent Handover Document (AGENTS.md)

## Project Context
**Project Name**: Video Poker (Jacks or Better)
**Framework**: .NET 8.0 (C#)
**UI Framework**: WPF (Windows Presentation Foundation)
**Current State**: Functional "Jacks or Better" game with sound, visual effects, and Double Up feature.

## Architecture Overview
The project follows a separation of concerns, though currently implemented with Code-Behind (not full MVVM yet).

### Core Logic (No UI dependencies)
- **`Card.cs`**: Represents a playing card (Rank, Suit). Implements `IComparable`.
- **`Deck.cs`**: Manages the deck. Uses `RandomNumberGenerator` (CSPRNG) for secure shuffling.
- **`HandEvaluator.cs`**: Static class to evaluate poker hands (Royal Flush, Straight, etc.) and identify winning cards.
- **`VideoPokerGame.cs`**: Main game controller / State Machine (`WaitingForBet` -> `Dealt` -> `DoubleUp` -> `GameOver`).
- **`Bankroll.cs`**: Manages player credits.
- **`SoundManager.cs`**: Handles audio playback with fallback to system beeps.

### UI Layer (WPF)
- **`MainWindow.xaml` / `.cs`**: Main game window. Handles game loop interactions and UI updates.
- **`CardControl.xaml` / `.cs`**: UserControl for displaying cards. Handles animations (Flip, Win Glow) and "Held" status.

## Key Implementation Details
- **RNG**: We use `System.Security.Cryptography.RandomNumberGenerator` for the shuffle to ensure fairness. Do not replace with `System.Random`.
- **Animations**: Card flips and glows are done via WPF `Storyboard` in `CardControl`.
- **Audio**: `SoundManager` looks for .wav files in `Assets/Sounds`. If missing, it uses `Console.Beep` on a background thread.

## Current Backlog & Next Steps
If you are picking up this project, here are the immediate priorities (derived from `GEMINI.md`):

1.  **Persistence (High Priority)**:
    - Implement saving/loading of `Bankroll` and game state to a local file (JSON/SQLite) so credits are preserved between sessions.
2.  **Statistics**:
    - Add a new Window/View to show stats: Hands played, Win rate, RTP, Best win.
3.  **Visual Polish**:
    - Replace vector card rendering with high-quality sprite assets.
4.  **Refactoring**:
    - Migrate `Tests.cs` to a proper xUnit test project.
    - Consider refactoring `MainWindow` to MVVM pattern if complexity grows.

## How to Run
- `dotnet run` in the project directory.
- Ensure `.wav` assets are in `bin/Debug/net8.0-windows/Assets/Sounds/` for full audio experience.
