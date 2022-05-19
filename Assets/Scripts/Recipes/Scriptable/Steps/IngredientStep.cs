using UnityEngine;

namespace Recipes.Scriptable.Steps
{
	[Step("Ingredient")]
	public class IngredientStep : Step
	{
		[SerializeField]
		private Ingredient ingredient = null;

		[SerializeField]
		private int quantity = 50;

		public bool Ingredient(out IngredientStep ingredient)
		{
			ingredient = this;
			return false;
		}
	}
}
