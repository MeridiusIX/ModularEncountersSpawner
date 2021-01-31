using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Spawners;

namespace ModularEncountersSpawner{

	public static class SuppressGroups{
		
		public static List<string> SuppressCargoShips = new List<string>();
		public static List<string> SuppressEncounters = new List<string>();
		
		public static void ApplySuppression(bool cargo, bool encounter){
			
			/*
			SuppressCargoShips.Add("Military1");
			SuppressCargoShips.Add("Military2");
			SuppressCargoShips.Add("Mining1");
			SuppressCargoShips.Add("Mining2");
			SuppressCargoShips.Add("R.U.S.T. Freighter");
			SuppressCargoShips.Add("Trade1");
			SuppressCargoShips.Add("Trade2");
			SuppressCargoShips.Add("Trade3");
			
			SuppressEncounters.Add("Encounter Debris A");
			SuppressEncounters.Add("Encounter Debris B");
			SuppressEncounters.Add("Encounter Ambasador A");
			SuppressEncounters.Add("Encounter Ambasador B");
			SuppressEncounters.Add("Encounter Blue frame");
			SuppressEncounters.Add("Encounter Corvette A");
			SuppressEncounters.Add("Encounter Corvette B");
			SuppressEncounters.Add("Encounter Droneyard");
			SuppressEncounters.Add("Encounter Ghoul Corvette A");
			SuppressEncounters.Add("Encounter Ghoul Corvette B");
			SuppressEncounters.Add("Encounter Ghoul Corvette C");
			SuppressEncounters.Add("Encounter Homing beacon");
			SuppressEncounters.Add("Encounter Hydro Tanker");
			SuppressEncounters.Add("Encounter Imp A");
			SuppressEncounters.Add("Encounter Imp B");
			SuppressEncounters.Add("Encounter MushStation A");
			SuppressEncounters.Add("Encounter MushStation B");
			SuppressEncounters.Add("Encounter Ponos-F1 A");
			SuppressEncounters.Add("Encounter Ponos-F1 B");
			SuppressEncounters.Add("Encounter RS-1217 Transporter A");
			SuppressEncounters.Add("Encounter Section-F");
			SuppressEncounters.Add("Encounter Skyheart A");
			SuppressEncounters.Add("Encounter Skyheart B");
			SuppressEncounters.Add("Encounter Stingray II A");
			SuppressEncounters.Add("Encounter Stingray II B");
			SuppressEncounters.Add("Encounter Vulture vessel");
			SuppressEncounters.Add("Encounter HEC Debris A");
			SuppressEncounters.Add("Encounter HEC Debris B");
			SuppressEncounters.Add("Encounter Hermit Station");
			SuppressEncounters.Add("Encounter Mining Vessel");
			SuppressEncounters.Add("Encounter Mining Outpost");
			SuppressEncounters.Add("Encounter RoidStation");
			SuppressEncounters.Add("Encounter Safehouse station");
			SuppressEncounters.Add("Encounter Shuttle");
			*/

			foreach(var spawn in SpawnGroupManager.SpawnGroups){

				if (cargo && spawn.SpaceCargoShip) {

					if (spawn.SpawnGroup?.Context != null) {

						if (spawn.SpawnGroup.Context.IsBaseGame)
							spawn.SpaceCargoShip = false;

					}

				}

				if (encounter && spawn.SpaceRandomEncounter) {

					if (spawn.SpawnGroup?.Context != null) {

						if (spawn.SpawnGroup.Context.IsBaseGame)
							spawn.SpaceRandomEncounter = false;

					}

				}

			}
			
		}
		
	}
	
}