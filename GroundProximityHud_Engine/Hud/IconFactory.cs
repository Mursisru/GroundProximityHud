using System.IO;
using BepInEx;
using UnityEngine;

namespace GroundProximityHud_Engine.Hud
{
    internal sealed class IconFactory
    {
        private const int Size = 48;
        private readonly Sprite _grassSprite;
        private readonly Sprite _runwaySprite;

        internal IconFactory()
        {
            _grassSprite = TryLoadTexturePack("gph_grass.png")
                ?? TryLoadOverride("GroundProximityHud", "grass.png")
                ?? BuildGrassSprite();
            _runwaySprite = TryLoadTexturePack("gph_runway.png")
                ?? TryLoadOverride("GroundProximityHud", "runway.png")
                ?? BuildRunwaySprite();
        }

        internal Sprite GetGrassSprite() => _grassSprite;
        internal Sprite GetRunwaySprite() => _runwaySprite;

        private static Sprite TryLoadTexturePack(string fileName)
        {
            return TryLoadOverride("GPH_Data", fileName);
        }

        private static Sprite TryLoadOverride(string subFolder, string fileName)
        {
            if (!GroundProximityHudPlugin.UsePngOverrides.Value)
                return null;

            string basePath = Paths.PluginPath;
            if (string.IsNullOrEmpty(basePath))
                return null;

            string path = Path.Combine(basePath, subFolder, fileName);
            if (!File.Exists(path))
                return null;

            byte[] bytes = File.ReadAllBytes(path);
            if (bytes == null || bytes.Length == 0)
                return null;

            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tex.LoadImage(bytes))
            {
                Object.Destroy(tex);
                return null;
            }

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite BuildGrassSprite()
        {
            var tex = NewTransparentTexture();
            int horizonY = Mathf.RoundToInt(Size * 0.38f);

            for (int x = 4; x < Size - 4; x++)
            {
                tex.SetPixel(x, horizonY, Color.white);
                if (x % 6 == 0)
                {
                    tex.SetPixel(x, horizonY + 1, Color.white);
                    tex.SetPixel(x, horizonY + 2, Color.white);
                }
            }

            tex.Apply(false, false);
            return Sprite.Create(tex, new Rect(0f, 0f, Size, Size), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite BuildRunwaySprite()
        {
            var tex = NewTransparentTexture();
            int horizonY = Mathf.RoundToInt(Size * 0.28f);
            int nearY = Size - 6;
            int center = Size / 2;

            // Draw a flat runway in perspective with slight right tilt.
            for (int y = horizonY; y <= nearY; y++)
            {
                float t = (float)(y - horizonY) / Mathf.Max(1f, nearY - horizonY);
                float halfWidth = Mathf.Lerp(3f, 13f, t);
                float tilt = Mathf.Lerp(-2f, 2f, t);

                int left = Mathf.RoundToInt(center - halfWidth + tilt);
                int right = Mathf.RoundToInt(center + halfWidth + tilt);
                tex.SetPixel(left, y, Color.white);
                tex.SetPixel(right, y, Color.white);
            }

            for (int y = horizonY + 2; y < nearY - 2; y += 5)
            {
                float t = (float)(y - horizonY) / Mathf.Max(1f, nearY - horizonY);
                float tilt = Mathf.Lerp(-2f, 2f, t);
                int mid = Mathf.RoundToInt(center + tilt);

                tex.SetPixel(mid, y, Color.white);
                tex.SetPixel(mid, y + 1, Color.white);
            }

            tex.Apply(false, false);
            return Sprite.Create(tex, new Rect(0f, 0f, Size, Size), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Texture2D NewTransparentTexture()
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    tex.SetPixel(x, y, clear);
                }
            }

            return tex;
        }
    }
}
