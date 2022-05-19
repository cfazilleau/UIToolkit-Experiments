using System;
using Recipes.Editor.Views;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Recipes.Editor
{
	public class RecipeEditor : EditorWindow
	{
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
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Recipes/Editor/RecipeEditor.uxml");
			visualTree.CloneTree(root);

			// A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Recipes/Editor/RecipeEditor.uss");
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