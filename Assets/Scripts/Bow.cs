using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour
{

	public GameObject arrow;
	public float drawSpeed = 2;
	public float drawForce = 5;
	public float drawDistance = 1.4f;

	[SerializeField]
	private bool _isLoaded = false;
	private Transform _loadedArrow;
	[SerializeField]
	private float _drawLength = 0;
	[SerializeField]
	private Transform _thisTransform;


	void Start ()
    {
		if (arrow == null)
			this.enabled = false;
		_thisTransform = transform;
	}
	
	void Update ()
    {
		if(_isLoaded)
		{
			if(Input.GetKey(KeyCode.Mouse0))
			{
				_drawLength += drawSpeed * Time.deltaTime;
				if (_drawLength > drawDistance)
					_drawLength = drawDistance;
				Vector3 arrowPos = _loadedArrow.localPosition;
				arrowPos.z = -1 * (_drawLength-drawDistance);
				_loadedArrow.localPosition = arrowPos;
			}
			if(Input.GetKeyUp(KeyCode.Mouse0) && _drawLength > 0)
			{
				ShootArrow();
			}
		}else
		{
			if(Input.GetKeyDown(KeyCode.Mouse0))
			{
				GameObject a = Instantiate(arrow, _thisTransform.position + (_thisTransform.forward * drawDistance), _thisTransform.rotation) as GameObject;
				_loadedArrow = a.transform;
				_loadedArrow.parent = _thisTransform;
				_isLoaded = true;
			}
		}
	}

	void ShootArrow()
	{
		Rigidbody rigid = _loadedArrow.GetComponent<Rigidbody>();
		rigid.isKinematic = false;
		rigid.AddForce(_loadedArrow.forward * ((_drawLength / drawDistance) * drawForce));
		_loadedArrow.parent = null;
		_loadedArrow = null;
		_isLoaded = false;
		_drawLength = 0;
	}
}
