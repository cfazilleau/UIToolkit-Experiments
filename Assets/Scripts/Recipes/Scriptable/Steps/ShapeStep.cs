namespace Recipes.Scriptable.Steps
{
	[Step("Shape")]
	public class ShapeStep : Step
	{
		public bool Shape(in Step input, out Step result)
		{
			result = input;
			return true;
		}
	}
}
