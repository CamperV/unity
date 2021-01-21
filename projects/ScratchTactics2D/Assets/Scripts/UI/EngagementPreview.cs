using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngagementPreview : MonoBehaviour
{
    // UI elements
    Image background;           // under GameObject "Background"
    Image banner;               // under GameObject "Banner"
    Image playerPortrait;       // under GameObject "PlayerPortrait"
    Image enemyPortrait;        // under GameObject "EnemyPortrait"

    Text pDamage;
    Text pHitRate;
    Text pCritRate;
    Text eDamage;
    Text eHitRate;
    Text eCritRate;

    public EngagementResults engagementResults { get; private set; }
    
    public Unit aggressor { get => engagementResults.aggressor; }
    public Unit defender { get => engagementResults.defender; }

    public Attack? firstAttack { get => engagementResults.firstAttack ?? null; }
    public Attack? secondAttack { get => engagementResults.secondAttack ?? null; }

    public static EngagementPreview Spawn(Transform parent, EngagementPreview prefab, EngagementResults engagementResults) {
        EngagementPreview ep = Instantiate(prefab, parent);
        ep.engagementResults = engagementResults;

        // set banner sprite based on who is aggressing
        // set potraits based on units that are in the combat preview
        if (ep.aggressor.isPlayerControlled) {
            ep.banner.sprite = ResourceLoader.GetSprite("engagement_preview");
            ep.playerPortrait.sprite = ep.aggressor.GetSprite();
            ep.enemyPortrait.sprite = ep.defender.GetSprite();

            // there must always be a firstAttack
            ep.pDamage.text   = ep.firstAttack.damage.ToString() + " DMG";
            ep.pHitRate.text  = ep.firstAttack.hitRate.ToString() + "% HIT";
            ep.pCritRate.text = ep.firstAttack.critRate.ToString() + "% CRIT";

            ep.eDamage.text   = (ep.secondAttack?.damage.ToString() ?? "-") + " DMG";
            ep.eHitRate.text  = (ep.secondAttack?.hitRate.ToString() ?? "-") + "% HIT";
            ep.eCritRate.text = (ep.secondAttack?.critRate.ToString() ?? "-") + "% CRIT";
        }
        
        /* This doesn't really exist yet. Some enemies will ALWAYS attack first, THEN implement this later
        else {
            ep.banner.sprite = ResourceLoader.GetSprite("engagement_preview_flip");
            ep.playerPortrait.sprite = ep.defender.GetSprite();
            ep.enemyPortrait.sprite = ep.aggressor.GetSprite();

            // there must always be a firstAttack
            ep.eDamage.text   = ep.firstAttack.damage.ToString() + " DMG";
            ep.eHitRate.text  = ep.firstAttack.hitRate.ToString() + "% HIT";
            ep.eCritRate.text = ep.firstAttack.critRate.ToString() + "% CRIT";

            ep.pDamage.text   = ep.secondAttack.damage.ToString() + " DMG";
            ep.pHitRate.text  = ep.secondAttack.hitRate.ToString() + "% HIT";
            ep.pCritRate.text = ep.secondAttack.critRate.ToString() + "% CRIT";
        }
        */

        return ep;
    }

    void Awake() {
        // DFS ALL children, including grandchildren
        var images     = GetComponentsInChildren<Image>();
        background     = images[0];
        banner         = images[1];
        playerPortrait = images[2];
        enemyPortrait  = images[3];

        var texts = GetComponentsInChildren<Text>();
        pDamage   = texts[0];
        pHitRate  = texts[1];
        pCritRate = texts[2];
        eDamage   = texts[3];
        eHitRate  = texts[4];
        eCritRate = texts[5];

        // debug
        banner.gameObject.SetActive(false);
    }
}
