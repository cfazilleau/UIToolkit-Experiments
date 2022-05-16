using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
	// Attributes for meta info
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class StepAttribute : Attribute
	{
		public readonly string StepName;
		public readonly string StepMethodName;
		public readonly Vector2Int InputsMinMax;
		public readonly Vector2Int OutputsMinMax;

		/// <summary>
		/// Step metadata
		/// </summary>
		/// <param name="stepName">Name of the step</param>
		/// <param name="stepMethodName">Name of the process method</param>
		/// <param name="inputsMin">Minimum inputs of the step</param>
		/// <param name="inputsMax">Maximum inputs of the step (if inferior than inputsMin, defaults to inputsMin)</param>
		/// <param name="outputsMin">Minimum outputs of the step</param>
		/// <param name="outputsMax">Maximum outputs of the step (if inferior than outputsMin, defaults to outputsMin)</param>
		public StepAttribute(string stepName, string stepMethodName, int inputsMin = 1, int inputsMax = 1, int outputsMin = 1, int outputsMax = 1)
		{
			this.StepName = stepName;
			this.StepMethodName = stepMethodName;
			InputsMinMax = new Vector2Int(inputsMin, Math.Max(inputsMin, inputsMax));
			OutputsMinMax = new Vector2Int(outputsMin, Math.Max(outputsMin, outputsMax));
		}
	}

	// Base class for a recipe's step
	public abstract class Step : ScriptableObject
	{
#if UNITY_EDITOR
		// Node graph info
		[HideInInspector] public string guid;
		[HideInInspector] public Vector2 position;
		[HideInInspector] public Vector2Int inputsMinMax;
		[HideInInspector] public Vector2Int outputsMinMax;
#endif
		[HideInInspector] public Step[] outputs = Array.Empty<Step>();
		[HideInInspector] public Step[] inputs = Array.Empty<Step>();

		private void Awake()
		{
			if (Attribute.GetCustomAttribute(GetType().GetTypeInfo(), typeof(StepAttribute)) is StepAttribute recipeStepInfo)
			{
				inputsMinMax = recipeStepInfo.InputsMinMax;
				outputsMinMax = recipeStepInfo.OutputsMinMax;
			}

			inputs = new Step[inputsMinMax.x];
			outputs = new Step[outputsMinMax.x];
		}
	}

	public List<Step> steps = new();

	public Step CreateStep(Type type)
	{
		Step step = CreateInstance(type) as Step;
		if (step == null)
			return step;

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
