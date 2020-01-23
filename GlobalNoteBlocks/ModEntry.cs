using System;
using System.Linq;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Harmony; // el diavolo

namespace GlobalNoteBlocks
{
	public class ModEntry : Mod
	{
		internal Config Config;
		internal static ModEntry Instance;
		public static bool IsEnabled { get; set; }
		internal ITranslationHelper i18n => Helper.Translation;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			IsEnabled = true;
			Config = helper.ReadConfig<Config>();
			helper.Events.Input.ButtonReleased += OnButtonReleased;

			var harmony = HarmonyInstance.Create("blueberry.GlobalNoteBlocks");
			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Object), "farmerAdjacentAction"),
				prefix: new HarmonyMethod(typeof(ObjectPatch), nameof(ObjectPatch.Prefix)));
		}

		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			var key = e.Button;
			if (key.Equals(Config.ToggleKey))
			{
				IsEnabled = !IsEnabled;
				Game1.showGlobalMessage($"{i18n.Get("gn.popup.msg")}"
				                        + $"\n{(IsEnabled ? i18n.Get("gn.popup.on") : i18n.Get("gn.popup.off"))}"
				                        + $"\n({IsEnabled})");
			}
			/*
			if (key.Equals(SButton.OemQuotes))
			{
				if (Game1.player.currentLocation.Name.Equals("Farm"))
					Game1.warpFarmer("Forest", 5, 5, 2);
				else
					Game1.warpFarmer("Farm", 28, 44, 2);
			}
			*/
		}
	}

	public class ObjectPatch
	{
		internal static bool Prefix(StardewValley.Object __instance)
		{
			try
			{
				if (!ModEntry.IsEnabled)
					return true;

				var name = __instance.Name;

				if (name == null || __instance.isTemporarilyInvisible)
					return false;

				if (!name.Equals("Flute Block") && !name.Equals("Drum Block"))
					return true;

				var index = __instance.preservedParentSheetIndex.Value;

				if ((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds - __instance.lastNoteBlockSoundTime < 1000
				    || Game1.dialogueUp)
					return false;

				// Sound cue for note blocks played in all game locations
				if (name.Equals("Flute Block"))
					foreach (var location in Game1.locations)
						location.playSoundPitched("flute", index);
				else if (name.Equals("Drum Block"))
					foreach (var location in Game1.locations)
						location.playSound($"drumkit{index}");

				__instance.lastNoteBlockSoundTime = (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds;

				return false;
			}
			catch (Exception e)
			{
				ModEntry.Instance.Monitor.Log("blueberry.GlobalNoteBlocks failed in"
				      + $" {nameof(ObjectPatch)}.{nameof(Prefix)}"
				      + $"\n{e}");
				return true;
			}
		}
	}
}
