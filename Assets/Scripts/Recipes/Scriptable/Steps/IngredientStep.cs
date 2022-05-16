using UnityEngine;

namespace Recipes.Scriptable.Steps
{
	[Recipe.Step("Ingredient", "Ingredient", 0, 0)]
	public class IngredientStep : Recipe.Step
	{
		[SerializeField]
		private Ingredient ingredient = null;

		[SerializeField]
		private int quantity = 50;

		public bool Ingredient(out bool truc)
		{
			truc = false;
			return false;
		}
	}
}
