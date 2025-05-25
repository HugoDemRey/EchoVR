# 🕹️ ECHO

Welcome! This repository contains the full source code and assets required to build and run the game.

### Produced Scripts

These were **entirely created by our team** for this project.

- `Scripts/Checkpoint/CheckpointManager.cs`
- `Scripts/Checkpoint/CheckpointState.cs`
- `Scripts/Checkpoint/MarkCheckpoint.cs`
- `Scripts/Checkpoint/MarkTutorialCheckpoint.cs`
- `Scripts/Checkpoint/TutorialCheckpointManager.cs`
- `Scripts/Climbing/ClimbHandSnapping.cs`
- `Scripts/Climbing/ClimbingMomentumManager.cs`
- `Scripts/Climbing/OverrideMomentumForce.cs`
- `Scripts/Cooking/CookingManager.cs`
- `Scripts/Cooking/EatSteak.cs`
- `Scripts/Cooking/PanNearFireDetector.cs`
- `Scripts/Cooking/SteakStickToPan.cs`
- `Scripts/Falling/FadingScript.cs`
- `Scripts/Falling/FallingCheckpoint.cs`
- `Scripts/FinalZone/FadeoutMusic.cs`
- `Scripts/Guitar/GuitarSoundController.cs`
- `Scripts/Guitar/ProximityStringDetector.cs`
- `Scripts/Harpoon/ArrowSocket.cs` - extends _XRSocketInteractor_ from _XR interaction Toolkit 3.1.2_
- `Scripts/Harpoon/HarpoonBehavior.cs`
- `Scripts/Other/ControllerTracking.cs`
- `Scripts/Other/ShowInEditModeOnly.cs`
- `Scripts/Other/XRLimitedGrabInteractable.cs` - extends _XRGrabInteractable_ from _XR interaction Toolkit 3.1.2_
- `Scripts/Other/MenuManager.cs`
- `Scripts/Rope/RopeBehavior.cs`
- `Scripts/Rope/ZipLineBehavior.cs` - extends _XRSocketInteractor_ from _XR interaction Toolkit 3.1.2_
- `Scripts/Rope/ZipLineHandleBehavior.cs` - extends _ClimbInteractable_ from _XR interaction Toolkit 3.1.2_
- `Scripts/Turn/SnapListener.cs`

### Adapted Scripts

These were **sourced externally but modified** to suit our game.

- `Scripts/Turn/CustomSnapTurnProvider` – adapted from the _SnapTurnProvider_ script from the _XR Interaction Toolkit 3.1.2_.
- `Assets/Turn/CustomContinuousTurnProvider.cs` – adapted from the _ContinuousTurnProvider_ script from the _XR Interaction Toolkit 3.1.2_


### Unmodified Scripts

These were **used as-is from external sources**.

- All scripts included in the default XR Origin Prefab prefab in the XRIT 3.0.7 Demo Scene - https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/samples-starter-assets.html#prefabs
- `ClimbInteractable.cs` from the _XR Interaction Toolkit 3.1.2_
- `GrabInteractable.cs` from the _XR Interaction Toolkit 3.1.2_
- `XRSocketInteractor` from the _XR Interaction Toolkit 3.1.2_
- `XRUIInputModule` from the _XR Interaction Toolkit 3.1.2_ 

### External Assets

**Unity Packages**

- [AA Medieval Low-Poly Environments](https://assetstore.unity.com/packages/3d/environments/aa-low-poly-medieval-environment-249533)
- [Acoustic Guitar](https://assetstore.unity.com/packages/3d/props/acoustic-guitar-21037)
- [Cartoon FX Remaster Free](https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565)
- [Wood Set Pieces](https://assetstore.unity.com/packages/3d/props/wood-set-pieces-33853)
- [Radio](https://assetstore.unity.com/packages/3d/props/radio-230712)
- [VFX URP - Fire Package](https://assetstore.unity.com/packages/vfx/particles/fire-explosions/vfx-urp-fire-package-305098)
- [25+ Free Realistic Textures - Nature, City, Home, Construction..](https://assetstore.unity.com/packages/2d/textures-materials/25-free-realistic-textures-nature-city-home-construction-more-240323)
- [Fantasy Skybox FREE](https://assetstore.unity.com/packages/2d/textures-materials/sky/fantasy-skybox-free-18353)
- [Pandazole - Nature Environment Low poly Pack](https://assetstore.unity.com/packages/3d/environments/pandazole-nature-environment-low-poly-pack-212621)
- [Stylized Nature Textures](https://assetstore.unity.com/packages/2d/textures-materials/stylized-nature-textures-228680)
- [Lowpoly Environment Pack - Mesa and Desert Rocks](https://assetstore.unity.com/packages/3d/environments/landscapes/lowpoly-environment-pack-mesa-and-desert-rocks-167831)
- [Low Poly Environment](https://assetstore.unity.com/packages/3d/environments/low-poly-environment-315184)
- [Fristy Fantasy Lands 1](https://assetstore.unity.com/packages/3d/environments/fantasy/fristy-fantasy-lands-1-198504)
- [Low poly rocks | Multiple variants](https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-rocks-multiple-variants-254461)
- [Low Poly Cliff Pack](https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-cliff-pack-67289)
- [Island Assets](https://assetstore.unity.com/packages/3d/environments/island-assets-56989)
- [Standard Assets 2018.4](https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-2018-4-check-out-starter-assets-first-person-thi-32351)
- [RPG Poly Pack - Lite](https://assetstore.unity.com/packages/3d/environments/landscapes/rpg-poly-pack-lite-148410)
- [Simple Foods](https://assetstore.unity.com/packages/3d/props/food/simple-foods-207032)

**Others**

- **Harpoon**
  - Sounds: [sample1](https://pixabay.com/sound-effects/grappling-gun-132447/), [sample2](https://pixabay.com/sound-effects/item-equip-6904/), [sample3](https://pixabay.com/sound-effects/pop-1-101427/), [sample4](https://pixabay.com/sound-effects/error-8-206492/), [sample5](https://pixabay.com/sound-effects/camera-shutter-107889/)
  - Crosshairs: [SVGViewer](https://www.svgviewer.dev/), [Freepik](https://www.freepik.com)

- **Guitar**
  - Guitar Music: [link](https://pixabay.com/music/folk-guitar-ambient-339839/)

- **Cooking**
  - Eating: [link](https://pixabay.com/sound-effects/eating-sound-effect-36186/)
  - Cooking: [link](https://pixabay.com/sound-effects/cooking-onions-72799/)

- **Ambients Sounds**
  - Music by *orangery* from Pixabay: [link](https://pixabay.com/music/acoustic-group-coniferous-forest-142569/)
  - Wind Sound: [link](https://pixabay.com/sound-effects/smooth-cold-wind-looped-135538/)
