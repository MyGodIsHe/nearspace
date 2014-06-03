using System;
using UnityEngine;
using System.Collections;

public class MainGUI : MonoBehaviour {
	
	public GUISkin skin;
	
	float kX;
	float kY;
	
	void Awake () {
		kX = Screen.width / 100f;
		kY = Screen.height / 100f;
	}

	void OnGUI() {
		GUI.skin = skin;

		GUIStyle btnStyle = new GUIStyle (skin.button);
		btnStyle.alignment = TextAnchor.MiddleCenter;
		btnStyle.fontSize = (int)(kY * 12f);
		GUI.color = Color.white;
		if (GUI.Button(getRect(50f, 35f, 20f, 14f), "Start", btnStyle)) {
			Application.LoadLevel("Game");
		}

		if (GUI.Button(getRect(50f, 65f, 20f, 14f), "Quit", btnStyle)) {
			Application.Quit();
		}
	}

	Rect getRect(float left, float top, float width, float height) {
		return new Rect((left - width / 2f) * kX, (top - height / 2f) * kY, width * kX, height * kY);
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit();
		}
	}
}
