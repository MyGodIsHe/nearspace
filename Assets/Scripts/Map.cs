using UnityEngine;
using System.Collections.Generic;


public class Map : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject roomPrefab;
	public Texture2D wallTexture;
	public Texture2D floorTexture;
	public Texture2D doorTexture;
	public GameObject roomTextPrefab;

	static bool isOpenGlobalMap;

	public static Room CurrentRoom;
	public static bool IsOpen {
		get {
			return isOpenGlobalMap;
		}
	}

	const int WIDTH = 5, HEIGHT = 5;
	GameObject player;
	StaticTop view;
	GameController gameController;
	GameGUI gameGUI;
	GameObject globalMap;

	void Awake() {
		gameGUI = Camera.main.GetComponent<GameGUI>();
		gameController = Camera.main.GetComponent<GameController>();
		view = GetComponent<StaticTop>();
		Time.timeScale = 1f;
		isOpenGlobalMap = false;
	}

	void Start() {
		createGlobalMap();

		Vector3 startPosition = new Vector3((WIDTH / 2) * Level.OUT_SIZE, (HEIGHT / 2) * Level.OUT_SIZE, 0);

		createPlayer(startPosition + Vector3.up * Cube.WORLD_HALF_SIZE);
		State.Play();
	}

	void createGlobalMap() {
		var texture = generateRoomTexture();
		var sprite = generateRoomSprite(texture);
		globalMap = new GameObject("GlobalMap");
		Level[,] levels = createLevels();
		for (int x = 0; x < WIDTH; x++) {
			for (int y = 0; y < HEIGHT; y++) {
				GameObject room = Instantiate(roomPrefab) as GameObject;
				room.GetComponent<Room>().Init(levels[x, y], sprite, roomTextPrefab);
				room.transform.parent = globalMap.transform;
				room.transform.position = new Vector3(x * Level.OUT_SIZE, y * Level.OUT_SIZE);
			}
		}
	}

	Level[,] createLevels() {
		Level[,] levels = new Level[WIDTH, HEIGHT];
		for (int x = 0; x < WIDTH; x++) {
			for (int y = 0; y < HEIGHT; y++) {
				if (x == WIDTH / 2 && y == HEIGHT / 2)
					continue;
				levels[x, y] = new Level(Random.Range(1, 7).ToString());  // [min, max)
				levels[x, y].RandomTranform();
			}
		}
		
		for (int x = 0; x < WIDTH; x++) {
			levels[x, 0].directions ^= Directions.Up;
			levels[x, HEIGHT - 1].directions ^= Directions.Down;
		}
		
		for (int y = 0; y < HEIGHT; y++) {
			levels[0, y].directions ^= Directions.Left;
			levels[WIDTH - 1, y].directions ^= Directions.Right;
		}

		Level zero = new Level("0");
		zero.RandomTranform();
		levels[WIDTH / 2,HEIGHT / 2] = zero;
		return levels;
	}

	void createPlayer(Vector3 position) {
		if (player != null)
			Destroy(player);
		player = Instantiate(playerPrefab) as GameObject;
		player.transform.position = position;
		view.SetTarget(player);
	}
	
	Texture2D generateRoomTexture() {
		var width = wallTexture.width * Level.OUT_SIZE;
		var height = wallTexture.height * Level.OUT_SIZE;
		var texture = new Texture2D(width, height, TextureFormat.ARGB32, true);

		width = wallTexture.width;
		height = wallTexture.height;

		Color[] colors = floorTexture.GetPixels();
		for (var x = 1; x < Level.OUT_SIZE - 1; x++) {
			for (var y = 1; y < Level.OUT_SIZE - 1; y++) {
				texture.SetPixels(x * width, y * height, width, floorTexture.height, colors);
			}
		}

		const int HALF = (Level.OUT_SIZE - 1) / 2;

		colors = wallTexture.GetPixels();
		for (var x = 0; x < Level.OUT_SIZE; x++) {
			if (x == HALF) {
				Color[] doorColors = doorTexture.GetPixels();
				// transponate
				for (var j = 0; j < doorTexture.height - 1; j++) {
					for (var i = j; i < doorTexture.width; i++) {
						Color tmpColor = doorColors[i + j * doorTexture.width];
						doorColors[i + j * doorTexture.width] = doorColors[j + i * doorTexture.width];
						doorColors[j + i * doorTexture.width] = tmpColor;
					}
				}
				texture.SetPixels(x * width, 0, width, height, doorColors);
				texture.SetPixels(x * width, (Level.OUT_SIZE - 1) * height, width, height, doorColors);
			} else {
				var y = 0;
				texture.SetPixels(x * width, y * height, width, height, colors);
				y = Level.OUT_SIZE - 1;
				texture.SetPixels(x * width, y * height, width, height, colors);
			}
		}
		
		for (var y = 0; y < Level.OUT_SIZE; y++) {
			if (y == HALF) {
				Color[] doorColors = doorTexture.GetPixels();
				texture.SetPixels(0, y * height, width, height, doorColors);
				texture.SetPixels((Level.OUT_SIZE - 1) * width, y * height, width, height, doorColors);
			} else {
				var x = 0;
				texture.SetPixels(x * width, y * height, width, height, colors);
				x = Level.OUT_SIZE - 1;
				texture.SetPixels(x * width, y * height, width, height, colors);
			}
		}

		
		texture.Apply(true);
		
		return texture;
	}
	
	Sprite generateRoomSprite(Texture2D texture) {
		return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, texture.width / Level.OUT_SIZE);
	}

	void Update() {
		if (player == null || GameGUI.IsOpen)
			return;
		if (gameController.IsDetonate) {
			//createPlayer(r.EnterPlayerPosition);
			//player.rigidbody2D.velocity = r.EnterPlayerVelosity;
			gameGUI.Life -= 100;
		}
		if (gameGUI.Life <= 0) {
			Destroy(player);
			player = null;
		}

		if (Input.GetKeyDown(KeyCode.Tab)) {
			isOpenGlobalMap = !isOpenGlobalMap;
			if (isOpenGlobalMap) {
				State.Stop();
				float size = Level.OUT_SIZE * WIDTH;
				view.SetViewRange(size);
				view.SetRange(new Rect(
					-Level.OUT_SIZE/2f,
					size-Level.OUT_SIZE/2f,
					size,
					size
					));
			} else {
				CurrentRoom.UpdateRange();
				State.Play();
			}
		}
	}

	public void UnlockRoom() {
		CurrentRoom.Unlock();
		gameGUI.Life += CurrentRoom.Level.cubes.Length;
	}
}
