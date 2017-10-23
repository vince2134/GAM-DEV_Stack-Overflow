using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackScript : MonoBehaviour {

    private const float BOUNDS_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_START_GAIN = 3; 

    [SerializeField] private GameObject[] stack;

    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int scoreCount = 0;
    private int stackIndex = 0;
    private int combo = 0;

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private bool isMovingOnX = true;
    private bool gameOver = false;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;

	// Use this for initialization
	void Start () {
        stackIndex = transform.childCount - 1;
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetMouseButton(0)) {

            if(PlaceTile()) {
                SpawnTile();
                scoreCount++;
                Debug.Log("mouse click");
            } else {
                EndGame();
            }

        }

        MoveTile();

        //Move the stack
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

    private void CreateRubble(Vector3 pos, Vector3 scale) {
        
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();


    }

    private void MoveTile() {

        if (gameOver)
            return;

        tileTransition += Time.deltaTime * tileSpeed;

        if(isMovingOnX)
            stack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
        else
            stack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);

    }

    private void SpawnTile() {

        lastTilePosition = stack[stackIndex].transform.localPosition;
        stackIndex--;

        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;

        desiredPosition = (Vector3.down) * scoreCount;
        stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        stack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
    }

    private bool PlaceTile() {
        Transform t = stack[stackIndex].transform;

        if(isMovingOnX) {
            float deltaX = lastTilePosition.x - t.position.x;

            if(Mathf.Abs(deltaX) > ERROR_MARGIN) {
                
                //CUT THE TILE
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);

                if (stackBounds.x <= 0)
                    return false;

                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                CreateRubble(
                    new Vector3((t.position.x > 0) 
                                ? t.position.x + (t.localScale.x / 2) 
                                : t.position.x - (t.localScale.x / 2) 
                                , t.position.y
                                , t.position.z),
                    new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)
                );

                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
            } else {

                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;

                    if(stackBounds.x > BOUNDS_SIZE)
                        stackBounds.x = BOUNDS_SIZE;
                    
                    float middle = lastTilePosition.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                    t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
                }
                
                combo++;

                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }
        else {
            float deltaZ = lastTilePosition.z - t.position.z;

            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {

                //CUT THE TILE
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);

                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                CreateRubble(
                    new Vector3(t.position.x
                            , t.position.y
                                , (t.position.z > 0)
                            ? t.position.z + (t.localScale.z / 2)
                            : t.position.z - (t.localScale.z / 2)),
                    new Vector3(Mathf.Abs(deltaZ), 1, t.localScale.z)
                );

                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
            } else {

                if(combo > COMBO_START_GAIN) {
                    stackBounds.y += STACK_BOUNDS_GAIN;

                    if (stackBounds.y > BOUNDS_SIZE)
                        stackBounds.y = BOUNDS_SIZE;

                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                }

                combo++;

                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }

        secondaryPosition = (isMovingOnX) ? t.localPosition.x : t.localPosition.z;

        isMovingOnX = !isMovingOnX;

        return true;
    }

    private void EndGame() {
        Debug.Log("Lose");
        gameOver = true;
        stack[stackIndex].AddComponent<Rigidbody>();
    }
}
