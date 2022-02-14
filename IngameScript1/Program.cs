using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;
using System.Threading;

namespace IngameScript
{
	partial class Program : MyGridProgram
	{
		#region mdk preserve
		/**
		 * Automatically run the script every second
		 * Turning this off will run the script only once
		 */
		static readonly bool autoUpdate = true;

		static readonly double fullBuffer = 75;		// Percentage of the cargo container inventory for a full capacity
		static readonly double emptyBuffer = 0;		// Percentage of the cargo container inventory for an empty capacity
		
		// Cargo Container Name
		static readonly string cargoContainerName = "Large Cargo Container";
		
		// Timer Blocks names
		static readonly string fullTimerBlockName = "Timer Block 1";
		static readonly string emptyTimerBlockName = "Timer Block 2";
		#endregion

		/**
		 * Do not edit beyond this line
		 */
		private readonly IMyCargoContainer container;
		private readonly IMyTimerBlock fullTimer;
		private readonly IMyTimerBlock emptyTimer;

		bool fullTimerStarted = false;
		bool emptyTimerStarted = false;

		public Program()
		{
			container = GridTerminalSystem.GetBlockWithName(cargoContainerName) as IMyCargoContainer;
			fullTimer = GridTerminalSystem.GetBlockWithName(fullTimerBlockName) as IMyTimerBlock;
			emptyTimer = GridTerminalSystem.GetBlockWithName(emptyTimerBlockName) as IMyTimerBlock;

			if (autoUpdate) {
				Runtime.UpdateFrequency = UpdateFrequency.Once | UpdateFrequency.Update100;
			}
		}

		public void Main(string argument, UpdateType updateSource)
		{
			bool _hasMissingBlocks = HasMissingBlocks();

			if (_hasMissingBlocks) {
				ShowMissingBlocks();
				return;
			}

			Echo("Script Active!");

			double maxVolume = (double)container.GetInventory().MaxVolume;
			double currentVolume = (double)container.GetInventory().CurrentVolume;
				
			double availablePercentage = Math.Round((currentVolume * 100) / maxVolume);

			if (availablePercentage >= fullBuffer)
			{
				if (!fullTimerStarted)
				{
					fullTimer.StartCountdown();
					fullTimerStarted = true;
					emptyTimerStarted = false;
					Echo("Full Timer Started");
				}
			}

			if (availablePercentage <= emptyBuffer) {
				if (!emptyTimerStarted)
				{
					emptyTimer.StartCountdown();
					emptyTimerStarted = true;
					fullTimerStarted = false;
					Echo("Empty Timer Started");
				}
			}


			Echo($"\nContainer Capacity: {availablePercentage}%");
		}

		public void ShowMissingBlocks() {
			string message = "Cannot start script!\n\nMissing Blocks:";
			
			if (container == null)
			{
				message += $"\n\t\t\t\t\t\t\t\t- {cargoContainerName}";
			}

			if (fullTimer == null)
			{
				message += $"\n\t\t\t\t\t\t\t\t- {fullTimerBlockName}";
			}

			if (emptyTimer == null)
			{
				message += $"\n\t\t\t\t\t\t\t\t- {emptyTimerBlockName}";
			}

			Echo(message);
		}

		public bool HasMissingBlocks() {
			if (container == null) { return true; }
			if (fullTimer == null) { return true; }
			if (emptyTimer == null) { return true; }

			return false;
		}
	}
}
