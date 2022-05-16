namespace Recipes.Scriptable.Steps
{
	[Recipe.Step("Shape", "Shape")]
	public class ShapeStep : Recipe.Step
	{
		public bool Shape(in Recipe.Step input, out Recipe.Step result)
		{
			result = input;
			return true;
		}
	}
}
