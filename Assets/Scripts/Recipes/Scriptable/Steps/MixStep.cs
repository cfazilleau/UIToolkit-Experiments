using System.Collections.Generic;
using UnityEngine;

namespace Recipes.Scriptable.Steps
{
	[Step("Mix")]
	public class MixStep : Step
	{
		[SerializeField]
		private Vector3 position;

		public bool Mix(in List<Step> ingredients, out Step result)
		{
			result = ingredients[0];
			return true;
		}
	}
}
