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
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""204ca8c3-b8bb-4d1f-900a-64224bcdfaac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""c26e8f8a-8c62-4918-8d81-e7349aea1f3c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MiddleMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""b56c3fd9-a15b-4a22-b894-5c9920106e41"",
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
                },
                {
                    ""name"": ""LeftMouseHold"",
                    ""type"": ""Button"",
                    ""id"": ""33eedb3c-fdad-482a-890a-72e8a8a7d0fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                },
                {
                    ""name"": ""MouseScroll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""547b00c7-1bdb-48c1-bab9-828619ae3937"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
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
                    ""action"": ""LeftMouseClick"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""c084fc85-aa3a-47c0-87d6-7efb00db2a06"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7bad1ff3-31a6-4330-8b71-ca9c6eb3b285"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82514940-7f36-4866-af92-839b909effe5"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MiddleMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd6d921e-32a6-48f9-af99-3899ba80f8e8"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseScroll"",
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
        m_MouseActionMap_LeftMouseClick = m_MouseActionMap.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_MouseActionMap_RightMouseClick = m_MouseActionMap.FindAction("RightMouseClick", throwIfNotFound: true);
        m_MouseActionMap_MiddleMouseClick = m_MouseActionMap.FindAction("MiddleMouseClick", throwIfNotFound: true);
        m_MouseActionMap_MousePosition = m_MouseActionMap.FindAction("MousePosition", throwIfNotFound: true);
        m_MouseActionMap_LeftMouseHold = m_MouseActionMap.FindAction("LeftMouseHold", throwIfNotFound: true);
        m_MouseActionMap_MouseScroll = m_MouseActionMap.FindAction("MouseScroll", throwIfNotFound: true);
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
    private readonly InputAction m_MouseActionMap_LeftMouseClick;
    private readonly InputAction m_MouseActionMap_RightMouseClick;
    private readonly InputAction m_MouseActionMap_MiddleMouseClick;
    private readonly InputAction m_MouseActionMap_MousePosition;
    private readonly InputAction m_MouseActionMap_LeftMouseHold;
    private readonly InputAction m_MouseActionMap_MouseScroll;
    public struct MouseActionMapActions
    {
        private @MouseInput m_Wrapper;
        public MouseActionMapActions(@MouseInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftMouseClick => m_Wrapper.m_MouseActionMap_LeftMouseClick;
        public InputAction @RightMouseClick => m_Wrapper.m_MouseActionMap_RightMouseClick;
        public InputAction @MiddleMouseClick => m_Wrapper.m_MouseActionMap_MiddleMouseClick;
        public InputAction @MousePosition => m_Wrapper.m_MouseActionMap_MousePosition;
        public InputAction @LeftMouseHold => m_Wrapper.m_MouseActionMap_LeftMouseHold;
        public InputAction @MouseScroll => m_Wrapper.m_MouseActionMap_MouseScroll;
        public InputActionMap Get() { return m_Wrapper.m_MouseActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActionMapActions instance)
        {
            if (m_Wrapper.m_MouseActionMapActionsCallbackInterface != null)
            {
                @LeftMouseClick.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnLeftMouseClick;
                @RightMouseClick.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnRightMouseClick;
                @MiddleMouseClick.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMiddleMouseClick;
                @MiddleMouseClick.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMiddleMouseClick;
                @MiddleMouseClick.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMiddleMouseClick;
                @MousePosition.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMousePosition;
                @LeftMouseHold.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnLeftMouseHold;
                @LeftMouseHold.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnLeftMouseHold;
                @LeftMouseHold.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnLeftMouseHold;
                @MouseScroll.started -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMouseScroll;
                @MouseScroll.performed -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMouseScroll;
                @MouseScroll.canceled -= m_Wrapper.m_MouseActionMapActionsCallbackInterface.OnMouseScroll;
            }
            m_Wrapper.m_MouseActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftMouseClick.started += instance.OnLeftMouseClick;
                @LeftMouseClick.performed += instance.OnLeftMouseClick;
                @LeftMouseClick.canceled += instance.OnLeftMouseClick;
                @RightMouseClick.started += instance.OnRightMouseClick;
                @RightMouseClick.performed += instance.OnRightMouseClick;
                @RightMouseClick.canceled += instance.OnRightMouseClick;
                @MiddleMouseClick.started += instance.OnMiddleMouseClick;
                @MiddleMouseClick.performed += instance.OnMiddleMouseClick;
                @MiddleMouseClick.canceled += instance.OnMiddleMouseClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @LeftMouseHold.started += instance.OnLeftMouseHold;
                @LeftMouseHold.performed += instance.OnLeftMouseHold;
                @LeftMouseHold.canceled += instance.OnLeftMouseHold;
                @MouseScroll.started += instance.OnMouseScroll;
                @MouseScroll.performed += instance.OnMouseScroll;
                @MouseScroll.canceled += instance.OnMouseScroll;
            }
        }
    }
    public MouseActionMapActions @MouseActionMap => new MouseActionMapActions(this);
    public interface IMouseActionMapActions
    {
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnRightMouseClick(InputAction.CallbackContext context);
        void OnMiddleMouseClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnLeftMouseHold(InputAction.CallbackContext context);
        void OnMouseScroll(InputAction.CallbackContext context);
    }
}
