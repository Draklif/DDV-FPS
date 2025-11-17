using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class InputManager : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensitivity = 25f;
    public float controllerSensitivity = 120f;

    [Header("Mappings")]
    [SerializeField] private List<GamepadIconMap> gamepadMaps;

    public static InputManager Instance { get; private set; }
    public InputDevice CurrentDevice { get; private set; }
    public Gamepad CurrentGamepad { get; private set; }
    public float LookSensitivity { get; private set; }

    private PlayerController controls;

    // Eventos de Player
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnInteract;
    public event Action OnDash;
    public event Action OnReload;
    public event Action OnNext;
    public event Action OnPrevious;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnCrouchPressed;
    public event Action OnCrouchReleased;
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action<bool> OnAim;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new PlayerController();

        // Values
        controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);
        controls.Player.Look.performed += ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Look.canceled += ctx => OnLook?.Invoke(Vector2.zero);
        controls.Player.Aim.started += _ => OnAim?.Invoke(true);
        controls.Player.Aim.canceled += _ => OnAim?.Invoke(false);

        // Buttons
        controls.Player.Interact.performed += _ => OnInteract?.Invoke();
        controls.Player.Dash.performed += _ => OnDash?.Invoke();
        controls.Player.Reload.performed += _ => OnReload?.Invoke();
        controls.Player.Next.performed += _ => OnNext?.Invoke();
        controls.Player.Previous.performed += _ => OnPrevious?.Invoke();
        controls.Player.Attack.performed+= _ => OnAttackStarted?.Invoke();
        controls.Player.Attack.canceled+= _ => OnAttackEnded?.Invoke();
        controls.Player.Jump.performed += _ => OnJumpPressed?.Invoke();
        controls.Player.Jump.canceled += _ => OnJumpReleased?.Invoke();
        controls.Player.Crouch.performed += _ => OnCrouchPressed?.Invoke();
        controls.Player.Crouch.canceled += _ => OnCrouchReleased?.Invoke();
        controls.Player.Sprint.performed += _ => OnSprintPressed?.Invoke();
        controls.Player.Sprint.canceled += _ => OnSprintReleased?.Invoke();

        // Detect Change
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed && obj is InputAction action)
        {
            InputDevice device = action.activeControl.device;

            if (device is Mouse) return;

            // Guardamos el dispositivo del último control usado
            CurrentDevice = device;

            // Guardamos el gamepad activo
            CurrentGamepad = Gamepad.current;

            // Sensibilidad según el dispositivo actual
            LookSensitivity = CurrentDevice is Gamepad
            ? controllerSensitivity
            : mouseSensitivity;
        }
    }

    void OnDestroy()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    public string GetKeyName(string actionName)
    {
        InputBinding? binding = FindBindingForAction(actionName);
        if (binding == null) return "";
        return GetBindingName(binding.Value);
    }

    public Sprite GetKeyIcon(string actionName)
    {
        InputBinding? binding = FindBindingForAction(actionName);
        if (binding == null) return null;
        return GetBindingIcon(binding.Value);
    }

    InputBinding? FindBindingForAction(string actionName)
    {
        var action = controls.asset.FindAction(actionName);
        if (action == null) return null;

        CurrentDevice ??= Keyboard.current;

        foreach (var binding in action.bindings)
        {
            if (binding.isComposite || binding.isPartOfComposite) continue;

            if (BindingMatchesDevice(binding)) return binding;
        }

        return null;
    }

    bool BindingMatchesDevice(InputBinding binding)
    {
        string path = binding.effectivePath;

        if (CurrentDevice is Keyboard) return path.Contains("Keyboard");

        if (CurrentDevice is Gamepad) return path.Contains("Gamepad");

        return false;
    }

    private GamepadIconMap GetMapForCurrentDevice()
    {
        if (CurrentDevice is Keyboard) return gamepadMaps.Find(m => m.gamepadType == GamepadType.PC);

        if (CurrentGamepad is DualShockGamepad) return gamepadMaps.Find(m => m.gamepadType == GamepadType.PlayStation);

        if (CurrentGamepad is XInputController or XInputControllerWindows) return gamepadMaps.Find(m => m.gamepadType == GamepadType.Xbox);

        return gamepadMaps.Find(m => m.gamepadType == GamepadType.Generic);
    }

    string GetBindingName(InputBinding binding)
    {
        GamepadIconMap map = GetMapForCurrentDevice();
        return map?.GetName(binding.effectivePath);
    }

    Sprite GetBindingIcon(InputBinding binding)
    {
        GamepadIconMap map = GetMapForCurrentDevice();
        return map?.GetIcon(binding.effectivePath);
    }

}
