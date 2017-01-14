using System;
using System.IO.Ports;

using UnityEngine;

public class ArduinoInterface {
    private const int _valLen = 5;

    private SerialPort _port;
    private bool _isValid = false;
    private float[] _values;

    public ArduinoInterface(string portName) {
        _port = new SerialPort(portName, 9600);

        try { _port.Open(); }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }
        
        _values = new float[_valLen];

        _isValid = true;
    }

    public void Update() {
        if (_isValid) {
            string[] rawInput = _port.ReadLine().Split(',');
            int length = rawInput.Length < _valLen ? rawInput.Length : _valLen;

            for (int i = 0; i < length; i++) {
                if (float.TryParse(rawInput[i], out _values[i])) {
                    if (_values[i] < 0.0f)
                        _values[i] = 0.0f;
                }
            }
        }
    }

    public float GetValue(int index) {
        if (index < _valLen)
            return _values[index];
        else return -1.0f;
    }
}
