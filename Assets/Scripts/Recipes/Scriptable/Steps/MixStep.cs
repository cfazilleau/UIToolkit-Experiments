using System.Collections.Generic;

namespace Recipes.Scriptable.Steps
{
	[Step("Mix")]
	public class MixStep : Step
	{
		public bool Mix(in List<Step> ingredients, out Step result)
		{
			result = ingredients[0];
			return true;
		}
	}
}
