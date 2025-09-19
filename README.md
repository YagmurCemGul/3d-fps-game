<img width="1472" height="704" alt="BootCamp FPS" src="https://github.com/user-attachments/assets/67e08a72-204b-4e53-9376-36c05464fa21" />

<h1 align="center">BootCamp FPS</h1>

<p align="center">
  Fast to play, easy to iterate — a Unity FPS sandbox built for the Oyun ve Uygulama Akademisi Bootcamp.
</p>

<p align="center">
  <a href="https://unity.com/releases/lts"><img alt="Unity 2022.3 LTS" src="https://img.shields.io/badge/Unity-2022.3_LTS-black?logo=unity"></a>
  <a href="https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@13.1/manual/index.html"><img alt="URP 13.1.8" src="https://img.shields.io/badge/URP-13.1.8-1269D3?logo=unity"></a>
  <img alt="Input System" src="https://img.shields.io/badge/Input_System-1.x-blue">
  <img alt="TextMeshPro" src="https://img.shields.io/badge/TextMeshPro-✔-informational">
  <img alt="License" src="https://img.shields.io/badge/license-Educational-yellow">
</p>

<p align="center">
  <a href="#-demo--downloads">Demo</a> •
  <a href="#overview">Overview</a> •
  <a href="#feature-highlights">Features</a> •
  <a href="#tech-stack">Tech</a> •
  <a href="#getting-started">Setup</a> •
  <a href="#gameplay-controls">Controls</a> •
  <a href="#project-structure">Structure</a> •
  <a href="#development-workflow">Workflow</a> •
  <a href="#quality-of-life-tools">QoL</a> •
  <a href="#-performance-checklist">Perf</a> •
  <a href="#-testing">Tests</a> •
  <a href="#-ci">CI</a> •
  <a href="#-git-lfs--repo-hygiene">LFS</a> •
  <a href="#troubleshooting">Troubleshooting</a> •
  <a href="#roadmap">Roadmap</a> •
  <a href="#contributing">Contributing</a> •
  <a href="#license">License</a>
</p>

---

## 🎮 Demo & Downloads

* **Latest build (Releases):** *Add a link after your first release*
* **Itch.io page:** *Optional — add if you host web/desktop builds*
* **Short gameplay video:** *Embed a YouTube link here*

---

## Overview

BootCamp FPS is a first-person shooter prototype that demonstrates modern Unity workflows. The project focuses on clean player movement, modular weapon handling, reactive HUD feedback, and a suite of in-editor fixers that remove the usual setup friction.

## Feature Highlights

* **Responsive FPS Controller:** Smooth WASD + mouse look, tuned for keyboard/mouse playtests.
* **Weapon & Ammo Loop:** Pickups, projectile logic, ammo tracking, debug scripts for rapid iteration.
* **Tactical HUD:** TextMeshPro HUD wired via `HUDManager` + auto-setup helpers.
* **Immersive Audio:** Spatial SFX through `SoundManager` + `SoundDetector` utilities.
* **Rendering Ready:** URP config with automatic material/renderer fixers.
* **Editor Safety Nets:** Diagnostics + auto-fix scripts to keep scenes/materials/packages healthy.

## Tech Stack

* **Engine:** Unity 2022.3 LTS
* **Rendering:** Universal Render Pipeline (URP) 13.1.8
* **Input:** Unity Input System (first-person bindings)
* **UI:** TextMeshPro + Unity UI
* **Audio:** 3D spatial audio (AudioSources, Mixers)
* **Scripting:** C# assemblies via `BootcampFPS.asmdef`

---

## Getting Started

1. **Clone**

```bash
git clone <your-repo-url>
cd "BootCamp FPS"
```

2. **Open in Unity Hub**
   Select **Unity 2022.3 LTS**.

3. **Load the sample scene**
   `Assets/Scenes/SampleScene.unity`

4. **Press Play** — you’re in the sandbox.

### Build the Project

1. `File → Build Settings`
2. Add `SampleScene` to **Scenes In Build**
3. Choose target (PC/Mac/Linux recommended)
4. **Build and Run**

---

## Gameplay Controls

| Input           | Action             |
| --------------- | ------------------ |
| `W / A / S / D` | Move               |
| Mouse           | Look               |
| `F`             | Interact / pick up |
| Left Mouse      | Fire               |
| `Esc`           | Pause              |

---

## Project Structure

```
Assets/
├─ Scenes/            # Sample scene + future levels
├─ Scripts/
│  ├─ Player/         # Movement, mouse look, weapon handling
│  ├─ HUD/            # HUD managers, auto-setup helpers
│  ├─ Audio/          # Sound detection, debugging, manager scripts
│  ├─ Systems/        # Interaction, ammo, gameplay managers
│  └─ Editor/         # Fixers, diagnostics, project utilities
├─ Prefabs/           # Reusable entities (weapons, pickups, UI)
├─ Materials/         # URP materials/shaders
├─ Models/            # 3D assets
└─ Sounds/            # Weapon SFX, ambience, cues
```

---

## Development Workflow

* **Iteration Loop:** Use `SampleScene` + `InstantDebugger` to validate runtime states.
* **Assemblies:** Gameplay code under `BootcampFPS` asmdef for faster compiles.
* **URP Validation:** Run `URPCompatibilityChecker` / `URPRendererFixer` after Unity/URP upgrades.
* **Material Health:** `MaterialAutoSetup` / `AdvancedMaterialManager` keep URP materials in sync.

---

## Quality-of-Life Tools

* `ComprehensiveGameFixer` — bundles auto-fix routines for missing refs.
* `PackageConflictDetector` / `PackageUpdater` — guard against package mismatches.
* `UnityFileSystemFixer` / `UnityRestarter` — unblock editor issues quickly.
* `SoundDebugger` / `RenderingDiagnostics` — surface scene-level problems in Inspector.

---

## ⚡ Performance Checklist

* **URP SRP Batcher** enabled, **GPU Instancing** for repeated meshes.
* **Static/ Dynamic Batching** where appropriate; avoid excessive material variants.
* **Post-processing**: keep effects minimal; reduce sample counts; clamp bloom.
* **Shadows**: lower cascades/resolution; bias tuned; use mixed/baked lights if possible.
* **Textures**: proper import sizes & compression; atlas where it helps.
* **Physics**: fixed timestep sane; layers/colliders trimmed; avoid per-frame allocations.
* **VSync/Target FPS**: align to your test targets; avoid unnecessary spikes.
* **Addressables (optional)**: prepare for scalable content loading.

---

## 🧪 Testing

Use **Unity Test Runner** (PlayMode/EditMode) with **NUnit**.

```text
Assets/Tests/
├─ EditMode/
│  └─ PlayerMovementTests.cs
└─ PlayMode/
   └─ WeaponLoopTests.cs
```

**Sample NUnit test (EditMode):**

```csharp
using NUnit.Framework;

public class MathSmokeTest {
  [Test] public void Addition_Works() => Assert.AreEqual(4, 2 + 2);
}
```

Run via **Window → General → Test Runner** (or CI, bkz: aşağıdaki “CI” bölümü).

---

## 🤖 CI

GitHub Actions ile build + test (örnek minimal iş akışı):

```yml
# .github/workflows/ci.yml
name: CI
on: [push, pull_request]
jobs:
  test-build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup LFS
        run: |
          git lfs install
          git lfs pull
      - name: Cache Library
        uses: actions/cache@v4
        with:
          path: Library
          key: Library-${{ runner.os }}-${{ hashFiles('**/Packages/manifest.json') }}
          restore-keys: |
            Library-${{ runner.os }}-
      # Buraya game-ci/unity-test-runner ve unity-builder adımlarını ekleyebilirsin.
      # Eğer lisanslama gerektiriyorsa UNITY_LICENSE secret'ı ayarlamayı unutma.
```

> **Not:** Unity build için yaygın yaklaşım **game-ci** action’larıdır (test-runner / builder). Projene göre ekle.

---

## 📦 Git LFS & Repo Hygiene

Unity projelerinde **LFS** kritik:

* **Kurulum:** `git lfs install`
* **Takip (örnek):**

  ```
  git lfs track "*.psd" "*.tga" "*.png" "*.jpg" "*.jpeg" "*.fbx" "*.wav" "*.mp3" "*.mp4" "*.aif" "*.anim" "*.prefab" "*.unity"
  ```
* `.gitattributes` dosyanı commit’le; ilk push’tan önce ayarlarsan en temizi.

**.gitignore (özet):**

```
[Ll]ibrary/
[Tt]emp/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Mm]emoryCaptures/
Obj/
*.csproj
*.sln
*.user
```

---

## Troubleshooting

1. **Version mismatch:** Unity 2022.3 LTS + URP 13.1.8 kurulu olduğundan emin ol.
2. **Render/Material breakage:** `Assets/Scripts/Editor` altındaki fixer’ları çalıştır.
3. **Import hataları:** `Library/` sil → projeyi yeniden aç.
4. **Console temizliği:** Build öncesi derleyici hatalarını `CompilerErrorFixer` ile çöz.
5. **Input çalışmıyor:** Input System etkin mi? PlayerInput bağları doğru mu?

---

## 🧭 Architecture (Mermaid)

```mermaid
flowchart LR
  PC[PlayerController] --> WS[WeaponSystem]
  PC --> IM[Input (Unity Input System)]
  WS --> PH[Projectile/HitScan]
  WS --> AM[AmmoManager]
  PC --> HM[HUDManager]
  HM --> UI[TextMeshPro UI]
  AU[AudioSources] --> SD[SoundDetector]
  R[URP Renderer] --> PP[Post Processing]

  subgraph Editor QoL
    FX[ComprehensiveGameFixer]
    UR[URPCompatibilityChecker]
    MF[MaterialAutoSetup]
  end

  R---UR
  PP---MF
  UI---FX
```

---

## Roadmap

* [ ] Weapon switching + reload animations
* [ ] Target dummies + simple AI for combat testing
* [ ] Polished vertical slice demo
* [ ] Editor utilities packaged as standalone tools
* [ ] Addressables & content profiles
* [ ] Basic save system (settings / sensitivity / audio)

---

## Contributing

Pull requests welcome!

* Issue açarken kısa **kapsam**, **beklenen davranış**, **ekran görüntüsü** ekle.
* Küçük düzeltmeler için `good first issue` etiketi var.
* Commit mesajları: **Conventional Commits** tercih edilir (`feat:`, `fix:`, `chore:` …).

### Code of Conduct

Lütfen topluluk davranış kurallarına uyun (`CODE_OF_CONDUCT.md`).

---

## License

This repository is provided **for educational purposes** as part of the Oyun ve Uygulama Akademisi Bootcamp.

