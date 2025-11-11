# Word Rice - Unity 6.2 Version

## ⚠️ Setup Required

This folder contains the **converted C# scripts** and structure for Unity 6.2+. You need to:

### **Step 1: Create Unity Project**
1. Open **Unity Hub**
2. Click **New Project**
3. Select **Unity 6.2** or latest 6.x version
4. Choose **2D (Core)** template
5. Name it: `Word Rice Unity`
6. Location: Use this `Unity-Version` folder
7. Click **Create Project**

### **Step 2: Project Will Generate**
Unity will create the complete project structure including:
- `Assets/` folder
- `ProjectSettings/` folder
- `Library/`, `Temp/`, etc.

### **Step 3: Copy Provided Scripts**
After Unity generates the project:
- Copy all scripts from `Assets/Scripts/` (I've provided)
- Import into your Unity project's `Assets/Scripts/` folder

## 📁 Provided Structure

```
Unity-Version/
├── Assets/
│   ├── Scripts/         ← C# scripts converted from Godot
│   ├── Scenes/          ← Create scenes in Unity Editor
│   ├── Prefabs/         ← Create prefabs for LetterTile, Particle
│   ├── Resources/       ← Store materials, sprites
│   └── UI/              ← UI elements
├── ProjectSettings/     ← Unity will generate this
└── README.md            ← This file
```

## 🔄 Godot → Unity Conversion Notes

### Key Differences

| Godot | Unity Equivalent |
|-------|------------------|
| GDScript | C# |
| `extends Node` | `: MonoBehaviour` |
| `@onready` | Cache in `Awake()` or `Start()` |
| `func _ready()` | `void Start()` |
| `func _process(delta)` | `void Update()` with `Time.deltaTime` |
| Signals | Events / UnityEvent |
| `Vector2` | `Vector2` (similar) |
| `Color("hex")` | `new Color(r, g, b, a)` |
| Tween | DOTween or Coroutines |
| Area2D | Collider2D with OnMouseDown |
| CanvasLayer | Canvas with Sort Order |

### Package Dependencies

Install these via Unity Package Manager:
- **TextMeshPro** (for better text rendering)
- **DOTween** (optional, for smooth animations)
- **Newtonsoft Json** (for leaderboard persistence)

## 🎮 Implementation Guide

### Core Systems to Implement

1. **GameManager (Singleton)**
   - Manages game state
   - Scene transitions
   - Global data access

2. **Solo Mode**
   - Letter tile spawning
   - Click detection
   - Task validation
   - Timer and scoring

3. **VS Mode**
   - Split-screen setup
   - AI opponent controller
   - Dual state management

4. **UI System**
   - Unity Canvas system
   - Responsive scaling
   - Panel animations

5. **Particle System**
   - Unity Particle System or custom sprites
   - Rice particle effects

## 🚧 Status

**Current:** Foundation scripts provided
**Next Steps:** 
1. Create Unity 6.1 project
2. Import scripts
3. Create UI in Unity Editor
4. Setup scenes
5. Implement particle effects
6. Test and refine

## 📝 Conversion Checklist

- [ ] Create Unity 6.1 project
- [ ] Import C# scripts
- [ ] Setup UI Canvas
- [ ] Create LetterTile prefab
- [ ] Implement particle system
- [ ] Create all scenes (Menu, Game, Score)
- [ ] Setup persistent data (JSON)
- [ ] Implement VS mode split-screen
- [ ] Test responsiveness
- [ ] Build Windows executable

## 💡 Quick Start

**Can't create Unity project right now?**

The provided scripts give you a head start. When ready:
1. Install Unity 6.2 (or any Unity 6.x version)
2. Create new 2D project in this folder
3. Scripts are ready to use
4. Follow Unity tutorials for scene setup

## 🔗 Useful Resources

- Unity 6 Documentation: https://docs.unity3d.com/
- Unity 2D Tutorial: https://learn.unity.com/
- C# for Unity: https://learn.unity.com/tutorial/scripts-actions-and-events

---

**Note:** Full Unity conversion requires Unity Editor to create scenes, UI, and configure components. The C# scripts provided are converted from Godot and ready to adapt to Unity's component system.

