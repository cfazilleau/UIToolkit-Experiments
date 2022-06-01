namespace Recipes.Steps
{
	[Step("Shape")]
	public class ShapeStep : Step
	{
		public bool Shape(in Preparation input, out Preparation result)
		{
			result = input;
			return true;
		}
	}
}
