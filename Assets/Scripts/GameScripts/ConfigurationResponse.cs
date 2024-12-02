using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationResponse : MonoBehaviour {
    public GameObject rootPauseScreen;
	public TMPro.TMP_Text labelPauseContinue;
	public float autoUnpauseTime = 1.0f;

	private void Awake() {
		UnpauseGame();
	}

	public void PauseGame() {
        rootPauseScreen.SetActive(true);
        Time.timeScale = 0.0f;
		labelPauseContinue.text = "(tap to continue)";
	}

    public void UnpauseGame() {
		rootPauseScreen.SetActive(false);
		Time.timeScale = 1.0f;
	}

	public void ConditionallyPauseGameOnOrientationChange(ConfigurationManager.OrientationInfo config) {
		// Only partially pause (don't require user input) if a configuration change occured, unless player taps sooner
		PauseGame();
		StartCoroutine(UnpauseInTime(autoUnpauseTime));
	}

	IEnumerator UnpauseInTime(float time) {
		while (time > 0.0f && rootPauseScreen.activeInHierarchy) {
			time -= Time.unscaledDeltaTime;
			labelPauseContinue.text = string.Format("(game continues in {0:0.0}...)", time);
			yield return null;
		}

		UnpauseGame();
	}

	public void ConditionallyPauseGameOnFoldChange(ConfigurationManager.FoldInfo foldInfo) {
		// Set pause if we changed from folded to unfolded configurations
		PauseGame();
	}
}