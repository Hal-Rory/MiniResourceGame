using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utility.Editor;

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