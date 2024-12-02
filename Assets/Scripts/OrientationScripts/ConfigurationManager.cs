using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Events;

public class ConfigurationManager : MonoBehaviour {
	public static Action<OrientationInfo> ActionOnOrientationChange;
	public static Action<FoldInfo> ActionOnFoldChange;

	public static ConfigurationManager Instance { get; private set; } = null;
	static AndroidJavaObject foldablePlayerActivity = null;
	static AndroidJavaObject windowMetricsCalculatorObject = null;
	public HingeSensor hingeSensor = null;

	// For any scene/inspector based interest in responding to events
	public UnityEvent<OrientationInfo> OnConfigurationChanged;
	public UnityEvent<FoldInfo> OnFoldChanged;

	// To use the JsonUtility.FromJson, we start with a serializable struct or class with public fields
	[System.Serializable]
	public class OrientationInfo {
		public string rotation;
		public string orientation;
		public int screenWidth;
		public int screenHeight;
		public int visibleFrameLeft;
		public int visibleFrameRight;
		public int visibleFrameTop;
		public int visibleFrameBottom;
	}

	[System.Serializable]
	public class FoldInfo {
		public string orientation;
		public string state;
		public int isSeparating;
		public int boundsLeft;
		public int boundsTop;
		public int boundsRight;
		public int boundsBottom;
	}

	void Awake() {
		Instance = this;

		// Grab a copy of the Android objects that we might reference for foldable activity or hinge info
		// There are only valid when we interface with our LargeScreenPlayableActivity.java class, and this is only loaded on Android builds
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			foldablePlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
			var staticCalcClass = new AndroidJavaClass("androidx.window.layout.WindowMetricsCalculator");
			windowMetricsCalculatorObject = staticCalcClass.CallStatic<AndroidJavaObject>("getOrCreate");
			hingeSensor = HingeSensor.Start();
		}

		ActionOnOrientationChange += HandleLocalOnConfigurationChanged;
		ActionOnFoldChange += HandleLocalOnFoldChanged;
	}

	private void OnDestroy() {
		Instance = null;
		if (Application.platform == RuntimePlatform.Android) {
			if (null != hingeSensor) {
				hingeSensor.Dispose();
				hingeSensor = null;
			}
		}

		ActionOnOrientationChange -= HandleLocalOnConfigurationChanged;
		ActionOnFoldChange -= HandleLocalOnFoldChanged;
	}

	// This will be called from the OverrideForLargeScreen.java class, from the activity callback onConfigurationChanged
	public void onConfigurationChanged(string strOrientationInfo) {
		OrientationInfo info = JsonUtility.FromJson<OrientationInfo>(strOrientationInfo);
		Debug.LogFormat(string.Format("orientation: {1}, rotation: {2}\n   width/height: {3}/{4}\n   l/r/t/b: {5}/{6}/{7}/{8}",
			0, info.orientation, info.rotation, info.screenWidth, info.screenHeight, info.visibleFrameLeft, info.visibleFrameRight, info.visibleFrameTop, info.visibleFrameBottom));

		// Always call the refresh from the main Unity thread, since this is where the UI updates occur
		StartCoroutine(ExecuteOnMainUnityThread(ActionOnOrientationChange, info));
	}

	public void onFoldChanged(string strFoldInfo) {
		FoldInfo info = JsonUtility.FromJson<FoldInfo>(strFoldInfo);
		Debug.LogFormat(string.Format("orientation: {0}, state: {1}, isSeparating: {2}\n   l/r/t/b: {3}/{4}/{5}/{6}",
			info.orientation, info.state, info.isSeparating, info.boundsLeft, info.boundsRight, info.boundsTop, info.boundsBottom));

		// Always call the refresh from the main Unity thread, since this is where the UI updates occur
		StartCoroutine(ExecuteOnMainUnityThread(ActionOnFoldChange, info));
	}

	public static string getFoldableState {
		get {
			if (Application.platform == RuntimePlatform.Android) {
				var foldingFeatureObject = foldablePlayerActivity.Call<AndroidJavaObject>("getFoldingFeature");
				if (foldingFeatureObject == null) {
					Debug.Log("[FoldableState] Returning NONE");
					return "NONE";
				} else {
					var state = foldingFeatureObject.Call<AndroidJavaObject>("getState");
					var stateString = state.Call<string>("toString");
					Debug.LogFormat("[FoldableState] Returning {0}", stateString);
					return stateString;
				}
			} else {
				return "NONE";
			}
		}
	}

	IEnumerator ExecuteOnMainUnityThread<T>(Action<T> whichAction, T data) {
		yield return null;  // Will be called from main thread on next update
		whichAction.Invoke(data);
	}

	void HandleLocalOnConfigurationChanged(OrientationInfo info) {
		OnConfigurationChanged?.Invoke(info);
	}
	void HandleLocalOnFoldChanged(FoldInfo info) {
		OnFoldChanged?.Invoke(info);
	}
}