using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGloveController : MonoBehaviour
{
    public string portName = "";
    private enum Mode
    {
        Default,
        Config
    }
    private Mode _mode = Mode.Config;

    private ArduinoInterface _interface = null;

    void Awake()
    {
        if (portName != "")
        {
            _interface = new ArduinoInterface(portName);
        }
    }

    void Update()
    {
        if (_interface != null)
        {
            switch (_mode)
            {
                case Mode.Default:
                    _interface.Update();
                    break;
                case Mode.Config:
                    if (_interface.Config())
                    {
                        _mode = Mode.Default;
                    }
                    break;
            }
        }
    }
}
