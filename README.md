# Ground Proximity HUD

BepInEx plugin for **Nuclear Option** that adds a central Flight HUD ground-proximity marker:

- marker appears when approaching terrain (within configurable AGL range);
- closer to ground -> marker moves closer to HUD center;
- marker is always tinted green by default (`Icons.MarkerColorHex`);
- marker is **part of the FlightHud canvas** (hidden when the stock flight HUD is off, e.g. settings/pause);
- marker tint matches **Settings → HUD color** by default (`Icons.UseHudColor`);
- vertical nudge via `General.ZeroAglDownShiftPixels` (default **16**);
- hidden when **AGL < 0.3 m**; **50% transparency** when **AGL > 50 m**;
- icon shape still switches between grass (gear up) and runway (gear down).

## Requirements

- Nuclear Option (Steam)
- BepInEx 5 x64
- BepInEx ConfigurationManager (optional)

## Install

1. Get `GroundProximityHud_Engine.dll`.
2. Copy it to `Nuclear Option\BepInEx\plugins\`.
3. Put GPH_Data in `Nuclear Option\BepInEx\plugins\`

## Configuration

File:
`BepInEx\config\com.at747.groundproximityhud.cfg`

- `General.Enabled` - master toggle
- `General.MaxDisplayAglMeters` - marker hidden above this AGL
- `General.CenterOffsetPixels` - distance from HUD center at far edge
- `General.ZeroAglDownShiftPixels` - extra downward shift from HUD center (reference px, all altitudes; default 16)
- `General.SmoothTime` - movement smoothing
- `General.ShowOnlyWhenFlightControlsEnabled` - hide in menus/paused control states
- `Icons.UsePngOverrides` - load PNG pack from `GPH_Data` (and legacy paths as fallback)
- `Icons.IconHeightPixels` - marker height (reference px); width = height × aspect — default **7** for 834×62 art
- `Icons.MaxIconWidthPixels` - max width (default 140); **0** = no cap
- `Icons.UseHudColor` - use game HUD RGB (`PlayerSettings`) for tint (default true)
- `Icons.MarkerColorHex` - tint when `UseHudColor` is false

## Manual test checklist

1. Descend with gear up: grass texture (`gph_grass.png`) tinted green moves toward HUD center as AGL drops.
2. Deploy gear mid-descent: icon switches to runway texture (`gph_runway.png`), same tint.
3. Climb above `MaxDisplayAglMeters`: marker should hide.
4. Pause/open non-flight state (if `ShowOnlyWhenFlightControlsEnabled=true`): marker hides.
5. With `GPH_Data` PNGs present and `UsePngOverrides=true`, restart: wide 834×62 art should display at configured `IconHeightPixels`.
