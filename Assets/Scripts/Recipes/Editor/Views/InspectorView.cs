using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
	public new class UxmlFactory : UxmlFactory<InspectorView, InspectorView.UxmlTraits> { };

	private Editor editor;

	public InspectorView()
	{
	}

	internal void UpdateSelection(StepNodeView stepNodeView)
	{
		Clear();
		UnityEngine.Object.DestroyImmediate(editor);

		if (stepNodeView != null)
		{
			editor = Editor.CreateEditor(stepNodeView.Step);
			IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
			Add(container);
		}
	}

}
