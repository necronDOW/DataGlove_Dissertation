using System;
using System.IO.Ports;
using UnityEngine;

public class ArduinoInterface
{
    private SerialPort _port;
	private bool _isValid = false;
	private float _timer = 0.0f;

	public ArduinoInterface(string portName)
    {
        _port = new SerialPort(portName, 9600);

        try { _port.Open(); }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }
        
		_isValid = true;
	}
    
    public float[] ReadRawSerial()
    {
        if (_isValid)
        {
            string[] rawInput = _port.ReadLine().Split(',');
            float[] output = new float[(rawInput.Length - 1)];

			for (int i = 0; i < output.Length; i++)
			{
				float tmpValue;

                if (float.TryParse(rawInput[i], out tmpValue))
                    output[i] = tmpValue;
                else output[i] = -1;
			}

            return output;
        }

        return null;
    }
}
