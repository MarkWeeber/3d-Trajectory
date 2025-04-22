using UnityEngine;

/// <summary>
/// A simple Player class
/// holds controls, movement
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _verticalLookSpeed = 2f;
    [SerializeField] private float _horizontalLookSpeed = 2f;
    [SerializeField] private Transform _viewTransform;

    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private Vector3 _moveDirection;
    private InputSystem _inputSystem;
    private float _xRotation;
    private bool _omitFirstFrame = true;

    private void Start()
    {
        _inputSystem = InputControls.Instance.Input;
    }

    private void Update()
    {
        ManageLook();
        ManageMovement();
    }

    private void ManageLook()
    {
        // omit first frame to avoid jitter
        if (_omitFirstFrame)
        {
            _omitFirstFrame = false;
            return;
        }
        // get look input
        _lookInput = _inputSystem.Player.Look.ReadValue<Vector2>();
        // rotate whole body around y axis
        transform.Rotate(transform.up, _lookInput.x * Time.deltaTime * _verticalLookSpeed);
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
}
