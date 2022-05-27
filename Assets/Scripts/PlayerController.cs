using System;
using UnityEngine;

namespace UI
{
	public class PlayerController : MonoBehaviour
	{
		private Inventory inventory = new Inventory(40);

		public Inventory Inventory => inventory;

		[SerializeField]
		private ItemStack[] startItems = Array.Empty<ItemStack>();

		private void Start()
		{
			foreach (ItemStack startItem in startItems)
				inventory.Add(startItem);
		}
	}
}
