using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypepadButton : MonoBehaviour
{
    public char value;

    private TypepadController _controller;

    private void Awake()
    {
        _controller = transform.parent.GetComponent<TypepadController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.tag == "Player" && _controller)
            _controller.Add(value);
    }
}
