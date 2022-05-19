using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Recipes.Scriptable.Steps;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Recipes.Editor.Views
{
	public sealed class StepNodeView : Node
	{
		private readonly Step _step;
		private readonly StepAttribute _stepInfo;
		public readonly List<Port> Inputs = new();
		public readonly List<Port> Outputs = new();

		private VisualElement _stepInspector;
		public Action<StepNodeView> OnStepViewSelected;

		public Step Step => _step;

		public StepNodeView(Step step)
		{
			_step = step;
			_stepInfo = Attribute.GetCustomAttribute(_step.GetType().GetTypeInfo(), typeof(StepAttribute)) as StepAttribute;

			if (_stepInfo == null)
			{
				Debug.LogError($"StepAttribute not found on step {_step}, {_step}", _step);
				return;
			}

			title = _stepInfo.StepName ?? step.name;
			viewDataKey = step.guid;
			capabilities |= Capabilities.Collapsible;

			style.left = step.position.x;
			style.top = step.position.y;

			// Clear null trailing references
			List<Step> ins = Step.inputs.ToList();
			for (int i = ins.Count - 1; i >= 0; i--)
			{
				if (ins[i] == null)
					ins.RemoveAt(i);
				else
					break;
			}
			Step.inputs = ins.ToArray();

			CreatePorts();
			CreateParametersPanel();

			RefreshExpandedState();
		}

		private void CreatePorts()
		{
			if (_stepInfo.StepName == null || Step.GetType().GetMethod(_stepInfo.StepName) is not { } info)
				return;

			ParameterInfo[] parameters = info.GetParameters();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				Type parameterType = parameterInfo.ParameterType.GetElementType();

				// If there is only one input parameter and it is a list, put a variable amount of ports.
				if (parameterType != null &&
				    parameters.Count(p => !p.IsOut) == 1 &&
				    parameterInfo.IsOut == false &&
				    parameterType.IsGenericType &&
				    parameterType.GetGenericTypeDefinition() == typeof(List<>) &&
				    parameterType.GenericTypeArguments.Length == 1)
				{
					Type genericType = parameterType.GenericTypeArguments[0];

					// Add All existing ports
					for (int i = 0; i < Step.inputs.Length; i++)
						CreateAndAddPort(i == 0 ? parameterInfo.Name : string.Empty, genericType, parameterInfo.IsOut);

					// Add one more to extend the node
					CreateAndAddPort(string.Empty, genericType, parameterInfo.IsOut);
					Array.Resize(ref Step.inputs, Step.inputs.Length + 1);
				}
				else
				{
					// Create Port and continue
					CreateAndAddPort(parameterInfo.Name, parameterType, parameterInfo.IsOut);
				}
			}
		}

		private void CreateAndAddPort(string portName, Type type, bool isOut)
		{
			// Get out / in parameters
			Direction dir;
			VisualElement container;
			List<Port> portsList;
			if (isOut)
			{
				dir = Direction.Output;
				container = outputContainer;
				portsList = Outputs;
			}
			else
			{
				dir = Direction.Input;
				container = inputContainer;
				portsList = Inputs;
			}

			// Instantiate and reference port
			Port port = InstantiatePort(Orientation.Horizontal, dir, Port.Capacity.Single, type);
			port.portName = portName;
			portsList.Add(port);
			container.Add(port);
		}

		private void CreateParametersPanel()
		{
			// Create parameters
			_stepInspector = new IMGUIContainer(OnDetailsInspector);
			_stepInspector.style.flexShrink = new StyleFloat(1f);

			IStyle exStyle = extensionContainer.style;
			exStyle.backgroundColor = new StyleColor(new Color(0.18f, 0.18f, 0.18f, 0.80f));
			exStyle.paddingTop = exStyle.paddingBottom = exStyle.paddingLeft = exStyle.paddingRight =
				new StyleLength(new Length(5, LengthUnit.Pixel));

			extensionContainer.Add(_stepInspector);
		}

		private void OnDetailsInspector()
		{
			SerializedObject obj = new(Step);
			_stepInspector.SetEnabled(false);
			obj.UpdateIfRequiredOrScript();

			using (new EditorGUI.DisabledScope(true))
			{
				SerializedProperty iterator = obj.GetIterator();
				iterator.NextVisible(true);
				while (iterator.NextVisible(false))
				{
					_stepInspector.SetEnabled(true);
					EditorGUILayout.PropertyField(iterator, true);
				}
			}
		}

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			Step.position = newPos.min;
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
	}
}
