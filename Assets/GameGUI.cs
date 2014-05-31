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



	void Awake () {
		int step = Screen.width / 100;

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
	}

	void OnGUI() {
		GUI.skin = skin;

		if (life > 0) {
			GUI.Label(lifeRect, Mathf.Max(0,life).ToString(), lifeStyle);
		} else {
			GUI.color = Color.black;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), background);
			
			GUI.color = Color.white;
			GUI.Label(scoreRect, "Game Over!\nScore: " + score.ToString(), scoreStyle);
			if (GUI.Button(btnRect, "Start Again", btnStyle)) {
				Debug.Log ("Arrrr");
				Application.LoadLevel(0);
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
