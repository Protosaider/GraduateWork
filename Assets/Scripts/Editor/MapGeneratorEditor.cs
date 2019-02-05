using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))]
public class MapPreviewEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapManager mapPreview = (MapManager)target;

        // Means if any value was changed
        if (base.DrawDefaultInspector())
        {
            if (mapPreview.autoUpdate)
            {
                mapPreview.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate Mesh"))
        {
            mapPreview.DrawMapInEditor();
        }
    }
}
