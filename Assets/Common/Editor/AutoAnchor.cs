using UnityEditor;
using UnityEngine;

public class AutoAnchor : MonoBehaviour
{
    [MenuItem("CONTEXT/RectTransform/Set size to anchors")]
    private static void SetSizeToAnchors()
    {
        RectTransform trans = (Selection.activeObject as GameObject)?.GetComponent<RectTransform>();
        Undo.RecordObject (trans, "Modify Offset");
        trans.offsetMax = Vector2.zero;
        trans.offsetMin = Vector2.zero;
    }

    [MenuItem("CONTEXT/RectTransform/Set anchors to size")]
    private static void SetAnchorsToSize()
    {
        RectTransform trans = (Selection.activeObject as GameObject)?.GetComponent<RectTransform>();
        RectTransform parent = trans.parent.transform is RectTransform ? trans.parent.transform as RectTransform : null;
        if (parent == null)
        {
            Debug.LogError("RectTransform missing parent RectTransform");
        }
        Vector2 anchorMin = trans.anchorMin;
        Vector2 anchorMax = trans.anchorMax;
        Vector2 offsetMin = trans.offsetMin;
        Vector2 offsetMax = trans.offsetMax;

        if (trans.anchorMin.x == trans.anchorMax.x)
        {
            anchorMin.x += trans.offsetMin.x / parent.rect.width;
            anchorMax.x += trans.offsetMax.x / parent.rect.width;
            offsetMin.x = 0;
            offsetMax.x = 0;
        }
        else if (trans.anchorMin.x != trans.anchorMax.x)
        {
            anchorMin.x += trans.offsetMin.x / parent.rect.width;
            anchorMax.x += trans.offsetMax.x / parent.rect.width;
            float anchorDeltaX = anchorMax.x - anchorMin.x;
            float posX = (anchorMin.x * parent.rect.width);
            float sizeX = parent.rect.width * anchorDeltaX;
            float offsetPosX = posX + sizeX;
            anchorMin.x = posX / parent.rect.width;
            anchorMax.x = offsetPosX / parent.rect.width;
            offsetMin.x = 0;
            offsetMax.x = 0;
        }

        if (trans.anchorMin.y == trans.anchorMax.y)
        {
            anchorMin.y += trans.offsetMin.y / parent.rect.height;
            anchorMax.y += trans.offsetMax.y / parent.rect.height;
            offsetMin.y = 0;
            offsetMax.y = 0;
        }
        else if (trans.anchorMin.y != trans.anchorMax.y)
        {
            anchorMin.y += trans.offsetMin.y / parent.rect.height;
            anchorMax.y += trans.offsetMax.y / parent.rect.height;
            float anchorDeltaY = anchorMax.y - anchorMin.y;
            float posY = (anchorMin.y * parent.rect.height);
            float sizeY = parent.rect.height * anchorDeltaY;
            float offsetPosY = posY + sizeY;
            anchorMin.y = posY / parent.rect.height;
            anchorMax.y = offsetPosY / parent.rect.height;
            offsetMin.y = 0;
            offsetMax.y = 0;
        }
        Undo.RecordObject (trans, "Set Anchor And Offset");
        trans.anchorMin = anchorMin;
        trans.anchorMax = anchorMax;
        trans.offsetMin = offsetMin;
        trans.offsetMax = offsetMax;
    }

    [MenuItem("CONTEXT/RectTransform/Toggle anchors and size")]
    private static void ToggleAnchorsAndSize()
    {
        RectTransform trans = (Selection.activeObject as GameObject)?.GetComponent<RectTransform>();
        RectTransform parent = trans.parent.transform is RectTransform ? trans.parent.transform as RectTransform : null;
        if (parent == null)
        {
            Debug.LogError("RectTransform missing parent RectTransform");
        }
        Vector2 anchorMin = trans.anchorMin;
        Vector2 anchorMax = trans.anchorMax;
        Vector2 offsetMin = trans.offsetMin;
        Vector2 offsetMax = trans.offsetMax;

        if (trans.anchorMin.x == trans.anchorMax.x)
        {
            anchorMin.x += trans.offsetMin.x / parent.rect.width;
            anchorMax.x += trans.offsetMax.x / parent.rect.width;
            offsetMin.x = 0;
            offsetMax.x = 0;
        }
        else
        {
            float anchorDelta = trans.anchorMax.x - trans.anchorMin.x;
            float pos = (trans.anchorMin.x * parent.rect.width);
            float size = parent.rect.width * anchorDelta;
            float offsetPos = pos + size;
            anchorMin.x = 0;
            anchorMax.x = 0;
            offsetMin.x = pos;
            offsetMax.x = offsetPos;
        }

        if (trans.anchorMin.y == trans.anchorMax.y)
        {
            anchorMin.y += trans.offsetMin.y / parent.rect.height;
            anchorMax.y += trans.offsetMax.y / parent.rect.height;
            offsetMin.y = 0;
            offsetMax.y = 0;
        }
        else
        {
            float anchorDelta = trans.anchorMax.y - trans.anchorMin.y;
            float pos = (trans.anchorMin.y * parent.rect.height);
            float size = parent.rect.height * anchorDelta;
            float offsetPos = pos + size;
            anchorMin.y = 0;
            anchorMax.y = 0;
            offsetMin.y = pos;
            offsetMax.y = offsetPos;
        }
        Undo.RecordObject (trans, "Modify Anchor And Offset");
        trans.anchorMin = anchorMin;
        trans.anchorMax = anchorMax;
        trans.offsetMin = offsetMin;
        trans.offsetMax = offsetMax;
    }
}