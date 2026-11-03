# UI setup checklist — Main Menu / HUD / Pause

## 1. Canvas Scaler (on every screen's Canvas)
- Render Mode: `Screen Space - Overlay`
- UI Scale Mode: `Scale With Screen Size`
- Landscape games (if you keep portrait, swap W/H): Reference Resolution **1920 x 1080**, Match = **1** (height)
- Portrait: Reference Resolution **1080 x 1920**, Match = **0** (width)
- Screen Match Mode: Match Width Or Height

## 2. Hierarchy per screen
```
Canvas (Canvas Scaler as above)
 └─ SafeArea (RectTransform stretched full-screen, SafeArea.cs attached)
     ├─ TopBar (hearts / score / pause button — HUD only)
     ├─ Controls (joystick / jump button — HUD only)
     └─ ... buttons / text for that screen
 └─ BackgroundArt (OUTSIDE SafeArea, stretched full-screen — can go under the notch)
```
Background art stays outside `SafeArea` so it still fills the whole screen; only
interactive elements (buttons, text) live inside `SafeArea`.

## 3. Anchors (per element)
- Hearts (top-left): anchor **(0, 1)**, pivot (0, 1)
- Score (top-center): anchor **(0.5, 1)**
- Pause button (top-right): anchor **(1, 1)**
- Joystick (bottom-left): anchor **(0, 0)**
- Jump / action button (bottom-right): anchor **(1, 0)**
- Full-screen panels (background, Pause overlay): anchors stretched **(0,0)–(1,1)**

## 4. Sizes
- Buttons: minimum **120 × 120 px** at the reference resolution
- Text: minimum **~36 px**, high-contrast against the art

## 5. Screens checklist
- **Main Menu:** game title, Play button, Quit button → `GameManager.PlayGame / QuitGame`
- **HUD:** hearts, score, pause button, on-screen controls → `HUDController`
- **Pause:** Resume / Restart / Menu buttons, panel starts **inactive** → `PausePanel`

## 6. Test matrix (Unity Device Simulator)
- 16:9 phone (e.g. 1920×1080)
- Notch phone, 19.5:9 or 20:9 (e.g. iPhone 14)
- 4:3 tablet (e.g. iPad)

Confirm: nothing is cut off, nothing sits under the notch, every button is tappable
and does its job (Play loads the level, Pause stops time, Resume restores it,
Menu returns to Main Menu).
