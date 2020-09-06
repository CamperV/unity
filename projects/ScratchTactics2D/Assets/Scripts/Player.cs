﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover, IPhasedObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	[HideInInspector] public bool phaseActionTaken { get; set; }
	
	public static Player Spawn(Player prefab) {
		Player player = Instantiate(prefab, GameManager.inst.worldGrid.RandomTileReal(), Quaternion.identity);
		player.ResetPosition(new Vector3Int(4, 4, 0));
		GameManager.inst.worldGrid.UpdateOccupantAt(player.gridPosition, player);
		return player;
	}

    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = SpritesResourcesLoader.GetSprite("yellow_skull");
		
		phaseActionTaken = false;
    }
	
	protected override void Start() {
		base.Start();
	}

    // Update is called once per frame
    void Update() {
		if (!MyPhase()) return;
		
		phaseActionTaken = TakePhaseAction();
    }
	
	public bool MyPhase() {
		return GameManager.inst.currentPhase == GameManager.Phase.player && phaseActionTaken == false;
	}
	
	public bool TakePhaseAction() {
		// phaseAction possibilities
		if 		(Input.GetKeyDown("left"))  { AttemptGridMove(-1,  0); }
		else if (Input.GetKeyDown("right")) { AttemptGridMove( 1,  0); }
		else if (Input.GetKeyDown("up"))    { AttemptGridMove( 0,  1); }
		else if (Input.GetKeyDown("down"))  { AttemptGridMove( 0, -1); }
		else if (Input.GetKeyDown("space")) { /* pass turn */ }
		else 								{ return false; } // no input taken
		// other possibilities
		//
		
		// if we've reached this code, the phaseAction has been taken
		// basically, any time Input is recorded a phaseAction is taken
		// if no Input happens, we auto-return before this line
		return true;
	}
		
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
	
	protected override void AttemptGridMove(int xdir, int ydir) {
		base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {
		Enemy hitEnemy = component as Enemy;
		hitEnemy.OnHit();
	}
}
