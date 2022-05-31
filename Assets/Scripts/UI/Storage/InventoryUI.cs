using System;
using System.Collections;
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
		private float slotSize = 72f;

		[SerializeField]
		private float slotBorder = 10f;

		// UI document
		private VisualElement _root;

		// Player Inventory
		private VisualElement _inventoryRoot;
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
			_inventoryRoot = _root.Query<VisualElement>("Inventory");
			_inventorySlotsUI = _inventoryRoot.Query<VisualElement>("SlotsContainer");

			//Search the root for the SlotContainer Visual Element
			_otherRoot = _root.Query<VisualElement>("Container");
			_otherSlotsUI = _otherRoot.Query<VisualElement>("SlotsContainer");

			_ghostItem = _root.Query<ItemSlot>("GhostItem");

			Hide(true);
		}

		private void Start()
		{
			InitInventory();

			player.OnOtherStorageOpen += OnPlayerOtherStorageOpen;
		}

		public void Show()
		{
			GetComponent<UIDocument>().enabled = true;
			_inventoryRoot.RemoveFromClassList("inventory--hidden");
			_otherRoot.RemoveFromClassList("inventory--hidden");
		}

		public void Hide(bool instant = false)
		{
			if (instant)
			{
				_inventoryRoot.AddToClassList("inventory-hidden");
				_otherRoot.AddToClassList("inventory-hidden");
				GetComponent<UIDocument>().enabled = false;
			}
			else
				StartCoroutine(HideCoroutine());
		}

		private IEnumerator HideCoroutine()
		{
			_inventoryRoot.AddToClassList("inventory-hidden");
			_otherRoot.AddToClassList("inventory-hidden");
			yield return new WaitForSeconds(0.2f);
			GetComponent<UIDocument>().enabled = false;
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
			for (int i = 0; i < player.Inventory.Length; i++)
			{
				ItemSlot slot = new ItemSlot(player.Inventory[i]);

				int index = i;
				slot.RegisterCallback<PointerDownEvent>(evt => OnPointerDown(evt, player.Inventory, index));

				_inventorySlots.Add(slot);
				_inventorySlotsUI.Add(slot);
			}

			// set maxWidth to colCount * slotSize (including spacing/borders)
			_inventorySlotsUI.style.maxWidth = slotBorder + slotSize * player.Inventory.Size.x;

			player.Inventory.OnStorageItemChanged += OnStorageItemChanged;
		}

		private void RefreshOtherContainerUI()
		{
			_otherRoot.style.display = _otherStorage == null ? DisplayStyle.None : DisplayStyle.Flex;
			_otherSlotsUI.Clear();
			_otherSlots.Clear();

			if (_otherStorage == null)
				return;

			//Create Slots and add them as children to the SlotContainer
			for (int i = 0; i < _otherStorage.Length; i++)
			{
				ItemSlot slot = new ItemSlot(_otherStorage[i]);

				int index = i;
				slot.RegisterCallback<PointerDownEvent>(evt => OnPointerDown(evt, _otherStorage, index));

				_otherSlots.Add(slot);
				_otherSlotsUI.Add(slot);
			}

			// set maxWidth to colCount * slotSize (including spacing/borders)
			_otherSlotsUI.style.maxWidth = slotBorder + slotSize * _otherStorage.Size.x;

			_otherRoot.AddToClassList("visibleOpacity");
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
				_otherStorage.OnStorageItemChanged -= OnOtherStorageItemChanged;

			_otherStorage = container;

			if (_otherStorage != null)
				_otherStorage.OnStorageItemChanged += OnOtherStorageItemChanged;

			if (container == null)
				Hide();
			else
				Show();

			RefreshOtherContainerUI();
		}

		private void OnOtherStorageItemChanged(int index)
		{
			_otherSlots[index].Stack = _otherStorage[index];
			_otherSlots[index].Refresh();
		}

		private void OnStorageItemChanged(int index)
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
