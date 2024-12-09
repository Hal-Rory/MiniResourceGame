using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class ColorPaletteUtilities
    {
        [field: SerializeField] public Color Positive { get; private set; } = Color.white;
        [field: SerializeField] public Color Negative { get; private set; } = Color.white;
        [field: SerializeField] public Color Icon { get; private set; } = Color.white;
        [field: SerializeField] public Color Basic { get; private set; } = Color.white;
        [field: SerializeField] public Color BG { get; private set; } = Color.white;
        [field: SerializeField] public Color MG { get; private set; } = Color.white;
        [field: SerializeField] public Color FG { get; private set; } = Color.white;

        public string PositiveHex { get; private set; }
        public string NegativeHex { get; private set; }
        public string IconHex { get; private set; }
        public string BasicHex { get; private set; }
        public string BGHex { get; private set; }
        public string MGHex { get; private set; }
        public string FGHex { get; private set; }

        public void GetHex()
        {
            PositiveHex = ColorUtility.ToHtmlStringRGB(Positive);
            NegativeHex =  ColorUtility.ToHtmlStringRGB(Negative);
            IconHex =  ColorUtility.ToHtmlStringRGB(Icon);
            BasicHex =  ColorUtility.ToHtmlStringRGB(Basic);
            BGHex =  ColorUtility.ToHtmlStringRGB(BG);
            MGHex =  ColorUtility.ToHtmlStringRGB(MG);
            FGHex =  ColorUtility.ToHtmlStringRGB(FG);
        }
    }
}