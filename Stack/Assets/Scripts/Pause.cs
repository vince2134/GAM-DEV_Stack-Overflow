using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {


	// Use this for initialization
	void Start () {
		Debug.Log ("OBSERVER MADE");
		EventBroadcaster.Instance.AddObserver (EventNames.PAUSE, this.pauseGame);
		EventBroadcaster.Instance.AddObserver (EventNames.RESUME, this.resume);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void pauseGame(){
		Debug.Log ("PAUSE");
	}

	public void resume() {
		Debug.Log ("Resume");

	}

	public void onClickQuit(){

		//SceneManager.LoadScene (SceneNames.MAIN_MENU_SCENE);	
	}
}
