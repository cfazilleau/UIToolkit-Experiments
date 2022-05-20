using System;
using System.Collections;
using System.Collections.Generic;
using Recipes.Scriptable;
using Recipes.Scriptable.Steps;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/Recipe")]
public class Recipe : ScriptableObject
{
	public List<Step> steps = new();

#if UNITY_EDITOR
	private void Awake()
	{
		EditorCoroutineUtility.StartCoroutine(CreateResultNode(), this);
	}

	private IEnumerator CreateResultNode()
	{
		// Wait for the asset to be created
		while (!EditorUtility.IsPersistent(this) || !AssetDatabase.Contains(this))
			yield return null;

		// Add ResultNode if it don't exist yet
		if (steps.Find(step => step.GetType() == typeof(ResultStep)) == null)
			CreateStep(typeof(ResultStep));
	}

	public Step CreateStep(Type type, Vector2 nodePosition = default)
	{
		Step step = CreateInstance(type) as Step;
		if (step == null)
			return step;

		step.name = type.Name;
		step._nodeGUID = GUID.Generate().ToString();
		step._nodePosition = nodePosition;

		steps.Add(step);

		AssetDatabase.AddObjectToAsset(step, this);
		AssetDatabase.SaveAssets();

		return step;
	}

	public void DeleteStep(Step step)
	{
		if (step is ResultStep)
			throw new Exception("Can't delete a ResultStep");

		steps.Remove(step);

		AssetDatabase.RemoveObjectFromAsset(step);
		AssetDatabase.SaveAssets();
	}

	public void SaveAsset()
	{
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssetIfDirty(this);
	}
#endif
}
