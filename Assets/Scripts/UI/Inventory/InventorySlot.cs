using UnityEngine.UIElements;
using Inventory;

namespace UI.Inventory
{
	public class InventorySlot : VisualElement
	{

		#region UXML
		// This nested class needs to be defined in order to create instances of this element from the UI Builder
		public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> { }

		// This nested class needs to be defined in order to edit properties from the UI Builder
		public new class UxmlTraits : VisualElement.UxmlTraits { }
		#endregion

		#region Variables
		private readonly Image _icon;
		private readonly Label _countLabel;

		private ItemStack _stack;
		#endregion

		#region Properties
		public Item Item => _stack?.item;

		public ItemStack Stack
		{
			get => _stack;
			set
			{
				_stack = value;
				Refresh();
			}
		}

		#endregion

		public InventorySlot() : this(null) {}

		public InventorySlot(ItemStack stack)
		{
			// Create and add Icon and label
			_icon = new Image();
			_countLabel = new Label();
			Add(_icon);
			Add(_countLabel);

			//Add USS style properties to the elements
			_icon.AddToClassList("inventory-slot-icon");
			_countLabel.AddToClassList("inventory-slot-count-label");
			AddToClassList("inventory-slot-container");

			// Set stack
			if (stack != null)
			{
				Stack = stack;
			}
		}

		public void Refresh()
		{
			if (_stack == null || _stack.IsEmpty)
			{
				_icon.image = null;
				_countLabel.text = string.Empty;
			}
			else
			{
				_icon.image = Stack.item.Icon;
				_countLabel.text = Stack.quantity < 2
					? string.Empty
					: Stack.quantity.ToString();
			}
		}
	}
}
