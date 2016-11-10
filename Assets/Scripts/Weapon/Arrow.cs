using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
	//Public
	public float flightSpeed = 1;
	public float penetrationDepth = 0.5f;
	public float damage = 500;
	public float powerFalloff = 0.25f;

	public bool hasHit = false;
	public Vector3 pos { get { return _thisTransform.position; } }
	public Vector3 forward { get { return _thisTransform.forward; } }
	public float mass { get { return _thisRigidBody.mass; } }

	private Vector3 _vel;
	private float _powMulti;


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
	private Transform _thisTransform;
	private bool _hasFired = false;

	private float _penetration;

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
		/*if (!hasHit && _hasFired)
		{
			hasHit = false;
			_thisRigidBody.isKinematic = false;
			_thisRigidBody.velocity = _vel * (1 - powerFalloff);
		}*/
		if (!_hasFired || _thisRigidBody.isKinematic || _thisRigidBody.velocity == Vector3.zero)
			return;
		if (_penetration >= penetrationDepth && _thisRigidBody.isKinematic)
			_thisRigidBody.isKinematic = true;
		//Make the arrow point in the direction of travel
		localRotation = Quaternion.LookRotation(_thisRigidBody.velocity);
	}
	
	//Fires arrow with no inherit velocity
	public void Fire(float power, float force)
	{
		Fire(power, force, Vector3.zero);
	}
	//Fires arrow with inherit velocity and readies it for flight
	public void Fire(float power, float force, Vector3 inherit)
	{
		_powMulti = power;
		_thisRigidBody.isKinematic = false;
		_thisRigidBody.AddForce(_thisTransform.forward * power * force * flightSpeed, ForceMode.Impulse);
		_thisRigidBody.AddForce(inherit, ForceMode.Impulse);
		SetParent(null);
		_trail.enabled = true;
		_hasFired = true;
	}

	//Detect collision and allows the arrow to penetrate into objects
	void OnCollisionEnter(Collision c)
	{
		if (!_hasFired)
			return;
		_vel = _thisRigidBody.velocity;
		_thisRigidBody.isKinematic = true;
		_thisTransform.Translate(new Vector3(0, 0, penetrationDepth), Space.Self);
		
	}

	void OnTriggerEnter(Collider c)
	{

	}

	void OnTriggerExit(Collider c)
	{

	}

	void OnTriggerStay(Collider c)
	{
		if (!_hasFired)
			return;
		hasHit = true;
	}

	//Sets the parent of the arrow
	public void SetParent(Transform t)
	{
		if (_thisTransform == null)
			Start();
		_thisTransform.SetParent(t);
	}

}
