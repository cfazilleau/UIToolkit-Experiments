using Storage;
using UnityEngine;

public class StorageChest : MonoBehaviour
{
	[SerializeField]
	private Vector2Int chestSize = new(2, 5);

	private ItemStorage _chestStorage;

	public ItemStorage ChestStorage => _chestStorage;

	private void Awake()
	{
		_chestStorage = new ItemStorage(chestSize);
	}
}
