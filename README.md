# 3D FPS Game

A first-person shooter game built with Unity 2022.3 and Universal Render Pipeline (URP).

## 🎮 Game Features

- **First-Person Movement**: WASD controls with mouse look
- **Weapon System**: Multiple weapons with F key pickup
- **Interactive Objects**: Ammo boxes and collectibles
- **Sound System**: Weapon firing sounds and ambient audio
- **Modern Graphics**: Built with URP for optimized rendering

## 🚀 Getting Started

### Requirements
- Unity 2022.3.x LTS
- Universal Render Pipeline 13.1.8

### How to Play
1. Open `Assets/Scenes/SampleScene.unity`
2. Press **Play** button
3. Use **WASD** to move
4. Use **Mouse** to look around
5. Press **F** near weapons to pick them up
6. **Left Click** to fire weapons

## 🛠️ Technical Details

- **Unity Version**: 2022.3.x LTS
- **Render Pipeline**: Universal Render Pipeline (URP) 13.1.8
- **Input System**: Unity Input System
- **Audio**: 3D Spatial Audio with AudioManager
- **UI System**: TextMeshPro with responsive HUD

## 📁 Project Structure

```
Assets/
├── Scripts/           # Game logic and player controllers
├── Scenes/           # Game scenes (start with SampleScene.unity)
├── Prefabs/          # Reusable game objects
├── Materials/        # URP materials and textures
├── Sounds/           # Audio files and sound effects
└── Models/           # 3D models and animations
```

## 🎯 Development

### Opening the Project
1. Clone this repository
2. Open Unity Hub
3. Add project from folder
4. Open with Unity 2022.3.x LTS

### Building the Game
1. Open `File > Build Settings`
2. Add `SampleScene` to build
3. Select your target platform
4. Click `Build and Run`

## 🚀 Deployment

### Pushing to GitHub
```bash
# Set main as default branch
git branch -M main

# Create repo on GitHub first, then add origin
git remote add origin https://github.com/<username>/<repository-name>.git

# Push to GitHub
git push -u origin main
```

### Git Setup
- Uses Unity-friendly `.gitignore`
- Sample assets excluded to keep repository lightweight
- Optional Git LFS for large assets

## 🎮 Controls

| Key | Action |
|-----|--------|
| W,A,S,D | Move |
| Mouse | Look Around |
| F | Pickup Weapon |
| Left Click | Fire Weapon |
| ESC | Pause/Menu |

## 🔧 Troubleshooting

If you encounter issues:
1. Ensure Unity 2022.3.x LTS is installed
2. Check that URP 13.1.8 is properly imported
3. Verify all scripts compile without errors
4. Make sure scene lighting is properly baked

## 📝 License

This project is for educational purposes.
