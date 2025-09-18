<img width="1472" height="704" alt="Gemini_Generated_Image_q9df7tq9df7tq9df" src="https://github.com/user-attachments/assets/67e08a72-204b-4e53-9376-36c05464fa21" />



# BootCamp FPS

> Fast to play, easy to iterate — a Unity FPS sandbox built for the Oyun ve Uygulama Akademisi Bootcamp.

[![Unity 2022.3](https://img.shields.io/badge/Unity-2022.3_LTS-black?logo=unity)](https://unity.com/releases/lts) [![URP 13](https://img.shields.io/badge/URP-13.1.8-1269D3?logo=unity)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@13.1/manual/index.html)

## Table of Contents
- [Overview](#overview)
- [Feature Highlights](#feature-highlights)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Gameplay Controls](#gameplay-controls)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Quality-of-Life Tools](#quality-of-life-tools)
- [Troubleshooting](#troubleshooting)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

## Overview
BootCamp FPS is a first-person shooter prototype that demonstrates modern Unity workflows. The project focuses on clean player movement, modular weapon handling, reactive HUD feedback, and a suite of in-editor fixers that remove the usual setup friction.

## Feature Highlights
- **Responsive FPS Controller**: Smooth WASD + mouse look, tuned for keyboard and mouse playtests.
- **Weapon & Ammo Loop**: Pickup interactions, projectile logic, ammo tracking, and debug scripts for rapid iteration.
- **Tactical HUD**: TextMeshPro-based heads-up display that wires itself through `HUDManager` and helper auto-setup scripts.
- **Immersive Audio**: Spatial sound effects handled by `SoundManager` and `SoundDetector` utilities.
- **Rendering Ready**: Universal Render Pipeline configuration with automatic material and renderer fixers.
- **Editor Safety Nets**: Diagnostics and auto-fix scripts that keep scenes, materials, and packages in a shippable state.

## Tech Stack
- **Engine**: Unity 2022.3 LTS
- **Rendering**: Universal Render Pipeline (URP) 13.1.8
- **Input**: Unity Input System (first-person bindings)
- **UI**: TextMeshPro + Unity UI
- **Audio**: 3D spatial audio via built-in AudioSources
- **Scripting**: C# assemblies scoped by `BootcampFPS.asmdef`

## Getting Started
1. **Clone the repo**
   ```bash
   git clone <your-repo-url>
   cd BootCamp\ FPS
   ```
2. **Open in Unity Hub**
   - Add the folder as an existing project.
   - Launch with Unity 2022.3 LTS.
3. **Load the sample scene**
   - Open `Assets/Scenes/SampleScene.unity`.
4. **Press Play** to jump into the FPS sandbox.

### Build the Project
1. `File → Build Settings`
2. Add `SampleScene` to the Scenes in Build list
3. Pick a target platform (PC/Mac/Linux recommended for playtests)
4. Click `Build and Run`

## Gameplay Controls
| Input | Action |
|-------|--------|
| `W / A / S / D` | Move |
| Mouse | Look around |
| `F` | Interact / pick up weapon |
| Left Mouse | Fire primary weapon |
| `Esc` | Pause |

## Project Structure
```
Assets/
├── Scenes/                  # Sample gameplay scene and future levels
├── Scripts/
│   ├── Player/              # Movement, mouse look, weapon handling
│   ├── HUD/                 # HUD managers and auto-setup helpers
│   ├── Audio/               # Sound detection, debugging, and manager scripts
│   ├── Systems/             # Interaction, ammo, and gameplay managers
│   └── Editor/              # Fixers, diagnostics, and project utilities
├── Prefabs/                 # Reusable FPS entities (weapons, pickups, UI)
├── Materials/               # URP materials and shaders
├── Models/                  # 3D assets used by the sandbox
└── Sounds/                  # Weapon effects, ambience, and cues
```

## Development Workflow
- **Iteration Loop**: Press Play in `SampleScene` for quick tests, use the `InstantDebugger` to validate runtime states.
- **Script Assembly**: All gameplay code compiles under the `BootcampFPS` assembly definition for faster builds.
- **URP Validation**: Run `URPCompatibilityChecker` and `URPRendererFixer` when upgrading Unity or URP packages.
- **Material Health**: `MaterialAutoSetup` and `AdvancedMaterialManager` make sure materials stay in sync with URP expectations.

## Quality-of-Life Tools
- `ComprehensiveGameFixer` bundles several auto-fix routines to repair missing references.
- `PackageConflictDetector` and `PackageUpdater` guard against package mismatches.
- `UnityFileSystemFixer` and `UnityRestarter` unblock editor issues without manual cleanup.
- `SoundDebugger` and `RenderingDiagnostics` surface scene-level problems directly in the inspector.

## Troubleshooting
1. Confirm you are running Unity 2022.3 LTS with URP 13.1.8 imported.
2. Re-run the fixers in `Assets/Scripts/Editor` if materials, renderers, or packages break after upgrades.
3. Clear the Library folder and re-open the project if you hit persistent import errors.
4. Keep the Console clear before builds; fix compiler errors surfaced by `CompilerErrorFixer`.

## Roadmap
- [ ] Add weapon switching and reload animations
- [ ] Expand AI targets for combat testing
- [ ] Ship a polished vertical slice demo
- [ ] Package editor utilities as standalone tools

## Contributing
Pull requests are welcome. Please open an issue describing the improvement or bug fix before submitting major changes.

## License
This repository is provided for educational purposes as part of the Oyun ve Uygulama Akademisi Bootcamp.
