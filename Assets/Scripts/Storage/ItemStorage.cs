using System;
using System.Linq;
using UnityEngine;

namespace Storage
{
	public class ItemStorage
	{
		private readonly ItemStack[] _stacks;

		private Vector2Int _size;

		public ItemStack this[int index] => _stacks[index];
		public ItemStack this[int x, int y] => this[y * _size.x + x];
		public int Length => _stacks.Length;
		public Vector2Int Size => _size;

		public event Action<int> OnStorageItemChanged;

		public ItemStorage(Vector2Int size)
		{
			_size = size;
			_stacks = new ItemStack[size.x * size.y];
		}

		/// <summary>
		/// Tries to add an item to the first empty slot of the storage.
		/// </summary>
		/// <param name="stackToAdd">ItemStack to add</param>
		/// <returns>The amount of items that could not find an empty slot</returns>
		public int Add(ItemStack stackToAdd)
		{
			do
			{
				// Look for an existing Item Stack that is not full
				ItemStack stack = _stacks.FirstOrDefault(stack => stack != null &&
				                                                  stack.item == stackToAdd.item &&
				                                                  stack.FreeSpace > 0);

				// If no stack is found
				if (stack == null)
				{
					// Find an empty slot index
					for (int i = 0; i < _stacks.Length; i++)
					{
						if (_stacks[i] == null)
						{
							// Create a new stack at this index
							stack = new ItemStack(stackToAdd.item);
							_stacks[i] = stack;
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
				OnStorageItemChanged?.Invoke(Array.IndexOf(_stacks, stack));
				stackToAdd.quantity -= add;

				// If we still have items to place, repeat operation
			} while (stackToAdd.quantity > 0);

			// return remaining quantity
			return stackToAdd.quantity;
		}

		/// <summary>
		/// Remove stack or part of stack from the storage and returns it
		/// </summary>
		/// <param name="index">Index of the item stack in the storage</param>
		/// <param name="quantity">Quantity of the stack to take (can't be bigger than stack size) or -1 to take the whole stack</param>
		/// <returns>The stack at this index, or null if not existing</returns>
		public ItemStack Take(int index, int quantity)
		{
			ItemStack target = _stacks[index];

			// If the stack exist, remove it from the storage
			if (target != null && target.quantity >= quantity)
			{
				if (quantity == -1)
					quantity = target.quantity;

				target.quantity -= quantity;

				if (target.quantity == 0)
					_stacks[index] = null;

				OnStorageItemChanged?.Invoke(index);

				return new ItemStack(target.item, quantity);
			}

			return null;
		}

		/// <summary>
		/// Remove stack or part of stack from the storage and returns it
		/// </summary>
		/// <param name="stack">Item Stack to place in the storage</param>
		/// <param name="quantity">Quantity of the stack to place (can't be bigger than stack size) or -1 to place the whole stack</param>
		/// <param name="index">Index of the item stack in the storage</param>
		/// <returns>True if the whole quantity requested has been placed, false otherwise</returns>
		public bool Place(ItemStack stack, int quantity, int index)
		{
			// Check values
			if (stack == null || quantity > stack.quantity)
				return false;

			// -1 quantity means full stack
			if (quantity == -1)
				quantity = stack.quantity;

			// Retrieve target stack in storage
			ItemStack targetStack = _stacks[index];

			// Create new stack if empty spot
			if (targetStack == null)
			{
				_stacks[index] = new ItemStack(stack) { quantity = quantity };
				stack.quantity -= quantity;
				OnStorageItemChanged?.Invoke(index);
				return true;
			}

			// Fill stack with remaining items if spot is of same item type and has free space
			if (targetStack.item == stack.item && targetStack.FreeSpace > 0)
			{
				int add = Math.Clamp(quantity, 0, targetStack.FreeSpace);
				targetStack.quantity += add;
				stack.quantity -= add;
				OnStorageItemChanged?.Invoke(index);
				return true;
			}

			return false;
		}
	}
}