using System;
using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	
	public Texture2D background;
	public GUISkin skin;
	public GUIStyle style;
	
	Rect lifeRect;
	GUIStyle lifeStyle;

	Rect scoreRect;
	GUIStyle scoreStyle;

	Rect btnRect;
	GUIStyle btnStyle;

	int life = 1000;
	int score = 0;

	float kX;
	float kY;

	static bool isOpenInGame;

	public static bool IsOpen {
		get {
			return isOpenInGame;
		}
	}

	Rect getRect(float left, float top, float width, float height) {
		return new Rect((left - width / 2f) * kX, (top - height / 2f) * kY, width * kX, height * kY);
	}

	void Awake () {
		int step = Screen.width / 100;

		kX = Screen.width / 100f;
		kY = Screen.height / 100f;

		lifeRect = new Rect (step, step, 0, 0);
		lifeStyle = new GUIStyle (style);
		lifeStyle.fontSize = Screen.height / 10;

		scoreRect = new Rect (Screen.width / 2, Screen.height / 2, 0, 0);
		scoreStyle = new GUIStyle (style);
		scoreStyle.alignment = TextAnchor.LowerCenter;
		scoreStyle.fontSize = Screen.height / 8;

		int fontSize = Screen.height / 8, w = 6 * fontSize, h = 2 * step + fontSize;
		btnRect = new Rect ((Screen.width - w) / 2, Screen.height / 2, w, h);
		btnStyle = new GUIStyle (skin.button);
		btnStyle.fontSize = fontSize;

		isOpenInGame = false;
	}

	void OnGUI() {
		GUI.skin = skin;

		if (life > 0) {
			GUI.Label(lifeRect, Mathf.Max(0,life).ToString(), lifeStyle);
			if (isOpenInGame) {
				GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), background);

				if (GUI.Button(getRect(50f, 35f, 30f, 14f), "Restart", btnStyle)) {
					Application.LoadLevel("Game");
				}
				
				if (GUI.Button(getRect(50f, 65f, 20f, 14f), "Quit", btnStyle)) {
					Application.Quit();
				}
			}
		} else {
			isOpenInGame = false;

			GUI.color = Color.black;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), background);
			
			GUI.color = Color.white;
			GUI.Label(scoreRect, "Game Over!\nScore: " + score.ToString(), scoreStyle);
			if (GUI.Button(btnRect, "Start Again", btnStyle)) {
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
	
	void Update () {
		if (life > 0 && Input.GetKeyDown (KeyCode.Escape)) {
			isOpenInGame = !isOpenInGame;
			if (!Map.IsOpen) {
				if (isOpenInGame) {
					State.Stop();
				} else {
					State.Play();
				}
			}
		}
	}

	public int Life {
		get {
			return life;
		}
		set {
			if (value > life)
				score += value - life;
			life = value;
		}
	}
}
