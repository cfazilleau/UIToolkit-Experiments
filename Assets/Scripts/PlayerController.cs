using System;
using Inventory;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private int inventorySize = 40;
	private Inventory.Inventory _inventory;

	public Inventory.Inventory Inventory => _inventory;

	[SerializeField]
	private ItemStack[] startItems = Array.Empty<ItemStack>();

	private void Awake()
	{
		_inventory = new Inventory.Inventory(inventorySize);

		foreach (ItemStack startItem in startItems)
			_inventory.Add(startItem);
	}
}