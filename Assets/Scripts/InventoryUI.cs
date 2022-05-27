using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI
{
	public class InventoryUI : MonoBehaviour
	{
		[SerializeField]
		private PlayerController player;

		#region UIElements
		private VisualElement _root;
		private VisualElement _slotContainer;

		// Ghost Item that follows the mouse
		private VisualElement _ghostItem;
		private Label _ghostItemLabel;
		#endregion

		private readonly List<InventorySlot> _inventorySlots = new();
		private ItemStack _movingStack = null;

		private void Awake()
		{
			//Store the root from the UI Document component
			_root = GetComponent<UIDocument>().rootVisualElement;

			//Search the root for the SlotContainer Visual Element
			_slotContainer = _root.Q<VisualElement>("InventoryContainer");

			_ghostItem = _root.Query<VisualElement>("GhostItem");
			_ghostItemLabel = _root.Query<Label>("GhostItemLabel");

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

		#region Callbacks
		private void OnPointerDown(PointerDownEvent evt, InventorySlot slot)
		{
			// Left Mouse Button
			if (evt.button == 0)
			{
				if (_movingStack == null)
					TakeStack(slot);
				else
					PlaceStack(slot);
			}
			// Right Mouse button
			else if (evt.button == 1)
			{
				if (_movingStack == null)
					TakeHalfStack(slot);
				else
					PlaceOne(slot);
			}
			else
			{
				return;
			}

			RefreshGhostCursor();
		}

		private void OnInventoryItemChanged(int index)
		{
			_inventorySlots[index].Refresh();
		}
		#endregion

		#region Inventory Controls
		private void TakeStack(InventorySlot slot)
		{
			_movingStack = slot.TakeStack();
		}

		private void PlaceStack(InventorySlot slot)
		{
			// If different item slot, swap items
			if (slot.Item != _movingStack.item)
			{
				ItemStack tmp = slot.TakeStack();
				slot.PlaceStack(_movingStack);
				_movingStack = tmp;
			}
			// If same item, fill slot
			else if (slot.PlaceStack(_movingStack))
			{
				if (_movingStack.quantity == 0)
					_movingStack = null;
			}
		}

		private void TakeHalfStack(InventorySlot slot)
		{
			_movingStack = slot.TakeHalf();
		}

		private void PlaceOne(InventorySlot slot)
		{
			// Add to existing stack or create a new one
			if (slot.Place(_movingStack, 1))
			{
				if (_movingStack.quantity == 0)
					_movingStack = null;
			}
		}
		#endregion

		private void InitInventory()
		{
			if (player == null)
				throw new NullReferenceException();

			//Create InventorySlots and add them as children to the SlotContainer
			for (int i = 0; i < player.Inventory.Size; i++)
			{
				InventorySlot slot = new InventorySlot(player.Inventory, i);

				slot.RegisterCallback<PointerDownEvent>(evt => OnPointerDown(evt, slot));

				_inventorySlots.Add(slot);
				_slotContainer.Add(slot);
			}

			player.Inventory.OnInventoryItemChanged += OnInventoryItemChanged;
		}

		private void RefreshGhostCursor()
		{
			_ghostItem.style.backgroundImage = _movingStack?.item.Icon;
			_ghostItemLabel.text = _movingStack == null || _movingStack.quantity < 2
				? string.Empty
				: _movingStack.quantity.ToString();
		}
	}
}
