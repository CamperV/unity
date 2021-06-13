﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using TMPro;

public abstract class EnemyArmy : Army
{
	// for visualization debug
	private int _ID = -1;
	public int ID {
		get => _ID;
		set {
	    	GetComponentsInChildren<TextMeshPro>()[0].SetText(value.ToString());
			_ID = value;
		}
	}

	// OVERRIDABLES
	public virtual int detectionRange { get { return 1; } }

	public override String ToString() {
		return base.ToString() + $" [{ID}]";
	}
	// OVERRIDABLES

	// OverworldEneies have a special property: tickPool
	// when the PlayerArmy moves, they add ticks to all active entities controlled by EnemyArmyController
	// the enemies must check their pool for when they want to move. This will allow enemies to sometimes move
	// multiple times in reference to the player - likewise, the player can move morethan once before an
	// enemy can theoretically move
	private int tickPool { get; set; }
	
	public Enum.EnemyState state;
	private EnemyArmyController parentController { get => GameManager.inst.enemyController; }

	public abstract void OnHit();
	public abstract void OnAlert();
	
    protected override void Awake() {
		base.Awake();
		//
		tickPool = 0;

		// devug visualization
		GetComponentsInChildren<MeshRenderer>()[0].sortingLayerName = "Overworld Entities";
		GetComponentsInChildren<MeshRenderer>()[0].sortingOrder = 0;
    }
	
    protected void Start() {
		state = Enum.EnemyState.idle;
    }
	
	public void TakeIdleAction() {
		return;
	}
	
	public bool InDetectionRange(FlowField flowField) {
		return gridPosition.ManhattanDistance(flowField.origin) <= detectionRange;
	}

	public void AddTicks(int ticks) {
		tickPool += ticks;
	}

	protected void SpendTicks(int ticks) {
		tickPool -= (int)(ticks / moveSpeed);
	}

	// for future expandability:
	// right now, the closest enemy acts first
	// but in the future, maybe stamina effects will happen
	public float CalculateInitiative() {
		int md = gridPosition.ManhattanDistance(GameManager.inst.player.gridPosition);

		float directionScore = 0.0f;
		switch (gridPosition - GameManager.inst.player.gridPosition) {
			case Vector3Int v when v.Equals(Vector3Int.up):
				directionScore = 0.0f;
				break;
			case Vector3Int v when v.Equals(Vector3Int.right):
				directionScore = 0.1f;
				break;
			case Vector3Int v when v.Equals(Vector3Int.down):
				directionScore = 0.2f;
				break;
			case Vector3Int v when v.Equals(Vector3Int.left):
				directionScore = 0.3f;
				break;
		}

		return (float)md + directionScore;
	}
	
	public bool FollowField(FlowField flowField, Component target) {		
		List<Vector3Int> potentialMoves = new List<Vector3Int>() {
			gridPosition + Vector3Int.up,		// N
			gridPosition + Vector3Int.right,	// E
			gridPosition + Vector3Int.down,		// S
			gridPosition + Vector3Int.left		// W
		};
		var grid = GameManager.inst.overworld;
		
		// don't move until within detection range
		int minCost = flowField.field[gridPosition];
		Vector3Int selectedMove = gridPosition + Vector3Int.zero;
		
		// select your next move
		// if move remains unselected, remain still (.zero)
		foreach(Vector3Int move in potentialMoves) {		
			// otherwise, we must
			// a) make sure the move is in the flowfield
			if (flowField.field.ContainsKey(move)) {
				if (parentController.currentEnemyPositions.Contains(move))
					continue;
				
				if(flowField.field[move] < minCost) {
					minCost = flowField.field[move];
					selectedMove = move;
				}
			}
		}
		
		// here, we check to see if we can even make this move (point pool)
		// NOTE: it's important that we select our move before checking if we can move
		// if we don't, you might have a little back and forth on a road
		//
		// Also, standardize the "attack" bump. Don't make them move into a square
		bool moveSuccess = false;
		var tickCost = (selectedMove == flowField.origin) ? Constants.standardTickCost : grid.TerrainAt(selectedMove).tickCost;
		
		if (tickPool > 0 && tickCost <= tickPool) {
			Vector3Int nextStep = ToPosition(selectedMove, 1);
			moveSuccess = GridMove(nextStep.x, nextStep.y);
			//
			SpendTicks(tickCost);
		}
		
		// return var is used to send a keepAlive signal to the phase
		// if you've depleted your pool, send a false
		// if you were able to move and the pool is not depleted, keep it alive
		return moveSuccess;
	}

	public override bool GridMove(int xdir, int ydir) {
		var overworld = GameManager.inst.overworld;
		Component occupant = AttemptGridMove(xdir, ydir, overworld);

		if (occupant?.MatchesType(typeof(PlayerArmy)) ?? false) {
			InitiateBattle();
		}
		return occupant == null;
	}

	public virtual bool CanAttackPlayer() {
		return tickPool > 0 && gridPosition.AdjacentTo(GameManager.inst.player.gridPosition);
	}

	public void InitiateBattle() {
		// entities can spend their entire remaining tickPool to attack a player
		SpendTicks(tickPool);
		BumpTowards(GameManager.inst.player.gridPosition, GameManager.inst.overworld);

		StartCoroutine(ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			Terrain thisTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);
			Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(GameManager.inst.player.gridPosition);
		
			GameManager.inst.EnterBattleState();
			GameManager.inst.tacticsManager.CreateActiveBattle(GameManager.inst.player, this, playerTerrain, thisTerrain, Enum.Phase.enemy);
		}));
	}

	public void JoinBattle() {
		// we don't need this function to start the battle phase, let the EnemyArmyController release back and Resume the battle
		// however, it will now resume with a new enemy joining
		SpendTicks(tickPool);
		BumpTowards(GameManager.inst.player.gridPosition, GameManager.inst.overworld);

		StartCoroutine(ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			Terrain thisTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);		
			GameManager.inst.tacticsManager.AddToActiveBattle(this, thisTerrain);
		}));
	}
}