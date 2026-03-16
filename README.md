# Treasure Tower

Treasure Tower is a 2D Unity platformer where the player climbs through five levels, collects coins and gems, avoids hazards, defeats enemies, and reaches the final exit door. Some levels include optional mini-boss doors that lead to boss arenas and let the player skip ahead if they win.

## Game Idea

The core loop is simple:

- move through platforming levels
- collect the required gem to unlock the exit
- avoid spikes, falling, enemy contact, and boss attacks
- choose between the normal route or risky mini-boss shortcuts
- reach the final level and finish the tower

The game includes:

- 5 main levels
- optional mini-boss scenes
- coins, gems, enemies, hazards, and boss fights
- HUD for lives, timer, deaths, and collectibles
- menus for start, pause, story, leaderboard, and game over

## Requirements

- Unity 6
- Windows recommended for the current project setup

## How To Open The Project

1. Clone the repository:

```bash
git clone https://github.com/alialsaffarcodexals/Treasure_Tower.git
```

2. Open Unity Hub.
3. Click `Add` or `Open`.
4. Select the cloned `Treasure_Tower` project folder.
5. Open the project with the Unity version that matches the project.

## How To Play

### Play Without Unity Editor

If you only want to run the game, use the Windows build:

- `Builds/Windows/TreasureTower.exe`

To run the game:

1. Open the folder `Builds/Windows`
2. Double-click `TreasureTower.exe`

Make sure the `.exe` stays together with the `TreasureTower_Data` folder and the other files inside `Builds/Windows`.

### Play In Unity

1. Open the `MainMenu` scene or press Play from the configured startup scene.
2. Use the menu to start the game.
3. Controls:

- `A / D` or arrow keys: move
- `Space`: jump
- `F`: shoot in mini-boss scenes
- `Esc`: pause

## Gameplay Rules

- collect the gem in a level to unlock the exit door
- coins increase your total coin count
- falling below the level causes death
- enemies can defeat the player on contact
- some ground enemies can be defeated by jumping on them
- mini-boss scenes give the player a gun
- mini-boss shortcuts can skip a level if the boss is defeated

## Repository Contents

The repository includes the files needed to open and run the Unity project:

- `Assets`
- `Builds/Windows`
- `Packages`
- `ProjectSettings`

Generated Unity folders like `Library`, `Logs`, and `Temp` are intentionally excluded from version control.

## Notes

- This is a first Unity project built as a personal effort, so it may include some mistakes or rough edges.
- The project may also include some downloaded assets that are not used in the final gameplay and were kept for testing purposes during development.
