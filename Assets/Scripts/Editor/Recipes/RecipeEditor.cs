using Editor.Recipes.Views;
using Recipes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Recipes
{
	public class RecipeEditor : EditorWindow
	{
		internal const string RecipeEditorUxml = "Assets/UI/Editor/RecipeEditor.uxml";
		internal const string RecipeEditorUss = "Assets/UI/Editor/RecipeEditor.uss";
		internal const string RecipeEditorNodeUxml = "Assets/UI/Editor/RecipeNode.uxml";
		internal const string RecipeEditorGraphUss = "Assets/UI/Editor/RecipeNode.uss";

		private RecipeEditorView _recipeEditorView;
		private InspectorView _inspectorView;

		[MenuItem("Window/RecipeEditor")]
		public static void OpenWindow()
		{
			RecipeEditor wnd = GetWindow<RecipeEditor>();
			wnd.titleContent = new GUIContent("Recipe Editor", EditorGUIUtility.IconContent("TreeEditor.Distribution On").image);
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// Import UXML
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(RecipeEditorUxml);
			visualTree.CloneTree(root);

			// A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(RecipeEditorUss);
			root.styleSheets.Add(styleSheet);

			_recipeEditorView = root.Query<RecipeEditorView>();
			_inspectorView = root.Query<InspectorView>();
			_recipeEditorView.OnStepViewSelected = OnNodeSelectionChanged;

			OnSelectionChange();
		}

		private void OnSelectionChange()
		{
			if (Selection.activeObject is Recipe recipe)
			{
				_recipeEditorView.PopulateView(recipe);
				recipe.RebuildTaskList();
			}
		}

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceId, int line)
		{
			if (Selection.activeObject is Recipe)
			{
				OpenWindow();
				return true;
			}
			return false;
		}

		private void OnNodeSelectionChanged(StepNodeView stepNodeView)
		{
			_inspectorView.UpdateSelection(stepNodeView);
		}
	}
}