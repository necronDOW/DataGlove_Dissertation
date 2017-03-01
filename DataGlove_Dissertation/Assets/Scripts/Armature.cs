using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ArmatureLib
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

    public class Data
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

        public static string[] Read(string path)
        {
            List<string> bufferList = new List<string>();
            StreamReader reader = new StreamReader(path);
            string buffer;

            while ((buffer = reader.ReadLine()) != null)
                bufferList.Add(buffer);

            return bufferList.ToArray();
        }

        public static Data[] Parse(string[] raw)
        {
            List<Data> bufferList = new List<Data>();

            for (int i = 0; i < raw.Length; i++)
            {
                if (Armature.Validate(raw[i]))
                    bufferList.Add(new Data(raw[i], StrToVec3(raw[i + 1]), StrToQuat(raw[i + 2])));
            }

            return bufferList.ToArray();
        }

        private static Vector3 StrToVec3(string str)
        {
            string[] splitStr = str.Split('=')[1].Split(',');
            Vector3 vec = Vector3.zero;

            if (splitStr.Length == 3)
                vec = new Vector3(float.Parse(splitStr[0]), float.Parse(splitStr[1]), float.Parse(splitStr[2]));

            return vec;
        }

        private static Quaternion StrToQuat(string str)
        {
            string[] splitStr = str.Split('=')[1].Split(',');
            Quaternion quat = new Quaternion(0, 0, 0, 0);

            if (splitStr.Length == 4)
                quat = new Quaternion(float.Parse(splitStr[0]), float.Parse(splitStr[1]), float.Parse(splitStr[2]), float.Parse(splitStr[3]));

            return quat;
        }
    }

    public class Armature
    {
        public Transform joint;
        private Vector3 restingP, rangeP;
        private Quaternion restingR, clenchedR;

        public Armature(Transform joint, Data dataOpen, Data dataClosed)
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

        public static Transform[] GetArmature(Transform t)
        {
            Transform[] allChildren = t.GetComponentsInChildren<Transform>();
            List<Transform> armature = new List<Transform>();

            for (int i = 0; i < allChildren.Length; i++)
            {
                if (Validate(allChildren[i].name))
                    armature.Add(allChildren[i]);
            }

            return armature.ToArray();
        }
        public static bool Validate(string str)
        {
            if (str.Contains(".R") || str.Contains(".L"))
                return true;
            else return false;
        }
    }
}