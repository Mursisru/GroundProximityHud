using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace GroundProximityHud_Engine
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class GroundProximityHudPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.at747.groundproximityhud";
        public const string PluginName = "Ground Proximity HUD";
        public const string PluginVersion = "1.1.0";

        internal static GroundProximityHudPlugin Instance { get; private set; }

        internal static ConfigEntry<bool> Enabled { get; private set; }
        internal static ConfigEntry<float> MaxDisplayAglMeters { get; private set; }
        internal static ConfigEntry<float> CenterOffsetPixels { get; private set; }
        internal static ConfigEntry<float> ZeroAglDownShiftPixels { get; private set; }
        internal static ConfigEntry<float> SmoothTime { get; private set; }
        internal static ConfigEntry<bool> ShowOnlyWhenFlightControlsEnabled { get; private set; }
        internal static ConfigEntry<bool> UsePngOverrides { get; private set; }
        internal static ConfigEntry<float> IconHeightPixels { get; private set; }
        internal static ConfigEntry<float> MaxIconWidthPixels { get; private set; }
        internal static ConfigEntry<bool> UseHudColor { get; private set; }
        internal static ConfigEntry<string> MarkerColorHex { get; private set; }

        private void Awake()
        {
            Instance = this;
            Enabled = Config.Bind("General", "Enabled", true, "Enable the ground-proximity HUD marker.");
            MaxDisplayAglMeters = Config.Bind("General", "MaxDisplayAglMeters", 300f,
                "Marker is hidden above this AGL altitude. At this altitude it appears farthest from HUD center.");
            CenterOffsetPixels = Config.Bind("General", "CenterOffsetPixels", 220f,
                "Maximum distance in pixels below HUD center when near max display altitude.");
            ZeroAglDownShiftPixels = Config.Bind("General", "ZeroAglDownShiftPixels", 16f,
                "Extra downward shift from HUD center (reference px), all altitudes — sits the strip lower on screen.");
            SmoothTime = Config.Bind("General", "SmoothTime", 0.08f,
                "Smoothing time for marker movement in seconds.");
            ShowOnlyWhenFlightControlsEnabled = Config.Bind("General", "ShowOnlyWhenFlightControlsEnabled", true,
                "When true, hides marker while menus or non-flight controls are active.");
            UsePngOverrides = Config.Bind("Icons", "UsePngOverrides", true,
                "If true, loads white PNGs from BepInEx/plugins/GPH_Data/ (gph_grass.png, gph_runway.png); tint follows HUD color when UseHudColor is on.");
            IconHeightPixels = Config.Bind("Icons", "IconHeightPixels", 7f,
                "Target height in CanvasScaler reference pixels; width = height * texture aspect. Wide 834:62 art: try ~6–10.");
            MaxIconWidthPixels = Config.Bind("Icons", "MaxIconWidthPixels", 140f,
                "Clamp width so the bar cannot span the whole HUD. Set 0 to disable. Height is reduced to keep aspect.");
            UseHudColor = Config.Bind("Icons", "UseHudColor", true,
                "If true, marker tint matches Settings → HUD color (PlayerSettings). If false, uses MarkerColorHex.");
            MarkerColorHex = Config.Bind("Icons", "MarkerColorHex", "#00FF66FF",
                "Marker tint when UseHudColor is false. Hex RGBA.");

            Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }

        private void Update()
        {
            GroundProximityHudController.Tick();
        }

        private void OnDestroy()
        {
            GroundProximityHudController.Shutdown();
        }
    }
}
