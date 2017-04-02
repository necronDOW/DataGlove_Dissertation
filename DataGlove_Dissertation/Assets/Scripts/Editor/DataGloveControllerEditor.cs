using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataGloveController))]
public class DataGloveControllerEditor : Editor
{
    private bool resistanceFoldout = true;
    private bool sensorFoldout = true;
    private float minR = 0;
    private float maxR = 100000;

    public override void OnInspectorGUI()
    {
        DataGloveController dgc = (DataGloveController)target;

        if ((dgc.portName = EditorGUILayout.TextField("Port", dgc.portName)) == "")
            dgc.scanDepth = Mathf.Clamp(EditorGUILayout.IntField("Scan Depth:", dgc.scanDepth), 0, int.MaxValue);

        if (sensorFoldout = EditorGUILayout.Foldout(sensorFoldout, "Sensor Mapping"))
        {
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add sensor"))
                    dgc.sensors.Add(new DataGloveController.Sensor());
                if (GUILayout.Button("Remove sensor") && dgc.sensors.Count > 0)
                    dgc.sensors.RemoveAt(dgc.sensors.Count - 1);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < dgc.sensors.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Mapping");
                    dgc.sensors[i].mapping = EditorGUILayout.IntField(dgc.sensors[i].mapping);

                    GUILayout.Label("Resistance Range");
                    dgc.sensors[i].range.min = EditorGUILayout.FloatField(dgc.sensors[i].range.min);
                    dgc.sensors[i].range.max = EditorGUILayout.FloatField(dgc.sensors[i].range.max);

                    EditorGUILayout.MinMaxSlider(ref dgc.sensors[i].range.min, ref dgc.sensors[i].range.max, minR, maxR);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
        }

        Validate(dgc);
    }

    void Validate(DataGloveController o)
    {
        foreach (DataGloveController.Sensor s in o.sensors)
        {
            s.range.min = Mathf.Clamp(s.range.min, minR, s.range.max);
            s.range.max = Mathf.Clamp(s.range.max, s.range.min, maxR);
        }
    }
}