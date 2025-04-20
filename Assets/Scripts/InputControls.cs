using System;

/// <summary>
/// A main singleton class for input controls.
/// </summary>
public class InputControls : SingletonInitializer<InputControls>, IInitializer, IDisposable
{
    private InputSystem _input;
    public InputSystem Input { get => _input; }

    public void Initialize()
    {
        _input = new InputSystem();
        _input.Player.Enable();
    }

    public void Dispose()
    {
        _input.Player.Disable();
    }
}
