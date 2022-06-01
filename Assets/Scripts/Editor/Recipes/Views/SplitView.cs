using UnityEngine.UIElements;

namespace Editor.Recipes.Views
{
	public class SplitView : TwoPaneSplitView
	{
		public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
	}
}
