using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class StepNodeView : Node
{
	public Action<StepNodeView> OnStepViewSelected;
	public Recipe.Step Step;
	public List<Port> inputs = new();
	public List<Port> outputs = new();

	public StepNodeView(Recipe.Step step)
	{
		Recipe.StepAttribute stepInfo = Attribute.GetCustomAttribute(step.GetType().GetTypeInfo(), typeof(Recipe.StepAttribute)) as Recipe.StepAttribute;

		this.Step = step;
		this.title = stepInfo?.stepName ?? step.name;
		this.viewDataKey = step.guid;

		style.left = step.position.x;
		style.top = step.position.y;

		CreateInputPorts();
		CreateOutputPorts();
	}

	private void CreateOutputPorts()
	{
		foreach (Recipe.Step step in Step.previousSteps)
		{
			Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
			inputs.Add(input);
			inputContainer.Add(input);
		}
	}

	private void CreateInputPorts()
	{
		foreach (Recipe.Step step in Step.nextSteps)
		{
			Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
			outputs.Add(output);
			outputContainer.Add(output);

			if (step != null)
			{
				// Get StepNodeView of step
				// Find input node connected to this current Node
				// Connect it
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
