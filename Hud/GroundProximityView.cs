using UnityEngine;
using UnityEngine.UI;

namespace GroundProximityHud_Engine.Hud
{
    internal sealed class GroundProximityView
    {
        private static readonly Color DefaultMarkerGreen = new Color(0f, 1f, 0.4f, 1f);
        /// <summary>Above this AGL, marker uses 50% of base alpha (semi-transparent).</summary>
        private const float HalfAlphaAboveAglMeters = 50f;
        private const float AlphaMultiplierWhenFar = 0.5f;

        private readonly GameObject _root;
        private readonly Image _icon;
        private readonly IconFactory _iconFactory;

        internal GroundProximityView(Transform flightHudCanvasTransform)
        {
            _iconFactory = new IconFactory();
            _root = new GameObject("GroundProximityMarker");
            _root.transform.SetParent(flightHudCanvasTransform, false);

            _icon = _root.AddComponent<Image>();
            _icon.raycastTarget = false;

            RectTransform rect = _icon.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            ApplyIconSize(rect, _iconFactory.GetGrassSprite());
        }

        internal bool NeedsRebuild(Transform flightHudCanvasTransform)
        {
            if (flightHudCanvasTransform == null || _root == null)
                return true;
            return _root.transform.parent != flightHudCanvasTransform;
        }

        internal void SetVisible(bool visible)
        {
            if (_root != null && _root.activeSelf != visible)
                _root.SetActive(visible);
        }

        internal void SetMarker(Vector3 hudCenterScreenPosition, float offsetBelowCenter, bool gearDeployed, float aglMeters)
        {
            Sprite sprite = gearDeployed ? _iconFactory.GetRunwaySprite() : _iconFactory.GetGrassSprite();
            _icon.sprite = sprite;
            Color c = ResolveMarkerColor();
            if (aglMeters > HalfAlphaAboveAglMeters)
                c.a *= AlphaMultiplierWhenFar;
            _icon.color = c;

            RectTransform rect = _icon.rectTransform;
            ApplyIconSize(rect, sprite);
            rect.position = new Vector3(hudCenterScreenPosition.x, hudCenterScreenPosition.y - offsetBelowCenter, 0f);
        }

        internal void Dispose()
        {
            if (_root != null)
                Object.Destroy(_root);
        }

        private static Color ResolveMarkerColor()
        {
            if (GroundProximityHudPlugin.UseHudColor.Value)
            {
                return new Color(
                    PlayerSettings.hudColorR / 255f,
                    PlayerSettings.hudColorG / 255f,
                    PlayerSettings.hudColorB / 255f,
                    1f);
            }

            string hex = GroundProximityHudPlugin.MarkerColorHex.Value;
            Color parsed;
            return !string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out parsed)
                ? parsed
                : DefaultMarkerGreen;
        }

        private static void ApplyIconSize(RectTransform rect, Sprite sprite)
        {
            float aspect = sprite != null && sprite.rect.height > 0.01f
                ? sprite.rect.width / sprite.rect.height
                : 1f;

            float h = Mathf.Clamp(GroundProximityHudPlugin.IconHeightPixels.Value, 4f, 120f);
            float w = h * aspect;

            float maxW = GroundProximityHudPlugin.MaxIconWidthPixels.Value;
            if (maxW > 0.01f && w > maxW)
            {
                w = maxW;
                h = w / Mathf.Max(0.01f, aspect);
            }

            rect.sizeDelta = new Vector2(w, h);
        }
    }
}
