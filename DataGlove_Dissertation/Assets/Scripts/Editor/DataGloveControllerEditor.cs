using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataGloveController))]
public class DataGloveControllerEditor : Editor
{
    private bool resistanceFoldout = true;
    private bool fingerFoldout = true;
    private float minR = DataGloveController.rBounds.min;
    private float maxR = DataGloveController.rBounds.max;
    private string[] fingers = new string[5] { "Thumb", "Index", "Middle", "Ring", "Pinky" };

    public override void OnInspectorGUI()
    {
        DataGloveController dgc = (DataGloveController)target;

        if ((dgc.portName = EditorGUILayout.TextField("Port", dgc.portName)) == "")
            dgc.scanDepth = Mathf.Clamp(EditorGUILayout.IntField("Scan Depth:", dgc.scanDepth), 0, int.MaxValue);

        if (resistanceFoldout = EditorGUILayout.Foldout(resistanceFoldout, "Resistance Range"))
        {
            EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Min");
                dgc.rRange.min = EditorGUILayout.FloatField(dgc.rRange.min);
                GUILayout.Label("Max");
                dgc.rRange.max = EditorGUILayout.FloatField(dgc.rRange.max);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref dgc.rRange.min, ref dgc.rRange.max, minR, maxR);
        }
        
        if (fingerFoldout = EditorGUILayout.Foldout(fingerFoldout, "Finger Mapping"))
        {
            for (int i = 0; i < dgc.fingerMapping.Length; i++)
                EditorGUILayout.IntField(fingers[i], dgc.fingerMapping[i]);
        }

        Validate(dgc);
    }

    void Validate(DataGloveController o)
    {
        o.rRange.min = Mathf.Clamp(o.rRange.min, minR, o.rRange.max);
        o.rRange.max = Mathf.Clamp(o.rRange.max, o.rRange.min, maxR);
    }
}