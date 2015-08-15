using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
	//Public
	public float moveSpeed;
	public float gravity = 9.8f;
	public float terminalVel = 100;
	public float jumpSpeed = 10;

	//Private
	[SerializeField]
	private Vector3 _moveVector = Vector3.zero;
	private Transform _thisTransform;
	private CharacterController _controller;
	private float _verticalVelocity;

	//Init
	void Start ()
	{
		_thisTransform = transform;
		_controller = GetComponent<CharacterController>();
	}
	
	void Update ()
	{
		if (_thisTransform == null)
			Start();
		//Forwards/Backwards movement
		if (Input.GetKey(KeyCode.W))
		{
			_moveVector.z = 1;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			_moveVector.z = -1;
		}
		//Left/Right Movement
		if (Input.GetKey(KeyCode.D))
		{
			_moveVector.x = 1;
		}
		else if (Input.GetKey(KeyCode.A))
		{
			_moveVector.x = -1;
		}
		//Transform vector to world space
		_moveVector = _thisTransform.TransformDirection(_moveVector);
		_moveVector *= moveSpeed;
		//Jump
		if (!_controller.isGrounded)
		{
			if(_verticalVelocity >= terminalVel*-1)
				_verticalVelocity -= gravity * Time.deltaTime;
		}else
		{
			if (Input.GetKeyDown(KeyCode.Space))
				_verticalVelocity = jumpSpeed;
			else
				_verticalVelocity = 0;
		}
		_moveVector.y = _verticalVelocity;
		_controller.Move(_moveVector * Time.deltaTime);
		_moveVector = Vector3.zero;
	}

	//Returns the move vector
	public Vector3 getVel()
	{
		return _moveVector;
	}
}
