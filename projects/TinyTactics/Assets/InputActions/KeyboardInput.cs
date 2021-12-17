// GENERATED AUTOMATICALLY FROM 'Assets/InputActions/KeyboardInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @KeyboardInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @KeyboardInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""KeyboardInput"",
    ""maps"": [
        {
            ""name"": ""KeyboardActionMap"",
            ""id"": ""52c45229-9194-4eea-8608-ba784345b30f"",
            ""actions"": [
                {
                    ""name"": ""MainInteractButton"",
                    ""type"": ""Button"",
                    ""id"": ""4ed8aca1-17a1-4943-9892-3f8ac4483d49"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b543a0f3-3e71-4f5a-bc5e-696d37a6ea19"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainInteractButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e06db6d-72a7-4296-959c-ac74074ddee8"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainInteractButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // KeyboardActionMap
        m_KeyboardActionMap = asset.FindActionMap("KeyboardActionMap", throwIfNotFound: true);
        m_KeyboardActionMap_MainInteractButton = m_KeyboardActionMap.FindAction("MainInteractButton", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // KeyboardActionMap
    private readonly InputActionMap m_KeyboardActionMap;
    private IKeyboardActionMapActions m_KeyboardActionMapActionsCallbackInterface;
    private readonly InputAction m_KeyboardActionMap_MainInteractButton;
    public struct KeyboardActionMapActions
    {
        private @KeyboardInput m_Wrapper;
        public KeyboardActionMapActions(@KeyboardInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MainInteractButton => m_Wrapper.m_KeyboardActionMap_MainInteractButton;
        public InputActionMap Get() { return m_Wrapper.m_KeyboardActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IKeyboardActionMapActions instance)
        {
            if (m_Wrapper.m_KeyboardActionMapActionsCallbackInterface != null)
            {
                @MainInteractButton.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnMainInteractButton;
                @MainInteractButton.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnMainInteractButton;
                @MainInteractButton.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnMainInteractButton;
            }
            m_Wrapper.m_KeyboardActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MainInteractButton.started += instance.OnMainInteractButton;
                @MainInteractButton.performed += instance.OnMainInteractButton;
                @MainInteractButton.canceled += instance.OnMainInteractButton;
            }
        }
    }
    public KeyboardActionMapActions @KeyboardActionMap => new KeyboardActionMapActions(this);
    public interface IKeyboardActionMapActions
    {
        void OnMainInteractButton(InputAction.CallbackContext context);
    }
}
