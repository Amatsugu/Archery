using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bow : MonoBehaviour
{
	//Public
	public GameObject arrow;
	public float drawSpeed = 2;
	public float drawForce = 5;
	public float drawDistance = 0.6f;
	public float startDistance = 1.4f;
	public float aimAngle = 80;
	public float aimSpeed = 1;
	public float aimPosition = -0.4f;
	public Vector3 arrowStartPos;
	public float arrowStartYaw = 10;
	public float reloadSpeed = 1;
	public float fovShift = 20;
	public Image reticle;
	public Color reticleColor = new Color(0,0,0,1);

	//Private
	[SerializeField]
	private bool _isLoaded = false;
	private Arrow _loadedArrow;
	[SerializeField]
	private float _drawProgress = 0;
	private Transform _thisTransform;
	private Vector3 _startPos;
	private float _aimProgress = 0;
	private float _loadingProgress = 0;
	private bool _isAimed = false;
	private Vector3 targetArrowPos;
	private Camera _cam;
	//private Movement _motor;
	private float _targetFOV;
	private float _startFOV;

	//Init
	void Start()
	{
		if (arrow == null)
			this.enabled = false;
		_thisTransform = transform;
		_startPos = _thisTransform.localPosition;
		targetArrowPos = new Vector3(0, 0, startDistance);
		_cam = Camera.main;
		reticle.color = reticleColor;
		//_motor = GetComponentsInParent<Movement>()[0];
		_targetFOV = _cam.fieldOfView - fovShift;
		_startFOV = _cam.fieldOfView;
    }

	void Update()
	{
		if (_thisTransform == null)
			Start();
		if (_loadedArrow == null && _isLoaded)
			_isLoaded = false;
		Aim();
		if (_isLoaded)
		{
			//Draw bow
			if (Input.GetKey(KeyCode.Mouse0))
			{
				if (_drawProgress < 1)
					_drawProgress += drawSpeed * Time.deltaTime;
				else
					_drawProgress = 1;
				_loadedArrow.localPosition = Vector3.Lerp(new Vector3(0, 0, startDistance), new Vector3(0, 0, drawDistance), _drawProgress);
			}
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
		if (Input.GetKey(KeyCode.Mouse1))
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
		ShowReticle();
	}

	//Calculate position of reticle and display it
	void ShowReticle()
	{
		RaycastHit hit;
		if (_isAimed)
			reticle.color = reticleColor;
		if (Physics.Raycast(new Ray(_thisTransform.position, _thisTransform.forward), out hit))
		{
			Vector2 pos = _cam.WorldToScreenPoint(hit.point);
			reticle.rectTransform.position = pos;
			reticle.color = reticleColor;
		}
		else if(!_isAimed)
			reticle.color = Color.clear;
	}

	//Fires the arrow and clears the data for the next arrow
	void ShootArrow()
	{
		_loadedArrow.Fire(_drawProgress * drawForce);//, _motor.getVel());
		_loadedArrow = null;
		_isLoaded = false;
		_drawProgress = 0;
	}

	//Create new Arrow and set it up for firing
	//trigger the arrow loading animation
	void ReloadArrow()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0) && _loadedArrow == null)
		{
			GameObject a = Instantiate(arrow, arrowStartPos, _thisTransform.rotation) as GameObject;
			_loadedArrow = a.GetComponent<Arrow>();
			_loadedArrow.SetParent(_thisTransform);
			_loadedArrow.localPosition = arrowStartPos;
		}

		if (_loadedArrow != null && _loadingProgress < 1)
		{
			AnimateArrowLoading(_loadingProgress);
			_loadingProgress += Time.deltaTime * reloadSpeed;
		}
		if (_loadingProgress >= 1)
		{

			_isLoaded = true;
			_loadingProgress = 0;
			AnimateArrowLoading(1);
		}
	}

	//Lerp the Aim animation
	void AnimateBowAiming(float progress)
	{
		_thisTransform.localPosition = Vector3.Lerp(_startPos, new Vector3(aimPosition, _startPos.y, _startPos.z), progress);
		_thisTransform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0, 0, aimAngle), progress);
		_cam.fieldOfView = Mathf.Lerp(_startFOV, _targetFOV, progress);
	}

	//Lerp the arrow laoding animation
	void AnimateArrowLoading(float progress)
	{

		_loadedArrow.localPosition = Vector3.Lerp(arrowStartPos, targetArrowPos, progress);
		if(_isAimed)
			_loadedArrow.localRotation = Quaternion.Lerp(Quaternion.Euler(0, arrowStartYaw*-1, 0), Quaternion.identity, progress);
		else
			_loadedArrow.localRotation = Quaternion.Lerp(Quaternion.Euler(0, arrowStartYaw, 0), Quaternion.identity, progress);
	}

}
