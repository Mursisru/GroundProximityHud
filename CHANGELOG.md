# Changelog

## [0.0.0] - 2026-06-30

### Changed
- Documentation refresh: Developer header, badges, GitHub Alerts, Keywords, gitignore hygiene.


## 1.1.0 - 2026-05-10

- Hide marker when **AGL < 0.3 m** (on ground).
- When **AGL > 50 m**, marker alpha is **50%** of the HUD/tint color (semi-transparent).

## 1.0.9 - 2026-05-10

- Marker is **parented under the FlightHud Canvas** (same object as `FlightHud` + `Canvas`), so it **hides with the flight HUD** when `FlightHud.EnableCanvas(false)` (pause, settings, etc.).
- Default **`ZeroAglDownShiftPixels` = 16**.
- **`UseHudColor`** (default on): tint from **`PlayerSettings.hudColorR/G/B`** (Settings → HUD color). **`MarkerColorHex`** when off.

## 1.0.8 - 2026-05-10

- Restored behavior from **v1.0.4** (“smaller + a bit lower”): defaults `IconHeightPixels` **7**, `MaxIconWidthPixels` **140**, `ZeroAglDownShiftPixels` **14**; **radarAlt-first** AGL; marker `position` set directly (no `RectTransformUtility` remap).

## 1.0.7 - 2026-05-10

- (Superseded by 1.0.8) Linecast-first AGL; CanvasScaler screen-point remap.

## 1.0.6 - 2026-05-10

- Default `ZeroAglDownShiftPixels` **138**.

## 1.0.5 - 2026-05-10

- Defaults: `IconHeightPixels` **8.5**, `ZeroAglDownShiftPixels` **130**.

## 1.0.4 - 2026-05-10

- Smaller default marker: `IconHeightPixels` **7**, `MaxIconWidthPixels` **140**.
- Lower on screen: `ZeroAglDownShiftPixels` default **14** (tune in cfg).

## 1.0.3 - 2026-05-10

- Fix oversized wide PNG (834:62): default `IconHeightPixels` lowered to **10**; added **`MaxIconWidthPixels`** (default **200**) to cap bar width and rescale height.
- CanvasScaler now uses **1920×1080** reference + match 0.5 for predictable HUD scale.

## 1.0.2 - 2026-05-10

- Load white PNG pack from `BepInEx/plugins/GPH_Data/gph_grass.png` and `gph_runway.png` (tint via `MarkerColorHex`).
- Legacy folder `plugins/GroundProximityHud/grass.png` / `runway.png` still used as fallback if pack files are missing.
- `Icons.IconHeightPixels` sets on-screen height; width follows sprite aspect (834×62 for pack art).

## 1.0.1 - 2026-05-10

- Marker tint is always a single configurable green (`Icons.MarkerColorHex`); removed altitude/gear-based red/half-alpha rules.
- Added `General.ZeroAglDownShiftPixels` to nudge the AGL=0 position slightly downward on screen.

## 1.0.0 - 2026-05-10

- Initial release of **Ground Proximity HUD**.
- Adds center-referenced ground-proximity marker for Flight HUD.
- Marker motion is based on AGL (`radarAlt`) with linecast fallback.
- Marker switches icon/color by landing gear state:
  - runway + green when gear is deployed
  - grass + red when gear is retracted
- Hybrid icon strategy:
  - procedural icons by default
  - optional PNG overrides from `BepInEx/plugins/GroundProximityHud/`.
