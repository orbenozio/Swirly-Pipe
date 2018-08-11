using UnityEngine;

public class Player : MonoBehaviour 
{
	[SerializeField] private PipeSystem _pipeSystem;
	[SerializeField] private float _velocity;
	[SerializeField] private float _rotationVelocity;

	private Pipe _currentPipe;
	private float _distanceTraveled;
	private float _deltaToRotation;
	private float _systemRotation;
	private Transform _world;
	private Transform _rotater;
	private float _worldRotation;
	private float _avatarRotation;

	private void Start () 
	{
		_world = _pipeSystem.transform.parent;
		_rotater = transform.GetChild(0);
		_currentPipe = _pipeSystem.SetupFirstPipe();
		SetupCurrentPipe();
	}
	
	private void Update () 
	{
		var delta = _velocity * Time.deltaTime;
		_distanceTraveled += delta;
		_systemRotation += delta * _deltaToRotation;
		
		if (_systemRotation >= _currentPipe.CurveAngle) {
			delta = (_systemRotation - _currentPipe.CurveAngle) / _deltaToRotation;
			_currentPipe = _pipeSystem.SetupNextPipe();
			SetupCurrentPipe();
			_systemRotation = delta * _deltaToRotation;
		}
		
		_pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, _systemRotation);

		UpdateAvatarRotation();
	}
	
	private void UpdateAvatarRotation () 
	{
		_avatarRotation += _rotationVelocity * Time.deltaTime * Input.GetAxis("Horizontal");
		
		if (_avatarRotation < 0f) 
		{
			_avatarRotation += 360f;
		}
		else if (_avatarRotation >= 360f) 
		{
			_avatarRotation -= 360f;
		}
		
		_rotater.localRotation = Quaternion.Euler(_avatarRotation, 0f, 0f);
	}
	
	private void SetupCurrentPipe () 
	{
		_deltaToRotation = 360f / (2f * Mathf.PI * _currentPipe.CurveRadius);
		_worldRotation += _currentPipe.RelativeRotation;
		
		if (_worldRotation < 0f) 
		{
			_worldRotation += 360f;
		}
		else if (_worldRotation >= 360f) 
		{
			_worldRotation -= 360f;
		}
		
		_world.localRotation = Quaternion.Euler(_worldRotation, 0f, 0f);
	}
}