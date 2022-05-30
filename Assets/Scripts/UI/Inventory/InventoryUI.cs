using System;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI.Inventory
{
	public class InventoryUI : MonoBehaviour
	{
		[SerializeField]
		private PlayerController player;

		#region UIElements
		private VisualElement _root;
		private VisualElement _slotContainer;

		// Ghost Item that follows the mouse
		private InventorySlot _ghostItem;
		#endregion

		private readonly List<InventorySlot> _inventorySlots = new();
		private ItemStack _movingStack;

		private void Awake()
		{
			//Store the root from the UI Document component
			_root = GetComponent<UIDocument>().rootVisualElement;

			//Search the root for the SlotContainer Visual Element
			_slotContainer = _root.Q<VisualElement>("InventoryContainer");

			_ghostItem = _root.Query<InventorySlot>("GhostItem");
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

		#region Callbacks
		private void OnPointerDown(PointerDownEvent evt, int index)
		{
			// Left Mouse Button
			if (evt.button == 0)
			{
				if (_movingStack == null)
					TakeStack(index);
				else
					PlaceStack(index);
			}
			// Right Mouse button
			else if (evt.button == 1)
			{
				if (_movingStack == null)
					TakeHalfStack(index);
				else
					Place(index, 1);
			}
			else
			{
				return;
			}

			if (_movingStack != null && _movingStack.IsEmpty)
				_movingStack = null;

			RefreshGhostCursor();
		}

		private void OnInventoryItemChanged(int index)
		{
			_inventorySlots[index].Stack = player.Inventory[index];
			_inventorySlots[index].Refresh();
		}
		#endregion

		#region Inventory Controls
		private void TakeStack(int index)
		{
			_movingStack = player.Inventory.Take(index, -1);
		}

		private void PlaceStack(int index)
		{
			// If different item slot, swap items
			if (_inventorySlots[index].Item != _movingStack.item)
			{
				ItemStack tmp = player.Inventory.Take(index, -1);
				player.Inventory.Place(_movingStack, -1, index);
				_movingStack = tmp;
			}
			// If same item, fill slot
			else
			{
				player.Inventory.Place(_movingStack, -1, index);
			}
		}

		private void TakeHalfStack(int index)
		{
			if (player.Inventory[index] == null)
				return;

			int quantity = player.Inventory[index].quantity;
			_movingStack = player.Inventory.Take(index, quantity - quantity / 2); // if odd, take biggest part
		}

		private void Place(int index, int quantity)
		{
			// Add to existing stack or create a new one
			player.Inventory.Place(_movingStack, quantity, index);
		}
		#endregion

		private void InitInventory()
		{
			if (player == null)
				throw new NullReferenceException();

			//Create InventorySlots and add them as children to the SlotContainer
			for (int i = 0; i < player.Inventory.Size; i++)
			{
				InventorySlot slot = new InventorySlot(player.Inventory[i]);

				int index = i;
				slot.RegisterCallback<PointerDownEvent>(evt => OnPointerDown(evt, index));

				_inventorySlots.Add(slot);
				_slotContainer.Add(slot);
			}

			player.Inventory.OnInventoryItemChanged += OnInventoryItemChanged;
		}

		private void RefreshGhostCursor()
		{
			_ghostItem.Stack = _movingStack;
			_ghostItem.visible = _movingStack != null;
		}
	}
}
