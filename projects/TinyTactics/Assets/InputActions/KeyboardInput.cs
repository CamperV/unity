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
                },
                {
                    ""name"": ""Axis"",
                    ""type"": ""Button"",
                    ""id"": ""e3481a30-9cc3-4b54-afc0-298f84fb35cf"",
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
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""224c5f7b-b928-4391-a348-8cc3ed22a81c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""01d9ae2e-165d-4b54-9e24-c25cedeb4f54"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ced2a549-6c97-4f48-a356-d83bc2d3f9ce"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1e8715fb-f6ab-40bc-9d5b-04efbc15aa0a"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""bcc2ba73-7100-45ec-ba90-f3539882091e"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""26d995d7-2a65-49ad-b32d-2fe72aafcfc1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ce1418ab-dfac-4f63-9a9e-995f86bb127a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bfbfcaa9-a3ff-4627-a939-0d37c9e9b540"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8cf05311-1d02-4d32-89f9-167131524cdb"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8f011877-c0ca-4f50-9460-0be226157e5d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // KeyboardActionMap
        m_KeyboardActionMap = asset.FindActionMap("KeyboardActionMap", throwIfNotFound: true);
        m_KeyboardActionMap_MainInteractButton = m_KeyboardActionMap.FindAction("MainInteractButton", throwIfNotFound: true);
        m_KeyboardActionMap_Axis = m_KeyboardActionMap.FindAction("Axis", throwIfNotFound: true);
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
    private readonly InputAction m_KeyboardActionMap_Axis;
    public struct KeyboardActionMapActions
    {
        private @KeyboardInput m_Wrapper;
        public KeyboardActionMapActions(@KeyboardInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MainInteractButton => m_Wrapper.m_KeyboardActionMap_MainInteractButton;
        public InputAction @Axis => m_Wrapper.m_KeyboardActionMap_Axis;
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
                @Axis.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnAxis;
                @Axis.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnAxis;
                @Axis.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnAxis;
            }
            m_Wrapper.m_KeyboardActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MainInteractButton.started += instance.OnMainInteractButton;
                @MainInteractButton.performed += instance.OnMainInteractButton;
                @MainInteractButton.canceled += instance.OnMainInteractButton;
                @Axis.started += instance.OnAxis;
                @Axis.performed += instance.OnAxis;
                @Axis.canceled += instance.OnAxis;
            }
        }
    }
    public KeyboardActionMapActions @KeyboardActionMap => new KeyboardActionMapActions(this);
    public interface IKeyboardActionMapActions
    {
        void OnMainInteractButton(InputAction.CallbackContext context);
        void OnAxis(InputAction.CallbackContext context);
    }
}
