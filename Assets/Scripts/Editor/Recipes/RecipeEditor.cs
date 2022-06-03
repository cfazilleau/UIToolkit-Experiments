using Editor.Recipes.Views;
using Recipes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
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
		private Button _saveButton;
		private Label _selectRecipeLabel;

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
			_saveButton = root.Query<Button>("SaveButton");
			_selectRecipeLabel = root.Query<Label>("SelectRecipeLabel");

			_recipeEditorView.OnStepViewSelected = OnNodeSelectionChanged;
			_saveButton.clicked += OnClickedSaveButton;

			OnSelectionChange();
		}

		private void OnClickedSaveButton()
		{
			if (_recipeEditorView.targetRecipe != null)
			{
				_recipeEditorView.targetRecipe.SaveAsset();
			}
		}

		private void OnSelectionChange()
		{
			_recipeEditorView.PopulateView(Selection.activeObject as Recipe);
			_selectRecipeLabel.style.display = _recipeEditorView.targetRecipe == null ? DisplayStyle.Flex : DisplayStyle.None;

			if (_recipeEditorView.targetRecipe != null)
			{
				_recipeEditorView.targetRecipe.RebuildTaskList();
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