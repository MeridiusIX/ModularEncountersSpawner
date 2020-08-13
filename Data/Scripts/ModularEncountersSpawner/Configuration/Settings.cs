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

namespace ModularEncountersSpawner.Configuration{
	
	public static class Settings{
		
		public static ConfigGeneral General = new ConfigGeneral();
		public static ConfigSpaceCargoShips SpaceCargoShips = new ConfigSpaceCargoShips();
		public static ConfigPlanetaryCargoShips PlanetaryCargoShips = new ConfigPlanetaryCargoShips();
		public static ConfigRandomEncounters RandomEncounters = new ConfigRandomEncounters();
		public static ConfigPlanetaryInstallations PlanetaryInstallations = new ConfigPlanetaryInstallations();
		public static ConfigBossEncounters BossEncounters = new ConfigBossEncounters();
		public static ConfigOtherNPCs OtherNPCs = new ConfigOtherNPCs();
		public static ConfigCustomBlocks CustomBlocks = new ConfigCustomBlocks();


		public static void InitSettings(){
			
			General = General.LoadSettings();
			SpaceCargoShips = SpaceCargoShips.LoadSettings();
			PlanetaryCargoShips = PlanetaryCargoShips.LoadSettings();
			RandomEncounters = RandomEncounters.LoadSettings();
			PlanetaryInstallations = PlanetaryInstallations.LoadSettings();
			BossEncounters = BossEncounters.LoadSettings();
			OtherNPCs = OtherNPCs.LoadSettings();
			CustomBlocks = CustomBlocks.LoadSettings();
			CheckGlobalEvents();
			
		}
		
		public static void CheckGlobalEvents(){
			
			if(General.UseGlobalEventsTimers == false){
				
				Logger.AddMsg("Global Events Timings Disabled. Using Default Or User Defined Settings.");
				return;
				
			}
			
			var allDefs = MyDefinitionManager.Static.GetAllDefinitions();
				
			foreach(MyDefinitionBase definition in allDefs.Where( x => x is MyGlobalEventDefinition)){
				
				var eventDef = definition as MyGlobalEventDefinition;
				
				if(eventDef.Id.SubtypeId.ToString() == "SpawnCargoShip"){
					
					Logger.AddMsg("Using Spawner Timings From Global Events For Space/Lunar Cargo Ships.");
					
					if(eventDef.FirstActivationTime != null){
						
						var span = (TimeSpan)eventDef.FirstActivationTime;
						SpaceCargoShips.FirstSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MinActivationTime != null){
						
						var span = (TimeSpan)eventDef.MinActivationTime;
						SpaceCargoShips.MinSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MaxActivationTime != null){
						
						var span = (TimeSpan)eventDef.MaxActivationTime;
						SpaceCargoShips.MaxSpawnTime = (int)span.TotalSeconds;
						
					}
					
				}
				
				if(eventDef.Id.SubtypeId.ToString() == "SpawnRandomEncounter"){
					
					Logger.AddMsg("Using Spawner Timings From Global Events For Random Encounters.");
					
					if(eventDef.FirstActivationTime != null){
						
						var span = (TimeSpan)eventDef.FirstActivationTime;
						//RandomEncounters.FirstSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MinActivationTime != null){
						
						var span = (TimeSpan)eventDef.MinActivationTime;
						//RandomEncounters.MinSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MaxActivationTime != null){
						
						var span = (TimeSpan)eventDef.MaxActivationTime;
						//RandomEncounters.MaxSpawnTime = (int)span.TotalSeconds;
						
					}
					
				}
				
				if(eventDef.Id.SubtypeId.ToString() == "SpawnBossEncounter"){
					
					Logger.AddMsg("Using Spawner Timings From Global Events For Boss Encounters.");
					
					if(eventDef.FirstActivationTime != null){
						
						var span = (TimeSpan)eventDef.FirstActivationTime;
						//BossEncounters.FirstSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MinActivationTime != null){
						
						var span = (TimeSpan)eventDef.MinActivationTime;
						//BossEncounters.MinSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MaxActivationTime != null){
						
						var span = (TimeSpan)eventDef.MaxActivationTime;
						//BossEncounters.MaxSpawnTime = (int)span.TotalSeconds;
						
					}
					
				}
				
				if(eventDef.Id.SubtypeId.ToString() == "SpawnAtmoCargoShip"){
					
					Logger.AddMsg("Using Spawner Timings From Global Events For Planetary Cargo Ships.");
					
					if(eventDef.FirstActivationTime != null){
						
						var span = (TimeSpan)eventDef.FirstActivationTime;
						PlanetaryCargoShips.FirstSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MinActivationTime != null){
						
						var span = (TimeSpan)eventDef.MinActivationTime;
						PlanetaryCargoShips.MinSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MaxActivationTime != null){
						
						var span = (TimeSpan)eventDef.MaxActivationTime;
						PlanetaryCargoShips.MaxSpawnTime = (int)span.TotalSeconds;
						
					}
					
				}
				
				if(eventDef.Id.SubtypeId.ToString() == "SpawnPlanetaryCargoShip"){
					
					Logger.AddMsg("Using Spawner Timings From Global Events For Planetary Installations.");
					
					if(eventDef.FirstActivationTime != null){
						
						var span = (TimeSpan)eventDef.FirstActivationTime;
						//PlanetaryInstallations.FirstSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MinActivationTime != null){
						
						var span = (TimeSpan)eventDef.MinActivationTime;
						//PlanetaryInstallations.MinSpawnTime = (int)span.TotalSeconds;
						
					}
					
					if(eventDef.MaxActivationTime != null){
						
						var span = (TimeSpan)eventDef.MaxActivationTime;
						//PlanetaryInstallations.MaxSpawnTime = (int)span.TotalSeconds;
						
					}
					
				}

				
			}

		}
		
	}
	
}