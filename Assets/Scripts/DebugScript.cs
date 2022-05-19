using UnityEngine;

public class DebugScript : MonoBehaviour
{
	private delegate void Action();

	private void DrawButton(string methodName)
	{
		if (GUILayout.Button(methodName))
		{
			Invoke(methodName, 0);
		}
	}

	private void OnGUI()
	{
		using (new GUILayout.VerticalScope())
		{
			DrawButton(nameof(Mix));
		}
	}

	private void Mix()
	{
		Debug.Log("Test");
	}
}
