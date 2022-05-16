using UnityEngine.UIElements;

namespace Recipes.Editor.Views
{
	public class SplitView : TwoPaneSplitView
	{
		public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
	}
}
