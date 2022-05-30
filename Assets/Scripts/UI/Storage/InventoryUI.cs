using System;
using System.Collections.Generic;
using Storage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI.Storage
{
	[RequireComponent(typeof(UIDocument))]
	public class InventoryUI : MonoBehaviour
	{
		[SerializeField]
		private PlayerController player;

		[SerializeField]
		private Vector2 slotSize = Vector2.one * 72;

		[SerializeField]
		private Vector2 slotsContainerBorder = Vector2.one * 10;

		// UI document
		private VisualElement _root;

		// Player Inventory
		private VisualElement _inventorySlotsUI;
		private List<ItemSlot> _inventorySlots = new();

		// Open Container
		private VisualElement _otherRoot;
		private VisualElement _otherSlotsUI;
		private List<ItemSlot> _otherSlots = new();
		private ItemStorage _otherStorage;

		// Moving Item
		private ItemSlot _ghostItem;
		private ItemStack _movingStack;

		private void Awake()
		{
			//Store the root from the UI Document component
			_root = GetComponent<UIDocument>().rootVisualElement;

			//Search the root for the SlotContainer Visual Element
			VisualElement inventoryRoot = _root.Query<VisualElement>("Inventory");
			_inventorySlotsUI = inventoryRoot.Query<VisualElement>("SlotsContainer");

			//Search the root for the SlotContainer Visual Element
			_otherRoot = _root.Query<VisualElement>("Container");
			_otherSlotsUI = _otherRoot.Query<VisualElement>("SlotsContainer");

			_ghostItem = _root.Query<ItemSlot>("GhostItem");

			player.OnOtherStorageOpen += OnPlayerOtherStorageOpen;
			OnPlayerOtherStorageOpen(player.OtherStorage);

			RefreshOtherContainerUI();
		}

		private void Start()
		{
			InitInventory();
		}

		private void Update()
		{
			if (_movingStack != null)
			{
				Vector2 pos = Mouse.current.position.ReadValue();

				_ghostItem.style.left = pos.x;
				_ghostItem.style.top = Screen.height - pos.y;
			}
		}

		private void InitInventory()
		{
			if (player == null)
				throw new NullReferenceException();

			_inventorySlotsUI.Clear();
			_inventorySlots.Clear();

			//Create InventorySlots and add them as children to the SlotContainer
			for (int i = 0; i < player.Inventory.Size; i++)
			{
				ItemSlot slot = new ItemSlot(player.Inventory[i]);

				int index = i;
				slot.RegisterCallback<PointerDownEvent>(evt => OnPointerDown(evt, player.Inventory, index));

				_inventorySlots.Add(slot);
				_inventorySlotsUI.Add(slot);
			}

			Vector2 size = slotSize * Mathf.Ceil(Mathf.Sqrt(player.Inventory.Size));
			size += slotsContainerBorder;
			_inventorySlotsUI.style.maxWidth = size.x;
			_inventorySlotsUI.style.maxHeight = size.y;

			player.Inventory.OnInventoryItemChanged += OnInventoryItemChanged;
		}

		private void RefreshOtherContainerUI()
		{
			_otherRoot.style.display = _otherStorage == null ? DisplayStyle.None : DisplayStyle.Flex;
			_otherSlotsUI.Clear();
			_otherSlots.Clear();

			if (_otherStorage == null)
				return;

			//Create Slots and add them as children to the SlotContainer
			for (int i = 0; i < _otherStorage.Size; i++)
			{
				ItemSlot slot = new ItemSlot(_otherStorage[i]);

				int index = i;
				slot.RegisterCallback<PointerDownEvent>(evt => OnPointerDown(evt, _otherStorage, index));

				_otherSlots.Add(slot);
				_otherSlotsUI.Add(slot);
			}

			Vector2 size = slotSize * Mathf.Ceil(Mathf.Sqrt(_otherStorage.Size));
			size += slotsContainerBorder;
			_otherSlotsUI.style.maxWidth = size.x;
			_otherSlotsUI.style.maxHeight = size.y;
		}

		private void RefreshGhostCursor()
		{
			_ghostItem.Stack = _movingStack;
			_ghostItem.visible = _movingStack != null;
		}

		#region Callbacks
		private void OnPointerDown(PointerDownEvent evt, ItemStorage itemStorage, int index)
		{
			if (_movingStack == null)
			{
				// Left Mouse Button
				if (evt.button == 0)
					TakeStack(itemStorage, index);
				// Right Mouse button
				else if (evt.button == 1)
					TakeHalfStack(itemStorage, index);
			}
			else
			{
				// Left Mouse Button
				if (evt.button == 0)
					PlaceStack(itemStorage, index);
				// Right Mouse button
				else if (evt.button == 1)
					Place(itemStorage, 1, index);
			}

			if (_movingStack is { IsEmpty: true })
				_movingStack = null;

			RefreshGhostCursor();
		}

		private void OnPlayerOtherStorageOpen(ItemStorage container)
		{
			if (_otherStorage != null)
				_otherStorage.OnInventoryItemChanged -= OnOtherStorageItemChanged;

			_otherStorage = container;

			if (_otherStorage != null)
				_otherStorage.OnInventoryItemChanged += OnOtherStorageItemChanged;

			RefreshOtherContainerUI();
		}

		private void OnOtherStorageItemChanged(int index)
		{
			_otherSlots[index].Stack = _otherStorage[index];
			_otherSlots[index].Refresh();
		}

		private void OnInventoryItemChanged(int index)
		{
			_inventorySlots[index].Stack = player.Inventory[index];
			_inventorySlots[index].Refresh();
		}
		#endregion

		#region Inventory Controls
		private void TakeStack(ItemStorage itemStorage, int index)
		{
			_movingStack = itemStorage.Take(index, -1);
		}

		private void PlaceStack(ItemStorage itemStorage, int index)
		{
			// If different item slot, swap items
			if (itemStorage[index] != null && itemStorage[index].item != _movingStack.item)
			{
				ItemStack tmp = itemStorage.Take(index, -1);
				itemStorage.Place(_movingStack, -1, index);
				_movingStack = tmp;
			}
			// If same item, merge stacks
			else
			{
				itemStorage.Place(_movingStack, -1, index);
			}
		}

		private void TakeHalfStack(ItemStorage itemStorage, int index)
		{
			if (itemStorage[index] == null)
				return;

			int quantity = itemStorage[index].quantity;
			_movingStack = itemStorage.Take(index, quantity - quantity / 2); // if odd, take biggest part
		}

		private void Place(ItemStorage itemStorage, int quantity, int index)
		{
			// Add to existing stack or create a new one
			itemStorage.Place(_movingStack, quantity, index);
		}
		#endregion
	}
}
