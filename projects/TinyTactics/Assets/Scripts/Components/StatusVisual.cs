using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class StatusVisual : MonoBehaviour
{	
   [SerializeField] private Image mainImage;

   public void SetImage(Sprite sprite) {
      mainImage.sprite = sprite;
   }
}
