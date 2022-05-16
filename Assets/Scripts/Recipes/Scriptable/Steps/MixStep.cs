namespace Recipes.Scriptable.Steps
{
	[Recipe.Step("Mix", "Mix", inputsMin: 2, inputsMax: int.MaxValue)]
	public class MixStep : Recipe.Step
	{
		public bool Mix(in Recipe.Step ingredient1, in Recipe.Step ingredient2, out Recipe.Step result)
		{
			result = ingredient1;
			return true;
		}
	}
}
