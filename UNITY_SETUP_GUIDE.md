# Word Rice - Unity 6.2 Setup Guide

## 🎯 Overview

This folder contains **C# scripts converted from the Godot version** of Word Rice. To create the full Unity project, you'll need to use Unity Editor 6.2 or later Unity 6.x version.

## ⚠️ Important Note

**Unity projects cannot be fully created via command line or scripts.** You must:
1. Use Unity Hub and Unity Editor 6.1
2. Create the project through the Editor
3. Import the provided C# scripts
4. Build the UI and scenes in the Editor

## 📋 Complete Setup Process

### Step 1: Install Unity 6.2

1. Download **Unity Hub** from https://unity.com/download
2. Install **Unity 6.2** (or latest 6.x) via Unity Hub
3. Make sure to include:
   - ✅ Windows Build Support
   - ✅ Visual Studio Community (for C# editing)

### Step 2: Create New Unity Project

1. Open **Unity Hub**
2. Click **New Project**
3. Select **Unity 6.1.x**
4. Choose **2D (Core)** template
5. **Project Name:** Word Rice
6. **Location:** Select the `Unity-Version` folder
7. Click **Create Project**

Unity will generate:
- Complete Assets/ structure
- ProjectSettings/
- Library/, Temp/, etc.

### Step 3: Install Required Packages

In Unity Editor:
1. Open **Window → Package Manager**
2. Install these packages:
   - **TextMeshPro** (better text rendering)
   - **2D Sprite** (for graphics)
   - **2D Tilemap Editor** (optional)

Optional but recommended:
- **DOTween** (from Asset Store - for smooth animations)

### Step 4: Copy Provided Scripts

The `Assets/Scripts/` folder already contains converted C# scripts:
- ✅ `GlobalData.cs` - Singleton game state manager
- ✅ `GameManager.cs` - Solo mode gameplay controller
- ✅ `LetterTile.cs` - Interactive letter component
- ✅ `RiceParticle.cs` - Particle effect component

**If you created the project elsewhere:**
- Copy these scripts to your Unity project's `Assets/Scripts/` folder

### Step 5: Create UI Canvas

1. **Right-click in Hierarchy** → UI → Canvas
2. Set Canvas Scaler:
   - UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1280 x 720`
   - Match: `0.5` (width/height balanced)

3. **Add UI Elements:**
   - Timer Label (TextMeshPro)
   - Score Label (TextMeshPro)
   - Progress Bar (Slider)
   - Task Display panel
   - Background images

### Step 6: Create Prefabs

**LetterTile Prefab:**
1. Create Empty GameObject → Name: `LetterTile`
2. Add components:
   - `Image` (for background)
   - `LetterTile.cs` script
   - `BoxCollider2D` or use Unity's Button component
3. Add child:
   - TextMeshPro for letter display
4. Drag to `Assets/Prefabs/` to create prefab

**RiceParticle Prefab:**
1. Create GameObject → Name: `RiceParticle`
2. Add components:
   - `Image` or `SpriteRenderer`
   - `RiceParticle.cs` script
3. Create sprite:
   - White oval/rice shape (can use Unity's built-in sprites)
4. Drag to `Assets/Prefabs/` to create prefab

### Step 7: Setup Scenes

Create these scenes:
1. **MainMenu** - Entry point with mode selection
2. **Instructions** - Quick tutorial with countdown
3. **GameScene** - Solo gameplay
4. **VSGameScene** - Split-screen VS mode
5. **ScoreScreen** - Results and leaderboard
6. **VSScoreScreen** - VS mode results

### Step 8: Configure GameManager

1. Create Empty GameObject in GameScene → Name: `GameManager`
2. Add `GameManager.cs` script
3. Assign references in Inspector:
   - Timer Label
   - Score Label
   - Progress Bar
   - Task Display Parent
   - Letter Pool Parent
   - Particle Parent
   - Letter Tile Prefab
   - Rice Particle Prefab

### Step 9: Create GlobalData Singleton

1. Create Empty GameObject → Name: `GlobalData`
2. Add `GlobalData.cs` script
3. **Important:** Right-click GameObject → Make Persistent
4. Or add to a "DontDestroyOnLoad" scene

### Step 10: Build and Test

1. Configure Build Settings:
   - File → Build Settings
   - Add all scenes in order
   - Platform: Windows
   - Architecture: x86_64

2. Test in Unity Editor (Play button)
3. Fix any missing references
4. Build executable when ready

## 🔄 Key Conversion Notes

### GDScript → C# Differences

| Godot GDScript | Unity C# | Notes |
|----------------|----------|-------|
| `extends Node` | `: MonoBehaviour` | Base class |
| `@onready var x = $Path` | Cache in `Start()` | Component references |
| `func _ready()` | `void Start()` | Initialization |
| `func _process(delta)` | `void Update()` | Per-frame |
| `var x: int = 0` | `private int x = 0;` | Variables |
| Signals | Events / Actions | Communication |
| `Vector2.ZERO` | `Vector2.zero` | Constants |
| `randf_range(a, b)` | `Random.Range(a, b)` | Random |
| `Color("hex")` | `new Color32(r,g,b,a)` | Colors |
| Tween | Coroutines / DOTween | Animations |
| `queue_free()` | `Destroy(gameObject)` | Cleanup |

### Unity-Specific Implementations

**UI System:**
- Godot: Control nodes, Labels, Panels
- Unity: Canvas, Image, TextMeshPro, Panels

**Input:**
- Godot: Area2D with input_event signal
- Unity: IPointerClickHandler interface or Button component

**Scenes:**
- Godot: .tscn files
- Unity: .unity scenes created in Editor

**Persistence:**
- Godot: `user://` path with FileAccess
- Unity: `Application.persistentDataPath` with System.IO

**Particles:**
- Godot: Custom Node2D with physics
- Unity: Particle System or custom sprite objects

## 📦 What's Provided

### ✅ Converted Scripts

1. **GlobalData.cs** - Complete singleton with:
   - Game state management
   - Leaderboard (JSON persistence)
   - AI stats tracking
   - 80+ word dictionary
   - 48 Malaysian names
   - Difficulty system

2. **GameManager.cs** - Solo mode with:
   - Letter tile spawning
   - Collision avoidance
   - Task management
   - Timer with color transitions
   - Scoring system
   - Particle spawning

3. **LetterTile.cs** - Interactive component with:
   - Click detection (IPointerClickHandler)
   - Hover effects
   - Selection marking
   - Flying animation
   - Shake animation

4. **RiceParticle.cs** - Particle effect with:
   - Physics simulation (gravity, air resistance)
   - Fade system (70% opaque, then fade)
   - Rotation
   - Auto-cleanup

### 🚧 What You Need to Create in Unity Editor

1. **UI Layouts** - Use Unity's Canvas system
2. **Scenes** - 6 scenes (Menu, Instructions, Game×2, Score×2)
3. **Prefabs** - LetterTile and RiceParticle
4. **Materials** - For rice particle sprite
5. **VS Mode Script** - Convert VSGameManager.gd to C#
6. **UI Scripts** - Menu, Score screens, etc.

## 💡 Next Steps

### Immediate Actions

1. **Create Unity 6.1 project** in this folder
2. **Verify scripts** are in `Assets/Scripts/`
3. **Install TextMeshPro** via Package Manager
4. **Create UI Canvas** with proper scaling
5. **Build LetterTile prefab** with Image + Script
6. **Create RiceParticle prefab** with sprite
7. **Setup GlobalData** as persistent singleton

### Development Order

1. ✅ Scripts converted (provided)
2. Create UI for MainMenu
3. Implement MainMenu.cs script
4. Create GameScene with GameManager
5. Test solo mode basics
6. Create VS mode (convert VSGameManager.gd)
7. Build all remaining scenes
8. Polish and test
9. Export to Windows

## 🎨 UI Guidelines

### Canvas Setup
```
Canvas (Screen Space - Overlay)
├── Background (Image - tan #BF9669)
├── TopSection (Panel)
│   ├── TimerPanel
│   │   └── TimerLabel (TextMeshPro)
│   └── ProgressBar (Slider)
├── TaskDisplay (HorizontalLayoutGroup)
│   └── 4× TaskSlot (Image + TextMeshPro)
├── ScorePanel
│   └── ScoreLabel (TextMeshPro)
└── LetterPool (Empty Transform)
    └── Letter tiles spawn here
```

### Colors (match Godot version)
- Background: `#BF9669` (tan)
- Panel BG: `#262019` (dark brown)
- Border: `#DAA460` (golden)
- Text: `#FFFEF0` (cream)
- Gold: `#FFD700`
- Green: `#25DD4B`
- Red: `#FF3333`

## 📚 Additional Resources

### Unity 6 Tutorials
- Unity Learn: https://learn.unity.com/
- 2D Game Development: https://learn.unity.com/tutorial/2d-game-kit
- UI System: https://learn.unity.com/tutorial/ui-components

### C# for Unity
- Unity Scripting: https://docs.unity3d.com/Manual/ScriptingSection.html
- C# Documentation: https://learn.microsoft.com/en-us/dotnet/csharp/

### DOTween (Recommended)
- Asset Store: Search "DOTween"
- Easier animations than Coroutines
- Similar to Godot's Tween system

## ⚡ Quick Reference

### Common Unity Patterns

**Singleton:**
```csharp
public static GlobalData Instance { get; private set; }
void Awake() {
    if (Instance == null) {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    } else {
        Destroy(gameObject);
    }
}
```

**Coroutine Animation:**
```csharp
IEnumerator AnimateScale(float duration) {
    float elapsed = 0f;
    while (elapsed < duration) {
        elapsed += Time.deltaTime;
        transform.localScale = Vector3.one * (1f + Mathf.Sin(elapsed * 10f) * 0.1f);
        yield return null;
    }
}
```

**Scene Loading:**
```csharp
using UnityEngine.SceneManagement;
SceneManager.LoadScene("SceneName");
```

## 🎯 Success Criteria

Your Unity version is complete when:
- ✅ Solo mode fully playable (30s, scoring, particles)
- ✅ VS mode with AI opponent works
- ✅ Leaderboard saves and loads
- ✅ UI scales properly across resolutions
- ✅ All animations smooth
- ✅ Can export to Windows .exe

## 📞 Support

**Stuck? Check:**
1. Unity Console for errors
2. Component references assigned in Inspector
3. Scripts compiled without errors
4. Scenes added to Build Settings
5. Prefabs properly configured

**Common Issues:**
- Missing references → Assign in Inspector
- NullReferenceException → Check Start() initialization
- UI not showing → Check Canvas settings
- Prefab errors → Recreate prefab from scratch

---

**Ready to build?** Follow the steps above to create your Unity 6.1 version of Word Rice! 🍚🎮

The foundation is here - Unity Editor will bring it to life! 🚀

