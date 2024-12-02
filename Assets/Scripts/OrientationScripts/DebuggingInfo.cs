using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingInfo : MonoBehaviour {
    public TMPro.TMP_Text labelInfo;
    public TMPro.TMP_Text labelTime;

	private void Awake() {
		SafeZoneUI.OnSafeZoneUpdated += SafeZoneUpdated;
	}

	private void Update() {
		labelTime.text = string.Format("{0:0.0}", Time.realtimeSinceStartup);
	}

	void SafeZoneUpdated(SafeZoneUI whichSafeZone) {
		labelInfo.text = whichSafeZone.GetSafeZoneDebugInfo();
	}
}
