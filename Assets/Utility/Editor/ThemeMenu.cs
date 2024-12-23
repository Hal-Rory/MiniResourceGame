using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    /// <summary>
    /// Context menu to open the Color Palette Editor when clicking a game object
    /// </summary>
    public class ThemeMenu : MonoBehaviour
    {
        [MenuItem("GameObject/Get UI Elements")]
        private static void GetUIElements()
        {
            GameObject go = Selection.activeObject as GameObject;
            if (!go) return;
            ColorPaletteEditor editor = EditorWindow.GetWindow<ColorPaletteEditor>();
            editor.SetTarget(go);
        }
    }
}