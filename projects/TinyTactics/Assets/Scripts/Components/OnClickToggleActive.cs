using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnClickToggleActive : MonoBehaviour
{
    [HideInInspector] public Button button;
    public GameObject toggleObject;

    void Awake() => button = GetComponent<Button>();

    void Start() {
        button.onClick.AddListener(ToggleObject);
    }

    private void ToggleObject() {
        toggleObject.SetActive( !toggleObject.activeInHierarchy );
    }
}