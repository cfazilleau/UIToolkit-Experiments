using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Recipes;
using Recipes.Steps;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Recipes.Views
{
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

			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(RecipeEditor.RecipeEditorUss);
			styleSheets.Add(styleSheet);
		}

		public void PopulateView(Recipe recipe)
		{
			_recipe = recipe;

			graphViewChanged -= OnGraphViewChanged;
			DeleteElements(graphElements);
			graphViewChanged += OnGraphViewChanged;

			// Create Nodes
			recipe.steps.ForEach(CreateStepNode);

			// Create Edges
			recipe.steps.ForEach(node =>
			{
				StepNodeView nodeView = GetNodeView(node);

				foreach (Step prevStep in node.inputs)
				{
					if (prevStep == null)
						continue;

					StepNodeView prevNode = GetNodeView(prevStep);

					int outputIndex = Array.IndexOf(prevStep.outputs, node);
					int inputIndex = Array.IndexOf(node.inputs, prevStep);

					if (outputIndex < 0 || inputIndex < 0)
					{
						Debug.LogError("One of the Indices was not found");
						continue;
					}

					// Connect ports at the same indexes as Steps
					Port output = prevNode.OutputPorts[outputIndex];
					Port input = nodeView.InputPorts[inputIndex];
					AddElement(output.ConnectTo(input));
				}
			});

			// Frame All
			EditorApplication.delayCall += () => FrameAll();
		}

		private StepNodeView GetNodeView(Step step)
		{
			return GetNodeByGuid(step._nodeGUID) as StepNodeView;
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
					_recipe.DeleteStep(stepView.Step);

				// Delete Edge
				if (elem is Edge edge)
					StepNodeView.DeleteEdge(edge);
			});

			graphViewChange.edgesToCreate?.ForEach(elem =>
			{
				// Create Edge
				StepNodeView.CreateEdge(elem);
			});

			return graphViewChange;
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			// base.BuildContextualMenu(evt);

			// Get Graph-relative mouse position
			Vector2 mousePos = evt.localMousePosition;
			mousePos -= (Vector2)contentViewContainer.transform.position;
			mousePos *= 1 / contentViewContainer.transform.scale.x;

			// Find All Steps
			TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Step>();
			foreach (Type type in types)
			{
				// Ignore if abstract or ResultStep
				if (type.IsAbstract || type == typeof(ResultStep))
					continue;

				StepAttribute recipeStepInfo = Attribute.GetCustomAttribute(type.GetTypeInfo(), typeof(StepAttribute)) as StepAttribute;

				// Ignore if no StepAttribute
				if (recipeStepInfo == null)
					continue;

				// Build Context Menu button
				evt.menu.AppendAction($"Step/{recipeStepInfo.StepName}", _ => CreateStep(type, mousePos));
			}
		}

		private void CreateStep(Type type, Vector2 nodePosition)
		{
			Step step = _recipe.CreateStep(type, nodePosition);
			CreateStepNode(step);
		}

		private void CreateStepNode(Step step)
		{
			StepNodeView stepView = new(step);
			stepView.OnStepViewSelected += OnStepViewSelected;
			AddElement(stepView);
		}
	}
}
