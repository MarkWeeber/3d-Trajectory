using UnityEngine;

public class TrajectoryData : MonoBehaviour
{
    [SerializeField] private Transform _playerView;
    [SerializeField] private float _startingVelocity = 10f;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private int _trajectoryResolution = 50;
    [SerializeField] private float _timeStep = 0.1f;

    private Ray _ray;
    private RaycastHit _hit;
    private Vector3 _gravity;
    private Vector3 _direction;
    private Vector3 _startPosition;
    private InputSystem _inputSystem;

    private void Start()
    {
        _gravity = Physics.gravity;
        _inputSystem = InputControls.Instance.Input;
        _inputSystem.Player.Fire.performed += FirePerformed;
        _lineRenderer.positionCount = _trajectoryResolution;
    }

    private void FirePerformed(UnityEngine.InputSystem.InputAction.CallbackContext callback)
    {
        _direction = _playerView.forward;
        //GetPositions();
        for (int index = 0; index < _trajectoryResolution; index++)
        {
            _lineRenderer.SetPosition(index, GetPosition());
        }
    }

    private Vector3 GetPosition()
    {
        // _directioin is a starting position
        // new direction with velocity, andjusted by time stepping
        _direction = _direction * _startingVelocity * _timeStep;
        // new direction with gravity adjusted by time stepping
        _direction += _gravity * _timeStep;
        return Vector3.left;
    }
}
