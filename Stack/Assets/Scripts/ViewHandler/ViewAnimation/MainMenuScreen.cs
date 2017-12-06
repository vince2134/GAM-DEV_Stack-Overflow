using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScreen : View {

    [SerializeField] Text highScoreText;

    private const string KEY_HIGH_SCORE = "HighScore";

	void Awake() {

	}

	// Use this for initialization
	void Start () {
		this.GetHighScore();
	}

	public void OnPlayClicked() {
		SceneManager.LoadScene (SceneNames.GAME_SCENE);	
	}

	public void OnQuitClicked() {
		Application.Quit();
	}

    private void GetHighScore() {
        this.highScoreText.text = PlayerPrefs.GetInt(KEY_HIGH_SCORE, 0).ToString();
    }
}
