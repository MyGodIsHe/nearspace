using UnityEngine;
using System;
using System.Collections.Generic;


public struct Level {

	public const int SIZE = 13;
	public const int OUT_SIZE = SIZE + 2;
	
	public LevelObject[] cubes;

	public Vector2 size;

	public Directions directions;

	string name;
	string filename;

	public string Name {
		get {
			return name;
		}
	}
	
	public Level(string name) {
		directions = Directions.All;
		string[] lines = (Resources.Load(name, typeof(TextAsset)) as TextAsset).text.Split('\n');
		int y = 0;
		int maxX = 0;
		List<LevelObject> cubes = new List<LevelObject>();
		foreach (string line in lines) {
			int x = 0;
			foreach (char c in line) {
				ObjectType type = ObjectType.None;
				switch (c) {
				case '#':
					type = ObjectType.Cube;
					break;
				case '+':
					type = ObjectType.StopCube;
					break;
				case 'x':
					type = ObjectType.Key;
					break;
				}
				if (type != ObjectType.None) {
					cubes.Add(new LevelObject(new Vector2(x, y), type));
				}
				x++;
			}
			maxX = Mathf.Max(maxX, x);
			y++;
		}
		size = new Vector2(maxX, y);
		if (size.x != size.y && size.x != SIZE)
			Debug.Log("Bad Size " + name);
		this.cubes = cubes.ToArray();
		filename = name;
		this.name = doName(filename, "1");
	}
	
	static string doName(string filename, string transposition) {
		string alph = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		return alph[int.Parse (filename)] + "#" + transposition;
	}

	public void Transposition() {
		for (int i = 0; i < cubes.Length; i++)
			cubes[i].position = new Vector2(cubes[i].position.y, cubes[i].position.x);
		size = new Vector2(size.y, size.x);
	}
	
	public void VerticalReflection() {
		for (int i = 0; i < cubes.Length; i++)
			cubes[i].position = new Vector2((size.x - 1) - cubes[i].position.x, cubes[i].position.y);
	}
	
	public void HorizontalReflection() {
		for (int i = 0; i < cubes.Length; i++)
			cubes[i].position = new Vector2(cubes[i].position.x, (size.y - 1) - cubes[i].position.y);
	}
	
	public void PointReflection() {
		for (int i = 0; i < cubes.Length; i++)
			cubes[i].position = new Vector2((size.x - 1) - cubes[i].position.x, (size.y - 1) - cubes[i].position.y);
	}

	public void RandomTranform() {
		int value = UnityEngine.Random.Range(1, 8);
		name = doName(filename, value.ToString());
		switch (value) {
		case 2:
			HorizontalReflection();
			break;
		case 3:
			VerticalReflection();
			break;
		case 4:
			PointReflection();
			break;
		case 5:
			Transposition();
			break;
		case 6:
			Transposition();
			HorizontalReflection();
			break;
		case 7:
			Transposition();
			VerticalReflection();
			break;
		case 8:
			Transposition();
			PointReflection();
			break;
		}
	}
}

public struct LevelObject {
	public Vector2 position;
	public ObjectType type;
	
	public LevelObject(Vector2 position, ObjectType type) {
		this.position = position;
		this.type = type;
	}
}

public enum ObjectType {
	None,
	Cube,
	StopCube,
	Key
}

[Flags]
public enum Directions {
	None = 0x00,
	Left = 0x01,
	Right = 0x02,
	Up = 0x04,
	Down = 0x08,
	All = 0x01+0x02+0x04+0x08
}