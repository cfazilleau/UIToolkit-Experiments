using UnityEngine;

namespace Recipes.Steps
{
	[Step("Rest")]
	public class RestStep : Step
	{
		[SerializeField]
		private int duration = 5000;

		public bool Rest(in Preparation input, out Preparation result)
		{
			result = input;
			return true;
		}
	}
}
