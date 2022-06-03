using System.Collections.Generic;

namespace Recipes.Steps
{
	[Step("Mix")]
	public class MixStep : Step
	{
		public bool Mix(in List<Preparation> ingredients, out Preparation result)
		{
			result = ingredients[0];
			return true;
		}
	}
}
