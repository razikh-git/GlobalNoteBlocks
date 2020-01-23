using StardewModdingAPI;

namespace GlobalNoteBlocks
{
	internal class Config
	{
		public SButton ToggleKey { get; set; }

		public Config()
		{
			ToggleKey = SButton.OemQuotes;
		}
	}
}
