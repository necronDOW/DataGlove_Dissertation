#define DEBUGGING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Armature
{
    public class Group
    {
        public string id { get; private set; }
        public float t;
        private Armature[] armature;

        public Group(string identifier, Armature[] raw)
        {
            id = identifier;

            List<Armature> bufferList = new List<Armature>();

            for (int i = 0; i < raw.Length; i++)
            {
                if (raw[i].joint.name.Contains(identifier))
                    bufferList.Add(raw[i]);
            }

            armature = bufferList.ToArray();
        }

        public void Update()
        {
            for (int i = 0; i < armature.Length; i++)
                armature[i].Update(t);
        }
    }

    private Transform joint;
    private Vector3 restingP, rangeP;
    private Quaternion restingR, clenchedR;

    public Armature(Transform joint, DataMapper.Data dataOpen, DataMapper.Data dataClosed)
    {
        this.joint = joint;
        restingP = dataOpen.position;
        rangeP = dataClosed.position - restingP;
        restingR = dataOpen.rotation;
        clenchedR = dataClosed.rotation;
    }

    public void Update(float t)
    {
        joint.localPosition = restingP + (rangeP * t);
        joint.localRotation = Quaternion.Lerp(restingR, clenchedR, t);
    }
}

public class DataMapper : MonoBehaviour
{
    public struct Data
    {
        public string id { get; private set; }
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }

        public Data(string i, Vector3 p, Quaternion r)
        {
            id = i;
            position = p;
            rotation = r;
        }
    }

    Armature.Group[] armatureGroups;

#if DEBUGGING
    [Range(0, 1)]
    public float tThumb, tIndex, tMiddle, tRing, tPinky;
#endif

    void Awake()
    {
        Transform[] armature = GetArmature();
        string[] openDataRaw = ReadData(Application.dataPath + "/ArmatureData/Hand_open.ARMATUREDATA");
        string[] closedDataRaw = ReadData(Application.dataPath + "/ArmatureData/Hand_closed.ARMATUREDATA");

        Armature[] rawArmature = MapData(armature, ParseData(openDataRaw), ParseData(closedDataRaw));
        CreateArmatureGroups(new string [5] { "thumb", "index", "mid", "ring", "pinky" }, rawArmature);
        Debug.Log(armatureGroups.Length);
    }

    private void Start()
    {

    }

    void Update()
    {
#if DEBUGGING
        float[] tValues = new float[5] { tThumb, tIndex, tMiddle, tRing, tPinky };

        for (int i = 0; i < armatureGroups.Length; i++)
        {
            armatureGroups[i].t = tValues[i];
            armatureGroups[i].Update();
        }
#endif
    }

    private Transform[] GetArmature()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        List<Transform> armature = new List<Transform>();

        for (int i = 0; i < allChildren.Length; i++)
        {
            if (IsArmature(allChildren[i].name))
                armature.Add(allChildren[i]);
        }

        return armature.ToArray();
    }

    private bool IsArmature(string str)
    {
        if (str.Contains(".R") || str.Contains(".L"))
            return true;
        else return false;
    }

    private string[] ReadData(string path)
    {
        List<string> bufferList = new List<string>();
        StreamReader reader = new StreamReader(path);
        string buffer;

        while ((buffer = reader.ReadLine()) != null)
            bufferList.Add(buffer);

        return bufferList.ToArray();
    }

    private Data[] ParseData(string[] raw)
    {
        List<Data> bufferList = new List<Data>();

        for (int i = 0; i < raw.Length; i++)
        {
            if (IsArmature(raw[i]))
                bufferList.Add(new Data(raw[i], StrToVec3(raw[i + 1]), StrToQuat(raw[i + 2])));
        }

        return bufferList.ToArray();
    }

    private Vector3 StrToVec3(string str)
    {
        string[] splitStr = str.Split('=')[1].Split(',');
        Vector3 vec = Vector3.zero;

        if (splitStr.Length == 3)
            vec = new Vector3(float.Parse(splitStr[0]), float.Parse(splitStr[1]), float.Parse(splitStr[2]));

        return vec;
    }

    private Quaternion StrToQuat(string str)
    {
        string[] splitStr = str.Split('=')[1].Split(',');
        Quaternion quat = new Quaternion(0,0,0,0);

        if (splitStr.Length == 4)
            quat = new Quaternion(float.Parse(splitStr[0]), float.Parse(splitStr[1]), float.Parse(splitStr[2]), float.Parse(splitStr[3]));

        return quat;
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
        armatureGroups = new Armature.Group[ids.Length];

        for (int i = 0; i < ids.Length; i++)
            armatureGroups[i] = new Armature.Group(ids[i], raw);
    }
}