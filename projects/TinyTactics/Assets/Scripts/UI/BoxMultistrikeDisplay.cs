using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxMultistrikeDisplay : UIMultistrikeDisplay
{
    [SerializeField] private GameObject container;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI multistrikeText;
    
    [SerializeField] private Color x2_Color;
    [SerializeField] private Color x3_Color;
    [SerializeField] private Color x4_Color;

    public override void DisplayMultistrike(int multistrikeValue) {
        Clear();
        		
		if (multistrikeValue > 0) {
			container.SetActive(true);
			// multistrikeText.SetText($"<shake>x{multistrikeValue+1}</shake>");
            multistrikeText.SetText($"<wave>x{multistrikeValue+1}</wave>");

            // if (multistrikeValue == 1)      background.color = x2_Color;
            // else if (multistrikeValue == 2) background.color = x3_Color;
            // else if (multistrikeValue >= 3) background.color = x4_Color;
            if (multistrikeValue == 1)      multistrikeText.color = x2_Color;
            else if (multistrikeValue == 2) multistrikeText.color = x3_Color;
            else if (multistrikeValue >= 3) multistrikeText.color = x4_Color;
		}
    }

    private void Clear() {
        multistrikeText.SetText($"x1");
        container.SetActive(false);
    }
}
