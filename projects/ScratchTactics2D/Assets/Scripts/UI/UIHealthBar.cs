using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public int pipsInRow = 10;
    public int reflection = 1;  // reversed or not: should be 1 or -1

    public int maxPips = 1;
    public int currVal;

    public Dictionary<Vector3Int, GameObject> pips = new Dictionary<Vector3Int, GameObject>();

    public static GameObject NewPip(Transform parent, Vector3Int offset, Sprite sprite, string name = "Pip") {
        GameObject pip = new GameObject();
        pip.name = name;
        pip.AddComponent<CanvasRenderer>();
        pip.AddComponent<RectTransform>();
        pip.AddComponent<Image>();
        //
        Image im = pip.GetComponent<Image>();
        im.sprite = sprite;
        im.SetNativeSize();

        pip.transform.SetParent(parent);
        pip.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        pip.transform.position = parent.position;
        pip.transform.position += new Vector3(offset.x * im.sprite.rect.width  * pip.transform.localScale.x * 3.0f,
                                              offset.y * im.sprite.rect.height * pip.transform.localScale.y * 3.0f, 0);
        return pip;
    }

    public void Init(int max, int current) {
        pips.Clear();

        maxPips = max;
        currVal = current;
        
        // fully draw here
        for (int i = 0; i < maxPips; i++) {
            int x = i % pipsInRow;
            int y = (int)(i / pipsInRow); // this should truncate, do you C#?
            
            Vector3Int pos = new Vector3Int(reflection*x, y, 0);
            Sprite sp = (i < currVal) ? ResourceLoader.GetSprite("full_pip") : ResourceLoader.GetSprite("empty_pip");
            GameObject pip = NewPip(transform, pos, sp, name: $"Pip{i}");

            pips[pos] = pip;
        }

        // finally: if you have multiple levels of health bar, i.e. > pipsInRow, translate appropriately
        // for every level, translate 1/2 of a block's height
        int levels = maxPips / pipsInRow;
        var reference = pips[Vector3Int.zero].GetComponent<Image>().sprite.rect;

        Vector3 translation = new Vector3(0, 0.5f * levels * reference.height * pips[Vector3Int.zero].transform.localScale.y * 3.0f, 0);
        transform.position -= translation;

        // apply the opposite to the text
        transform.GetChild(0).position += translation;
    }

    public void FlashPotentialDamage(int incomingDamage) {
        int postDamage = Mathf.Max(0, (currVal - incomingDamage));

        for (int i = postDamage; i < currVal; i++) {
            int x = i % pipsInRow;
            int y = (int)(i / pipsInRow); // this should truncate, do you C#?
            
            Vector3Int pos = new Vector3Int(reflection*x, y, 0);
            GameObject pip = pips[pos];

            pip.GetComponent<CanvasRenderer>().SetAlpha(0.5f);
            //Sprite fp = ResourceLoader.GetSprite("full_pip");
            //Sprite ep = ResourceLoader.GetSprite("empty_pip");
           // GameObject pip = NewPip(transform, pos, fp, name: $"Pip{i}");
        }
    }
}
