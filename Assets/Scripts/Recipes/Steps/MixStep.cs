using System.Collections.Generic;

namespace Recipes.Steps
{
	[Step("Mix")]
	public class MixStep : Step
	{
		public override string InstructionLine => GetInstruction();

		public bool Mix(in List<Preparation> ingredients, out Preparation result)
		{
			result = ingredients[0];
			return true;
		}

		public string GetInstruction()
		{
			string str = "Mix ";
			foreach (Step input in inputs)
			{
				if (input == null) continue;

				if (input is IngredientStep step && step != null && step.ingredient != null)
					str += step.ingredient.name + ", ";
				else
				{
					str += input.name + ", ";
				}
			}
			return str;
		}
	}
}
