using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class Recipe : ScriptableObject
{
	// Attributes for meta info
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class StepAttribute : Attribute
	{
		public readonly string stepName;
		public readonly Vector2Int inputsMinMax;
		public readonly Vector2Int outputsMinMax;

		/// <summary>
		/// Step metadata
		/// </summary>
		/// <param name="stepName">Name of the step</param>
		/// <param name="inputsMin">Minimum inputs of the step</param>
		/// <param name="inputsMax">Maximum inputs of the step (if inferior than inputsMin, defaults to inputsMin)</param>
		/// <param name="outputsMin">Minimum outputs of the step</param>
		/// <param name="outputsMax">Maximum outputs of the step (if inferior than outputsMin, defaults to outputsMin)</param>
		public StepAttribute(string stepName, int inputsMin = 1, int inputsMax = 1, int outputsMin = 1, int outputsMax = 1)
		{
			this.stepName = stepName;
			inputsMinMax = new Vector2Int(inputsMin, Math.Max(inputsMin, inputsMax));
			outputsMinMax = new Vector2Int(outputsMin, Math.Max(outputsMin, outputsMax));
		}
	}

	// Base class for a recipe's step
	public abstract class Step : ScriptableObject
	{
#if UNITY_EDITOR
		// Node graph info
		[HideInInspector]
		public string guid;
		[HideInInspector]
		public Vector2 position;
		[HideInInspector]
		public Vector2Int inputsMinMax;
		[HideInInspector]
		public Vector2Int outputsMinMax;
#endif

		[SerializeField]
		public Step[] nextSteps = Array.Empty<Step>();
		[SerializeField]
		public Step[] previousSteps = Array.Empty<Step>();

		private void Awake()
		{
			if (Attribute.GetCustomAttribute(GetType().GetTypeInfo(), typeof(StepAttribute)) is StepAttribute recipeStepInfo)
			{
				inputsMinMax = recipeStepInfo.inputsMinMax;
				outputsMinMax = recipeStepInfo.outputsMinMax;
			}

			previousSteps = new Step[inputsMinMax.x];
			nextSteps = new Step[outputsMinMax.x];
		}
	}

	public List<Step> steps = new();

	public Step CreateStep(Type type)
	{
		Step step = ScriptableObject.CreateInstance(type) as Step;
		step.name = type.Name;
		step.guid = GUID.Generate().ToString();
		steps.Add(step);

		AssetDatabase.AddObjectToAsset(step, this);
		AssetDatabase.SaveAssets();
		return step;
	}

	public void DeleteStep(Step step)
	{
		steps.Remove(step);

		AssetDatabase.RemoveObjectFromAsset(step);
		AssetDatabase.SaveAssets();
	}
}
