using UnityEngine;

namespace Recipes.Steps
{
	[Step("Ingredient")]
	public class IngredientStep : Step
	{

		[SerializeField]
		public Ingredient ingredient;

		[SerializeField]
		private int quantity = 50;

		public override string StepTitle => ingredient != null ? $"{ingredient.name} (Ingredient)" : null;

		public bool Ingredient(out Preparation preparation)
		{
			preparation = CreateInstance<Preparation>();
			return false;
		}
	}
}
