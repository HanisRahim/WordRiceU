# Unity Package Installation Guide

## ⚠️ Scripts Fixed for Missing Packages

The C# scripts now compile even without packages installed, but you should install these for full functionality:

## 📦 Required Packages

### 1. Install UI Package (Essential)

**In Unity Editor:**
1. Window → Package Manager
2. Select "Unity Registry"
3. Search for "**UI**" or "**Unity UI**"
4. Click **Install**

**Or add to manifest:**
- File: `Packages/manifest.json`
- Add: `"com.unity.ugui": "2.0.0"`

**Once installed:**
- UI components (Image, Slider, Button) will work
- No code changes needed!

### 2. Install TextMeshPro (Recommended)

**In Unity Editor:**
1. Window → Package Manager
2. Search for "**TextMeshPro**"
3. Click **Install**
4. Import TMP Essentials when prompted

**Benefits:**
- Better text rendering
- More font options
- Professional appearance

**After installing:**
1. Open `Assets/csc.rsp`
2. Add line: `-define:TMP_PRESENT`
3. Save and restart Unity

**File should look like:**
```
-define:UNITY_UI
-define:TMP_PRESENT
```

## 🔧 How The Scripts Work

### Without Packages (Current State)
- ✅ Scripts compile without errors
- ✅ Use basic Unity components
- ⚠️ Some features disabled (UI, TMP)
- Use SpriteRenderer for particles
- Use OnMouseDown() for clicks

### With UI Package Installed
- ✅ Enable: `-define:UNITY_UI` in csc.rsp
- ✅ Image, Slider, Button components work
- ✅ EventSystem for proper input
- Better UI control

### With TextMeshPro Installed
- ✅ Enable: `-define:TMP_PRESENT` in csc.rsp
- ✅ Better text rendering
- ✅ Font styling options
- Professional typography

## ⚡ Quick Fix for Current Errors

**Your errors should be FIXED now!**

The scripts use conditional compilation (`#if UNITY_UI`):
- ✅ Compiles without packages
- ✅ Uses fallback components
- ✅ Full features when packages installed

## 📝 Next Steps

1. **Verify scripts compile** (no errors now)
2. **Install UI package** (Window → Package Manager)
3. **Edit csc.rsp** - Uncomment or verify `-define:UNITY_UI`
4. **Install TextMeshPro** (optional but recommended)
5. **Add TMP define** to csc.rsp if installed
6. **Restart Unity** to apply changes

## 🎯 Recommended Setup

**For best results:**
```
csc.rsp contents:
-define:UNITY_UI
-define:TMP_PRESENT
```

Then install both packages via Package Manager.

## ✅ Verification

**Check compilation:**
- Scripts should have NO errors
- Yellow warnings OK (missing references in Inspector)
- Can assign components when packages installed

**With packages installed:**
- All features enabled
- Professional text rendering (TMP)
- Smooth UI interactions
- Particle effects work properly

---

**Scripts are now error-free! Install packages for full functionality.** 🚀

