using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A simple Player class
/// holds controls, movement
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
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private float _cannonBallSpeed = 10f;

    private GameObject _spawnedObject;
    private CannonBall _cannonBall;
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private Vector3 _moveDirection;
    private InputSystem _inputSystem;
    private float _xRotation;
    private SimpleRateLimiter _rateLimiter;

    private void Start()
    {
        _inputSystem = InputControls.Instance.Input;
        _rateLimiter.DropTime = _fireRate + Time.time;
        _inputSystem.Player.Fire.performed += Fire;
    }

    private void OnDestroy()
    {
        _inputSystem.Player.Fire.performed -= Fire;
    }

    private void Update()
    {
        ManageLook();
        ManageMovement();
    }

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

    private void Fire(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (_rateLimiter.IsReady(Time.time))
        {
            _spawnedObject = Instantiate(_cannonBallPrefab, _viewTransform.position, _viewTransform.rotation);
            if (_spawnedObject.TryGetComponent<CannonBall>(out _cannonBall))
            {
                _cannonBall.Fire(_viewTransform.forward, _cannonBallSpeed);
            }
            _rateLimiter.SetNewRate(Time.time, _fireRate);
        }
    }
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