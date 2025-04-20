using UnityEngine;
using UnityEngine.EventSystems;

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
        // get look input
        _lookInput = _inputSystem.Player.Look.ReadValue<Vector2>();
        // rotate whole body around y axis
        transform.Rotate(transform.up, _lookInput.x * Time.deltaTime * _verticalLookSpeed);
        // rotate view trasnform around x axis
        _viewTransform.Rotate(-_lookInput.y * Time.deltaTime * _horizontalLookSpeed, 0, 0);
        // clapm rotation to prevent flipping
        _viewTransform.localEulerAngles = new Vector3(
            Mathf.Clamp(_viewTransform.localEulerAngles.x, -89f, 89f),
            _viewTransform.localEulerAngles.y,
            _viewTransform.localEulerAngles.z
            );

    }

    private void ManageMovement()
    {
        _movementInput = _inputSystem.Player.Move.ReadValue<Vector2>();
        _moveDirection = new Vector3(_movementInput.x, 0, _movementInput.y) * _moveSpeed * Time.deltaTime;
        transform.position += transform.TransformDirection(_moveDirection);
    }
}
