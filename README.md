# Ashen Crown — mobile MOBA vertical slice

Оригинальная MOBA в тёмном фэнтези. Стартовый Unity-проект с процедурной ареной и локальным матчем против ботов. 3D-геометрия пока заглушечная, не финальные игровые ассеты.

## Unity project

- Unity 2022.3 LTS
- Android build target (GitHub Actions workflow)
- Runtime prototype: `Assets/AshenCrown/Runtime/AshenCrownGame.cs`

## Local setup

Open this repository as a Unity project using Unity 2022.3 LTS with Android Build Support installed. Open `Assets/Scenes/AshenCrown.unity` once it is added, or create a scene, add an empty `GameRoot` object, and attach `AshenCrownGame.cs`. Add an EventSystem if needed, then press Play. On desktop, move with WASD/arrow keys, attack with Space, and use abilities with Q/W/E. On touch devices, use the on-screen joystick and HUD buttons.

## CI build

The workflow `.github/workflows/unity-android.yml` runs on pushes and can be started manually. Add a Unity Personal/Plus/Pro license as the repository Actions secret `UNITY_LICENSE` before the Unity build step can succeed. Build output is uploaded as a workflow artifact when the build succeeds.

## Scope and limitations

This is a prototype, not a finished commercial game. Models and audio are placeholders; online multiplayer is not implemented; device performance has not been measured. The vertical slice includes a basic economy, levels, abilities, bots, towers, creep waves, and win/loss conditions. A production MOBA still needs authoritative server simulation, navigation/pathing, more complete itemization, art, audio, QA, and device profiling.
