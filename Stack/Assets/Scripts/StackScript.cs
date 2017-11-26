using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackScript : MonoBehaviour {

    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
	[SerializeField] private GameObject gameOverPanel;
    [SerializeField] private float baseSpeed = 2.5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float speedMultiplier = 1.05f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float baseErrorMargin = 0.1f;
    [SerializeField] private float maxErrorMargin = 0.5f;
    [SerializeField] private float errorMarginMultiplier = 1.2f;
    [SerializeField] private float currentErrorMargin;

    public Color32[] gameColors = new Color32[4];
    public Material stackMat;

    private const float BOUNDS_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_START_GAIN = 3; 

    private const string KEY_HIGH_SCORE = "HighScore";

    [SerializeField] private GameObject[] stack;

    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int scoreCount = 0;
    private int stackIndex = 0;
    private int combo = 0;
    private int highScore;

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

        foreach (GameObject cube in stack) {
            ColorMesh(cube.GetComponent<MeshFilter>().mesh);
		}
		gameOverPanel.SetActive (false);
        this.currentSpeed = this.baseSpeed;
        this.currentErrorMargin = this.baseErrorMargin;

        this.GetHighScore();
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)) {

            if(PlaceTile()) {
                SpawnTile();
                scoreCount++;

                scoreText.text = scoreCount.ToString();

                if (scoreCount % 10 == 0) {

                    if (this.currentSpeed < this.maxSpeed) {
                        this.currentSpeed *= this.speedMultiplier;
                    }

                    if (this.currentErrorMargin < this.maxErrorMargin) {
                        this.currentErrorMargin *= this.errorMarginMultiplier;
                    }
                }

            } else {
                EndGame();
            }

        }

        MoveTile();

        //Move the stack
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);      
	}

    private void CreateRubble(Vector3 pos, Vector3 scale) {
        
//        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        go.transform.localPosition = pos;
//        go.transform.localScale = scale;
//        go.AddComponent<Rigidbody>();
//
//        go.GetComponent<MeshRenderer>().material = stackMat;
//
//        ColorMesh(go.GetComponent<MeshFilter>().mesh);

//		Vector3 rubblePos = new Vector3 ((t.position.x > 0) 
//			? t.position.x + (t.localScale.x / 2) 
//			: t.position.x - (t.localScale.x / 2) 
//			, t.position.y
//			, t.position.z);
//
//		Vector3 rubbleScale = new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z);

		Parameters parameters = new Parameters ();
		parameters.PutObjectExtra ("position", pos);
		parameters.PutObjectExtra ("scale", scale);
		parameters.PutObjectExtra ("material", stackMat);

		EventBroadcaster.Instance.PostEvent (EventNames.ON_REQUEST_RUBBLE, parameters);
    }

    private void MoveTile() {

        if (gameOver)
            return;

        tileTransition += Time.deltaTime * this.currentSpeed;
        // tileTransition += Time.deltaTime * this.tileSpeed;

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

        ColorMesh(stack[stackIndex].GetComponent<MeshFilter>().mesh);
    }

    private bool PlaceTile() {
        Transform t = stack[stackIndex].transform;

        if(isMovingOnX) {
            float deltaX = lastTilePosition.x - t.position.x;

            if(Mathf.Abs(deltaX) > this.currentErrorMargin) {
                
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

            if (Mathf.Abs(deltaZ) > this.currentErrorMargin)
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
                    new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))
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

    private void ColorMesh (Mesh mesh) {

        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(scoreCount * 0.25f);

        for (int i = 0; i < vertices.Length; i++) {
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        }

        mesh.colors32 = colors;

    }

    private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t) {

        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);


    }

	private void EndGame() {
        Debug.Log("Lose");
        gameOver = true;
		stack[stackIndex].AddComponent<Rigidbody>();
		gameOverPanel.SetActive (true);

        this.SaveHighScore();

		Debug.Log ("OBSERVER CALLED");
		EventBroadcaster.Instance.PostEvent (EventNames.GAME_OVER);
    }

    private void GetHighScore() {
        this.highScore = PlayerPrefs.GetInt(KEY_HIGH_SCORE, 0);
        this.highScoreText.text = this.highScore.ToString();
    }

    private void SaveHighScore() {
        if (this.scoreCount > this.highScore) {
            PlayerPrefs.SetInt(KEY_HIGH_SCORE, this.scoreCount);
            PlayerPrefs.Save();
        }
    }
}
