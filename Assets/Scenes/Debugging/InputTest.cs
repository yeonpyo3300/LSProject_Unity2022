using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    public TMPro.TMP_Text labelDebug;

	private void Update() {
		if (Input.GetAxis("Fire1") > 0.0f) {
			labelDebug.text = string.Format("[{0:0.00}] Button 1 pressed", Time.realtimeSinceStartup);
		}
	}
}
