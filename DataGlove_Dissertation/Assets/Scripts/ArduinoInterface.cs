using System;
using System.IO.Ports;
using UnityEngine;

public class ArduinoInterface
{
	const int baudRate = 9600;

    private SerialPort _port;

	public bool Valid
	{
		get { return _port.IsOpen; }
		private set { }
	}

	public ArduinoInterface(uint scanDepth = 5)
	{
		for (int i = 0; i < scanDepth; i++)
		{
			string portName = "COM" + i;
			_port = new SerialPort(portName, baudRate);

			try { _port.Open(); }
			catch { }

			if (_port.IsOpen)
			{
				Debug.Log("Found Port " + portName + ".");
				return;
			}
		}

		Debug.LogError("Port Scanning failed.");
	}

	public ArduinoInterface(string portName)
    {
        _port = new SerialPort(portName, baudRate);

        try { _port.Open(); }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }
	}

	public float[] ReadRawSerial()
    {
        if (Valid)
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
