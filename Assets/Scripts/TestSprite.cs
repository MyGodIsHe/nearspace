using UnityEngine;
using System.Collections;

public class TestSprite : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);

		texture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.5f));
		texture.SetPixel(1, 0, Color.clear);
		texture.SetPixel(0, 1, Color.white);
		texture.SetPixel(1, 1, Color.black);
		
		Color[] colors = texture.GetPixels(0, 0, 2, 2);
		texture.SetPixels(2, 2, 2, 2, colors);

		texture.filterMode = FilterMode.Point;
		texture.Apply();

		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
	
		transform.localScale = Vector3.one * 200;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
