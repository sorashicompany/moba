# Ashen Crown — Unity mobile MOBA prototype

Original dark-fantasy arena prototype built with Unity 2022.3 LTS. The current vertical slice uses procedural primitive geometry and placeholder visuals.

## Project layout

- `Assets/AshenCrown/Runtime/AshenCrownGame.cs` — runtime arena, hero, enemy actors, basic combat, abilities and HUD.
- `Assets/Editor/AshenCrownBuild.cs` — creates a boot scene and builds an Android APK in batch mode.
- `.github/workflows/unity-android.yml` — build on pushes to `main` or manual dispatch.

## Automatic Android build

1. In the repository's **Settings → Secrets and variables → Actions**, add a repository secret named `UNITY_LICENSE` containing a valid Unity license file for the Unity version used by this project.
2. Open the **Actions** tab and select **Unity Android Build**. The workflow also runs automatically on pushes to `main`.
3. On success, download the `AshenCrown-Android` artifact from the workflow run.

The workflow has been committed and its first run has been queued. A successful APK is not yet confirmed; Unity licensing and build logs must be checked.

## Current prototype controls

On desktop: WASD/arrow keys to move, Space to attack, Q/W/E for abilities. The HUD contains touch-sized action buttons. Touch movement still needs a proper virtual joystick implementation.

## Limitations

This is an early prototype, not a finished commercial MOBA. It has no online multiplayer, final art/audio, full tower/core objectives, or production-quality pathfinding. It has not yet been compiled or tested on a physical Android device.