using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace LuminousVector
{
	public class Bow : Pauseable
	{
		//Public
		public GameObject arrow;
		public float drawSpeed = 2;
		public float drawForce = 5;
		public float drawDistance = 0.6f;
		public float startDistance = 1.4f;
		public float bowDistance = 1f;
		public float aimAngle = 80;
		public float aimSpeed = 1;
		public Vector3 aimOffset;
		public Vector3 arrowStartPos;
		public float arrowStartYaw = 10;
		public float reloadSpeed = 1;
		public float fovShift = 20;
		public Color reticleColor = new Color(0, 0, 0, 1);
		public LineRenderer line;

		//Private
		[SerializeField]
		private bool _isLoaded = false;
		private Arrow _loadedArrow;
		[SerializeField]
		private float _drawProgress = 0;
		private Transform _thisTransform;
		private Vector3 _startPos;
		private Quaternion _startRot;
		private float _aimProgress = 0;
		private float _reloadProgress = 0;
		private bool _isAimed = false;
		private Vector3 targetArrowPos;
		private Camera _cam;
		//private Movement _motor;
		private float _targetFOV;
		private float _startFOV;
		private bool _cancel = false;
		private bool _canShoot = true;

		//Init
		void Start()
		{
			if (arrow == null)
				enabled = false;
			_thisTransform = transform;
			_startPos = _thisTransform.localPosition;
			_startRot = _thisTransform.localRotation;
			targetArrowPos = new Vector3(0, 0, startDistance);
			_cam = Camera.main;
			//_motor = GetComponentsInParent<Movement>()[0];
			_targetFOV = _cam.fieldOfView - fovShift;
			_startFOV = _cam.fieldOfView;

		}

		public override void Pause()
		{
			//throw new NotImplementedException();
		}

		public override void UnPause()
		{
			//throw new NotImplementedException();
		}

		void Update()
		{
			if (Game.IS_PAUSED)
				return;
			if (_thisTransform == null)
				Start();
			if (_loadedArrow == null && _isLoaded)
				_isLoaded = false;
			Aim();
			if (_isLoaded)
			{
				//Draw bow
				if (Input.GetKey(KeyCode.Mouse0) && !_cancel && _canShoot)
				{
					if (_drawProgress < 1)
						_drawProgress += drawSpeed * Time.deltaTime;
					else
						_drawProgress = 1;
					_loadedArrow.localPosition = Vector3.Lerp(new Vector3(0, 0, startDistance), new Vector3(0, 0, drawDistance), _drawProgress);
				}
				//Cancel Shot
				if (Input.GetKeyUp(KeyCode.R))
				{
					_cancel = true;
					_canShoot = false;
				}
				//Cancel Shot Anim
				if(_cancel)
				{
					if (_drawProgress > 0)
						_drawProgress -= drawSpeed * Time.deltaTime * 2;
					else
					{
						_drawProgress = 0;
						_cancel = false;
					}
					_loadedArrow.localPosition = Vector3.Lerp(new Vector3(0, 0, startDistance), new Vector3(0, 0, drawDistance), _drawProgress);
				}
				//Re-Allow Firing
				if (Input.GetKeyUp(KeyCode.Mouse0))
					_canShoot = true;
				//Fire arrow
				if (Input.GetKeyUp(KeyCode.Mouse0) && _drawProgress != 0)
				{
					ShootArrow();
				}
			}
			else
			{
				ReloadArrow();
			}
		}

		//Aim
		void Aim()
		{
			if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse0))
			{
				if (!_isAimed)
				{
					//Animate Aim In
					if (_aimProgress < 1)
					{
						AnimateBowAiming(_aimProgress);
						_aimProgress += Time.deltaTime * aimSpeed;
					}
					if (_aimProgress > 1)
					{
						_aimProgress = 1;
						AnimateBowAiming(_aimProgress);
						_isAimed = true;
					}
				}
			}
			else
			{
				//Animate Aim Out
				if (_aimProgress > 0)
				{
					AnimateBowAiming(_aimProgress);
					_aimProgress -= Time.deltaTime * aimSpeed;
				}
				if (_aimProgress < 0)
				{
					_aimProgress = 0;
					AnimateBowAiming(_aimProgress);
					_isAimed = false;
				}
			}
			if (Input.GetKeyUp(KeyCode.Mouse1))
			{
				_isAimed = false;
			}
			AimBow();
			ShowReticle();
		}

		void AimBow()
		{
			
			Ray bowPos = new Ray(_cam.transform.position, _cam.transform.parent.forward);
			Debug.DrawLine(_cam.transform.position, bowPos.GetPoint(bowDistance), Color.blue);
			if(_aimProgress == 0)
			{
				_thisTransform.position = bowPos.GetPoint(bowDistance) - new Vector3(0, bowDistance, 0);
				//Vector3.Lerp(_thisTransform.position, bowPos.GetPoint(bowDistance) - new Vector3(0, bowDistance, 0), _aimProgress);
																										//Vector3 baseRot = _thisTransform.localRotation.eulerAngles;
																										//baseRot.x = _cam.transform.localRotation.eulerAngles.x;
																										//_thisTransform.localRotation = Quaternion.Euler(baseRot);
			}
			else
			{
				_thisTransform.localPosition = Vector3.Lerp(_thisTransform.localPosition, _startPos, _aimProgress);
			}
		}

		//Calculate position of reticle and display it
		void ShowReticle()
		{
			if (_isLoaded && _drawProgress > 0)
			{
				Vector3 force = _drawProgress * drawForce * _loadedArrow.forward;
				Vector3[] traj = TrajectoryMapper.GetTrajectory(_loadedArrow.pos, _loadedArrow.mass, force);
				line.SetVertexCount(traj.Length);
				line.SetPositions(traj);
			}else
			{
				line.SetVertexCount(0);
			}
		}

		//Fires the arrow and clears the data for the next arrow
		void ShootArrow()
		{
			_loadedArrow.Fire(_drawProgress, drawForce);//, _motor.getVel());
			_loadedArrow = null;
			_isLoaded = false;
			_drawProgress = 0;
		}

		//Create new Arrow and set it up for firing
		//trigger the arrow loading animation
		void ReloadArrow()
		{
			if (_loadedArrow == null)
			{
				GameObject a = Instantiate(arrow, arrowStartPos, _thisTransform.rotation) as GameObject;
				_loadedArrow = a.GetComponent<Arrow>();
				_loadedArrow.SetParent(_thisTransform);
				_loadedArrow.localPosition = arrowStartPos;
			}

			if (_loadedArrow != null && _reloadProgress < 1)
			{
				AnimateArrowLoading(_reloadProgress);
				_reloadProgress += Time.deltaTime * reloadSpeed;
			}
			if (_reloadProgress >= 1)
			{

				_isLoaded = true;
				_reloadProgress = 0;
				AnimateArrowLoading(1);
			}
		}

		//Lerp the Aim animation
		void AnimateBowAiming(float progress)
		{
			_thisTransform.localPosition = Vector3.Lerp(_startPos, new Vector3(aimOffset.x, _startPos.y + aimOffset.y, _startPos.z + aimOffset.z), progress);
			_thisTransform.localRotation = Quaternion.Lerp(_startRot, Quaternion.Euler(0, 0, aimAngle), progress);
			_cam.fieldOfView = Mathf.Lerp(_startFOV, _targetFOV, progress);
		}
		//Lerp the arrow laoding animation
		void AnimateArrowLoading(float progress)
		{

			_loadedArrow.localPosition = Vector3.Lerp(arrowStartPos, targetArrowPos, progress);
			if (_isAimed)
				_loadedArrow.localRotation = Quaternion.Lerp(Quaternion.Euler(0, arrowStartYaw * -1, 0), Quaternion.identity, progress);
			else
				_loadedArrow.localRotation = Quaternion.Lerp(Quaternion.Euler(0, arrowStartYaw, 0), Quaternion.identity, progress);
		}

	}
}
