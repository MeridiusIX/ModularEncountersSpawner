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

namespace ModularEncountersSpawner.Templates{
	
	public class PlayerWatcher{
		
		public IMyPlayer Player;
		
		public int SpaceCargoShipTimer;
		public int AtmoCargoShipTimer;
		public int LunarCargoShipTimer;
		public int RandomEncounterCheckTimer;
		public int PlanetaryInstallationCheckTimer;
		public int BossEncounterCheckTimer;
		public int CreatureCheckTimer;

		public int RandomEncounterCoolDownTimer;
		public int PlanetaryInstallationCooldownTimer;
		public int BossEncounterCooldownTimer;
		
		public bool BossEncounterActive;
		
		public Vector3D RandomEncounterDistanceCoordCheck;
		public Vector3D InstallationDistanceCoordCheck;
				
		public PlayerWatcher(){
			
			Player = null;
			
			SpaceCargoShipTimer = Settings.SpaceCargoShips.FirstSpawnTime;
			LunarCargoShipTimer = Settings.SpaceCargoShips.FirstSpawnTime;
			AtmoCargoShipTimer = Settings.PlanetaryCargoShips.FirstSpawnTime;
			RandomEncounterCheckTimer = Settings.RandomEncounters.SpawnTimerTrigger;
			PlanetaryInstallationCheckTimer = Settings.PlanetaryInstallations.SpawnTimerTrigger;
			BossEncounterCheckTimer = Settings.BossEncounters.SpawnTimerTrigger;
			CreatureCheckTimer = SpawnResources.rnd.Next(Settings.Creatures.MinCreatureSpawnTime, Settings.Creatures.MaxCreatureSpawnTime);

			RandomEncounterCoolDownTimer = 0;
			PlanetaryInstallationCooldownTimer = 0;
			BossEncounterCooldownTimer = 0;
			
			BossEncounterActive = false;
			
			RandomEncounterDistanceCoordCheck = new Vector3D(0,0,0);
			InstallationDistanceCoordCheck = new Vector3D(0,0,0);
			
		}
		
	}
	
}