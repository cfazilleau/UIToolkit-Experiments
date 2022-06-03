using UnityEngine;

namespace Recipes.Steps
{
	[Step("Cook")]
	public class CookStep : Step
	{
		[SerializeField]
		private int temperature;

		[SerializeField]
		private int duration;

		public bool Cook(in Preparation input, out Preparation result)
		{
			result = input;
			return true;
		}
	}
}