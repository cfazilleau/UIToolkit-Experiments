using UnityEngine;

namespace Storage
{
	[CreateAssetMenu]
	public class Item : ScriptableObject
	{
		#region Serialized data
		[SerializeField]
		private Texture2D icon;

		[SerializeField]
		private string itemName;

		[SerializeField]
		private int maxStackSize = 100;
		#endregion

		#region Properties
		public Texture2D Icon => icon;
		public string ItemName => itemName;
		public int MaxStackSize => maxStackSize;
		#endregion

		private void OnValidate()
		{
			if (string.IsNullOrWhiteSpace(itemName))
				itemName = name;
		}
	}
}
