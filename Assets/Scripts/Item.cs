using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
	[SerializeField]
	private Texture2D icon;

	[SerializeField]
	private int maxStackSize = 100;

	public Texture2D Icon => icon;

	public int MaxStackSize => maxStackSize;
}
