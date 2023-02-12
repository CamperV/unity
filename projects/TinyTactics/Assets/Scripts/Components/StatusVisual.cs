using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class StatusVisual : MonoBehaviour, ITooltip
{	

   [SerializeField] private Sprite buffSprite;
   [SerializeField] private Sprite debuffSprite;
   [SerializeField] private Sprite defaultSprite;

   [SerializeField] private Image mainImage;
   private string tooltip; 

   public void SetInfo(so_Status status) {
      // mainImage.sprite = status.sprite;
      mainImage.sprite = GetSprite(status.statusCode);

		tooltip = $"{status.mutatorDisplayData.name}".RichTextTags_TMP(bold: true);
		tooltip += "\n";
		tooltip += $"{status.mutatorDisplayData.description}".RichTextTags_TMP(italics: true);
	}

    // ITooltip
    public string GetTooltip() => tooltip;

   private Sprite GetSprite(so_Status.StatusCode statusCode) {
      switch (statusCode) {
         case so_Status.StatusCode.Buff:
               return buffSprite;
         case so_Status.StatusCode.Debuff:
               return debuffSprite;
         default:
               return defaultSprite;
      }
   }
}
