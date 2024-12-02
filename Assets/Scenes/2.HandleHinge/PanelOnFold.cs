using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


/// <summary>
/// Example using the OrientationChange event and FoldChange event to toggle an additional panel
/// and change the cameras in the scene to reflect the size and position of the hinge.
/// </summary>
public class PanelOnFold : MonoBehaviour {
	public Camera mainCamera;
	public Camera splitFillCamera;
	public RectTransform panelRect;

	public List<GameObject> disableOnFold;
	public List<GameObject> enableOnFold;

	public bool simulateFoldInEditor;

	private void Awake() {
		ResetCameras();
		ConfigurationManager.ActionOnFoldChange += OnFoldChange;
		ConfigurationManager.ActionOnOrientationChange += OnOrientationChange;
	}

	void ResetCameras() {
		mainCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
		splitFillCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
		splitFillCamera.enabled = false;
		panelRect.gameObject.SetActive(false);
		panelRect.anchorMax = new Vector2(0.0f, 1.0f);
		foreach (var go in disableOnFold) { go.SetActive(true); }
		foreach (var go in enableOnFold) { go.SetActive(false); }
	}

	void OnOrientationChange(ConfigurationManager.OrientationInfo orientationInfo) {
		if ((orientationInfo.rotation != "ROTATION_90" && orientationInfo.rotation != "ROTATION_270") || ConfigurationManager.getFoldableState == "NONE") {
			// Check fold state, if it's unknown then reset the cameras
			ResetCameras();
		}
	}

	void OnFoldChange(ConfigurationManager.FoldInfo foldInfo) {
		// If we are in a separating state and half-opened, split the screen
		if (foldInfo.isSeparating == 1 && foldInfo.orientation == "HINGE_ORIENTATION_HORIZONTAL") {
			float yAnchor = (float)foldInfo.boundsBottom / Screen.height;

			// Resize the main camera to fit in the "top" portion of the screen
			mainCamera.rect = new Rect(0.0f, yAnchor, 1.0f, 1.0f);

			// ...while the panelRect is set on the lower portion of the screen.
			// Since panelRect is set to render as a ScreenSpace-Camera, let the camera rect determine render size
			splitFillCamera.rect = new Rect(0.0f, -yAnchor, 1.0f, 1.0f);
			
			panelRect.gameObject.SetActive(true);
			foreach (var go in disableOnFold) { go.SetActive(false); }
			foreach (var go in enableOnFold) { go.SetActive(true); }

			panelRect.anchorMin = Vector2.zero;
			panelRect.anchorMax = new Vector2(1.0f, 1.0f);
			panelRect.ForceUpdateRectTransforms();

			splitFillCamera.enabled = true;
		} else {
			ResetCameras();
		}
	}

	private void Update() {
		if (Application.isEditor) {
			if (simulateFoldInEditor) {
				OnFoldChange(new ConfigurationManager.FoldInfo() { isSeparating = 1, orientation = "HINGE_ORIENTATION_HORIZONTAL", boundsBottom = Screen.height / 2 });
				simulateFoldInEditor = false;
			}

		}
	}
}