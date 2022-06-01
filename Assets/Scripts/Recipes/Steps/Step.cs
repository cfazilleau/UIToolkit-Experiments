using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Recipes.Steps
{
	// Base class for a recipe's step
	public abstract class Step : ScriptableObject
	{
#if UNITY_EDITOR
		// Node graph info
		[SerializeField, HideInInspector] public string _nodeGUID;
		[SerializeField, HideInInspector] public Vector2 _nodePosition;
#endif
		[SerializeField, HideInInspector] public Step[] outputs = Array.Empty<Step>();
		[SerializeField, HideInInspector] public Step[] inputs = Array.Empty<Step>();
		
		private StepAttribute _stepInfo;
		private MethodInfo _stepMethodInfo;
		public readonly List<ParameterInfo> InputInfos = new();
		public readonly List<ParameterInfo> OutputInfos = new();

		public virtual string StepTitle => _stepInfo?.StepName ?? name;
		public virtual string InstructionLine => GetType().Name;

		// Is there only one input parameter and is it a List
		public bool InputSingleAndList => InputInfos.Count == 1 &&
		                                  InputInfos[0] is { } info &&
		                                  info.ParameterType.GetElementType() is { IsGenericType: true } type &&
		                                  type.GetGenericTypeDefinition() == typeof(List<>) &&
		                                  type.GenericTypeArguments.Length == 1;

		protected Step()
		{
			RefreshStepInfo();

			if (!InputSingleAndList)
				Array.Resize(ref inputs, InputInfos.Count);

			Array.Resize(ref outputs, OutputInfos.Count);
		}

		private void RefreshStepInfo()
		{
			// Get StepInfo
			_stepInfo = Attribute.GetCustomAttribute(GetType().GetTypeInfo(), typeof(StepAttribute)) as StepAttribute;
			if (_stepInfo == null)
				throw new Exception($"StepAttribute not found on step {name}");

			// Get MethodInfo
			_stepMethodInfo = GetType().GetMethod(_stepInfo.StepName);
			if (_stepMethodInfo == null)
				throw new Exception($"Step Method {_stepInfo.StepName} not found in step {name}");

			// Get Parameter types
			InputInfos.Clear();
			OutputInfos.Clear();
			foreach (ParameterInfo parameter in _stepMethodInfo.GetParameters())
			{
				if (parameter.IsOut)
					OutputInfos.Add(parameter);
				else
					InputInfos.Add(parameter);
			}
		}

		public void ClearNullReferencesIfInputList()
		{
			if (InputSingleAndList)
			{
				// Clear null trailing references in the list
				List<Step> ins = inputs.ToList();
				for (int i = ins.Count - 1; i >= 0; i--)
				{
					if (ins[i] == null)
						ins.RemoveAt(i);
					else
						break;
				}
				// Always add a free port at the end
				ins.Add(null);
				inputs = ins.ToArray();
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
