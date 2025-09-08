# BootCamp FPS

Unity FPS project.

## Local Development
- Unity Version: URP project (see `Assets/Settings` for pipeline assets)
- Open `Assets/Scenes/SampleScene.unity`

## Git Setup
- Uses a Unity-friendly `.gitignore`
- Optional Git LFS patterns in `.gitattributes` (install with `git lfs install`)

## Pushing to GitHub
```bash
# set main as default branch
git branch -M main

# create repo on GitHub first, then add origin
# replace <user> and <repo>
git remote add origin https://github.com/<user>/<repo>.git

# or via SSH
# git remote add origin git@github.com:<user>/<repo>.git

# push
git push -u origin main
```
