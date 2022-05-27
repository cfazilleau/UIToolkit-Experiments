using System;

namespace UI
{
	[Serializable]
	public class ItemStack
	{
		public Item item;
		public int quantity;

		public int FreeSpace => item.MaxStackSize - quantity;
		public bool IsFull => FreeSpace == 0;

		public ItemStack(Item item, int quantity = 0)
		{
			this.item = item;
			this.quantity = quantity;
		}

		public ItemStack(ItemStack stack)
			: this(stack.item, stack.quantity)
		{
		}
	};
}
