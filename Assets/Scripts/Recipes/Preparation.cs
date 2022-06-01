using System.Collections.Generic;
using Recipes.Steps;
using Storage;

namespace Recipes
{
	public class Preparation : Item
	{
		public Recipe selectedRecipe;

		public List<Step> recipeSteps;
	}
}
