using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
	//Public
	public Transform _thisTransform;
	public float flightSpeed = 1;
	public float penetrationDepth = 0.5f;
	public bool hasHit = false;

	//Create a mirror to transform.localPostion
	public Vector3 localPosition
	{
		get
		{
			if (_thisTransform == null)
				Start();
			return _thisTransform.localPosition;
		}
		set
		{
			_thisTransform.localPosition = value;
		}
	}
	//Create a mirror to transfrom.localRotation
	public Quaternion localRotation
	{
		get
		{
			if (_thisTransform == null)
				Start();
			return _thisTransform.localRotation;
		}
		set
		{
			_thisTransform.localRotation = value;
		}
	}

	//Private
	private TrailRenderer _trail;
	private Rigidbody _thisRigidBody;
	private bool _hasFired = false;

	//Init
	void Start ()
	{
		_trail = GetComponent<TrailRenderer>();
		_thisRigidBody = GetComponent<Rigidbody>();
		_thisRigidBody.isKinematic = true;
		_thisTransform = transform;
    }

	void FixedUpdate()
	{
		if (!_hasFired || _thisRigidBody.isKinematic || _thisRigidBody.velocity == Vector3.zero)
			return;
		//Make the arrow point in the direction of travel
		localRotation = Quaternion.LookRotation(_thisRigidBody.velocity);
	}
	
	//Fires arrow with no inherit velocity
	public void Fire(float power)
	{
		Fire(power, Vector3.zero);
	}
	//Fires arrow with inherit velocity and readies it for flight
	public void Fire(float power, Vector3 inherit)
	{
		_thisRigidBody.isKinematic = false;
		_thisRigidBody.AddForce(_thisTransform.forward * power * flightSpeed);
		_thisRigidBody.AddForce(inherit);
		SetParent(null);
		_trail.enabled = true;
		_hasFired = true;
	}

	//Detect collision and allows the arrow to penetrate into objects
	void OnCollisionEnter(Collision c)
	{
		if (!_hasFired)
			return;
		_thisRigidBody.isKinematic = true;
		_thisTransform.Translate(new Vector3(0, 0, penetrationDepth), Space.Self);
		hasHit = false;
	}

	//Sets the parent of the arrow
	public void SetParent(Transform t)
	{
		if (_thisTransform == null)
			Start();
		_thisTransform.SetParent(t);
	}

}
