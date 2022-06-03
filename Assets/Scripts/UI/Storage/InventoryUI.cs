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
		private float slotSize = 72f;

		[SerializeField]
		private float slotBorder = 10f;

		// UI document
		private VisualElement _root;

		// Player Inventory
		private VisualElement _inventoryRoot;
		private VisualElement _inventorySlotsUI;
		private readonly List<ItemSlot> _inventorySlots = new();

		// Open Container
		private VisualElement _otherRoot;
		private VisualElement _otherSlotsUI;
		private readonly List<ItemSlot> _otherSlots = new();
		private ItemStorage _otherStorage;

		// Moving Item
		private ItemSlot _ghostItem;
		private ItemStack _ghostStack;

		public bool Shown => _inventoryRoot is { enabledSelf: true };
		public event Action OnShow;
		public event Action OnHide;

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

			Hide(false);
		}

		private void Start()
		{
			InitInventory();

			player.OnStorageOpen += s => Show(s);
			player.OnStorageClose += () => Hide();
		}

		private void Update()
		{
			// Update ghostItem with mousePosition
			if (_ghostStack != null)
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
		}

		private void RefreshGhostCursor()
		{
			_ghostItem.Stack = _ghostStack;
			_ghostItem.visible = _ghostStack != null;
		}

		private void Show(ItemStorage otherStorage, bool notify = true)
		{
			SetOtherStorage(otherStorage);

			_inventoryRoot.SetEnabled(true);
			_otherRoot.SetEnabled(_otherStorage != null);

			if (notify)
				OnShow?.Invoke();
		}

		private void Hide(bool notify = true)
		{
			_inventoryRoot.SetEnabled(false);
			_otherRoot.SetEnabled(false);

			if (notify)
				OnHide?.Invoke();
		}

		#region Callbacks

		private void OnPointerDown(PointerDownEvent evt, ItemStorage itemStorage, int index)
		{
			OnSlotClicked(evt.button, itemStorage, index);
		}

		public void OnSlotClicked(int mouseButton, ItemStorage itemStorage, int index)
		{
			if (_ghostStack == null && itemStorage[index] != null)
			{
				// Left Mouse Button
				if (mouseButton == 0)
				{
					// Take whole stack
					_ghostStack = itemStorage.Take(index, -1);
				}
				// Right Mouse button
				else if (mouseButton == 1)
				{
					// Take Half stack
					_ghostStack = itemStorage.Take(index, Mathf.CeilToInt(itemStorage[index].quantity / 2f));
				}
			}
			else
			{
				// Left Mouse Button
				if (mouseButton == 0)
				{
					// Place full stack
					itemStorage.Place(_ghostStack, -1, index);
				}
				// Right Mouse button
				else if (mouseButton == 1)
				{
					// Place one item
					itemStorage.Place(_ghostStack, 1, index);
				}
			}

			if (_ghostStack is { IsEmpty: true })
				_ghostStack = null;

			RefreshGhostCursor();
		}

		private void SetOtherStorage(ItemStorage container)
		{
			if (_otherStorage != null)
				_otherStorage.OnStorageItemChanged -= OnOtherStorageItemChanged;

			_otherStorage = container;

			if (_otherStorage != null)
				_otherStorage.OnStorageItemChanged += OnOtherStorageItemChanged;

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
	}
}
