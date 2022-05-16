namespace Recipes.Scriptable.Steps
{
	[Recipe.Step("Cook", "Cook")]
	public class CookStep : Recipe.Step
	{
		public bool Cook(in Recipe.Step input, out Recipe.Step result)
		{
			result = input;
			return true;
		}
	}
}