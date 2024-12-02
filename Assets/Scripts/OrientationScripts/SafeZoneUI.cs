using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeZoneUI : MonoBehaviour {
	private RectTransform rectTransform;
	private Rect safeArea = new Rect();
	private Vector2 anchorMin = Vector2.zero;
	private Vector2 anchorMax = Vector2.zero;

	public static Action<SafeZoneUI> OnSafeZoneUpdated;

	private void Start() {
		rectTransform = GetComponent<RectTransform>();
		if (null == rectTransform) {
			enabled = false;
			Debug.LogWarningFormat(gameObject, "SafeZoneUI needs a Panel to resize properly.  None found on object {0}, so this component will be disabled.", name);
			return;
		}

		ConfigurationManager.ActionOnOrientationChange += ApplySafeZone;
		ApplySafeZone(null);
	}

	public void ApplySafeZone(ConfigurationManager.OrientationInfo _) {
		safeArea = Screen.safeArea;
		anchorMin = safeArea.position;
		anchorMax = safeArea.position + safeArea.size;

		if (Screen.width > 0 && Screen.height > 0) {
			anchorMin.x /= Screen.width;
			anchorMin.y /= Screen.height;
			anchorMax.x /= Screen.width;
			anchorMax.y /= Screen.height;

			if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0) {
				rectTransform.anchorMin = anchorMin;
				rectTransform.anchorMax = anchorMax;
			}
		}

		OnSafeZoneUpdated?.Invoke(this);
	}

	public string GetSafeZoneDebugInfo() {
		return string.Format("[{8:0.00}] SafeZoneUI updated on GameObject {0}\n   screen width/height at {1}/{2}\n   anchorMin/Max at {3}/{4}\n   safe area width/height {5}/{6}\n   original safeArea:\n    {7}", gameObject.name, safeArea.width, safeArea.height, anchorMin, anchorMax, Screen.width, Screen.height, safeArea, Time.realtimeSinceStartup);
	}
}