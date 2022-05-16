using UnityEditor;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
	public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { };

	private Editor _editor;

	internal void UpdateSelection(StepNodeView stepNodeView)
	{
		Clear();
		UnityEngine.Object.DestroyImmediate(_editor);

		if (stepNodeView == null)
			return;

		_editor = Editor.CreateEditor(stepNodeView.Step);
		IMGUIContainer container = new(() => { _editor.OnInspectorGUI(); });
		Add(container);
	}

}
