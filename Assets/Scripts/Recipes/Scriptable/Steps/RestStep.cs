using UnityEngine;

namespace Recipes.Scriptable.Steps
{
	[Step("Rest")]
	public class RestStep : Step
	{
		[SerializeField]
		private int duration = 5000;

		public bool Rest(in Step input, out Step result)
		{
			result = input;
			return true;
		}
	}
}
