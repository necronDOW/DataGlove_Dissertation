using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabScript : MonoBehaviour
{
    private DataGloveController _dgc;
    private Transform _attachPoint = null;
    private GameObject _attachedObject = null;
    private Vector3 _lastPosition;

    private void Awake()
    {
        _dgc = GetComponentInParent<DataGloveController>();
        _attachPoint = transform.FindChild("attach");
    }

    private void Update()
    {
        if (_attachedObject)
        {
            if (_dgc.currentAction == DataGloveController.ActionFlag.Open)
                Attach(_attachedObject, _attachedObject.GetComponent<Rigidbody>(), false);
            else _lastPosition = _attachedObject.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_attachPoint && _dgc && _dgc.currentAction == DataGloveController.ActionFlag.Closed)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb && !rb.isKinematic && !_attachedObject)
                Attach(rb.gameObject, rb);
        }
    }

    private void Attach(GameObject g, Rigidbody rb, bool attached = true)
    {
        _attachedObject = attached ? g : null;

        g.transform.SetParent(attached ? _attachPoint : null);
        if (attached)
        {
            g.transform.localPosition = Vector3.zero;
            g.transform.localEulerAngles = Vector3.zero;
            _lastPosition = g.transform.position;
        }
        rb.useGravity = !attached;
        rb.isKinematic = attached;
        rb.velocity = attached ? Vector3.zero : transform.position - _lastPosition;
    }
}
