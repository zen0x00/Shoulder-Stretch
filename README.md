# Shoulder Stretch Warrior

A keyboard-controlled fitness game prototype built in Unity. Players auto-run forward and perform combat actions using keyboard keys, with a comprehensive fitness analytics dashboard at session end.

## Keyboard Controls

| Key | Simulated Pose | Game Action |
|-----|----------------|-------------|
| **A** | Right shoulder stretch | Left Punch |
| **D** | Left shoulder stretch | Right Punch |
| **W** | Raise both hands | Pick up Gun |
| **S** | Arms cross and stretch | Shoot |
| **Space** | Hug pose | Activate Shield |
| **Escape** | - | Pause/Resume |

## Quick Setup

### Option 1: Automatic Setup (Recommended)

1. Open Unity and load this project
2. Open `Assets/Scenes/SampleScene.unity`
3. Create an empty GameObject named "Setup"
4. Add the `GameSetup` component to it
5. Right-click the `GameSetup` component → **"Setup Complete Scene"**
6. Delete the Setup GameObject
7. Press **Play** to start the game!

### Option 2: Manual Setup

Create these GameObjects manually:

```
Hierarchy
├── GameManager (Empty)
│   ├── GameStateManager
│   ├── DifficultyScaler
│   ├── InputSystem
│   ├── FitnessTrackingSystem
│   ├── ScoringSystem
│   └── SafetyFatigueSystem
│
├── Player (Capsule)
│   ├── PlayerController
│   ├── CombatSystem
│   └── RunnerMovementSystem (child object)
│
├── EnemySpawner (Empty)
│   └── EnemySpawner
│
├── Environment
│   └── Directional Light
│
├── Main Camera
│   └── CameraFollow
│
└── UI Canvas
    ├── UIManager
    ├── MenuPanel
    ├── HUDPanel (with HUD component)
    └── DashboardPanel (with AnalyticsDashboard component)
```

## Project Structure

```
Assets/Scripts/
├── Core/
│   ├── GameStateManager.cs    # State machine (Menu/Playing/Paused/GameOver/Dashboard)
│   ├── DifficultyScaler.cs    # Difficulty settings (Beginner/Moderate/Expert)
│   └── InputSystem.cs         # Keyboard input with cooldowns
├── Gameplay/
│   ├── PlayerController.cs    # Player health, actions, visual feedback
│   ├── CombatSystem.cs        # Punch/shoot/shield logic
│   ├── Enemy.cs               # Enemy behavior and health
│   ├── EnemySpawner.cs        # Enemy spawning with object pooling
│   └── CameraFollow.cs        # Smooth camera follow
├── Systems/
│   └── RunnerMovementSystem.cs # Auto-run with ground recycling
├── Fitness/
│   ├── FitnessTrackingSystem.cs # Action tracking, calorie estimation
│   ├── ScoringSystem.cs       # Score and combos
│   └── SafetyFatigueSystem.cs # Fatigue monitoring
├── UI/
│   ├── UIManager.cs           # Panel management
│   ├── HUD.cs                 # In-game display
│   └── AnalyticsDashboard.cs  # End session stats
└── GameSetup.cs               # One-click scene builder
```

## Difficulty Levels

| Setting | Beginner | Moderate | Expert |
|---------|----------|----------|--------|
| Player Speed | 4 | 6 | 8 |
| Enemy Spawn Interval | 4s | 2.5s | 1.5s |
| Max Enemies | 3 | 5 | 8 |
| Timing Window | 1.5s | 1.0s | 0.5s |
| Intensity Factor | 0.5x | 1.0x | 1.5x |

## Game Flow

1. **Menu** → Select difficulty → Click Start
2. **Playing** → Auto-run, punch enemies, collect gun, use shield
3. **Game Over** → When health reaches 0
4. **Dashboard** → Shows fitness analytics (time, actions, calories, score)

## Fitness Tracking

The game tracks:
- Left/Right action counts (for balance assessment)
- Total actions and accuracy percentage
- Session duration
- Estimated calories: `ActionCount × IntensityFactor × TimeFactor`
- Overall fitness score (0-100)

## System Requirements

- Unity 2022.3 LTS or later
- Platform: PC (Windows)
- Target: 60 FPS

## Future Enhancements

- Pose detection via camera (MediaPipe/OpenCV)
- More enemy types
- Power-ups
- Leaderboards
