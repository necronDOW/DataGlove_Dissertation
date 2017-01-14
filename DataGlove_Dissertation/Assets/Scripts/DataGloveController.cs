using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGloveController : MonoBehaviour {
    public string portName = "";

    private ArduinoInterface _interface = null;

    void Awake()
    {
        if (portName != "")
            _interface = new ArduinoInterface(portName);
    }

    void Update()
    {
        if (_interface != null)
            _interface.Update();
    }
}
