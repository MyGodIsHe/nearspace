using System;
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	public GUISkin skin;

	private bool isActive = false;
	private readonly int boxPadding = 20;
	private readonly int buttonWidth = 120;
	private readonly int buttonHeight = 60;
	private readonly int buttonPadding = 10;
	private string[] levels;

	void Awake () {
		object[] rawLevels = Resources.LoadAll("", typeof(TextAsset));
		levels = new string[rawLevels.Length];
		int i = 0;
		foreach (TextAsset level in rawLevels) {
			levels[i++] = level.name;
		}
	}

	void OnGUI() {
		if (!isActive)
			return;

		GUI.skin = skin;

		GUI.Box(new Rect(boxPadding, boxPadding,
		                 Screen.width - boxPadding*2, Screen.height - boxPadding*2),
		        "Levels");
		int cols = Convert.ToInt32((Screen.width - boxPadding * 2 - buttonPadding) / (buttonWidth + buttonPadding));
		int rows = (int)Math.Ceiling((float)levels.Length / cols);

		var enumLevels = levels.GetEnumerator();
		if (!enumLevels.MoveNext ()) return;

		for (int j = 0; j < rows; j++) {
			for (int i = 0; i < cols; i++) {
				string level = enumLevels.Current as string;
				
				if (GUI.Button (new Rect (boxPadding + buttonPadding + i * (buttonWidth + buttonPadding),
				                          15 + boxPadding + buttonPadding + j * (buttonHeight + buttonPadding),
				                          buttonWidth, buttonHeight),
				                level)) {
					// none
				}
				if (!enumLevels.MoveNext())
					break;
			}
		}
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit();
			//isActive = !isActive;
		}
	}
}
