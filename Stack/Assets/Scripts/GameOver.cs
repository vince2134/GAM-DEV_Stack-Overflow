using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

	[SerializeField] private Text score;
	[SerializeField] private Text gameOverScore;

	// Use this for initialization
	void Start () {

		Debug.Log ("OBSERVER MADE");
		EventBroadcaster.Instance.AddObserver (EventNames.GAME_OVER, this.updateGameOverScore);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void updateGameOverScore(){
		Debug.Log ("GAME OVER");
		gameOverScore.text = score.text;
	}

	public void onClickPlayAgain(){
		LoadManager.Instance.LoadScene (SceneNames.GAME_SCENE);	
	}

	public void onClickQuit(){

		//SceneManager.LoadScene (SceneNames.MAIN_MENU_SCENE);	
	}

}
