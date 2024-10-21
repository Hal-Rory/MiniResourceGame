using UnityEngine;
using UnityEngine.UI;

namespace Common.Utility
{
    public static class ColorExtensions
    {
        public static ColorBlock Copy(this ColorBlock other)
        {
            ColorBlock colors = new ColorBlock();
            colors.normalColor= other.normalColor;
            colors.highlightedColor= other.highlightedColor;
            colors.colorMultiplier= other.colorMultiplier;
            colors.disabledColor= other.disabledColor;
            colors.selectedColor= other.selectedColor;
            colors.pressedColor= other.pressedColor;
            colors.fadeDuration= other.fadeDuration;

            return colors;
        }

        public static bool IsColor(this Color color, Color other)
        {
            return Mathf.Approximately(color.r, other.r) 
                   && Mathf.Approximately(color.g, other.g) 
                   && Mathf.Approximately(color.b, other.b) 
                   && Mathf.Approximately(color.a, other.a);
        }

    }
}
