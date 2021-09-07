using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngagementPreview : MonoBehaviour
{
    // UI elements
    private Image background;           // under GameObject "Background"
    private Image playerPortrait;       // under GameObject "PlayerPortrait"
    private Image enemyPortrait;        // under GameObject "EnemyPortrait"
    private Image playerEmblem;         // under GameObject "PlayerEmblem"
    private Image enemyEmblem;          // under GameObject "EnemyEmblem"

    private Text pDamage;
    private Text pHitRate;
    private Text pCritRate;
    private Text eDamage;
    private Text eHitRate;
    private Text eCritRate;

    private Text playerHealthValue;
    private Text enemyHealthValue;
    private UIHealthBar playerHealthBar;
    private UIHealthBar enemyHealthBar;

    public EngagementResults engagementResults { get; private set; }
    
    public Unit aggressor { get => engagementResults.aggressor; }
    public Unit defender { get => engagementResults.defender; }

    public Attack firstAttack { get => engagementResults.firstAttack ?? null; }
    public Attack secondAttack { get => engagementResults.secondAttack ?? null; }

    public static EngagementPreview Spawn(Transform parent, EngagementPreview prefab, EngagementResults engagementResults) {
        EngagementPreview ep = Instantiate(prefab, parent);
        ep.engagementResults = engagementResults;

        // set potraits based on units that are in the combat preview
        ep.playerPortrait.sprite = ep.aggressor.portrait;
        ep.enemyPortrait.sprite = ep.defender.portrait;

        ep.playerEmblem.sprite = Emblem.FromWeapon(ep.aggressor.equippedWeapon);
        ep.enemyEmblem.sprite = Emblem.FromWeapon(ep.defender.equippedWeapon);

        // there must always be a firstAttack
        ep.pDamage.text   = ep.firstAttack.damage.ToString();
        ep.pHitRate.text  = ep.firstAttack.hitRate.ToString();
        ep.pCritRate.text = ep.firstAttack.critRate.ToString();

        ep.eDamage.text   = (ep.secondAttack?.damage.ToString() ?? "-");
        ep.eHitRate.text  = (ep.secondAttack?.hitRate.ToString() ?? "-");
        ep.eCritRate.text = (ep.secondAttack?.critRate.ToString() ?? "-");

        ep.playerHealthValue.text = $"{ep.aggressor._HP}";
        ep.enemyHealthValue.text = $"{ep.defender._HP}";

        ep.playerHealthBar.Init(ep.aggressor.VITALITY, ep.aggressor._HP);
        ep.enemyHealthBar.Init(ep.defender.VITALITY, ep.defender._HP);

        ep.playerHealthBar.FlashPotentialDamage(ep.secondAttack?.damage ?? 0);
        ep.enemyHealthBar.FlashPotentialDamage(ep.firstAttack.damage);
        return ep;
    }

    void Awake() {
        // DFS ALL children, including grandchildren. Also yourself?
        var images     = GetComponentsInChildren<Image>();
        background     = images[0];
        playerPortrait = images[1];
        playerEmblem   = images[2];
        enemyPortrait  = images[3];
        enemyEmblem    = images[4];

        var texts = GetComponentsInChildren<Text>();
        pDamage   = texts[0];
        pHitRate  = texts[1];
        pCritRate = texts[2];
        eDamage   = texts[3];
        eHitRate  = texts[4];
        eCritRate = texts[5];
        //
        playerHealthValue = texts[6];
        enemyHealthValue  = texts[7];

        var healthBars = GetComponentsInChildren<UIHealthBar>();
        playerHealthBar = healthBars[0];
        enemyHealthBar  = healthBars[1];
    }
}