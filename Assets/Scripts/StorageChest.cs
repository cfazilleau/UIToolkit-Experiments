using Storage;
using UnityEngine;

public class StorageChest : MonoBehaviour
{
	[SerializeField]
	private Vector2Int _chestSize = new(2, 5);

	private ItemStorage _chestStorage;

	public ItemStorage ChestStorage => _chestStorage;

	private void Awake()
	{
		_chestStorage = new ItemStorage(_chestSize);
		_chestStorage.OnStorageItemChanged += OnStorageItemChanged;
	}

	private void OnStorageItemChanged(int index)
	{
		ItemStack stack = _chestStorage[index];
		Debug.Log($"item {stack?.item} at index {index} changed.");
	}
}
