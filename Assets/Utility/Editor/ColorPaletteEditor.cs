using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utility.Editor
{
    public class ColorPaletteEditor : EditorWindow
    {
        private enum Options
        {
            Images = 0,
            Selectables = 1,
            Text = 2
        }
        private enum SelectableColoring
        {
            Positive = 0,
            Negative = 1,
            Alternative
        }
        private ColorPaletteObject _palette;
        private List<bool> _showWindows = new List<bool>();
        private GameObject _parent;
        private List<UnityEditor.Editor> _componentsEditors;
        private Options _option;
        private SelectableColoring _coloring;
        private GUIStyle _colors;
        private Vector2 _scroll;

        [MenuItem ("ColorSystem/Color Palette")]
        public static void  ShowWindow () {
            GetWindow(typeof(ColorPaletteEditor));
        }

        public void SetTarget(GameObject target)
        {
            _parent = target;
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            _parent = (GameObject)EditorGUILayout.ObjectField(
                $"Target parent:",
                _parent,
                typeof(GameObject),
                true);
            _palette = (ColorPaletteObject)EditorGUILayout.ObjectField(
            $"Palette:",
            _palette,
            typeof(ColorPaletteObject),
            false);
            if (_parent)
            {
                _option = (Options)EditorGUILayout.EnumPopup("Show:", _option);
                if (GUILayout.Button("Get UI"))
                {
                    DestroyUIEditors();
                    UIBehaviour[] components = _parent.GetComponentsInChildren<UIBehaviour>(true);
                    if (components != null)
                    {
                        _componentsEditors = new List<UnityEditor.Editor>();
                        foreach (UIBehaviour ui in components)
                        {
                            switch (_option)
                            {
                                case Options.Images when ui is Image:
                                case Options.Selectables when ui is Selectable:
                                case Options.Text when ui is Text:
                                case Options.Text when ui is TextMeshProUGUI:
                                    _componentsEditors.Add(UnityEditor.Editor.CreateEditor(ui));
                                    _showWindows.Add(false);
                                    break;
                            }
                        }
                    }
                }

                if (_componentsEditors != null)
                {
                    _scroll = GUILayout.BeginScrollView(_scroll);
                    for (int c = 0; c < _componentsEditors.Count; c++)
                    {
                        if (!_componentsEditors[c].target) break;
                        GUILayout.BeginHorizontal();
                        _showWindows[c] = EditorGUILayout.Foldout(_showWindows[c], _componentsEditors[c].target.name);
                        if (GUILayout.Button("Ping"))
                        {
                            Selection.activeObject = _componentsEditors[c].target;
                            EditorGUIUtility.PingObject(_componentsEditors[c].target);
                        }
                        GUILayout.EndHorizontal();
                        if (_showWindows[c])
                        {
                            _componentsEditors[c].OnInspectorGUI();
                        }
                        switch (_option)
                        {
                            case Options.Selectables when _componentsEditors[c].target is Selectable target:
                            {
                                _coloring = (SelectableColoring)EditorGUILayout.EnumPopup("Change colors to:", _coloring);
                                if (GUILayout.Button("Swap"))
                                {
                                    ColorBlock colors = target.colors;
                                    Color disabled = Color.white;
                                    switch (_coloring)
                                    {
                                        case SelectableColoring.Positive:
                                            colors.normalColor = _palette.Yellow;
                                            colors.highlightedColor = _palette.Red;
                                            colors.pressedColor = _palette.White;
                                            colors.selectedColor = _palette.Yellow;
                                            disabled = _palette.LightGray;
                                            disabled.a = .2f;
                                            colors.disabledColor = disabled;
                                            break;
                                        case SelectableColoring.Negative:
                                            colors.normalColor = _palette.Red;
                                            colors.highlightedColor = _palette.Yellow;
                                            colors.pressedColor = _palette.White;
                                            colors.selectedColor = _palette.Red;
                                            disabled = _palette.LightGray;
                                            disabled.a = .2f;
                                            colors.disabledColor = disabled;
                                            break;
                                        case SelectableColoring.Alternative:
                                            colors.normalColor = _palette.LightBrown;
                                            colors.highlightedColor = _palette.Brown;
                                            colors.pressedColor = _palette.Yellow;
                                            colors.selectedColor = _palette.LightBrown;
                                            disabled = _palette.LightGray;
                                            disabled.a = .2f;
                                            colors.disabledColor = disabled;
                                            break;
                                        default:
                                            colors = target.colors;
                                            break;
                                    }
                                    target.colors = colors;
                                    SceneView.RepaintAll();
                                    EditorUtility.SetDirty(target);
                                }
                                break;
                            }
                            case Options.Text when _componentsEditors[c].target is Text txt:
                                txt.color = DrawColorSwatch(txt.color);
                                EditorUtility.SetDirty(txt);
                                SceneView.RepaintAll();
                                break;
                            case Options.Text when _componentsEditors[c].target is TextMeshProUGUI tmp:
                                tmp.color = DrawColorSwatch(tmp.color);
                                EditorUtility.SetDirty(tmp);
                                SceneView.RepaintAll();
                                break;
                            case Options.Images when _componentsEditors[c].target is Image img:
                                img.color = DrawColorSwatch(img.color);
                                EditorUtility.SetDirty(img);
                                SceneView.RepaintAll();
                                break;
                        }
                        GUILayout.Space(100);
                    }
                    GUILayout.EndScrollView();
                }
            }
            else
            {
                DestroyUIEditors();
            }





            // for (int i = 0; i < _text.Count; i++)
            // {
            //     _text[i] = (TextMeshProUGUI)EditorGUILayout.ObjectField(
            //         $"Target Text:",
            //         _text[i],
            //         typeof(TextMeshProUGUI),
            //         true);
            //     if (_text[i] == null) continue;
            // }
            //
            // if (GUILayout.Button("Add"))
            // {
            //     _text.Add(null);
            // }
            //
            // _palette = (ColorPaletteObject)EditorGUILayout.ObjectField(
            //     $"Palette:",
            //     _palette,
            //     typeof(ColorPaletteObject),
            //     false);
            //     GUILayout.BeginHorizontal();
            //     _target = (Selectable)EditorGUILayout.ObjectField(
            //         $"Target:",
            //         _target,
            //         typeof(Selectable),
            //         true);
            //     _option = (Options)EditorGUILayout.EnumPopup("Change to:", _option);
            //     GUILayout.EndHorizontal();
            //

            GUILayout.EndVertical();
        }

        private Color DrawColorSwatch(Color original)
        {
            GUILayout.BeginHorizontal();
            Color bg = GUI.color;
            if (GUILayout.Button(""))
            {
                GUILayout.EndHorizontal();
                return Color.white;
            }
            foreach (Color color in _palette.Colors)
            {
                GUI.color = color;
                if (!GUILayout.Button("")) continue;
                GUILayout.EndHorizontal();
                return color;
            }
            GUILayout.EndHorizontal();
            GUI.color = bg;
            return original;
        }

        private void DestroyUIEditors()
        {
            _showWindows.Clear();
            if (_componentsEditors != null)
            {
                foreach (UnityEditor.Editor ui in _componentsEditors)
                {
                    DestroyImmediate(ui);
                }
            }
            _componentsEditors = null;
        }
    }
}