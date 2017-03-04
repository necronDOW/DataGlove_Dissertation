using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataGloveController))]
public class DataGloveControllerEditor : Editor
{
    private bool resistanceFoldout = true;
    private bool fingerFoldout = true;
    private float minR = DataGloveController.minResistance;
    private float maxR = DataGloveController.maxResistance;
    private string[] fingers = new string[5] { "Thumb", "Index", "Middle", "Ring", "Pinky" };

    public override void OnInspectorGUI()
    {
        DataGloveController dgc = (DataGloveController)target;

        if ((dgc.portName = EditorGUILayout.TextField("Port", dgc.portName)) == "")
            dgc.scanDepth = Mathf.Clamp(EditorGUILayout.IntField("Scan Depth:", dgc.scanDepth), 0, int.MaxValue);

        if (resistanceFoldout = EditorGUILayout.Foldout(resistanceFoldout, "Resistance Range"))
        {
            EditorGUILayout.HelpBox("Use this to set the resistance values for bending and straight fingers. This will be used to interpolate between hand model states.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Straight");
                dgc.straightResistance = EditorGUILayout.FloatField(dgc.straightResistance);
                GUILayout.Label("Bend");
                dgc.bendResistance = EditorGUILayout.FloatField(dgc.bendResistance);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref dgc.straightResistance, ref dgc.bendResistance, minR, maxR);
        }
        
        if (fingerFoldout = EditorGUILayout.Foldout(fingerFoldout, "Finger Mapping"))
        {
            EditorGUILayout.HelpBox("The values provided here should match to the desired analog input pins for each finger. E.g. 0 on 'Thumb' means that analog pin A0 will influence the thumb.", MessageType.Info);

            for (int i = 0; i < dgc.fingerMapping.Length; i++)
                EditorGUILayout.IntField(fingers[i], dgc.fingerMapping[i]);
        }

        Validate(dgc);
    }

    void Validate(DataGloveController o)
    {
        o.straightResistance = Mathf.Clamp(o.straightResistance, minR, o.bendResistance);
        o.bendResistance = Mathf.Clamp(o.bendResistance, o.straightResistance, maxR);
    }
}
