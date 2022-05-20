using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Recipes.Scriptable;
using Recipes.Scriptable.Steps;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Recipes.Editor
{
	public sealed class StepNodeView : Node
	{
		private readonly Step _step;
		public readonly List<Port> InputPorts = new();
		public readonly List<Port> OutputPorts = new();

		private IMGUIContainer _stepInspector;
		public Action<StepNodeView> OnStepViewSelected;

		public Step Step => _step;

		public StepNodeView(Step step) : base("Assets/Scripts/Recipes/Editor/RecipeNode.uxml")
		{
			_step = step;

			title = step.StepTitle ?? step.StepInfo.StepName ?? step.name;
			viewDataKey = step._nodeGUID;

			style.left = step._nodePosition.x;
			style.top = step._nodePosition.y;

			Step.ClearNullReferencesIfInputList();

			CreatePorts();
			CreateNodeDetails();

			RefreshExpandedState();
		}

		private void CreatePorts()
		{
			// If input is a List, handle List ports
			if (_step.InputSingleAndList)
			{
				ParameterInfo parameterInfo = _step.inputInfos[0];
				Type genericType = parameterInfo.ParameterType.GetElementType()?.GenericTypeArguments[0];

				// Only first port has a name
				CreateAndAddPort(parameterInfo.Name, genericType, parameterInfo.IsOut);

				// Add All existing ports
				for (int i = 1; i < Step.Inputs.Length; i++)
					CreateAndAddPort(string.Empty, genericType, parameterInfo.IsOut);
			}
			else
			{
				// Create Input ports
				foreach (ParameterInfo info in _step.inputInfos)
					CreateAndAddPort(info);
			}

			// Create Output ports
			foreach (ParameterInfo info in _step.outputInfos)
				CreateAndAddPort(info);
		}

		private void CreateAndAddPort(ParameterInfo info)
		{
			CreateAndAddPort(info.Name, info.ParameterType.GetElementType(), info.IsOut);
		}

		private void CreateAndAddPort(string portName, Type type, bool isOut)
		{
			// Instantiate and reference port
			Port port = InstantiatePort(0, isOut ? Direction.Output : Direction.Input, 0, type);
			port.portName = portName;

			// Add to corresponding list and container
			if (isOut)
			{
				OutputPorts.Add(port);
				outputContainer.Add(port);
			}
			else
			{
				InputPorts.Add(port);
				inputContainer.Add(port);
			}
		}

		private void CreateNodeDetails()
		{
			// Create parameters
			_stepInspector = mainContainer.Q<IMGUIContainer>();
			_stepInspector.onGUIHandler = OnNodeDetails;
		}

		private void OnNodeDetails()
		{
			SerializedObject obj = new(Step);
			float nodeWidth = extensionContainer.contentRect.width;

			obj.UpdateIfRequiredOrScript();

			using (new EditorGUI.DisabledScope(true))
			{
				SerializedProperty iterator = obj.GetIterator();
				iterator.NextVisible(true);

				while (iterator.NextVisible(false))
				{
					EditorGUIUtility.labelWidth = nodeWidth * 0.4f;
					EditorGUILayout.PropertyField(iterator, true, GUILayout.MaxWidth(nodeWidth));
				}
			}
		}

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			Step._nodePosition = newPos.min;
		}

		public override void OnSelected()
		{
			base.OnSelected();
			OnStepViewSelected?.Invoke(this);
		}

		public override void OnUnselected()
		{
			base.OnUnselected();
			OnStepViewSelected?.Invoke(null);
		}

		public static void CreateEdge(Edge edge)
		{
			if (edge.input.node is StepNodeView endNode &&
			    edge.output.node is StepNodeView startNode)
			{
				int outputIndex = startNode.OutputPorts.IndexOf(edge.output);
				int inputIndex = endNode.InputPorts.IndexOf(edge.input);

				startNode.Step.Outputs[outputIndex] = endNode.Step;
				endNode.Step.Inputs[inputIndex] = startNode.Step;
			}
		}

		public static void DeleteEdge(Edge edge)
		{
			if (edge.input.node is StepNodeView endNode &&
			    edge.output.node is StepNodeView startNode)
			{
				int outputIndex = startNode.OutputPorts.IndexOf(edge.output);
				int inputIndex = endNode.InputPorts.IndexOf(edge.input);

				startNode.Step.Outputs[outputIndex] = null;
				endNode.Step.Inputs[inputIndex] = null;
			}
		}
	}
}
