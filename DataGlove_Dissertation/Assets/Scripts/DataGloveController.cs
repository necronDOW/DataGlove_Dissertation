using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGloveController : MonoBehaviour
{
    public string portName = "";
    public int scanDepth;
    public List<Sensor> sensors;

	private ArduinoInterface _interface = null;
    private DataMapper _dataMapper = null;

    private void OnValidate()
    {
        if (scanDepth < 0)
            scanDepth = 0;
    }

    void Awake()
    {
		if (portName != "")
			_interface = new ArduinoInterface(portName);
		else _interface = new ArduinoInterface();

        _dataMapper = GetComponent<DataMapper>();
    }

    void Update()
    {
        if (_interface != null)
        {
            float[] arduinoValues = _interface.ReadRawSerial();

			for (int i = 0; i < arduinoValues.Length; i++)
            {
                float normalized = Normalize(Mathf.Abs(arduinoValues[i]), sensors[i].range.min, sensors[i].range.max);
                arduinoValues[i] = Mathf.Clamp01(normalized);
            }

            if (_dataMapper)
                _dataMapper.UpdateMapping(arduinoValues, sensors);
        }
    }

    private float Normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    [System.Serializable]
    public struct Range
    {
        public float min;
        public float max;

        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [System.Serializable]
    public class Sensor
    {
        public int mapping = 0;
        public Range range = new Range(0, 100000);
    }
}