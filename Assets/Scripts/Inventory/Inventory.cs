using System;
using System.Linq;

namespace Inventory
{
	public class Inventory
	{
		private readonly ItemStack[] _inventory;

		public ItemStack this[int index] => _inventory[index];
		public int Size => _inventory.Length;

		public event Action<int> OnInventoryItemChanged;

		public Inventory(int size)
		{
			_inventory = new ItemStack[size];
		}

		/// <summary>
		/// Tries to add an item to the first empty slot of the inventory.
		/// </summary>
		/// <param name="stackToAdd">ItemStack to add</param>
		/// <returns>The amount of items that could not find an empty slot</returns>
		public int Add(ItemStack stackToAdd)
		{
			do
			{
				// Look for an existing Item Stack that is not full
				ItemStack stack = _inventory.FirstOrDefault(stack => stack != null &&
				                                                     stack.item == stackToAdd.item &&
				                                                     !stack.IsFull);

				// If no stack is found
				if (stack == null)
				{
					// Find an empty slot index
					for (int i = 0; i < _inventory.Length; i++)
					{
						stack = _inventory[i];
						if (stack == null)
						{
							// Create a new stack at this index
							stack = new ItemStack(stackToAdd.item);
							_inventory[i] = stack;
							break;
						}
					}
				}

				// If there is still no stack, stop here
				if (stack == null)
					break;

				// Fill stack with remaining items
				int add = Math.Clamp(stackToAdd.quantity, 0, stack.FreeSpace);
				stack.quantity += add;
				OnInventoryItemChanged?.Invoke(Array.IndexOf(_inventory, stack));
				stackToAdd.quantity -= add;

				// If we still have items to place, repeat operation
			} while (stackToAdd.quantity > 0);

			// return remaining quantity
			return stackToAdd.quantity;
		}

		/// <summary>
		/// Remove stack or part of stack from the inventory and returns it
		/// </summary>
		/// <param name="index">Index of the item stack in the inventory</param>
		/// <param name="quantity">Quantity of the stack to take (can't be bigger than stack size) or -1 to take the whole stack</param>
		/// <returns>The stack at this index, or null if not existing</returns>
		public ItemStack Take(int index, int quantity)
		{
			ItemStack target = _inventory[index];

			// If the stack exist, remove it from the inventory
			if (target != null && target.quantity >= quantity)
			{
				if (quantity == -1)
					quantity = target.quantity;

				target.quantity -= quantity;

				if (target.quantity == 0)
					_inventory[index] = null;

				OnInventoryItemChanged?.Invoke(index);

				return new ItemStack(target.item, quantity);
			}

			return null;
		}

		/// <summary>
		/// Remove stack or part of stack from the inventory and returns it
		/// </summary>
		/// <param name="stack">Item Stack to place in the inventory</param>
		/// <param name="quantity">Quantity of the stack to place (can't be bigger than stack size) or -1 to place the whole stack</param>
		/// <param name="index">Index of the item stack in the inventory</param>
		/// <returns>True if the whole quantity requested has been placed, false otherwise</returns>
		public bool Place(ItemStack stack, int quantity, int index)
		{
			if (stack == null || quantity > stack.quantity)
				return false;

			if (quantity == -1)
				quantity = stack.quantity;

			ItemStack target = _inventory[index];

			// Place new stack if empty spot
			if (target == null)
			{
				_inventory[index] = new ItemStack(stack);
				stack.quantity = 0;
				OnInventoryItemChanged?.Invoke(index);
				return true;
			}

			// If spot of different item type, ignore
			if (target.item != stack.item)
			{
				return false;
			}

			// Fill stack with remaining items
			int add = Math.Clamp(quantity, 0, target.FreeSpace);
			target.quantity += add;
			stack.quantity -= add;
			OnInventoryItemChanged?.Invoke(index);
			return true;
		}
	}
}