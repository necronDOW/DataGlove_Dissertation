using System;
using System.IO.Ports;
using UnityEngine;

public class ArduinoInterface
{
	private const int baudRate = 9600;
    private SerialPort _port;

	public bool valid
	{
		get { return _port.IsOpen; }
		private set { }
	}

	public ArduinoInterface(int scanDepth = 5)
	{
		for (int i = 0; i < scanDepth; i++)
		{
			string portName = "COM" + i;
			if (OpenPort(portName, baudRate, out _port))
			{
				Debug.Log("Connected... \nPort: " + portName + ", Baud: " + baudRate + ".");
				return;
			}
		}

		Debug.LogError("Unable to connect...\nSearched: COM0 -> COM" + scanDepth + ".");
	}

	public ArduinoInterface(string portName)
    {
        OpenPort(portName, baudRate, out _port, true);
	}

	public float[] ReadRawSerial()
    {
        if (valid)
        {
            string[] rawInput = _port.ReadLine().Split(',');
            float[] output = new float[(rawInput.Length - 1)];

			for (int i = 0; i < output.Length; i++)
			{
                if (!float.TryParse(rawInput[i], out output[i]))
                    output[i] = -1;
			}

            return output;
        }

        return null;
    }

    private bool OpenPort(string name, int baudRate, out SerialPort port, bool log = false)
    {
        port = new SerialPort(name, baudRate);

        try { _port.Open(); }
        catch (Exception e)
        {
            if (log)
                Debug.LogError(e.Message);
            return false;
        }

        return true;
    }
}