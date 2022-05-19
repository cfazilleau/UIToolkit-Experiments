namespace Recipes.Scriptable.Steps
{
	[Step("Cook")]
	public class CookStep : Step
	{
		public bool Cook(in Step input, out Step result)
		{
			result = input;
			return true;
		}
	}
}