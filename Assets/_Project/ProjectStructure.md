# Project Structure

This Unity 2D project uses the following baseline layout:

- `Assets/Art`: Sprites, tilesets, animation clips, materials, shaders, and UI art.
- `Assets/Audio`: Music, sound effects, and audio mixers.
- `Assets/Input`: Input action assets and related input setup.
- `Assets/Physics`: Physics materials and collision-related assets.
- `Assets/Prefabs`: Reusable game objects for characters, enemies, environment, interactables, and UI.
- `Assets/Scenes`: Bootstrap, menus, gameplay levels, and test scenes.
- `Assets/Scripts`: Runtime and editor code grouped by feature area.
- `Assets/ScriptableObjects`: Config/data assets for characters, items, levels, and audio.
- `Assets/Settings`: Unity render pipeline and project-level asset settings.
- `Assets/Textures`: Shared textures, atlases, and render textures.
- `Assets/ThirdParty`: External packages or imported assets kept separate from game code.
- `Assets/_Project`: Lightweight project notes and internal organization docs.

Suggested next step:

Create the first gameplay scene in `Assets/Scenes/Levels` and the first player controller script in `Assets/Scripts/Player`.
