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
using Sandbox.Game.Weapons;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Weapons;
using SpaceEngineers.Game.ModAPI;
using ProtoBuf;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Spawners;

namespace ModularEncountersSpawner.Spawners{

	public class WaveSpawner{
		
		public int CurrentWaveTimer;
		public int NextWaveTrigger;
		
		public string SpawnType;
		
		public bool SpawnWaves;
		public int NextSpawnTimer;
		public int SpawnedWaves;
		public Dictionary<Vector3D, int> WaveClusterPositions;
		
		public bool IsServer;
		public bool SetupComplete;
		
		public WaveSpawner(string spawnerType){
			
			CurrentWaveTimer = 0;
			NextWaveTrigger = 0;
			
			SpawnType = spawnerType;
			
			SpawnWaves = false;
			NextSpawnTimer = 0;
			SpawnedWaves = 0;
			WaveClusterPositions = new Dictionary<Vector3D, int>();
			
			IsServer = false;
			SetupComplete = false;
			
		}
		
		public void WaveSpawnerRun(){
			
			if(SetupComplete == false){
				
				SetupComplete = true;
				IsServer = MyAPIGateway.Multiplayer.IsServer;
				
				if(IsServer == true){
					
					int tempTrigger = 0;
					int tempTimer = 0;

					if(MyAPIGateway.Utilities.GetVariable<int>("MES-WaveSpawner-Trigger-" + SpawnType, out tempTrigger) == false){

						NextWaveTrigger = SpawnResources.rnd.Next(Settings.SpaceCargoShips.MinWaveSpawnTime, Settings.SpaceCargoShips.MaxWaveSpawnTime);
						MyAPIGateway.Utilities.SetVariable<int>("MES-WaveSpawner-Trigger-" + SpawnType, NextWaveTrigger);
						
					}else{
						
						NextWaveTrigger = tempTrigger;
						
					}
					
					if(MyAPIGateway.Utilities.GetVariable<int>("MES-WaveSpawner-Timer-" + SpawnType, out tempTimer) == false){
						
						MyAPIGateway.Utilities.SetVariable<int>("MES-WaveSpawner-Timer-" + SpawnType, CurrentWaveTimer);
						
					}else{
						
						CurrentWaveTimer = tempTimer;
						
					}

				}
				
			}
			
			if(IsServer == false){
				
				return;
				
			}

			CurrentWaveTimer++;
			MyAPIGateway.Utilities.SetVariable<int>("MES-WaveSpawner-Timer-" + SpawnType, CurrentWaveTimer);
			
			if(CurrentWaveTimer >= NextWaveTrigger){
				
				Logger.AddMsg("Wave Spawner (Space Cargo Ship) Activated");
				CurrentWaveTimer = 0;
				MyAPIGateway.Utilities.SetVariable<int>("MES-WaveSpawner-Timer-" + SpawnType, CurrentWaveTimer);
				NextSpawnTimer = 0;
				SpawnedWaves = 0;
				WaveClusterPositions.Clear();
				NextWaveTrigger = SpawnResources.rnd.Next(Settings.SpaceCargoShips.MinWaveSpawnTime, Settings.SpaceCargoShips.MaxWaveSpawnTime);
				MyAPIGateway.Utilities.SetVariable<int>("MES-WaveSpawner-Trigger-" + SpawnType, NextWaveTrigger);
				var playerList = new List<IMyPlayer>();
				MyAPIGateway.Players.GetPlayers(playerList);
				SpawnWaves = true;
				
				foreach(var player in playerList){
					
					if(player.IsBot == true || player.Character == null){
						
						continue;
						
					}
					
					bool tooClose = false;
					
					foreach(var coords in WaveClusterPositions.Keys){
						
						if(Vector3D.Distance(coords, player.GetPosition()) < Settings.SpaceCargoShips.PlayerClusterDistance){
							
							tooClose = true;
							break;
							
						}
						
					}
					
					if(tooClose == true){
						
						continue;
						
					}
					
					if(WaveClusterPositions.ContainsKey(player.GetPosition()) == false){
						
						WaveClusterPositions.Add(player.GetPosition(), 0);
						
					}

				}
				
			}
			
			if(SpawnWaves == false){
				
				return;
				
			}
			
			NextSpawnTimer++;
			
			if(NextSpawnTimer < Settings.SpaceCargoShips.TimeBetweenWaveSpawns){
				
				return;
				
			}
			
			NextSpawnTimer = 0;
			
			foreach(var coords in WaveClusterPositions.Keys.ToList()){
				
				List<string> SpecificGroup = new List<string>(Settings.SpaceCargoShips.UseSpecificRandomGroups.ToList());
				SpecificGroup.Remove("SomeSpawnGroupNameHere");
				SpecificGroup.Remove("AnotherSpawnGroupNameHere");
				SpecificGroup.Remove("EtcEtcEtc");
				
				if(SpecificGroup.Count > 0){
					
					SpawnGroupManager.AdminSpawnGroup = SpecificGroup[SpawnResources.rnd.Next(0, SpecificGroup.Count)];
					
				}
				
				if(SpawnType == "SpaceCargoShip"){
					
					Logger.AddMsg("Wave Spawner Event:");
					var result = SpaceCargoShipSpawner.AttemptSpawn(coords);
					Logger.AddMsg(result);
					
				}
				
				WaveClusterPositions[coords]++;
				
				if(WaveClusterPositions[coords] >= Settings.SpaceCargoShips.TotalSpawnEventsPerCluster){
					
					WaveClusterPositions.Remove(coords);
					
				}
				
				break;
				
			}
			
			if(WaveClusterPositions.Keys.Count == 0){
				
				Logger.AddMsg("Wave Spawner (Space Cargo Ship) Suspended.");
				SpawnWaves = false;
				
			}
			
		}
		
	}
	
}