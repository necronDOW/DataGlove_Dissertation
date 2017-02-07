using System;
using System.IO.Ports;
using UnityEngine;

public class ArduinoInterface
{
	private const uint MAX_LEN = 5;
	private const float TIMER_THRESHOLD = 5.0f;

	private struct ResistorBounds
	{
		public static readonly uint STRAIGHT_R_SET = 1;
		public static readonly uint BEND_R_SET = 1;
		public static readonly uint BOTH_R_SET = STRAIGHT_R_SET + BEND_R_SET;

		private float[] _straightR;
		private float[] _bendR;
		private bool _ready;

		public ResistorBounds(uint size = MAX_LEN)
		{
			_straightR = new float[size];
			for (uint i = 0; i < size; i++)
				_straightR[i] = -1.0f;

			_bendR = new float[size];
			for (uint i = 0; i < size; i++)
				_bendR[i] = -1.0f;

			_ready = false;
		}

		public void SetStraightR(float value, uint index)
		{
			if (index < _straightR.Length)
				_straightR[index] = value;
		}

		public void SetStraightR(float[] array)
		{
			int len = _straightR.Length;
			if (array.Length < len)
				len = array.Length;

			for (int i = 0; i < len; i++)
				_straightR[i] = array[i];
		}

		public float GetStraightR(uint index)
		{
			if (index < _straightR.Length)
				return _straightR[index];

			return -1.0f;
		}

		public void SetBendR(float value, uint index)
		{
			if (index < _bendR.Length)
				_bendR[index] = value;
		}

		public void SetBendR(float[] array)
		{
			int len = _straightR.Length;
			if (array.Length < len)
				len = array.Length;

			for (int i = 0; i < len; i++)
				_bendR[i] = array[i];
		}

		public float GetBendR(uint index)
		{
			if (index < _bendR.Length)
				return _bendR[index];

			return -1.0f;
		}

		public bool IsReady()
		{
			if (!_ready)
				_ready = (BoundsSet() == BOTH_R_SET);

			return _ready;
		}

		public uint BoundsSet()
		{
			uint value = STRAIGHT_R_SET + BEND_R_SET;

			for (uint i = 0; i < _straightR.Length; i++)
			{
				if (_straightR[i] < 0.0f)
				{
					value -= STRAIGHT_R_SET;
					break;
				}
			}

			for (uint i = 0; i < _bendR.Length; i++)
			{
				if (_bendR[i] < 0.0f)
				{
					value -= BEND_R_SET;
					break;
				}
			}

			return value;
		}
	}
	private class ValueArray
	{
		private float[] _values;

		public ValueArray(uint size = MAX_LEN)
		{
			_values = new float[size];
		}

		public int Length
		{
			get { return _values.Length; }
			private set { }
		}

		public float[] Raw
		{
			get { return _values; }
			private set { }
		}

		public bool HasVariance(ValueArray other, float threshold)
		{
			int size = Length;
			if (other.Length < size)
				size = other.Length;

			for (uint i = 0; i < size; i++)
			{
				if (Mathf.Abs(_values[i] - other[i]) > threshold)
					return true;
			}

			return false;
		}

		public float this[uint i]
		{
			get { return _values[i]; }
			set { _values[i] = value; }
		}

		public float this[int i]
		{
			get { return _values[i]; }
			set { _values[i] = value; }
		}

		public ValueArray(ValueArray other)
		{
			_values = new float[other.Length];
			for (uint i = 0; i < _values.Length; i++)
				_values[i] = other[i];
		}
	}
	
    private SerialPort _port;
	private ResistorBounds rBounds;
	private ValueArray _values;
	private ValueArray _savedValues;
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
        
		rBounds = new ResistorBounds();
		_values = new ValueArray();
		_isValid = true;
	}

    public void Update()
    {
        for (uint i = 0; i < ReadRawSerial(); i++)
        {
            _values[i] = MapToAngle(_values[i], rBounds.GetStraightR(i), rBounds.GetBendR(i));

            Debug.Log(_values[i]);
        }
    }
	
    public bool Config()
    {
		ReadRawSerial();
		UpdateTimer(ref _timer);

		if (_timer > TIMER_THRESHOLD)
		{
			if (rBounds.BoundsSet() != ResistorBounds.STRAIGHT_R_SET)
				rBounds.SetStraightR(_values.Raw);
			else if (rBounds.BoundsSet() != ResistorBounds.BEND_R_SET)
				rBounds.SetBendR(_values.Raw);

			_timer = 0.0f;
		}

        return rBounds.IsReady();
    }

	private void UpdateTimer(ref float timer)
	{
		if (_savedValues == null || _savedValues.HasVariance(_values, 1000.0f))
		{
			_savedValues = _values;
			_timer = 0.0f;
		}
		else _timer += Time.deltaTime;
	}

    private int ReadRawSerial()
    {
        if (_isValid && rBounds.IsReady())
        {
            string[] rawInput = _port.ReadLine().Split(',');
			int length = (rawInput.Length - 1);

			for (int i = 0; i < length; i++)
			{
				float tmpValue;
				float.TryParse(rawInput[i], out tmpValue);
				_values[i] = tmpValue;
			}

            return length;
        }

        return -1;
    }

    private float MapToAngle(float value, float lower, float upper)
    {
        return (90.0f * ((value - lower) / (upper - lower)));
    }
	
    public float GetValue(int index)
    {
        if (index < MAX_LEN)
            return _values[index];
        else return -1.0f;
    }
}
