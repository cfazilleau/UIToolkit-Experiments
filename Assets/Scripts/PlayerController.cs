using System;
using Storage;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private Vector2Int inventorySize = new Vector2Int(4, 3);

	[SerializeField]
	private ItemStack[] startItems = Array.Empty<ItemStack>();

	[SerializeField]
	private StorageChest chest;

	private ItemStorage _inventory;

	/// <summary>
	/// Player Inventory
	/// </summary>
	public ItemStorage Inventory => _inventory;

	/// <summary>
	/// On Open inventory (parameter is other container or null)
	/// </summary>
	public event Action<ItemStorage> OnStorageOpen;

	/// <summary>
	/// On Close Inventory
	/// </summary>
	public event Action OnStorageClose;

	private void Awake()
	{
		_inventory = new ItemStorage(inventorySize);
	}

	private void Start()
	{
		// Fill inventory with startItems
		foreach (ItemStack startItem in startItems)
			_inventory.Add(startItem);
	}

	private void OnGUI()
	{
		using (new GUILayout.VerticalScope())
		{
			if (GUILayout.Button("Open Chest"))
			{
				OnStorageOpen?.Invoke(chest.ChestStorage);
			}
			if (GUILayout.Button("Open Inventory"))
			{
				OnStorageOpen?.Invoke(null);
			}
			if (GUILayout.Button("Close"))
			{
				OnStorageClose?.Invoke();
			}
		}
	}
}