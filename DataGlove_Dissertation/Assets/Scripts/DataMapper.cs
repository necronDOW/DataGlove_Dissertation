//#define DEBUG_INDIV

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ArmatureLib;

public class DataMapper : MonoBehaviour
{
    Group[] armatureGroups;

	#if DEBUG_INDIV
    [Range(0, 1)]
    public float tThumb, tIndex, tMiddle, tRing, tPinky;
	#endif

    void Awake()
    {
        Transform[] armature = Armature.GetArmature(transform);
        string[] openDataRaw = Data.Read(Application.dataPath + "/ArmatureData/Hand_open.ARMATUREDATA");
        string[] closedDataRaw = Data.Read(Application.dataPath + "/ArmatureData/Hand_closed.ARMATUREDATA");

        Armature[] rawArmature = MapData(armature, Data.Parse(openDataRaw), Data.Parse(closedDataRaw));
        CreateArmatureGroups(new string [5] { "thumb", "index", "mid", "ring", "pinky" }, rawArmature);
    }
    
    #if DEBUG_INDIV
    void Update()
    {
        UpdateMapping(new float[5] { tThumb, tIndex, tMiddle, tRing, tPinky });
    }
    #endif

    public void UpdateMapping(float[] input, int[] mappingIndices)
    {
        int length = Mathf.Min(input.Length, armatureGroups.Length);

        for (int i = 0; i < length; i++)
        {
            if (mappingIndices[i] >= 0)
            {
                armatureGroups[i].t = input[mappingIndices[i]];
                armatureGroups[i].Update();
            }
        }
    }
    
    private Armature[] MapData(Transform[] armatureRaw, Data[] open, Data[] closed)
    {
        Armature[] armature;

        if (armatureRaw.Length == open.Length && armatureRaw.Length == closed.Length)
        {
            armature = new Armature[open.Length];

            for (int i = 0; i < armature.Length; i++)
            {
                if (armatureRaw[i].name == open[i].id && armatureRaw[i].name == closed[i].id)
                    armature[i] = new Armature(armatureRaw[i], open[i], closed[i]);
                else
                {
                    Debug.LogError("Failed to map armature: Inconsistent names.");
                    return null;
                }
            }

            return armature;
        }
        else
        {
            Debug.LogError("Failed to map armature: Length did not match loaded length.");
            return null;
        }
    }

    private void CreateArmatureGroups(string[] ids, Armature[] raw)
    {
        armatureGroups = new Group[ids.Length];

        for (int i = 0; i < ids.Length; i++)
            armatureGroups[i] = new Group(ids[i], raw);
    }
}