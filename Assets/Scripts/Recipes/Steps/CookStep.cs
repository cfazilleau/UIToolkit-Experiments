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

		public override string InstructionLine => $"Cook at {temperature} during {duration} minutes";

		public bool Cook(in Preparation input, out Preparation result)
		{
			result = input;
			return true;
		}
	}
}