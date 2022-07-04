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
                },
                {
                    ""name"": ""QuickBar_0"",
                    ""type"": ""Button"",
                    ""id"": ""a0058240-3ba1-4490-a347-fa2cb6270c31"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_1"",
                    ""type"": ""Button"",
                    ""id"": ""9df98a24-a477-4361-a43d-482129d3d9bb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_2"",
                    ""type"": ""Button"",
                    ""id"": ""3a1b7a28-8199-41e7-9213-d60270cc96f7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_3"",
                    ""type"": ""Button"",
                    ""id"": ""9659df24-4e92-4606-9e55-c00ef26b841e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_4"",
                    ""type"": ""Button"",
                    ""id"": ""89ab94df-08ae-4e46-8af2-981cad591e0e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_5"",
                    ""type"": ""Button"",
                    ""id"": ""e6ad46d5-7fe7-4ec0-87da-91be640a494c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_6"",
                    ""type"": ""Button"",
                    ""id"": ""8a683ea6-8bfa-4c68-b06f-0f19cc184564"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_7"",
                    ""type"": ""Button"",
                    ""id"": ""d27b5b9b-0aae-471c-846f-664e368521fb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_8"",
                    ""type"": ""Button"",
                    ""id"": ""1aae32c9-a872-407c-99f1-c79052780934"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickBar_9"",
                    ""type"": ""Button"",
                    ""id"": ""f9f9f7fc-6dc5-4397-8e43-22c10cc3b84d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelectNextUnit"",
                    ""type"": ""Button"",
                    ""id"": ""b9cdc993-5d2f-4ccd-8957-b4b55fc4ed35"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""ed1fd9f0-4874-4dd2-a3c7-6a1a04910156"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PrevWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""0801b673-8083-48a5-95f3-af3c5acc785d"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""2f1ef81c-1695-4f2e-8d21-94dc22ee4635"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c2ebc8b-97f4-45ce-b276-b59c6bbba8b4"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b246b417-1a33-4983-b0f1-97599f87af5b"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""116c3610-f7ea-4278-816d-33f65da6981a"",
                    ""path"": ""<Keyboard>/numpad2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a4b0c56-2c3b-4ce8-b6b1-3ce6d84ba07c"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d15bf1ec-5424-4fd1-9e45-71cdb6d75e73"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a12c8806-3660-4bb0-900a-593600b9ff2e"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""552aa747-70a4-455c-a70f-c5dc60fb9f27"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1252166e-f642-497f-8a2a-3a87f35fcbe3"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21356111-fbc9-4f98-b81c-da4e865f040a"",
                    ""path"": ""<Keyboard>/numpad5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65b68eb9-833d-42ad-803f-be211f2a792a"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""902d9bc6-cf4f-4012-9aa8-95d31d1b93f8"",
                    ""path"": ""<Keyboard>/numpad6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19197445-912c-4550-82c2-8b88bcd6e4f0"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7e65b63-58c4-4219-8b09-3c1ce82fe69d"",
                    ""path"": ""<Keyboard>/numpad7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8f0ba66-12ec-4945-a5ed-f8b39ec2cfab"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14cfc784-8cf4-49b1-ad5c-083f745a3e77"",
                    ""path"": ""<Keyboard>/numpad8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc81e865-d78a-4f08-8b76-1632f814975b"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_9"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""903329f5-4e69-4a84-8ed0-1e310d39a106"",
                    ""path"": ""<Keyboard>/numpad9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_9"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ff06fe2-e775-4448-beb4-0a743273d51e"",
                    ""path"": ""<Keyboard>/0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed8ad254-4f2e-4f81-a546-f4f7451c3d45"",
                    ""path"": ""<Keyboard>/numpad0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickBar_0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""87163974-e3ce-48c9-a467-3b88b35eb838"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectNextUnit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ada9ee51-4571-4f6a-861f-69919320e8fe"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ae07aae-a1da-473a-bb82-499d83c5ab8b"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrevWeapon"",
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
        m_KeyboardActionMap_Axis = m_KeyboardActionMap.FindAction("Axis", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_0 = m_KeyboardActionMap.FindAction("QuickBar_0", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_1 = m_KeyboardActionMap.FindAction("QuickBar_1", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_2 = m_KeyboardActionMap.FindAction("QuickBar_2", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_3 = m_KeyboardActionMap.FindAction("QuickBar_3", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_4 = m_KeyboardActionMap.FindAction("QuickBar_4", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_5 = m_KeyboardActionMap.FindAction("QuickBar_5", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_6 = m_KeyboardActionMap.FindAction("QuickBar_6", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_7 = m_KeyboardActionMap.FindAction("QuickBar_7", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_8 = m_KeyboardActionMap.FindAction("QuickBar_8", throwIfNotFound: true);
        m_KeyboardActionMap_QuickBar_9 = m_KeyboardActionMap.FindAction("QuickBar_9", throwIfNotFound: true);
        m_KeyboardActionMap_SelectNextUnit = m_KeyboardActionMap.FindAction("SelectNextUnit", throwIfNotFound: true);
        m_KeyboardActionMap_NextWeapon = m_KeyboardActionMap.FindAction("NextWeapon", throwIfNotFound: true);
        m_KeyboardActionMap_PrevWeapon = m_KeyboardActionMap.FindAction("PrevWeapon", throwIfNotFound: true);
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
    private readonly InputAction m_KeyboardActionMap_QuickBar_0;
    private readonly InputAction m_KeyboardActionMap_QuickBar_1;
    private readonly InputAction m_KeyboardActionMap_QuickBar_2;
    private readonly InputAction m_KeyboardActionMap_QuickBar_3;
    private readonly InputAction m_KeyboardActionMap_QuickBar_4;
    private readonly InputAction m_KeyboardActionMap_QuickBar_5;
    private readonly InputAction m_KeyboardActionMap_QuickBar_6;
    private readonly InputAction m_KeyboardActionMap_QuickBar_7;
    private readonly InputAction m_KeyboardActionMap_QuickBar_8;
    private readonly InputAction m_KeyboardActionMap_QuickBar_9;
    private readonly InputAction m_KeyboardActionMap_SelectNextUnit;
    private readonly InputAction m_KeyboardActionMap_NextWeapon;
    private readonly InputAction m_KeyboardActionMap_PrevWeapon;
    public struct KeyboardActionMapActions
    {
        private @KeyboardInput m_Wrapper;
        public KeyboardActionMapActions(@KeyboardInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MainInteractButton => m_Wrapper.m_KeyboardActionMap_MainInteractButton;
        public InputAction @Axis => m_Wrapper.m_KeyboardActionMap_Axis;
        public InputAction @QuickBar_0 => m_Wrapper.m_KeyboardActionMap_QuickBar_0;
        public InputAction @QuickBar_1 => m_Wrapper.m_KeyboardActionMap_QuickBar_1;
        public InputAction @QuickBar_2 => m_Wrapper.m_KeyboardActionMap_QuickBar_2;
        public InputAction @QuickBar_3 => m_Wrapper.m_KeyboardActionMap_QuickBar_3;
        public InputAction @QuickBar_4 => m_Wrapper.m_KeyboardActionMap_QuickBar_4;
        public InputAction @QuickBar_5 => m_Wrapper.m_KeyboardActionMap_QuickBar_5;
        public InputAction @QuickBar_6 => m_Wrapper.m_KeyboardActionMap_QuickBar_6;
        public InputAction @QuickBar_7 => m_Wrapper.m_KeyboardActionMap_QuickBar_7;
        public InputAction @QuickBar_8 => m_Wrapper.m_KeyboardActionMap_QuickBar_8;
        public InputAction @QuickBar_9 => m_Wrapper.m_KeyboardActionMap_QuickBar_9;
        public InputAction @SelectNextUnit => m_Wrapper.m_KeyboardActionMap_SelectNextUnit;
        public InputAction @NextWeapon => m_Wrapper.m_KeyboardActionMap_NextWeapon;
        public InputAction @PrevWeapon => m_Wrapper.m_KeyboardActionMap_PrevWeapon;
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
                @QuickBar_0.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_0;
                @QuickBar_0.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_0;
                @QuickBar_0.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_0;
                @QuickBar_1.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_1;
                @QuickBar_1.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_1;
                @QuickBar_1.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_1;
                @QuickBar_2.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_2;
                @QuickBar_2.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_2;
                @QuickBar_2.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_2;
                @QuickBar_3.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_3;
                @QuickBar_3.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_3;
                @QuickBar_3.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_3;
                @QuickBar_4.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_4;
                @QuickBar_4.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_4;
                @QuickBar_4.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_4;
                @QuickBar_5.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_5;
                @QuickBar_5.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_5;
                @QuickBar_5.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_5;
                @QuickBar_6.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_6;
                @QuickBar_6.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_6;
                @QuickBar_6.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_6;
                @QuickBar_7.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_7;
                @QuickBar_7.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_7;
                @QuickBar_7.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_7;
                @QuickBar_8.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_8;
                @QuickBar_8.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_8;
                @QuickBar_8.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_8;
                @QuickBar_9.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_9;
                @QuickBar_9.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_9;
                @QuickBar_9.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnQuickBar_9;
                @SelectNextUnit.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnSelectNextUnit;
                @SelectNextUnit.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnSelectNextUnit;
                @SelectNextUnit.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnSelectNextUnit;
                @NextWeapon.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnNextWeapon;
                @PrevWeapon.started -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnPrevWeapon;
                @PrevWeapon.performed -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnPrevWeapon;
                @PrevWeapon.canceled -= m_Wrapper.m_KeyboardActionMapActionsCallbackInterface.OnPrevWeapon;
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
                @QuickBar_0.started += instance.OnQuickBar_0;
                @QuickBar_0.performed += instance.OnQuickBar_0;
                @QuickBar_0.canceled += instance.OnQuickBar_0;
                @QuickBar_1.started += instance.OnQuickBar_1;
                @QuickBar_1.performed += instance.OnQuickBar_1;
                @QuickBar_1.canceled += instance.OnQuickBar_1;
                @QuickBar_2.started += instance.OnQuickBar_2;
                @QuickBar_2.performed += instance.OnQuickBar_2;
                @QuickBar_2.canceled += instance.OnQuickBar_2;
                @QuickBar_3.started += instance.OnQuickBar_3;
                @QuickBar_3.performed += instance.OnQuickBar_3;
                @QuickBar_3.canceled += instance.OnQuickBar_3;
                @QuickBar_4.started += instance.OnQuickBar_4;
                @QuickBar_4.performed += instance.OnQuickBar_4;
                @QuickBar_4.canceled += instance.OnQuickBar_4;
                @QuickBar_5.started += instance.OnQuickBar_5;
                @QuickBar_5.performed += instance.OnQuickBar_5;
                @QuickBar_5.canceled += instance.OnQuickBar_5;
                @QuickBar_6.started += instance.OnQuickBar_6;
                @QuickBar_6.performed += instance.OnQuickBar_6;
                @QuickBar_6.canceled += instance.OnQuickBar_6;
                @QuickBar_7.started += instance.OnQuickBar_7;
                @QuickBar_7.performed += instance.OnQuickBar_7;
                @QuickBar_7.canceled += instance.OnQuickBar_7;
                @QuickBar_8.started += instance.OnQuickBar_8;
                @QuickBar_8.performed += instance.OnQuickBar_8;
                @QuickBar_8.canceled += instance.OnQuickBar_8;
                @QuickBar_9.started += instance.OnQuickBar_9;
                @QuickBar_9.performed += instance.OnQuickBar_9;
                @QuickBar_9.canceled += instance.OnQuickBar_9;
                @SelectNextUnit.started += instance.OnSelectNextUnit;
                @SelectNextUnit.performed += instance.OnSelectNextUnit;
                @SelectNextUnit.canceled += instance.OnSelectNextUnit;
                @NextWeapon.started += instance.OnNextWeapon;
                @NextWeapon.performed += instance.OnNextWeapon;
                @NextWeapon.canceled += instance.OnNextWeapon;
                @PrevWeapon.started += instance.OnPrevWeapon;
                @PrevWeapon.performed += instance.OnPrevWeapon;
                @PrevWeapon.canceled += instance.OnPrevWeapon;
            }
        }
    }
    public KeyboardActionMapActions @KeyboardActionMap => new KeyboardActionMapActions(this);
    public interface IKeyboardActionMapActions
    {
        void OnMainInteractButton(InputAction.CallbackContext context);
        void OnAxis(InputAction.CallbackContext context);
        void OnQuickBar_0(InputAction.CallbackContext context);
        void OnQuickBar_1(InputAction.CallbackContext context);
        void OnQuickBar_2(InputAction.CallbackContext context);
        void OnQuickBar_3(InputAction.CallbackContext context);
        void OnQuickBar_4(InputAction.CallbackContext context);
        void OnQuickBar_5(InputAction.CallbackContext context);
        void OnQuickBar_6(InputAction.CallbackContext context);
        void OnQuickBar_7(InputAction.CallbackContext context);
        void OnQuickBar_8(InputAction.CallbackContext context);
        void OnQuickBar_9(InputAction.CallbackContext context);
        void OnSelectNextUnit(InputAction.CallbackContext context);
        void OnNextWeapon(InputAction.CallbackContext context);
        void OnPrevWeapon(InputAction.CallbackContext context);
    }
}
