using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A simple Player class
/// holds controls, movement, firing
/// calls UI to update power bar
/// once fired instantiates a cannon ball
/// the more fire action is held down the more power is gained to launch the cannon ball
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Aim and Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _verticalLookSpeed = 2f;
    [SerializeField] private float _horizontalLookSpeed = 2f;
    [SerializeField] private Transform _viewTransform;
    [Header("Firing cannon balls")]
    [SerializeField] private GameObject _cannonBallPrefab;
    [SerializeField] private Transform _firePort;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private float _launchInitialPower = 10f; // initial speed for cannon ball
    [SerializeField] private float _laucnPowerGainRate = 2f; // how much speed to gain per second as fire is held down
    [SerializeField] private float _maxLaunchPower = 20f; // max speed for cannon ball
    [Header("Simulating trajectory")]
    [SerializeField] private SimulatedPhysicsScene _simulatedPhysicsScene;

    private GameObject _spawnedObject;
    private CannonBall _cannonBall;
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private Vector3 _moveDirection;
    private InputSystem _inputSystem;
    private float _xRotation;
    private SimpleRateLimiter _rateLimiter;
    private float _launchPowerValue;
    private float _powerRateValue;
    private float _fireStartTime;
    private bool _fireStarted;
    private IEnumerator _powerGainRoutine;

    private void Start()
    {
        _inputSystem = InputControls.Instance.Input;
        _rateLimiter.DropTime = _fireRate + Time.time;
        _inputSystem.Player.Fire.started += FireStarted;
        _inputSystem.Player.Fire.canceled += FireFinished;
        _powerGainRoutine = LaunchPowerGainRoutine();
    }

    private void OnDestroy()
    {
        _inputSystem.Player.Fire.performed -= FireStarted;
        _inputSystem.Player.Fire.canceled -= FireFinished;
    }

    private void Update()
    {
        ManageLook();
        ManageMovement();
    }

    # region Look and Movement
    private void ManageLook()
    {
        // get look input
        _lookInput = _inputSystem.Player.Look.ReadValue<Vector2>();
        // rotate whole body around y axis
        transform.Rotate(transform.up, _lookInput.x * Time.deltaTime * _horizontalLookSpeed);
        // get and calculate necessary x rotation
        _xRotation = _viewTransform.localRotation.eulerAngles.x;
        _xRotation -= _lookInput.y * Time.deltaTime * _verticalLookSpeed;
        // apply x rotation with clamping
        if (_xRotation > 180f)
        {
            _xRotation -= 360f;
        }
        _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);
        _viewTransform.localRotation = Quaternion.AngleAxis(_xRotation, Vector3.right);
    }

    private void ManageMovement()
    {
        _movementInput = _inputSystem.Player.Move.ReadValue<Vector2>();
        _moveDirection = new Vector3(_movementInput.x, 0, _movementInput.y) * _moveSpeed * Time.deltaTime;
        transform.position += transform.TransformDirection(_moveDirection);
    }
    #endregion

    #region Firing
    // once player presses fire button a coroutine to measure the time starts
    private void FireStarted(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (_rateLimiter.IsReady(Time.time))
        {
            _fireStarted = true;
            _fireStartTime = Time.time;
            _rateLimiter.SetNewRate(Time.time, _fireRate);
            StartCoroutine(_powerGainRoutine);
        }
    }

    // when player releases fire button a cannon ball is instantiated
    private void FireFinished(InputAction.CallbackContext obj)
    {
        if (!enabled) return;
        if(_fireStarted)
        {
            _fireStarted = false;
            StopCoroutine(_powerGainRoutine);
            FireCannonBall(_launchPowerValue);
            _simulatedPhysicsScene.ClearLineRenderer();
            UIManager.Instance.UpdatePowerBar(0f);
            _launchPowerValue = _launchInitialPower;
        }
    }

    // a coroutine to measure the power of cannon ball launch
    // calls to SimulatedPhysicsScene to simulate trajectory
    private IEnumerator LaunchPowerGainRoutine()
    {
        while (_fireStarted)
        {
            _launchPowerValue = Mathf.Clamp(_launchPowerValue + _laucnPowerGainRate * (Time.time - _fireStartTime), _launchInitialPower, _maxLaunchPower);
            _powerRateValue = (_launchPowerValue - _launchInitialPower) / (_maxLaunchPower - _launchInitialPower);
            _simulatedPhysicsScene.SimulatePhysics(_cannonBallPrefab, _firePort.position, _launchPowerValue * _firePort.forward);
            UIManager.Instance.UpdatePowerBar(_powerRateValue);
            _fireStartTime = Time.time;
            yield return new WaitForFixedUpdate();
        }
    }

    private void FireCannonBall(float power)
    {
        _spawnedObject = Instantiate(_cannonBallPrefab, _firePort.position, _firePort.rotation);
        if (_spawnedObject.TryGetComponent<CannonBall>(out _cannonBall))
        {
            _cannonBall.Fire(_firePort.forward, power);
        }
    }

    #endregion
}

/// <summary>
/// A simple rate limiter
/// used to limit the rate of fire of the player
/// </summary>
public struct SimpleRateLimiter
{
    public float DropTime;
    private float _timeToDrop;
    private bool _ready;
    public bool IsReady(float time)
    {
        _ready = false;
        _timeToDrop = DropTime - time;
        if (_timeToDrop <= 0f)
        {
            _ready = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetNewRate(float time, float newRate)
    {
        if (_ready)
        {
            DropTime = time + newRate;
        }
    }
}