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
	List<GameObject> garbage = new List<GameObject>();

	void Start() {
		levels = new Level[width, height];
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++) {
				levels[x, y] = new Level(Random.Range(1, 5).ToString());
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

		player = Instantiate(playerPrefab) as GameObject;
		player.transform.position = room.transform.position;

		view = GetComponent<StaticTop>();
		view.SetTarget(player);
		updateRange();
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
		foreach (GameObject obj in garbage)
			Destroy (obj);
		garbage.Clear();
	}

	public void MoveTo(int x, int y) {
		if (x == 0 && y == 0)
			return;
		Vector2 oldSize = level.size;

		position.x += x;
		position.y += y;

		garbage.Add(room);
		room = Instantiate(roomPrefab, room.transform.position, Quaternion.identity) as GameObject;
		if (x != 0)
			room.transform.position += (new Vector3((level.size.x + 2) / 2 + (oldSize.x + 2) / 2, 0, 0)) * x;
		if (y != 0)
			room.transform.position -= (new Vector3(0, 0, (level.size.y + 2) / 2 + (oldSize.y + 2) / 2)) * y;
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
