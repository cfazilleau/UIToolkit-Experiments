using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public sealed class StepNodeView : Node
{
	public Action<StepNodeView> OnStepViewSelected;
	public readonly Recipe.Step Step;
	public readonly List<Port> Inputs = new();
	public readonly List<Port> Outputs = new();

	private readonly VisualElement _stepInspector;

	public StepNodeView(Recipe.Step step)
	{
		Recipe.StepAttribute stepInfo = Attribute.GetCustomAttribute(step.GetType().GetTypeInfo(), typeof(Recipe.StepAttribute)) as Recipe.StepAttribute;

		if (stepInfo == null)
			return;

		Step = step;
		title = stepInfo.StepName ?? step.name;
		viewDataKey = step.guid;
		capabilities |= Capabilities.Collapsible;

		style.left = step.position.x;
		style.top = step.position.y;

		if (step.GetType().GetMethod(stepInfo.StepMethodName) is { } info)
		{
			ParameterInfo[] parameters = info.GetParameters();

			foreach (ParameterInfo parameterInfo in parameters)
			{
				// ReSharper disable once PossibleMistakenCallToGetType.2
				Type parameterType = parameterInfo.ParameterType.GetType();

				if (parameterInfo.IsOut)
				{
					Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, parameterType);
					port.portName = parameterInfo.Name;
					Outputs.Add(port);
					outputContainer.Add(port);
				}
				else
				{
					Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, parameterType);
					port.portName = parameterInfo.Name;
					Inputs.Add(port);
					inputContainer.Add(port);
				}
			}
		}

		_stepInspector = new IMGUIContainer(OnDetailsInspector);

		_stepInspector.style.flexShrink = new StyleFloat(1f);
		extensionContainer.style.backgroundColor = new StyleColor(new Color(0.18f, 0.18f, 0.18f, 0.80f));
		extensionContainer.style.paddingTop = extensionContainer.style.paddingBottom = extensionContainer.style.paddingLeft = extensionContainer.style.paddingRight =
			new StyleLength(new Length(5, LengthUnit.Pixel));

		extensionContainer.Add(_stepInspector);
		RefreshExpandedState();
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
