using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class Tools
{
    [MenuItem("Tools/Write Armature Data to File")]
    public static void WriteArmatureToDataFile()
    {
        if (Selection.activeTransform == null)
            Debug.LogError("No object selected.");
        else
        {
            Transform[] children = Selection.activeTransform.GetComponentsInChildren<Transform>();
            StreamWriter sw = null;

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name.Contains(".R") || children[i].name.Contains(".L"))
                {
                    if (sw == null)
                        sw = new StreamWriter(Application.dataPath + "/ArmatureData/" + Selection.activeTransform.name + ".ARMATUREDATA");

                    sw.WriteLine(children[i].name);
                    sw.WriteLine("p=" + VecToStr(children[i].localPosition));
                    sw.WriteLine("q=" + QuatToStr(children[i].localRotation));
                    sw.WriteLine("");
                }
            }

            if (sw == null)
                Debug.LogWarning("No valid armature found, ensure that you have prefixed .R or .L to relevant armanture names.");
            else sw.Close();
        }
    }

    static string VecToStr(Vector3 vec)
    {
        return vec.x + "," + vec.y + "," + vec.z;
    }

    static string QuatToStr(Quaternion quat)
    {
        return quat.x + "," + quat.y + "," + quat.z + "," + quat.w;
    }
}