using UnityEngine;
using GroundProximityHud_Engine.Hud;

namespace GroundProximityHud_Engine
{
    internal static class GroundProximityHudController
    {
        private const float MaxRaycastDistance = 10000f;
        private const int TerrainMask = 2112;
        /// <summary>Below this AGL the marker is hidden (on ground / wheel height).</summary>
        private const float MinVisibleAglMeters = 0.3f;

        private static GroundProximityView _view;
        private static float _smoothedOffset;
        private static float _smoothVelocity;

        internal static void Tick()
        {
            if (!GroundProximityHudPlugin.Enabled.Value)
            {
                Hide();
                return;
            }

            if (!AllowHud())
            {
                Hide();
                return;
            }

            Aircraft aircraft;
            if (!GameManager.GetLocalAircraft(out aircraft) || aircraft == null || aircraft.disabled)
            {
                Hide();
                return;
            }

            Transform hudCenter = ResolveHudCenter();
            if (hudCenter == null)
            {
                Hide();
                return;
            }

            float aglMeters;
            if (!TryGetAglMeters(aircraft, out aglMeters))
            {
                Hide();
                return;
            }

            if (aglMeters < MinVisibleAglMeters)
            {
                Hide();
                return;
            }

            float maxDisplayAgl = Mathf.Max(10f, GroundProximityHudPlugin.MaxDisplayAglMeters.Value);
            if (aglMeters > maxDisplayAgl)
            {
                Hide();
                return;
            }

            float maxOffset = Mathf.Max(10f, GroundProximityHudPlugin.CenterOffsetPixels.Value);
            float normalized = Mathf.Clamp01(aglMeters / maxDisplayAgl);
            float targetOffset = normalized * maxOffset;

            float smoothTime = Mathf.Clamp(GroundProximityHudPlugin.SmoothTime.Value, 0f, 1f);
            _smoothedOffset = smoothTime <= 0f
                ? targetOffset
                : Mathf.SmoothDamp(_smoothedOffset, targetOffset, ref _smoothVelocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

            if (!EnsureView())
                return;

            _view.SetVisible(true);
            float zeroShift = Mathf.Max(0f, GroundProximityHudPlugin.ZeroAglDownShiftPixels.Value);
            _view.SetMarker(hudCenter.position, _smoothedOffset + zeroShift, aircraft.gearDeployed, aglMeters);
        }

        internal static void Shutdown()
        {
            if (_view != null)
            {
                _view.Dispose();
                _view = null;
            }
        }

        private static bool AllowHud()
        {
            GameState state = GameManager.gameState;
            if (state != GameState.SinglePlayer && state != GameState.Multiplayer)
                return false;

            if (GroundProximityHudPlugin.ShowOnlyWhenFlightControlsEnabled.Value && !GameManager.flightControlsEnabled)
                return false;

            FlightHud fh = SceneSingleton<FlightHud>.i;
            if (fh == null)
                return false;
            Canvas flightCanvas = fh.GetComponent<Canvas>();
            if (flightCanvas != null && !flightCanvas.gameObject.activeSelf)
                return false;

            return SceneSingleton<CameraStateManager>.i != null;
        }

        private static Transform ResolveHudCenter()
        {
            FlightHud flightHud = SceneSingleton<FlightHud>.i;
            return flightHud == null ? null : flightHud.GetHUDCenter();
        }

        private static bool TryGetAglMeters(Aircraft aircraft, out float aglMeters)
        {
            // Primary: built-in radar altitude (same as pre–1.0.5 tuning).
            aglMeters = aircraft.radarAlt;
            if (!float.IsNaN(aglMeters) && !float.IsInfinity(aglMeters) && aglMeters >= 0f)
                return true;

            RaycastHit hit;
            if (Physics.Linecast(aircraft.transform.position, aircraft.transform.position - Vector3.up * MaxRaycastDistance, out hit, TerrainMask))
            {
                float spawnOffset = (aircraft.definition != null) ? aircraft.definition.spawnOffset.y : 0f;
                aglMeters = Mathf.Max(0f, hit.distance - spawnOffset);
                return true;
            }

            aglMeters = 0f;
            return false;
        }

        /// <summary>FlightHud canvas transform (marker is parented here so it hides with the stock HUD in menus/pause).</summary>
        private static Transform GetFlightHudCanvasTransform()
        {
            FlightHud fh = SceneSingleton<FlightHud>.i;
            if (fh == null)
                return null;
            Canvas c = fh.GetComponent<Canvas>();
            return c != null ? c.transform : fh.transform;
        }

        /// <returns>false if FlightHud canvas is not ready.</returns>
        private static bool EnsureView()
        {
            Transform parent = GetFlightHudCanvasTransform();
            if (parent == null)
                return false;

            if (_view != null && !_view.NeedsRebuild(parent))
                return true;

            if (_view != null)
            {
                _view.Dispose();
                _view = null;
            }

            _view = new GroundProximityView(parent);
            return true;
        }

        private static void Hide()
        {
            if (_view != null)
                _view.SetVisible(false);
        }
    }
}
