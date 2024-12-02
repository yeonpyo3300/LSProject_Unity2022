using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NotchAware : MonoBehaviour {
	Texture2D debugTexture;

	private void Awake() {
		debugTexture = new Texture2D(1, 1);
		debugTexture.SetPixel(0, 0, Color.green);
		debugTexture.Apply();

		Debug.Log("Notch cutouts:");
		foreach (var cutout in Screen.cutouts) {
			Debug.LogFormat("{0}", cutout.ToString());
		}
	}

	public List<Rect> NotchInScreenSpace() {
		List<Rect> rects = new List<Rect>();

		foreach (var cutout in Screen.cutouts) {
			rects.Add(new Rect(cutout.x, Screen.height - cutout.y - cutout.height, cutout.width, cutout.height));
		}

		return rects;
	}

	void OnGUI() {
		// Rect coordinates are relative from left top corner
		// Screen.safeArea is using screen space coordinates system(origin (0,0) is at left - bottom corner), so you are getting correct numbers.
		// UICanvas(which uses UI space coordinates system, origin(0, 0) is at left - top corner), so you get things upside down.

		GUI.skin.box.normal.background = debugTexture;
		foreach (var cutout in NotchInScreenSpace()) {
			GUI.Box(cutout, GUIContent.none);
		}
	}
}