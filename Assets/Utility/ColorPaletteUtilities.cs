using System;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// A utility that stores the hex values for a given color palette for fast conversion in ui
    /// </summary>
    [Serializable]
    public static class ColorPaletteUtilities
    {
        public static string YellowHex { get; private set; }
        public static string RedHex { get; private set; }
        public static string GreenHex { get; private set; }
        public static string TanHex { get; private set; }
        public static string DarkBrownHex { get; private set; }
        public static string BrownHex { get; private set; }
        public static string LightBrownHex { get; private set; }
        public static string LightGrayHex { get; private set; }

        public static void GetHex(ColorPaletteObject palette)
        {
            YellowHex = ColorUtility.ToHtmlStringRGB(palette.Yellow);
            RedHex =  ColorUtility.ToHtmlStringRGB(palette.Red);
            GreenHex =  ColorUtility.ToHtmlStringRGB(palette.Green);
            TanHex =  ColorUtility.ToHtmlStringRGB(palette.White);
            DarkBrownHex =  ColorUtility.ToHtmlStringRGB(palette.DarkBrown);
            BrownHex =  ColorUtility.ToHtmlStringRGB(palette.Brown);
            LightBrownHex =  ColorUtility.ToHtmlStringRGB(palette.LightBrown);
            LightGrayHex =  ColorUtility.ToHtmlStringRGB(palette.LightGray);
        }
    }
}