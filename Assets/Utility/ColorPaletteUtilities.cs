using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class ColorPaletteUtilities
    {
        public ColorPaletteObject Palette;

        public string YellowHex { get; private set; }
        public string RedHex { get; private set; }
        public string GreenHex { get; private set; }
        public string TanHex { get; private set; }
        public string DarkBrownHex { get; private set; }
        public string BrownHex { get; private set; }
        public string LightBrownHex { get; private set; }

        public void GetHex()
        {
            YellowHex = ColorUtility.ToHtmlStringRGB(Palette.Yellow);
            RedHex =  ColorUtility.ToHtmlStringRGB(Palette.Red);
            GreenHex =  ColorUtility.ToHtmlStringRGB(Palette.Green);
            TanHex =  ColorUtility.ToHtmlStringRGB(Palette.White);
            DarkBrownHex =  ColorUtility.ToHtmlStringRGB(Palette.DarkBrown);
            BrownHex =  ColorUtility.ToHtmlStringRGB(Palette.Brown);
            LightBrownHex =  ColorUtility.ToHtmlStringRGB(Palette.LightBrown);
        }
    }
}