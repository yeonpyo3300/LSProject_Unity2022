using System;
using UnityEngine;

public class HingeSensor : IDisposable {
	AndroidJavaObject plugin;

	/// <summary>
	/// Only get an object when the plugin is created
	/// </summary>
	HingeSensor(AndroidJavaObject sensorPlugin) {
		plugin = sensorPlugin;
	}
	/// <summary>
	/// Create an object to read the hinge sensor
	/// </summary>
	public static HingeSensor Start() {
		if (Application.platform == RuntimePlatform.Android) {
			var sensor = OnPlayer.Run(p => {
				var context = p.GetStatic<AndroidJavaObject>("currentActivity");

				var plugin = new AndroidJavaClass("com.unity.lostcryptlargescreenexample.HingeAngleSensor")
						.CallStatic<AndroidJavaObject>("getInstance", context);

				if (plugin != null) {
					plugin.Call("setupSensor");
					return new HingeSensor(plugin);
				} else {
					return null;
				}
			});

			return sensor;
		} else {
			return null;
		}
	}

	/// <summary>
	/// Get the angle between the two screens
	/// </summary>
	/// <returns>0 to 360 (closed to fully opened), or -1 if error</returns>
	public float GetHingeAngle() {
		return (Application.platform == RuntimePlatform.Android && plugin != null) ? plugin.Call<float>("getHingeAngle") : -1.0f;
	}

	public bool IsHingeEnabled() {
		return Application.platform == RuntimePlatform.Android && plugin != null && plugin.Call<int>("isHingeEnabled") > 0;
	}

	public void StopSensing() {
		if (plugin != null) {
			plugin.Call("dispose");
			plugin = null;
		}
	}
	public void Dispose() {
		if (plugin != null) {
			plugin.Call("dispose");
			plugin = null;
		}
	}
}

internal class OnPlayer {
	public static T Run<T>(Func<AndroidJavaClass, T> runner) {
		if (runner == null)
			throw new ArgumentNullException(nameof(runner));

		using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			return runner(player);
		}
	}
}