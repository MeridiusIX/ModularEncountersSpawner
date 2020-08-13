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

namespace ModularEncountersSpawner.Templates{
	
	public class PlayerWatcher{
		
		public IMyPlayer Player {get; set;}
		
		public int SpaceCargoShipTimer {get; set;}
		public int AtmoCargoShipTimer {get; set;}
		public int LunarCargoShipTimer {get; set;}
		public int RandomEncounterCheckTimer {get; set;}
		public int PlanetaryInstallationCheckTimer {get; set;}
		public int BossEncounterCheckTimer {get; set;}
		
		public int RandomEncounterCoolDownTimer {get; set;}
		public int PlanetaryInstallationCooldownTimer {get; set;}
		public int BossEncounterCooldownTimer {get; set;}
		
		public bool BossEncounterActive {get; set;}
		
		public Vector3D RandomEncounterDistanceCoordCheck {get; set;}
		public Vector3D InstallationDistanceCoordCheck {get; set;}
				
		public PlayerWatcher(){
			
			Player = null;
			
			SpaceCargoShipTimer = Settings.SpaceCargoShips.FirstSpawnTime;
			LunarCargoShipTimer = Settings.SpaceCargoShips.FirstSpawnTime;
			AtmoCargoShipTimer = Settings.PlanetaryCargoShips.FirstSpawnTime;
			RandomEncounterCheckTimer = Settings.RandomEncounters.SpawnTimerTrigger;
			PlanetaryInstallationCheckTimer = Settings.PlanetaryInstallations.SpawnTimerTrigger;
			BossEncounterCheckTimer = Settings.BossEncounters.SpawnTimerTrigger;
			
			RandomEncounterCoolDownTimer = 0;
			PlanetaryInstallationCooldownTimer = 0;
			BossEncounterCooldownTimer = 0;
			
			BossEncounterActive = false;
			
			RandomEncounterDistanceCoordCheck = new Vector3D(0,0,0);
			InstallationDistanceCoordCheck = new Vector3D(0,0,0);
			
		}
		
	}
	
}