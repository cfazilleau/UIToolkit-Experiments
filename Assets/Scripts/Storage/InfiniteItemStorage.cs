using UnityEngine;

namespace Storage
{
	public class InfiniteItemStorage : ItemStorage
	{
		public InfiniteItemStorage(Vector2Int size) : base(size) { }

		// Taking an item from an infinite inventory creates a new one
		public override ItemStack Take(int index, int quantity)
		{
			ItemStack target = _stacks[index];

			if (target != null)
			{
				if (quantity == -1)
					quantity = target.item.MaxStackSize;

				return new ItemStack(target.item, quantity);
			}

			return null;
		}

		// Placing an item in an infinite inventory simply destroys it
		public override bool Place(ItemStack stack, int quantity, int index)
		{
			if (stack == null)
				return false;

			stack.quantity = 0;
			return true;
		}
	}
}
