# Treasure Tower

Treasure Tower is a 2D Unity platformer where the player climbs through five main levels, collects coins and gems, avoids hazards, defeats enemies, and reaches the final exit door. Some levels include optional mini-boss doors that lead to boss arenas and let the player skip ahead if they win.

## Game Idea

The game is built around risk, route choice, and platforming pressure.

- climb through 5 main tower levels
- collect the required gem in each level to unlock the exit door
- avoid spikes, falling, enemy contact, and boss attacks
- choose between the normal route or risky mini-boss shortcuts
- finish the final tower level and clear the game

The project includes:

- 5 main levels
- 2 optional mini-boss scenes
- coins, gems, enemies, hazards, and boss fights
- HUD for lives, timer, deaths, and collectibles
- menus for start, pause, story, leaderboard, and game over
- a Windows standalone build for playing without Unity

## Requirements

- Unity 6
- Windows recommended for the included standalone build

## How To Open The Project In Unity

1. Clone the repository:

```bash
git clone https://github.com/alialsaffarcodexals/Treasure_Tower.git
```

2. Open Unity Hub.
3. Click `Add` or `Open`.
4. Select the cloned `Treasure_Tower` project folder.
5. Open the project with the Unity version that matches the project.

## How To Run The PC Build

If you only want to play the game, use the included Windows build:

- `Builds/Windows/TreasureTower.exe`
- downloadable zip: `https://github.com/alialsaffarcodexals/Treasure_Tower/raw/main/Builds/TreasureTower_Windows.zip`

To run it:

1. Open the folder `Builds/Windows`
2. Double-click `TreasureTower.exe`

If you want to share the game with someone else, send them this build zip link:

- [Download Treasure Tower for Windows](https://github.com/alialsaffarcodexals/Treasure_Tower/raw/main/Builds/TreasureTower_Windows.zip)

After downloading:

1. Extract `TreasureTower_Windows.zip`
2. Open the extracted folder
3. Double-click `TreasureTower.exe`

Important:

- Keep `TreasureTower.exe` together with the `TreasureTower_Data` folder and the other files inside `Builds/Windows`
- The included build is intended to reflect the latest committed project version

## Controls

- `A / D` or `Left / Right Arrow`: move
- `Space`: jump
- `F`: shoot in mini-boss scenes
- `Esc`: pause

## Gameplay Rules

- collect the gem in a level to unlock the exit door
- coins increase your total coin count
- falling below the level causes death
- enemies can defeat the player on contact
- many normal ground enemies can be defeated by jumping on them
- mini-boss scenes give the player a gun
- mini-boss shortcuts can skip a level if the boss is defeated
- each level is designed to become harder as the player climbs higher

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
