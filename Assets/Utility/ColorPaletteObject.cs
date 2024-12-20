using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "UIUtilities/Create color palette")]
    public class ColorPaletteObject : ScriptableObject
    {
        public Color Yellow = Color.white;
        public Color Red = Color.white;
        public Color Green = Color.white;
        public Color White = Color.white;
        public Color DarkBrown = Color.white;
        public Color Brown = Color.white;
        public Color LightBrown = Color.white;
        public Color LightGray = Color.white;

        public List<Color> Colors => new List<Color>
        {
            Yellow, Red, Green, White, DarkBrown, Brown, LightBrown, LightGray
        };
    }
}