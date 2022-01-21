using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Button))]
public class OnClickSwapSprite : MonoBehaviour, IResettableToggle
{
    [HideInInspector] public Button button;
    [HideInInspector] public Image image;

    private Sprite previousSprite;
    private Color previousColor;
    private Sprite originalSprite;
    private Color originalColor;
    public Sprite swapSprite;
    public Color swapColor;

    void Awake() {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    void Start() {
        originalSprite = image.sprite;
        originalColor = image.color;
        //
        previousSprite = swapSprite;
        previousColor = swapColor;

        button.onClick.AddListener(SwapSprite);
    }

    private void SwapSprite() {
        Sprite _tempSprite = image.sprite;
        Color _tempColor = image.color;

        image.sprite = previousSprite;
        image.color = previousColor;

        previousSprite = _tempSprite;
        previousColor = _tempColor;
    }

    public void ResetToggle(Vector3 mouseClickPos) {
        image.sprite = originalSprite;
        image.color = originalColor;
        //
        previousSprite = swapSprite;
        previousColor = swapColor;
    }
}