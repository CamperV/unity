using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public Battle battle;
    public Vector3 focalPoint;
    public Vector3 focalPivot;

    private Vector3 dragOffset;
    private bool draggingView;
    //
    private Vector3 lockedPosition;
    private Vector3 lockedScale;
    private bool viewLock;

    public float currentZoomLevel { get => battle.transform.localScale.x; }

	private Dictionary<KeyCode, Action> getKeyActionBindings = new Dictionary<KeyCode, Action>();
    private Dictionary<KeyCode, Action> getKeyDownActionBindings = new Dictionary<KeyCode, Action>();
    private bool ScreenDrag { get => Input.GetMouseButton(1); }
    private Func<Vector3Int, Vector3Int> RotateLeft  = v => new Vector3Int(v.y, -v.x, v.z);
    private Func<Vector3Int, Vector3Int> RotateRight  = v => new Vector3Int(-v.y, v.x, v.z);

    void Awake() {
        battle = GetComponent<Battle>();
        focalPoint = battle.transform.position; // the battle will be constantly moved to align with this focal point
        focalPivot = battle.transform.position; // this will be init'd to the center of the screen (and never touched again)
        Zoom(1.0f); // default zoom level

        dragOffset = Vector3.zero;
        draggingView = false;
        //
        lockedPosition = Vector3.zero;
        lockedScale = Vector3.one;
        viewLock = false;

		getKeyActionBindings[KeyCode.W] = () => Pan(Vector3.up, rate: 6f);
		getKeyActionBindings[KeyCode.A] = () => Pan(Vector3.left, rate: 8f);
		getKeyActionBindings[KeyCode.S] = () => Pan(Vector3.down, rate: 6f);
		getKeyActionBindings[KeyCode.D] = () => Pan(Vector3.right, rate: 8f);
        //
        getKeyDownActionBindings[KeyCode.Q] = () => Rotate(RotateRight);
        getKeyDownActionBindings[KeyCode.E] = () => Rotate(RotateLeft);
    }

    void Update() {
        //DragUpdate();
        //ScrollUpdate();

        // this allows each held action to update the focalPoint
        foreach (KeyCode kc in getKeyActionBindings.Keys) {
			if (Input.GetKey(kc)) getKeyActionBindings[kc]();
		}

        // this allows each KeyDown action to update, but doesn't check if held
        foreach (KeyCode kc in getKeyDownActionBindings.Keys) {
			if (Input.GetKeyDown(kc)) getKeyDownActionBindings[kc]();
		}
    }

    void LateUpdate() {
    	// update focalPoint tracking
        Vector3 focalOffset = focalPoint - focalPivot;
        Vector3 toPosition = focalPivot - focalOffset;
    	if (Vector3.Distance(battle.transform.position, toPosition) > 0.01f) {
		    battle.transform.position = Vector3.Lerp(battle.transform.position, toPosition, Time.deltaTime*5);
		} else {
			battle.transform.position = toPosition;
		}
    }

    //
    // RMB drag view
    //
    public void DragUpdate() {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ScreenDrag && !draggingView) {
            dragOffset = battle.transform.position - mouseWorldPos;
            draggingView = true;
        }
        // update pos by offset, release drag when mouse goes up
        if (draggingView) {
            battle.transform.position = mouseWorldPos + dragOffset;
        }

        // make sure we can drop out of the dragging mode
        if (draggingView && !ScreenDrag) {
            draggingView = false;
        }
    }

    //
    // mouse view control
    //
    public void ScrollUpdate() {
        if (!draggingView) {
            // calculate what the new scale WILL be
            // and calculate the scale ratio. Just use X, because our scale is uniform on all axes
            var updatedScale = battle.transform.localScale + (Input.GetAxis("Mouse ScrollWheel") * 0.75f) * Vector3.one;
            float scaleRatio = updatedScale.x / battle.transform.localScale.x;

            if (scaleRatio != 1.0f) {
                Vector3 localToMouse = battle.transform.position - GameManager.inst.mouseManager.mouseWorldPos;
                
                //update the scale, and position based on the new scale
                battle.transform.localScale = updatedScale;
                battle.transform.position = GameManager.inst.mouseManager.mouseWorldPos + (localToMouse * scaleRatio);
            }
        }
    }

    public void Zoom(float zoomLevel) {
        Vector3 updatedScale = zoomLevel * Vector3.one;
        StartCoroutine( SmoothCameraMovement(0.15f, battle.transform.position, updatedScale) );
    }

    public void ZoomToAndLock(Vector3 target, float zoomLevel) {
        lockedPosition = battle.transform.position;
        lockedScale = battle.transform.localScale;
        viewLock = true;

        Vector3 screenPoint = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f);
        Vector3 updatedScale = zoomLevel * Vector3.one;
        float scaleRatio = updatedScale.x / lockedScale.x;
        
        // move the selected target position to Camera.main.x/y position
        Vector3 toPosition = screenPoint - (target - battle.transform.position)*scaleRatio;
        StartCoroutine( SmoothCameraMovement(0.15f, toPosition, updatedScale) );
    }

    public void ReleaseLock() {
        if (viewLock) {
            StartCoroutine( SmoothCameraMovement(0.15f, lockedPosition, lockedScale) );
            lockedPosition = battle.transform.position;
            lockedScale = battle.transform.localScale;
            viewLock = false;
        }
    }

    // this is a flat speed, with no acceleration
    // all "acceleration" is from the battle -> focal Lerp
    // use Tile.deltaTime to decouple from frame rate
    private void Pan(Vector3 unitDirection, float rate = 1f) {
        focalPoint = focalPoint + (Time.deltaTime*rate)*unitDirection;
    }

    // this rotates the Tilemap
    // animates it
    // updates the appropriate data structures
    private void Rotate(Func<Vector3Int, Vector3Int> Transformer) {
        // record current tiles at current locations for anim part 1
        var prerotationTileDict = battle.grid.GetTilemapDict<TacticsTile>(battle.grid.baseTilemap);
        var prerotationPosition = battle.grid.GetGridCenterReal();
        BattleMap.RotateTilemap(battle.grid.baseTilemap, Transformer);
        
        // finally, move the battle grid back to where it was pre-rotation
        Vector3 correctionVector = battle.grid.GetGridCenterReal() - prerotationPosition;
        battle.transform.position -= correctionVector;
        focalPoint += correctionVector;

		// animate the motion by spawning sprites and going _from -> _to
		foreach (Vector3Int _from in prerotationTileDict.Keys) {
            Vector3Int _to = Transformer(_from);
			Vector3 from = battle.grid.baseTilemap.GetCellCenterWorld(_from);
			Vector3 to = battle.grid.baseTilemap.GetCellCenterWorld(_to);
			
			MovingSprite anim = MovingSprite.ConstructWith(from + correctionVector, prerotationTileDict[_from].sprite, "Tactics Entities");
			anim.SendToAndDestruct(to, 1f);
		}
        battle.InvisibleFor(1f);

        // update tacticsGrid translation2D
		Dictionary<Vector2Int, Vector3Int> _translation2D = new Dictionary<Vector2Int, Vector3Int>();
		foreach (Vector3Int tilePos in battle.grid.translation2D.Values) {
            Vector3Int rotated = Transformer(tilePos);
			_translation2D[new Vector2Int(rotated.x, rotated.y)] = rotated;
		}
        battle.grid.translation2D = _translation2D;

        // then change each units gridPosition
        // then occupancyGrid
        // then update real position
        Dictionary<Vector3Int, Component> _occupancyGrid = new Dictionary<Vector3Int, Component>();
        foreach(MovingGridObject mo in battle.GetRegisteredInBattle()) {
            Vector3Int rotated = Transformer(mo.gridPosition);
            _occupancyGrid[rotated] = mo;
            mo.UpdateGridPosition(rotated, battle.grid);
        }
    	battle.grid.occupancyGrid = _occupancyGrid;
    }

    private IEnumerator SmoothCameraMovement(float fixedTime, Vector3 toPosition, Vector3 toScale) {
		float timeRatio = 0.0f;
        Vector3 startPos = battle.transform.position;
        Vector3 startScale = battle.transform.localScale;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			battle.transform.position = Vector3.Lerp(startPos, toPosition, timeRatio);
            battle.transform.localScale = Vector3.Lerp(startScale, toScale, timeRatio);
			yield return null;
		}
		
		// after the while loop is broken:
		battle.transform.position = toPosition;
        battle.transform.localScale = toScale;
    }
}
