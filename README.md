# MASK // LUMIN

**Game Jam 2026 Entry**

A 2D side-scrolling action platformer set in a decaying, bioluminescent world where survival depends on wearing borrowed identities.

## 🎮 Concept

The player must run, jump, fight, and navigate a hostile environment while managing a **mask-based countdown mechanic**. Each mask grants abilities but is temporary. When the mask depletes, the player begins to physically deteriorate until a new mask is found.

## 🎯 Controls

- **A/D or Arrow Keys**: Move left/right
- **Space**: Jump
- **R**: Restart level
- **Escape**: Quit

## 🎭 Mask Types

| Mask | Speed | Jump | Special |
|------|-------|------|---------|
| **Runner** | +50% | +30% | Fast traversal |
| **Hunter** | -20% | -10% | Shooting enabled |
| **Ghost** | Normal | Normal | Phase through obstacles, faster drain |

## ⚡ Core Mechanics

1. **Mask Timer**: Each mask has a countdown timer
2. **Depletion**: When timer reaches zero, health drains steadily
3. **Pickup**: Collecting a new mask refills the timer and changes abilities
4. **Survival**: Find the exit before losing all health

## 🎨 Visual Style

- Painted 2D sprites
- Bioluminescent atmosphere
- Dark environments with glowing accents
- 2.5D depth via parallax scrolling

## 🛠️ Technical Details

- **Engine**: Unity 6000.x (2D)
- **Rendering**: URP 2D
- **Platform**: Windows / WebGL

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Player/          # Movement, Health
│   ├── Masks/           # Mask system, pickups
│   ├── World/           # Hazards, parallax, camera
│   ├── UI/              # HUD elements
│   └── Enemies/         # (Optional) Enemy AI
├── Prefabs/
├── Art/
│   ├── Characters/
│   ├── Masks/
│   ├── Environment/
│   ├── Backgrounds/
│   └── UI/
└── Scenes/
```

## 🏃 Quick Start

1. Open project in Unity 6000.x
2. Open `Assets/Scenes/SampleScene.unity`
3. Create a player GameObject with:
   - Sprite Renderer
   - Rigidbody2D (Freeze Rotation Z)
   - Capsule Collider 2D
   - PlayerMovement, PlayerHealth, MaskSystem scripts
4. Add ground with Box Collider 2D on "Ground" layer
5. Press Play!

## 📜 License

Game Jam project - All rights reserved.

---

*"Identity is temporary. Survival requires adaptation."*
