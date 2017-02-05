using System;
using System.IO.Ports;

using UnityEngine;

public class ArduinoInterface
{
    private const int _valLen = 5;

    private SerialPort _port;
    private bool _isValid = false;
    private bool _isReady = false;
    private float[] _values;
    private float _straightR = -1.0f;
    private float _bendR = -1.0f;

    public ArduinoInterface(string portName)
    {
        _port = new SerialPort(portName, 9600);

        try
        {
            _port.Open();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }
        
        _values = new float[_valLen];
        _isValid = true;
    }

    public void Update()
    {
        for (int i = 0; i < ReadRawSerial(); i++)
        {
            MapToAngle(ref _values[i]);
            Debug.Log(_values[i]);
        }
    }

    public bool Config()
    { 
        ReadRawSerial();
        SetStraightR(11622.0f);
        SetBendR(31278.0f);

        return _isReady;
    }

    private int ReadRawSerial()
    {
        if (_isValid && _isReady)
        {
            string[] rawInput = _port.ReadLine().Split(',');
            int length = (rawInput.Length - 1) < _valLen ? (rawInput.Length - 1) : _valLen;

            for (int i = 0; i < length; i++)
            {
                float.TryParse(rawInput[i], out _values[i]);
            }

            return length;
        }

        return -1;
    }

    private void MapToAngle(ref float value)
    {
        value = (90.0f * ((value - _straightR) / (_bendR - _straightR)));
    }

    private bool SetStraightR(float value)
    {
        _straightR = value;

        if (_bendR >= 0.0f)
        {
            _isReady = true;
        }

        return _isReady;
    }

    private bool SetBendR(float value)
    {
        _bendR = value;

        if (_straightR >= 0.0f)
        {
            _isReady = true;
        }

        return _isReady;
    }

    public float GetValue(int index)
    {
        if (index < _valLen)
        {
            return _values[index];
        }
        else
        {
            return -1.0f;
        }
    }
}
