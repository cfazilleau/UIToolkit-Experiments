using UnityEngine;

namespace Recipes.Scriptable
{
	[CreateAssetMenu(fileName = "ingredient", menuName = "Recipe/Ingredient")]
	public class Ingredient : ScriptableObject
	{
		[SerializeField]
		private string ingredientName;

		[SerializeField]
		private Sprite image;
	}
}
