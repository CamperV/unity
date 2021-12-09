using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerUnit))]
public class PlayerUnitPhase : MonoBehaviour, IUnitPhase
{
    private TurnManager turnManager;
    private PlayerUnit unit;

    public bool active { get; set; } = true;
    public bool moveAvailable { get; set; } = true;
    public bool attackAvailable { get; set; } = true;

    private Color originalSpriteColor;

    void Awake() {
        turnManager = GetComponentInParent<TurnManager>();
        unit = GetComponent<PlayerUnit>();
    }

    void Start() {
        originalSpriteColor = unit.GetComponent<SpriteRenderer>().color;

        turnManager.playerPhase.StartEvent += TriggerPhase;
        turnManager.playerPhase.EndEvent += EndPhase;
    }

    public void Refresh() {
        active = true;
        moveAvailable = true;
        attackAvailable = true;

        unit.GetComponent<SpriteRenderer>().color = originalSpriteColor;
    }

    // CHANGE THESE TO BE EVENT BASED
    // IE, ISSUE AN EVENT WHEN ANY UNIT ATTACKS/MOVES
    public void TriggerPhase() {
        Refresh();
        unit.ChangeState(PlayerUnit.PlayerUnitFSM.Idle);

        Debug.Log($"I, {unit}, saw the Player phase activate");
    }

    public void EndPhase() {
        Debug.Log($"I, {unit}, was witness to the Player phase END");
        Refresh();
	}

    public void Finish() {
        Debug.Log($"{unit} is spent");
        active = false;
        moveAvailable = false;
        attackAvailable = false;
        unit.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0.75f, 0.75f, 1f);
    }
}
