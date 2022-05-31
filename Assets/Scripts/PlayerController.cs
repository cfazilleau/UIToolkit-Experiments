using System;
using Storage;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private Vector2Int inventorySize = new Vector2Int(4, 3);

	[SerializeField]
	private ItemStack[] startItems = Array.Empty<ItemStack>();

	private ItemStorage _inventory;
	public ItemStorage Inventory => _inventory;

	public StorageChest chest;

	public Action<ItemStorage> OnOtherStorageOpen;

	private void Awake()
	{
		_inventory = new ItemStorage(inventorySize);
	}

	private void Start()
	{
		foreach (ItemStack startItem in startItems)
		{
			_inventory.Add(startItem);
		}
	}

	private void OnGUI()
	{
		using (new GUILayout.VerticalScope())
		{
			if (GUILayout.Button("Open Chest"))
			{
				OnOtherStorageOpen?.Invoke(chest.ChestStorage);
			}
			if (GUILayout.Button("Close Chest"))
			{
				OnOtherStorageOpen?.Invoke(null);
			}
		}
	}
}