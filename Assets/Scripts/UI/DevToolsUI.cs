using System;
using System.Collections.Generic;
using Storage;
using UI.Storage;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
	[RequireComponent(typeof(UIDocument))]
	public class DevToolsUI : MonoBehaviour
	{
		[SerializeField]
		private InventoryUI inventoryUI;

		// UI document
		private VisualElement _root;

		private VisualElement _inventoryRoot;
		private VisualElement _inventorySlotsUI;
		private readonly List<ItemSlot> _inventorySlots = new();

		private ItemStorage _creativeInventory;

		private void Awake()
		{
			//Store the root from the UI Document component
			_root = GetComponent<UIDocument>().rootVisualElement;

			//Search the root for the SlotContainer Visual Element
			_inventoryRoot = _root.Query<VisualElement>("CreativeInventoryTab");
			_inventorySlotsUI = _inventoryRoot.Query<VisualElement>("SlotsContainer");
		}

		private void Start()
		{
			InitInventory();

			inventoryUI.OnHide += Hide;
			inventoryUI.OnShow += Show;

			if (inventoryUI.Shown)
				Show();
			else
				Hide();
		}

		private void InitInventory()
		{
			_inventorySlotsUI.Clear();
			_inventorySlots.Clear();

			Item[] items = Resources.FindObjectsOfTypeAll(typeof(Item)) as Item[];

			if (items == null || items.Length == 0)
				throw new Exception("No items were found");

			// Create and fill inventory
			Debug.Log($"Creating infinite inventory for {items.Length} items");
			_creativeInventory = new InfiniteItemStorage(new Vector2Int(2, Mathf.CeilToInt(items.Length / 2f)));

			//Create InventorySlots and add them as children to the SlotContainer
			for (int i = 0; i < _creativeInventory.Length; i++)
			{
				_creativeInventory.Add(new ItemStack(items[i], 1));

				ItemSlot slot = new ItemSlot(_creativeInventory[i]);

				int index = i;
				slot.RegisterCallback<PointerDownEvent>(evt => OnPointerDown(evt, index));

				_inventorySlots.Add(slot);
				_inventorySlotsUI.Add(slot);
			}
		}

		private void OnPointerDown(PointerDownEvent evt, int index)
		{
			inventoryUI.OnSlotClicked(evt.button, _creativeInventory, index);
		}

		private void Show()
		{
			_inventoryRoot.SetEnabled(true);
		}

		private void Hide()
		{
			_inventoryRoot.SetEnabled(false);
		}
	}
}
