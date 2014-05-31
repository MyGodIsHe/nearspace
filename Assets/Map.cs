using UnityEngine;
using System.Collections.Generic;


public class Map : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject roomPrefab;

	readonly int width = 5, height = 5;
	Level[,] levels;
	Position position;
	GameObject room;
	GameObject player;
	StaticTop view;
	GameObject previousRoom;
	GameController gameController;
	GameGUI gameGUI;

	void Awake() {
		gameGUI = Camera.main.GetComponent<GameGUI>();
		gameController = Camera.main.GetComponent<GameController>();
		view = GetComponent<StaticTop>();
	}

	void Start() {
		levels = new Level[width, height];
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++) {
				levels[x, y] = new Level(Random.Range(1, 6).ToString());
				levels[x, y].RandomTranform();
		}

		for (int x = 0; x < width; x++) {
			levels[x, 0].directions ^= Directions.Up;
			levels[x, height - 1].directions ^= Directions.Down;
		}
		
		for (int y = 0; y < height; y++) {
			levels[0, y].directions ^= Directions.Left;
			levels[width - 1, y].directions ^= Directions.Right;
		}

		position = new Position(Mathf.FloorToInt(width / 2),
		                        Mathf.FloorToInt(height / 2));
		room = Instantiate(roomPrefab) as GameObject;
		levels[position.x, position.y] = new Level("start");
		levels[position.x, position.y].RandomTranform();
		room.GetComponent<Room>().LoadLevel(level);

		createPlayer(room.transform.position);
		updateRange();
	}

	void createPlayer(Vector3 position) {
		if (player != null)
			Destroy(player);
		player = Instantiate(playerPrefab) as GameObject;
		player.transform.position = position;
		view.SetTarget(player);
	}

	void Update() {
		if (player == null)
			return;
		if (gameController.IsDetonate) {
			GameObject oldRoom = room;
			LoadRoom(room.transform.position);
			Room r = oldRoom.GetComponent<Room>();
			Destroy(oldRoom);
			createPlayer(r.EnterPlayerPosition);
			player.rigidbody.velocity = r.EnterPlayerVelosity;
			gameGUI.Life -= 100;
		}
		if (gameGUI.Life <= 0) {
			Destroy(player);
			player = null;
		}
	}

	void updateRange() {
		view.SetViewRange(level.size.x + 2);
		view.SetRange(new Rect(
			room.transform.position.x - (level.size.x / 2 + 1),
			room.transform.position.z + (level.size.y / 2 + 1),
			level.size.x + 2,
			level.size.y + 2
		));
	}
	
	public Level level {
		get {
			return levels[position.x, position.y];
		}
	}

	public Vector3 GetCenter() {
		return room.transform.position;
	}

	public void UnlockRoom() {
		room.GetComponent<Room>().Unlock();
		if (previousRoom != null) {
			Destroy (previousRoom);
			previousRoom = null;
		}
		gameGUI.Life += level.cubes.Length;
	}

	public void MoveTo(int x, int y) {
		if (x == 0 && y == 0)
			return;

		position.x += x;
		position.y += y;

		Vector2 oldSize = level.size;
		Vector3 roomPosition = room.transform.position;
		if (x != 0)
			roomPosition += (new Vector3((level.size.x + 2) / 2 + (oldSize.x + 2) / 2, 0, 0)) * x;
		if (y != 0)
			roomPosition -= (new Vector3(0, 0, (level.size.y + 2) / 2 + (oldSize.y + 2) / 2)) * y;
		
		previousRoom = room;

		LoadRoom(roomPosition);
	}

	void LoadRoom(Vector3 position) {
		room = Instantiate(roomPrefab, position, Quaternion.identity) as GameObject;
		room.GetComponent<Room>().LoadLevel(level);
		updateRange();
	}

	struct Position {
		public int x;
		public int y;

		public Position(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}
}
