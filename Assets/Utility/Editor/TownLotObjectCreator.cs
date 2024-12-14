using System.Collections.Generic;
using System.Linq;
using Town.TownObjectData;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    public class TownLotObjectCreator  : EditorWindow
    {
        private TownLotObj _townLot;
        private UnityEditor.Editor _townLotEditor;
        private Vector2 _townLotObjSelectionScroll;
        private Vector2 _townLotObjScroll;
        private string _saveLocation = "Town/TownObjectData/Resources/TownObjects";
        private string _lotName;

        [MenuItem ("TownLots/TownLot Creator")]
        public static void  ShowWindow () {
            GetWindow(typeof(TownLotObjectCreator));
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal("box");
            _townLotObjSelectionScroll = GUILayout.BeginScrollView(_townLotObjSelectionScroll, new GUIStyle
            {
                fixedWidth = position.size.x / 3
            });
            if (GUILayout.Button("Workplace"))
            {
                CreateNewLot<WorkplaceLotObj>();
            }
            if (GUILayout.Button("House"))
            {
                CreateNewLot<HousingLotObj>();
            }
            if (GUILayout.Button("Casual"))
            {
                CreateNewLot<CasualLotObj>();
            }
            GUILayout.EndScrollView();

            GUILayout.BeginVertical();
            if (_townLot)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Saving to: Assets/");
                _saveLocation = GUILayout.TextField(_saveLocation);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Town Lot name");
                _lotName = GUILayout.TextField(_lotName);
                GUILayout.EndHorizontal();
                bool validLot = !string.IsNullOrEmpty(_lotName) && !string.IsNullOrEmpty(_saveLocation);
                if (validLot)
                {
                    if (GUILayout.Button("Save"))
                    {
                        CreateTownLot();
                    }
                }
                else
                {
                    GUI.color = Color.red;
                    GUILayout.Label("Need Name/Location");
                    GUI.color = Color.white;
                }

                _townLotObjScroll = GUILayout.BeginScrollView(_townLotObjScroll);
                if(_townLot) _townLotEditor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select type to save");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (!GUILayout.Button("Cleanup Old Lots")) return;
            CleanupLots();
        }

        private void CreateNewLot<T>()
        where T : TownLotObj
        {
            DestroyImmediate(_townLotEditor);
            DestroyImmediate(_townLot);
            _townLot = CreateInstance<T>();
            _townLotEditor = UnityEditor.Editor.CreateEditor(_townLot);
        }

        private void CleanupLots()
        {
            List<TownLotObj> lots = FindObjectsOfType<TownLotObj>().ToList();
            Debug.Log($"Found {lots.Count} lot(s)");
            foreach (TownLotObj lot in lots)
            {
                Debug.Log(lot != _townLot ? "REMOVING invalid lot" : "SKIPPING the current lot");
                if (string.IsNullOrEmpty(lot.name) && lot != _townLot) DestroyImmediate(lot);
            }
        }

        public void CreateTownLot()
        {
            string lotPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/{_saveLocation}/{_lotName}.asset");
            AssetDatabase.CreateAsset(_townLot, lotPath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(_townLot);
            EditorUtility.SetDirty(_townLot);
            Selection.activeObject = _townLot;
            _townLot = null;
        }
    }
}