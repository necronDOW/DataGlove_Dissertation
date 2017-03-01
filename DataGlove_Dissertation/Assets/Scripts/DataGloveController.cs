using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGloveController : MonoBehaviour
{
    private const int fingerCount = 5;

    public string portName = "";
    public int straightResistance = 0;
    public int bendResistance = 0;
    public int[] fingerMapping;

	private ArduinoInterface _interface = null;
    private DataMapper _dataMapper = null;

    private void OnValidate()
    {
        if (fingerMapping.Length != fingerCount)
        {
            System.Array.Resize(ref fingerMapping, fingerCount);
        }
    }

    void Awake()
    {
        if (portName != "")
        {
            _interface = new ArduinoInterface(portName);
        }

        _dataMapper = GetComponent<DataMapper>();
    }

    void Update()
    {
        if (_interface != null)
        {
            float[] arduinoValues = _interface.ReadRawSerial();

            for (int i = 0; i < arduinoValues.Length; i++)
            {
                float normalized = Normalize(arduinoValues[i], straightResistance, bendResistance);
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
