using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

public class RecipeEditorView : GraphView
{
	public Action<StepNodeView> OnStepViewSelected;

	public new class UxmlFactory : UxmlFactory<RecipeEditorView, UxmlTraits> { };

	private Recipe _recipe;

	public RecipeEditorView()
	{
		Insert(0, new GridBackground());

		this.AddManipulator(new ContentZoomer());
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());

		var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Recipes/Editor/RecipeEditor.uss");
		styleSheets.Add(styleSheet);
	}

	public void PopulateView(Recipe recipe)
	{
		this._recipe = recipe;

		graphViewChanged -= OnGraphViewChanged;
		DeleteElements(graphElements);
		graphViewChanged += OnGraphViewChanged;

		// Create Nodes
		recipe.steps.ForEach(CreateStepNode);

		// Create Edges
		recipe.steps.ForEach(node =>
		{
			StepNodeView nodeView = GetNodeView(node);

			foreach (Recipe.Step nextStep in node.outputs)
			{
				if (nextStep == null)
					continue;

				int outputIndex = Array.IndexOf(node.outputs, nextStep);
				int inputIndex = Array.IndexOf(nextStep.inputs, node);

				// Connect ports at the same indexes as Steps
				Edge edge = nodeView.Outputs[outputIndex].ConnectTo(GetNodeView(nextStep).Inputs[inputIndex]);
				AddElement(edge);
			}
		});
	}

	private StepNodeView GetNodeView(Recipe.Step step)
	{
		return GetNodeByGuid(step.guid) as StepNodeView;
	}

	public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
	{
		return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
	}

	private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
	{
		graphViewChange.elementsToRemove?.ForEach(elem => {
			// Delete Step (will nullify references)
			if (elem is StepNodeView stepView)
			{
				_recipe.DeleteStep(stepView.Step);
			}

			// Delete Edge
			if (elem is Edge edge)
			{
				if (edge.input.node is StepNodeView endNode &&
				    edge.output.node is StepNodeView startNode)
				{
					int outputIndex = startNode.Outputs.IndexOf(edge.output);
					startNode.Step.outputs[outputIndex] = null;

					int inputIndex = endNode.Inputs.IndexOf(edge.input);
					endNode.Step.inputs[inputIndex] = null;
				}
			}
		});

		graphViewChange.edgesToCreate?.ForEach(edge => {
			// Create Edge connections
			if (edge.input.node is StepNodeView endNode &&
			    edge.output.node is StepNodeView startNode)
			{
				int outputIndex = startNode.Outputs.IndexOf(edge.output);
				startNode.Step.outputs[outputIndex] = endNode.Step;

				int inputIndex = endNode.Inputs.IndexOf(edge.input);
				endNode.Step.inputs[inputIndex] = startNode.Step;
			}
		});

		return graphViewChange;
	}

	public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		//base.BuildContextualMenu(evt);

		TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Recipe.Step>();
		foreach (Type type in types)
		{
			if (type.IsAbstract)
				continue;

			Recipe.StepAttribute recipeStepInfo = Attribute.GetCustomAttribute(type.GetTypeInfo(), typeof(Recipe.StepAttribute)) as Recipe.StepAttribute;

			if (recipeStepInfo == null)
				continue;

			evt.menu.AppendAction($"Step/{recipeStepInfo.StepName}", _ => CreateStep(type));
		}
	}

	private void CreateStep(Type type)
	{
		Recipe.Step step = _recipe.CreateStep(type);
		CreateStepNode(step);
	}

	private void CreateStepNode(Recipe.Step step)
	{
		StepNodeView stepView = new(step);
		stepView.OnStepViewSelected += OnStepViewSelected;
		AddElement(stepView);
	}
}