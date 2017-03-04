using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGloveController : MonoBehaviour
{
    public const float minResistance = 0;
    public const float maxResistance = 100000;

    public string portName = "";
    public int scanDepth;
    public float straightResistance = minResistance;
    public float bendResistance = maxResistance;
    public int[] fingerMapping = new int[5];

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
                float normalized = Normalize(Mathf.Abs(arduinoValues[i]), straightResistance, bendResistance);
                arduinoValues[i] = Mathf.Clamp01(normalized);
            }

            if (_dataMapper)
                _dataMapper.UpdateMapping(arduinoValues, fingerMapping);
        }
    }

    private float Normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
}
