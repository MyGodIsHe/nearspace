using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour {

	public float viewDelay = 1;
	public float detonateDelay = 3;

	float[] times;

	void Awake() {
		int ln = Enum.GetNames(typeof(Action)).Length;
		times = new float[ln];
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.A))
			start(Action.Left);
		if (Input.GetKeyUp(KeyCode.A ))
			stop(Action.Left);
		
		if (Input.GetKeyDown(KeyCode.D))
			start(Action.Right);
		if (Input.GetKeyUp(KeyCode.D))
			stop(Action.Right);
		
		
		if (Input.GetKeyDown(KeyCode.W))
			start(Action.Up);
		if (Input.GetKeyUp(KeyCode.W))
			stop(Action.Up);
		
		if (Input.GetKeyDown(KeyCode.S))
			start(Action.Down);
		if (Input.GetKeyUp(KeyCode.S))
			stop(Action.Down);
		
		
		if (Input.GetKeyDown(KeyCode.Space))
			start(Action.Jump);
		if (Input.GetKeyUp(KeyCode.Space))
			stop(Action.Jump);
	}
	
	void start(Action action) {
		times[(int)action] = Time.time;
	}

	void stop(Action action) {
		times[(int)action] = 0;
	}

	bool isActiveDirection(Action action) {
		float time = times[(int)action];
		return time != 0 && Time.time - time > viewDelay;
	}

	public Vector3 GetViewDirectio() {
		Vector3 direction = Vector3.zero;

		if (isActiveDirection(Action.Left))
			direction += Vector3.left;
		
		if (isActiveDirection(Action.Right))
			direction += Vector3.right;
		
		if (isActiveDirection(Action.Up))
			direction += Vector3.forward;
		
		if (isActiveDirection(Action.Down))
			direction += Vector3.back;

		return direction;
	}
	
	public float GetDetonateProgress() {
		float time = times[(int)Action.Jump];
		return time != 0 ? (Time.time - time) / detonateDelay : 0;
	}

	public bool IsDetonate {
		get {
			float time = times[(int)Action.Jump];
			bool result = time != 0 && Time.time - time > detonateDelay;
			if (result)
				times[(int)Action.Jump] = 0;
			return result;
		}
	}
}

enum Action {
	Left,
	Right,
	Up,
	Down,
	Jump
}