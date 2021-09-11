using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerState
{
	// gettables
	public static PlayerArmy army { get => GameManager.inst.playerArmy; }
	public static PlayerArmyController controller { get => GameManager.inst.playerArmyController; }

	private static int _currentFoodStore = int.MinValue;
	public static int currentFoodStore { get => _currentFoodStore; }

	private static HashSet<Vector3Int> _previouslyRevealedOverworldPositions = new HashSet<Vector3Int>();
	public static HashSet<Vector3Int> previouslyRevealedOverworldPositions { get => _previouslyRevealedOverworldPositions; }

	public static void SetFood(int amount) {
		_currentFoodStore = amount;
		UIManager.inst?.UpdateFoodStoreDisplay();
	}

	public static void UpdateFood(int amount) {
		_currentFoodStore += amount;
		UIManager.inst?.UpdateFoodStoreDisplay();
	}

	public static void UpdateRevealedPositions(Vector3Int toAdd) {
		_previouslyRevealedOverworldPositions.Add(toAdd);
	}
}
