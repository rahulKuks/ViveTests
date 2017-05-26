using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class FixedJointPickUp : MonoBehaviour {

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    FixedJoint fixedJoint;
    Vector3 pickableObjectOriginalPosition;
    bool resetFlag;
    [SerializeField]
    GameObject pickableObject;
    [SerializeField]
    Rigidbody rb;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start ()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        pickableObjectOriginalPosition = pickableObject.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            resetFlag = true;
            pickableObject.transform.position = pickableObjectOriginalPosition;
        }
	}

    void FixedUpdate()
    {
        if(resetFlag)
        {
            Rigidbody pickableObjectRigidBody = pickableObject.GetComponent<Rigidbody>();
            pickableObjectRigidBody.velocity = Vector3.zero;
            pickableObjectRigidBody.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerStay(Collider other)
    {
       if(fixedJoint == null && device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            //create fixed joint
            fixedJoint = other.gameObject.AddComponent<FixedJoint>();
            //attach connectedbody
            fixedJoint.connectedBody = rb;
        }
       else if(fixedJoint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            //extract rigidbody
            GameObject go = fixedJoint.gameObject;
            Rigidbody fjRigidBody = go.GetComponent<Rigidbody>();

            //destroy fixed joint
            Object.Destroy(fixedJoint);
            fixedJoint = null;

            //throw
            ThrowObject(fjRigidBody);
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
