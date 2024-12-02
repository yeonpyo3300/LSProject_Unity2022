using UnityEditor;
using UnityEngine;

public static class LargeScreenHelperTools {
	[MenuItem("Android Large Screen/Add Resizeable Option")]
	private static void AddResizableOption() {
		Debug.Log("Setting resizableWindow in Player settings under Resolution and Presentation");
		PlayerSettings.resizableWindow = true;
		Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
	}
}