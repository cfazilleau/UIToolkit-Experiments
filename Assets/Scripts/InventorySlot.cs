using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : VisualElement
{

	#region UXML
	// This nested class needs to be defined in order to create instances of this element from the UI Builder
	public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> { }

	// This nested class needs to be defined in order to edit properties from the UI Builder
	public new class UxmlTraits : VisualElement.UxmlTraits { }
	#endregion

	#region Variables
	private Image _icon;
	private Label _countLabel;

	private Inventory _inventory;
	private int _index;
	#endregion

	#region Properties
	public bool IsEmpty => _inventory[_index] == null;
	public Item Item => _inventory[_index]?.item ;
	#endregion

	public InventorySlot() : this(null, 0) {}

	public InventorySlot(Inventory inventory, int index)
	{
		if (inventory != null)
		{
			_inventory = inventory;
			_index = index;
		}

		// Create and add Icon and label
		_icon = new Image();
		_countLabel = new Label();
		Add(_icon);
		Add(_countLabel);

		//Add USS style properties to the elements
		_icon.AddToClassList("inventory-slot-icon");
		_countLabel.AddToClassList("inventory-slot-count-label");
		AddToClassList("inventory-slot-container");
	}

	public void HideIcon()
	{
		_icon.image = null;
	}

	public void Refresh()
	{
		if (_inventory != null)
		{
			ItemStack stack = _inventory[_index];
			_icon.image = stack?.item.Icon;
			_countLabel.text = stack == null || stack.quantity < 2
				? string.Empty
				: stack.quantity.ToString();
		}
	}

	public ItemStack TakeStack()
	{
		return _inventory.TakeStack(_index);
	}

	public bool PlaceStack(ItemStack stack)
	{
		return _inventory.PlaceStack(stack, _index);
	}

	public ItemStack TakeHalf()
	{
		if (_inventory[_index] == null)
			return null;

		return _inventory.Take(_index, _inventory[_index].quantity / 2);
	}

	public bool Place(ItemStack stack, int quantity)
	{
		return _inventory.Place(stack, quantity, _index);
	}
}
