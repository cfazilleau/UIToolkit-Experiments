using UnityEngine;

namespace Recipes.Scriptable.Steps
{
	[Recipe.Step("Rest", "Rest")]
	public class RestStep : Recipe.Step
	{
		[SerializeField]
		private int duration = 5000;

		public bool Rest(in Recipe.Step input, out Recipe.Step result)
		{
			result = input;
			return true;
		}
	}
}
