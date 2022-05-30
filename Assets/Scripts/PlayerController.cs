using System;
using Storage;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private Vector2Int inventorySize = new Vector2Int(4, 3);

	[SerializeField]
	private Vector2Int containerSize = new Vector2Int(3, 3);

	[SerializeField]
	private ItemStack[] startItems = Array.Empty<ItemStack>();

	private ItemStorage _inventory;
	public ItemStorage Inventory => _inventory;

	private ItemStorage _otherStorage;
	public ItemStorage OtherStorage => _otherStorage;

	public Action<ItemStorage> OnOtherStorageOpen;

	private void Awake()
	{
		_inventory = new ItemStorage(inventorySize.x * inventorySize.y);
	}

	private void Start()
	{
		foreach (ItemStack startItem in startItems)
		{
			_inventory.Add(startItem);
		}

		_otherStorage = new ItemStorage(containerSize.x * containerSize.y);
		OnOtherStorageOpen?.Invoke(_otherStorage);
	}
}