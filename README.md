# 🎰 Unity Slot Machine Game

A playable slot machine game built in Unity as part of a game development assignment.

The project implements a complete slot machine gameplay loop with randomized outcomes, smooth reel movement, selectable betting amounts, weighted payouts, audio feedback, win presentation, and a dedicated Game Over flow.

---

## 🎮 Game Overview

The player starts with a limited number of credits and can select one of three betting amounts:

- **10 Credits**
- **20 Credits**
- **50 Credits**

After selecting a bet, the machine automatically:

1. Accepts the selected bet.
2. Animates the lever.
3. Starts the reel spin.
4. Stops the three reels sequentially.
5. Determines the final symbols.
6. Calculates the payout using the symbol's reward weight.
7. Updates the player's credits.
8. Displays a win presentation when applicable.

When the player's credits reach zero, the game transitions to the **Game Over** screen.

---

## ✨ Features

### 🎰 Slot Machine Gameplay

- Three independently animated reels.
- Randomized final outcomes using Unity's random number generation.
- Target symbols align with the center of the reel when the reels stop.
- Sequential/staggered reel stopping for a more natural slot-machine feel.
- Smooth reel movement using easing and a settling effect.
- Controlled visual symbol sequences during the spin while final outcomes remain randomized.

### 💰 Betting System

| Bet | Value |
|---|---:|
| Small | 10 Credits |
| Medium | 20 Credits |
| Large | 50 Credits |

Selecting a bet automatically starts the machine. The betting UI provides hover, pressed, selected, and locked visual states.

### 🏆 Winning System

A win occurs when all three reels display the same symbol.

The payout is calculated using:

```text
Total Win = Bet Amount × Symbol Reward Weight
```

The reward weights are defined in the game's payout configuration.

### 🔊 Audio Feedback

Dedicated audio is provided for:

- Lever activation
- Slot machine spinning
- Jackpot/winning result

### 🥳 Win Presentation

When the player wins, a presentation appears above the machine:

```text
YOU WIN <amount> CREDITS
```

The betting controls remain locked while the jackpot presentation is active and are restored after the jackpot audio finishes.

### 💀 Game Over

When the player's credits reach zero, the game transitions to a dedicated Game Over scene containing:

```text
GAME OVER

RESTART
```

The Restart button returns the player to the Main scene.

---

## 🕹️ How to Play

The game is controlled using the mouse.

### Place a Bet

1. Hover over a betting option.
2. Select **10**, **20**, or **50 Credits**.
3. The selected bet is applied.
4. The lever automatically animates.
5. The reels begin spinning.
6. Wait for the reels to stop and see the result.

### Winning

If all three reels show the same symbol, the player receives a payout based on:

```text
Bet Amount × Symbol Reward Weight
```

### Game Over

When the player's credits reach zero:

1. The Game Over screen appears.
2. Click **RESTART**.
3. The game returns to the Main scene.

---

## 🧠 Technical Approach

The project uses a component-based Unity architecture with separate responsibilities for the major gameplay systems.

### Main Architecture

```text
GameManager
├── Wallet
├── ReelManager
│   └── ReelColumn × 3
├── PayoutTable
├── BetButtonController
├── LeverController
├── SlotAudioController
├── WinPresentationController
└── UIManager
```

### Gameplay Flow

```text
Player selects bet
        ↓
BetButtonController
        ↓
Set bet amount
        ↓
GameManager.Spin()
        ↓
Deduct bet
        ↓
Generate random outcomes
        ↓
ReelManager.StartSpin()
        ↓
ReelColumn × 3
        ↓
Sequential reel stopping
        ↓
Evaluate final symbols
        ↓
PayoutTable
        ↓
Calculate winnings
        ↓
Update Wallet / UI
        ↓
Win or Loss presentation
```

---

## 🎲 RNG & Winning Logic

The final reel outcomes are generated independently from the visual scrolling sequence.

This separates:

- **Gameplay outcome generation**
- **Visual reel presentation**

The final result remains randomized while the symbols shown during the scrolling animation follow a controlled visual sequence. This prevents visually awkward repeated symbols while keeping outcome generation separate from presentation.

A winning combination requires:

```text
Reel 1 = Reel 2 = Reel 3
```

The payout is calculated using:

```text
Payout = Bet × Reward Weight
```

---

## 🎞️ Reel Animation

Each reel is represented by a `ReelColumn`.

When a spin begins, each reel:

1. Builds its visual symbol strip.
2. Places the selected target symbol at the required stopping position.
3. Scrolls the strip smoothly.
4. Uses easing to slow the movement.
5. Applies a small settling effect.
6. Stops with the target symbol aligned in the center.

The three reels use staggered stop delays to create a more natural slot-machine presentation.

---

## 🔊 Audio System

Audio is handled through a centralized `SlotAudioController`.

The controller manages the major gameplay sounds:

```text
Lever Sound
     ↓
Spin Sound
     ↓
Jackpot Sound
```

The system keeps normal spin audio and jackpot audio from overlapping incorrectly and uses the jackpot audio completion to control the end of the win presentation.

---

## 🏗️ Project Structure

```text
Assets/
├── Editor/
│   └── BuildScene.cs
│
├── Scripts/
│   ├── Core/
│   │   └── GameManager.cs
│   ├── Reel/
│   │   ├── ReelColumn.cs
│   │   └── ReelManager.cs
│   ├── UI/
│   │   ├── UIManager.cs
│   │   ├── LeverController.cs
│   │   ├── BetButtonController.cs
│   │   └── WinPresentationController.cs
│   ├── Audio/
│   │   └── SlotAudioController.cs
│   └── ...
│
├── Sounds/
│   ├── slot_machine.wav
│   ├── lever.wav
│   └── win_jackpot.wav
│
├── Scenes/
│   ├── Main.unity
│   └── GameOver.unity
│
├── Prefabs/
├── UI/
└── ...
```

---

## 🛠️ Technologies Used

- **Unity**
- **C#**
- **Unity UI**
- **TextMeshPro**
- **Unity Input System**
- **Unity Audio System**
- **WebGL**

---

## 🌐 Running the WebGL Build

The WebGL build should be placed under:

```text
Build/WebGL/
```

WebGL builds should be served through a local HTTP server rather than opened directly using `file://`.

For example, with Python:

```bash
cd Build/WebGL
python -m http.server 8000
```

Then open:

```text
http://localhost:8000
```

in your browser.

### Recommended Browsers

- Google Chrome
- Microsoft Edge
- Mozilla Firefox

---

## 🔨 Building the Project

To create a WebGL build in Unity:

1. Open the project in Unity.
2. Open **File → Build Settings**.
3. Select **WebGL**.
4. Make sure the required scenes are included in the build.
5. Select **Build**.
6. Choose:

```text
Build/WebGL
```

---

## 🧩 Scenes

### Main Scene

The Main scene contains:

- Slot machine artwork
- Three reel masks
- Three reels
- Credit display
- Bet display
- Win display
- Betting controls
- Lever visual
- Win presentation
- Audio controller
- Event System

### Game Over Scene

The Game Over scene contains:

- Shared slot machine background
- Game Over message
- Restart button
- Event System

---

## 🎨 UI / UX

The UI was designed to make the game state and available interactions easy to understand.

### Betting Buttons

Each betting option provides visual feedback when:

- Hovered
- Pressed
- Selected
- Locked during a spin

### Gameplay Feedback

The player receives feedback through:

- Automatic lever animation
- Reel animation
- Audio
- Credit updates
- Win amount display
- Jackpot presentation
- Game Over screen

---

## 🧹 Code Organization

The project follows component-based and object-oriented design principles.

| Component | Responsibility |
|---|---|
| `GameManager` | Controls overall game flow |
| `Wallet` | Manages credits and bet amount |
| `ReelManager` | Coordinates all reels |
| `ReelColumn` | Handles individual reel animation |
| `PayoutTable` | Defines payout calculations |
| `BetButtonController` | Handles betting interaction |
| `LeverController` | Controls lever presentation |
| `SlotAudioController` | Controls gameplay audio |
| `WinPresentationController` | Handles win presentation |
| `UIManager` | Updates gameplay UI |

---

## 💡 Design Decisions

### Fixed Visual Reel Sequence

The visual symbols displayed while the reels are spinning follow a controlled sequence rather than being completely random.

This prevents consecutive repeated symbols from appearing unexpectedly during the visual animation.

The actual target outcome is still selected independently using the game's RNG system.

### Separate Gameplay and Presentation

The final outcome is determined separately from the visual animation.

This makes it easier to tune the reel animation without changing the underlying winning logic.

### Audio-Synchronized Win State

The betting controls remain locked while the jackpot presentation is active.

The jackpot audio completion is used as the point at which the presentation can end and betting controls can become available again.

### Component-Based Architecture

Gameplay responsibilities are split across focused components. This makes individual systems easier to understand and modify without affecting unrelated parts of the game.

---

## ⭐ Additional Features

Beyond the core slot machine requirements, the project includes:

- Three selectable betting amounts.
- Interactive hover and pressed states.
- Automatic lever animation after selecting a bet.
- Staggered reel stopping.
- Reel settling effect.
- Dedicated slot machine audio.
- Dedicated jackpot audio.
- Win amount presentation.
- Audio-synchronized win presentation.
- Dedicated Game Over scene.
- Restart functionality.
- WebGL support.

---

## 📋 Assignment Requirements

| Requirement | Status |
|---|---|
| Winning when all slots match | ✅ |
| Smooth reel animation | ✅ |
| Clear symbol display | ✅ |
| Randomized outcomes / RNG | ✅ |
| Winning combinations & payouts | ✅ |
| Betting system | ✅ |
| Audio feedback | ✅ |
| Win presentation | ✅ |
| Game Over flow | ✅ |
| Restart functionality | ✅ |
| WebGL build support | ✅ |
| Organized project structure | ✅ |
| Object-oriented C# architecture | ✅ |
| Component-based code separation | ✅ |

---

## 👤 Author

**Biswadeep Bhattacharjee**

Unity / Game Development / AI-ML Engineer

---

## 📄 Assignment

This project was developed as part of the **Unity Slot Game Assignment**, following the supplied assets, gameplay requirements, and technical expectations.
