// GENERATED AUTOMATICALLY FROM 'Assets/InputActions/MouseInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MouseInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MouseInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MouseInput"",
    ""maps"": [
        {
            ""name"": ""MouseActionMap"",
            ""id"": ""1e3f6243-0680-46ed-aa48-35b02156681f"",
            ""actions"": [
                {
                    ""name"": ""MouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""204ca8c3-b8bb-4d1f-900a-64224bcdfaac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""88724fa8-c1be-4ac3-8b72-e24f19b6c585"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3acd2cb5-5ea6-497a-8053-4ffeca3ac111"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e5a4827d-2b95-4e11-8b8c-da033b1dc00b"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MouseActionMap
        m_MouseActionMap = asset.FindActionMap("MouseActionMap", throwIfNotFound: true);
        m_MouseActionMap_MouseClick = m_MouseActionMap.FindAction("MouseClick", throwIfNotFound: true);
        m_MouseActionMap_MousePosition = m_MouseActionMap.FindAction("MousePosition", throwIfNotFound: true);
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

    // MouseActionMap
    private readonly InputActionMap m_MouseActionMap;
    private IMouseActionMapActions m_MouseActionMapActionsCallbackInterface;
    private readonly InputAction m_MouseActionMap_MouseClick;
    private readonly InputAction m_MouseActionMap_MousePosition;
    public struct MouseActionMapActions
    {
        private @MouseInput m_Wrapper;
        public MouseActionMapActions(@MouseInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseClick => m_Wrapper.m_MouseActionMap_MouseClick;
        public InputAction @MousePosition => m_Wrapper.m_MouseActionMap_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_MouseActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActionMapActions instance)
        {
            if (m_Wrapper.m_MouseActionMapActionsCallbackInterface != null)
            {
                @MouseClick.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMouseClick;
                @MouseClick.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMouseClick;
                @MouseClick.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMouseClick;
                @MousePosition.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_MouseActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseClick.started += instance.OnMouseClick;
                @MouseClick.performed += instance.OnMouseClick;
                @MouseClick.canceled += instance.OnMouseClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public MouseActionMapActions @MouseActionMap => new MouseActionMapActions(this);
    public interface IMouseActionMapActions
    {
        void OnMouseClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}
