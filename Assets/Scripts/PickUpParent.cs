using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickUpParent : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    bool resetFlag;
    Vector3 pickableObjectResetPosition;
    [SerializeField]
    GameObject pickableObject;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start ()
    {
        pickableObjectResetPosition = pickableObject.transform.position;
        device = SteamVR_Controller.Input((int)trackedObj.index);
        resetFlag = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //catch the input
		if(device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            resetFlag = true;
            pickableObject.transform.position = pickableObjectResetPosition;
        }
	}

    void FixedUpdate()
    {
        if(resetFlag)
        {
            Rigidbody rb = pickableObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            resetFlag = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger collision with " + other.name);

        if(other.attachedRigidbody != null)
        {
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                Debug.Log("Trigger touched during collision with " + other.name);
                other.attachedRigidbody.isKinematic = true;
                other.gameObject.transform.SetParent(this.transform);
            }

            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                Debug.Log("Trigger up event during collision with " + other.name);
                other.gameObject.transform.SetParent(null);
                other.attachedRigidbody.isKinematic = false;

                ThrowObject(other.attachedRigidbody);
            }
        }
       
    }

    private void ThrowObject(Rigidbody objectRigidBody)
    {
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;

        if (origin != null)
        {
            objectRigidBody.velocity = origin.TransformVector(device.velocity);
            objectRigidBody.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            objectRigidBody.velocity = device.velocity;
            objectRigidBody.angularVelocity = device.angularVelocity;
        }
        
    }
}
