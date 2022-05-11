using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class RecipeEditor : EditorWindow
{
	RecipeEditorView recipeEditorView;
	InspectorView inspectorView;

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
		var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Recipes/Editor/RecipeEditor.uxml");
		visualTree.CloneTree(root);

		// A stylesheet can be added to a VisualElement.
		// The style will be applied to the VisualElement and all of its children.
		var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Recipes/Editor/RecipeEditor.uss");
		root.styleSheets.Add(styleSheet);

		recipeEditorView = root.Query<RecipeEditorView>();
		inspectorView = root.Query<InspectorView>();
		recipeEditorView.OnStepViewSelected = OnNodeSelectionChanged;

		OnSelectionChange();
	}

	private void OnSelectionChange()
	{
		if (Selection.activeObject is Recipe recipe)
		{
			recipeEditorView.PopulateView(recipe);
		}
	}

	private void OnNodeSelectionChanged(StepNodeView stepNodeView)
	{
		inspectorView.UpdateSelection(stepNodeView);
	}
}