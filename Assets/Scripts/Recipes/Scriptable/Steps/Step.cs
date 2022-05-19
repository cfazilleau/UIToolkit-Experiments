using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Recipes.Scriptable.Steps
{
	// Base class for a recipe's step
	public abstract class Step : ScriptableObject
	{
#if UNITY_EDITOR
		// Node graph info
		[SerializeField, HideInInspector] public string guid;
		[SerializeField, HideInInspector] public Vector2 position;
#endif
		[SerializeField] public Step[] outputs = Array.Empty<Step>();
		[SerializeField] public Step[] inputs = Array.Empty<Step>();

		private void Awake()
		{
			if (Attribute.GetCustomAttribute(GetType().GetTypeInfo(), typeof(StepAttribute)) is StepAttribute stepInfo &&
			    GetType().GetMethod(stepInfo.StepName) is { } info)
			{
				ParameterInfo[] parameters = info.GetParameters();
				Array.Resize(ref inputs, parameters.Count(p => !p.IsOut));
				Array.Resize(ref outputs, parameters.Count(p => p.IsOut));
			}
		}
	}

	// Attributes for meta info
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class StepAttribute : Attribute
	{
		public readonly string StepName;

		/// <summary>Step metadata</summary>
		/// <param name="stepName">Name of the step (The process methods needs to be of the same name)</param>
		public StepAttribute(string stepName)
		{
			StepName = stepName;
		}
	}
}
