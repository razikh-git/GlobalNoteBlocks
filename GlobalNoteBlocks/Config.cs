﻿using StardewModdingAPI;

namespace GlobalNoteBlocks
{
	class Config
	{
		public SButton debugKey { get; set; }

		public Config()
		{
			debugKey = SButton.J;
		}
	}
}
