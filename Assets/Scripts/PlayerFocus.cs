using UnityEngine;

/// <summary>
/// A simple class to control focus during the play mode
/// </summary>
public class PlayerFocus : MonoBehaviour
{
    [SerializeField] private Player _player;
    private InputSystem _inputSystem;
    private bool _focusWasLost = false;

    private void Start()
    {
        _inputSystem = InputControls.Instance.Input;
        _inputSystem.Player.Escape.performed += EscapePerformed;
        _inputSystem.Player.Fire.performed += FirePerformed;
    }

    private void OnDestroy()
    {
        _inputSystem.Player.Escape.performed -= EscapePerformed;

    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            GainFocus();
        }
        else
        {
            LoseFocus();
        }
    }

    private void EscapePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        LoseFocus();
    }

    private void FirePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // if focus was lost and user clicked on game screen then user needs to click once again
        if (_focusWasLost)
        {
            _focusWasLost = false;
            return;
        }
        GainFocus();
    }


    private void LoseFocus()
    {
        Cursor.lockState = CursorLockMode.None;
        _player.enabled = false;
        _focusWasLost = true;
    }

    private void GainFocus()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _player.enabled = true;
    }


}
