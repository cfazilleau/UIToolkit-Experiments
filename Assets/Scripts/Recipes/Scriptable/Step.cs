using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Recipes.Scriptable
{
	// Base class for a recipe's step
	public abstract class Step : ScriptableObject
	{
#if UNITY_EDITOR
		// Node graph info
		[SerializeField, HideInInspector] public string _nodeGUID;
		[SerializeField, HideInInspector] public Vector2 _nodePosition;
#endif
		[SerializeField, HideInInspector] public Step[] Outputs = Array.Empty<Step>();
		[SerializeField, HideInInspector] public Step[] Inputs = Array.Empty<Step>();

		public List<ParameterInfo> inputInfos = new();
		public List<ParameterInfo> outputInfos = new();

		public StepAttribute StepInfo;
		public MethodInfo StepMethodInfo;

		public virtual string StepTitle => null;

		// Is there only one input parameter and is it a List
		public bool InputSingleAndList => inputInfos.Count == 1 &&
		                                  inputInfos[0] is { } info &&
		                                  info.ParameterType.GetElementType() is { IsGenericType: true } type &&
		                                  type.GetGenericTypeDefinition() == typeof(List<>) &&
		                                  type.GenericTypeArguments.Length == 1;

		public Step()
		{
			RefreshStepInfo();

			if (StepMethodInfo != null)
			{
				if (!InputSingleAndList)
				{

					Array.Resize(ref Inputs, inputInfos.Count);
				}

				Array.Resize(ref Outputs, outputInfos.Count);
			}
		}

		private void RefreshStepInfo()
		{
			// Get StepInfo
			StepInfo = Attribute.GetCustomAttribute(GetType().GetTypeInfo(), typeof(StepAttribute)) as StepAttribute;
			if (StepInfo == null)
				throw new Exception($"StepAttribute not found on step {name}");

			// Get MethodInfo
			StepMethodInfo = GetType().GetMethod(StepInfo.StepName);
			if (StepMethodInfo == null)
				throw new Exception($"Step Method {StepInfo.StepName} not found in step {name}");

			// Get Parameter types
			inputInfos.Clear();
			outputInfos.Clear();
			foreach (ParameterInfo parameter in StepMethodInfo.GetParameters())
			{
				if (parameter.IsOut)
					outputInfos.Add(parameter);
				else
					inputInfos.Add(parameter);
			}
		}

		public void ClearNullReferencesIfInputList()
		{
			if (InputSingleAndList)
			{
				// Clear null trailing references in the list
				List<Step> ins = Inputs.ToList();
				for (int i = ins.Count - 1; i >= 0; i--)
				{
					if (ins[i] == null)
						ins.RemoveAt(i);
					else
						break;
				}
				// Always add a free port at the end
				ins.Add(null);
				Inputs = ins.ToArray();
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
