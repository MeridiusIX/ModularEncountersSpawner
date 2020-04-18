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
using ModularEncountersSpawner.Api;
using DefenseShields;

namespace ModularEncountersSpawner{
	
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
	
	public class MES_SessionCore : MySessionComponentBase{
		
		public static float ModVersion = 1.081f;
		public static string SaveName = "";
		public static int PlayerWatcherTimer = 0;
		public static Dictionary<IMyPlayer, PlayerWatcher> playerWatchList = new Dictionary<IMyPlayer, PlayerWatcher>();
		public static List<IMyPlayer> PlayerList = new List<IMyPlayer>();
		public static List<ulong> ActiveMods = new List<ulong>();
		
		//Voxel Clearing List
		public static bool VoxelsToRemove = false;
		public static bool OtherTick = false;
		public static List<MatrixD> RemoveVoxels = new List<MatrixD>();
		public static List<MatrixD> RemoveVoxelsAgain = new List<MatrixD>();
		
		//Wave Spawner Instances
		public static WaveSpawner SpaceCargoShipWaveSpawner;
		
		public static long modId = 1521905890;
		
		//Mod Message Receivers
		public static long blockReplacerModId = 1521905890001;
		public static long manualSpawnRequestModId = 1521905890002;

		public static IMyGps BossEncounterGps;
		
		public static bool NPCWeaponUpgradesModDetected = false;
		public static bool SpaceWaveSpawnerModDetected = false;
		public static bool spawningInProgress = false;

		//WeaponCore and DefenseSheld API Requirements
		internal static MES_SessionCore Instance { get; private set; }
		public ulong ShieldModId = 1365616918;
		public bool ShieldMod { get; set; }
		public bool ShieldApiLoaded { get; set; }
		public ShieldApi DefenseShields = new ShieldApi();

		public ulong WeaponCoreModId = 1918681825;
		public bool WeaponCoreMod { get; set; }
		public bool WeaponCoreLoaded { get; set; }
		public WcApi WeaponCore = new WcApi();

		bool scriptInit = false;
		bool scriptFail = false;
		int tickCounter = 0;
		int tickCounterIncrement = 1;
		Random rnd = new Random();
		
		[Serializable]
		public struct SyncContents{
			
			long PlayerId;
			ulong SteamId;
			string MessageType;
			string Message;
			
		}

		public override void LoadData() {

			if (MyAPIGateway.Multiplayer.IsServer == false)
				return;

			Instance = this;

			foreach (var mod in MyAPIGateway.Session.Mods) {

				if (mod.PublishedFileId == ShieldModId) {

					Logger.AddMsg("Defense Shield Mod Detected");
					ShieldMod = true;
					continue;

				}

				if (mod.PublishedFileId == WeaponCoreModId || mod.Name.Contains("WeaponCore-Local")) {

					Logger.AddMsg("WeaponCore Mod Detected");
					WeaponCoreMod = true;

				}

			}

		}

		public override void BeforeStart() {

			if (!MyAPIGateway.Multiplayer.IsServer)
				return;

			SpawnerLocalApi.SendApiToMods();

			try {

				if (WeaponCoreMod && !Instance.WeaponCore.IsReady) {

					Logger.AddMsg("WeaponCore API Loading");
					WeaponCore.Load(WcApiCallback, true);

				} else {

					Logger.AddMsg("WeaponCore API Failed To Load For Unknown Reason");

				}

			} catch (Exception exc) {

				Logger.AddMsg("WeaponCore API Failed To Load");
				Logger.AddMsg(exc.ToString());

			}

			try {

				Logger.AddMsg("Defense Shields API Loading");

				if (!ShieldMod) {

					//Check for compatible alts
					var definitions = MyDefinitionManager.Static.GetAllDefinitions();
					var npcEmitter = new MyDefinitionId(typeof(MyObjectBuilder_UpgradeModule), "NPCEmitterLB");

					foreach (var def in definitions) {

						if (def.Id == npcEmitter) {

							Logger.AddMsg("Alternate Defense Shield Mod Detected");
							ShieldMod = true;
							break;

						}
					
					}
				
				}

				if (ShieldMod && !ShieldApiLoaded) {

					Instance.DefenseShields.Load();

				}

			} catch (Exception exc) {

				Logger.AddMsg("Defense Shields API Failed To Load With Exception: ");
				Logger.AddMsg(exc.ToString());

			}

		}

		public void WcApiCallback() {

			if (MES_SessionCore.Instance.WeaponCore.IsReady) {

				Logger.AddMsg("WeaponCore Successfully Loaded");
				MES_SessionCore.Instance.WeaponCoreLoaded = true;
				MES_SessionCore.Instance.WeaponCore.GetAllCoreWeapons(GridBuilderManipulation.AllWeaponCoreIDs);
				MES_SessionCore.Instance.WeaponCore.GetAllCoreStaticLaunchers(GridBuilderManipulation.AllWeaponCoreStaticIDs);
				MES_SessionCore.Instance.WeaponCore.GetAllCoreTurrets(GridBuilderManipulation.AllWeaponCoreTurretIDs);

				if (Logger.LoggerDebugMode) {

					Logger.AddMsg("Static Weapons", true);
					foreach (var statics in GridBuilderManipulation.AllWeaponCoreStaticIDs) {

						Logger.AddMsg(" - " + statics.ToString(), true);

					}

					Logger.AddMsg("Turret Weapons", true);
					foreach (var turret in GridBuilderManipulation.AllWeaponCoreTurretIDs) {

						Logger.AddMsg(" - " + turret.ToString(), true);

					}

				}
	
			} else {

				Logger.AddMsg("WeaponCore API Failed To Load");

			}

		}

		public override void UpdateBeforeSimulation(){
			
			if(scriptInit == false){
				
				scriptInit = true;
				SetupScript();
				
			}
			
			if(scriptFail == true){
				
				return;
				
			}
			
			//Voxels to Remove Get Processed Twice - Sometimes game misses some cells.
			if(VoxelsToRemove == true){
				
				if(OtherTick == false){
					
					if(RemoveVoxels.Count > 0){
						
						var index = RemoveVoxels.Count - 1;
						CutVoxelsAtAirtightPositions(RemoveVoxels[index]);
						RemoveVoxelsAgain.Add(RemoveVoxels[index]);
						RemoveVoxels.RemoveAt(index);
						
					}else{
						
						OtherTick = true;
						
					}
					
				}else{
					
					if(RemoveVoxelsAgain.Count > 0){
						
						var index = RemoveVoxelsAgain.Count - 1;
						CutVoxelsAtAirtightPositions(RemoveVoxelsAgain[index]);
						RemoveVoxelsAgain.RemoveAt(index);
						
					}else{
						
						OtherTick = false;
						VoxelsToRemove = false;
						
					}

				}
		
			}

			if(NPCWatcher.DeleteGrids == true && MyAPIGateway.Multiplayer.IsServer == true) {

				NPCWatcher.DeletionTimer++;

				if(NPCWatcher.DeletionTimer >= 10) {

					NPCWatcher.DeletionTimer = 0;
					NPCWatcher.DeleteGridsProcessing();

				}

			}

			tickCounter += tickCounterIncrement;
			
			if(tickCounter < 60){
				
				return;
				
			}
			
			tickCounter = 0;
						
			if(MyAPIGateway.Multiplayer.IsServer == false){
				
				return;
				
			}

			if(SaveName != MyAPIGateway.Session.Name) {

				Logger.AddMsg("New Save Detected. Applying Existing Settings To Save File.");
				SaveName = MyAPIGateway.Session.Name;
				Settings.General.SaveSettings(Settings.General);
				Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
				Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
				Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
				Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
				Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
				Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
				Settings.CustomBlocks.SaveSettings(Settings.CustomBlocks);

			}

			//Temporary Until Keen Fixes NPCs Randomly Stopping
			CargoShipWatcher.ProcessCargoShipSpeedWatcher();
			
			if(NPCWatcher.PendingNPCs.Count == 0){
				
				bool waveSpawnerActive = false;
				
				//WaveSpawner - SpaceCargoShip
				if(Settings.SpaceCargoShips.EnableWaveSpawner == true || SpaceWaveSpawnerModDetected == true){
					
					SpaceCargoShipWaveSpawner.WaveSpawnerRun();
					
					if(SpaceCargoShipWaveSpawner.SpawnWaves == true){
						
						waveSpawnerActive = true;
						
					}
					
				}
				
				//Regular Spawners
				if(waveSpawnerActive == false){
					
					PlayerWatcherTimer--;
					
					if(PlayerWatcherTimer <= 0){

						RelationManager.InitialReputationFixer();

						PlayerWatcherTimer = Settings.General.PlayerWatcherTimerTrigger;
						ProcessPlayerWatchList();
						
					}

				}
				
			}
			
			TerritoryManager.TerritoryWatcher();
			NPCWatcher.BossSignalWatcher();
			NPCWatcher.ActiveNpcMonitor();

		}
		
		public void SetupScript(){
			
			Logger.AddMsg("Loading Settings From Spawner Version: " + ModVersion.ToString());

			//Save File Validation
			SaveName = MyAPIGateway.Session.Name;

			//Setup Watchers and Handlers
			MyAPIGateway.Multiplayer.RegisterMessageHandler(8877, ChatCommand.MESMessageHandler);
			MyAPIGateway.Utilities.MessageEntered += ChatCommand.MESChatCommand;	
			var thisPlayer = MyAPIGateway.Session.LocalHumanPlayer;

			if (MyAPIGateway.Multiplayer.IsServer == false) {

				//Client Setup Ends Here
				return;

			}

			//Rival AI Stuff
			Logger.AddMsg("Initializing RivalAI Helper");
			RivalAIHelper.SetupRivalAIHelper();

			//Disable Vanilla Spawners
			Logger.AddMsg("Checking World Settings.");
			if(MyAPIGateway.Session.SessionSettings.CargoShipsEnabled == true){
				
				Logger.AddMsg("Disabling Cargo Ships World Setting. Spawner Handles This Functionality.");
				MyAPIGateway.Session.SessionSettings.CargoShipsEnabled = false;
				
			}
			
			if(MyAPIGateway.Session.SessionSettings.EnableEncounters == true){
				
				Logger.AddMsg("Disabling Random Encounters World Setting. Spawner Handles This Functionality.");
				MyAPIGateway.Session.SessionSettings.EnableEncounters = false;
				
			}
			
			/*
			if(MyAPIGateway.Multiplayer.IsServer == false){

				if(thisPlayer == null){
					
					Logger.AddMsg("Player Doesn't Exist. Cannot Search For Existing Boss GPS.");
					return;
					
				}

				Logger.AddMsg("Searching For Existing Boss Encounter GPS.");
				var chatMsg = "MESClientGetBossGPS\n" + thisPlayer.IdentityId.ToString() + "\n" + thisPlayer.SteamUserId.ToString() + "\n" + "Msg";
				var sendData = MyAPIGateway.Utilities.SerializeToBinary<string>(chatMsg);
				var sendMsg = MyAPIGateway.Multiplayer.SendMessageToServer(8877, sendData);
				
				return;
				
			}
			*/
			
			//All Block SubtypeIds
			try{
				
				var allDefs = MyDefinitionManager.Static.GetAllDefinitions();
				
				foreach(MyDefinitionBase definition in allDefs.Where( x => x is MyCubeBlockDefinition)){
					
					var blockDef = definition as MyCubeBlockDefinition;
					SpawnResources.BlockDefinitionIdList.Add(definition.Id.SubtypeName);

					if(ChatCommand.BlockDefinitionList.Contains(blockDef) == false) {

						ChatCommand.BlockDefinitionList.Add(blockDef);

					}
					
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("Something Failed While Building List Of CubeBlock SubtypeIds.");
				
			}
			
			
			//Drop Containers Names
			var dropContainerErrorLog = new StringBuilder();
			try{
				
				dropContainerErrorLog.Append("Getting List of DropContainer Definitions").AppendLine();
				var dropContainerDefs = MyDefinitionManager.Static.GetDropContainerDefinitions();
				dropContainerErrorLog.Append("Beginning Loop Of Definition List").AppendLine();
				
				foreach(var dropContainer in dropContainerDefs.Keys){
					
					dropContainerErrorLog.Append("Checking Drop Container Prefab: ").Append(dropContainerDefs[dropContainer].Id.SubtypeName).AppendLine();
					
					foreach(var grid in dropContainerDefs[dropContainer].Prefab.CubeGrids){
						
						dropContainerErrorLog.Append("Checking Drop Container Prefab Name...").AppendLine();
						if(string.IsNullOrEmpty(grid.DisplayName) == true){
							
							dropContainerErrorLog.Append("Prefab Grid Name Null Or Empty - Skipping").AppendLine();
							continue;
							
						}
						
						dropContainerErrorLog.Append("Added Prefab Grid Name To DropContainerNames: ").Append(grid.DisplayName).AppendLine();
						NPCWatcher.DropContainerNames.Add(grid.DisplayName);
						
					}
		
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("Something Failed While Building List Of Drop Container (Unknown Signal) Prefab Names. See Below:");
				Logger.AddMsg(dropContainerErrorLog.ToString());
				
			}

			//Economy Stations
			
			Logger.AddMsg("The Following Economy Stations Will Not Be Monitored By Spawner Mod:", true);
			try {

				NPCWatcher.EconomyStationNames.Add("Economy_MiningStation_1");
				NPCWatcher.EconomyStationNames.Add("Economy_MiningStation_2");
				NPCWatcher.EconomyStationNames.Add("Economy_MiningStation_3");
				NPCWatcher.EconomyStationNames.Add("Economy_OrbitalStation_1");
				NPCWatcher.EconomyStationNames.Add("Economy_OrbitalStation_2");
				NPCWatcher.EconomyStationNames.Add("Economy_OrbitalStation_3");
				NPCWatcher.EconomyStationNames.Add("Economy_OrbitalStation_4");
				NPCWatcher.EconomyStationNames.Add("Economy_Outpost_1");
				NPCWatcher.EconomyStationNames.Add("Economy_Outpost_2");
				NPCWatcher.EconomyStationNames.Add("Economy_Outpost_3");
				NPCWatcher.EconomyStationNames.Add("Economy_Outpost_4");
				NPCWatcher.EconomyStationNames.Add("Economy_Outpost_5");
				NPCWatcher.EconomyStationNames.Add("Economy_Outpost_6");
				NPCWatcher.EconomyStationNames.Add("Economy_Outpost_7");
				NPCWatcher.EconomyStationNames.Add("Economy_SpaceStation_1");
				NPCWatcher.EconomyStationNames.Add("Economy_SpaceStation_2");
				NPCWatcher.EconomyStationNames.Add("Economy_SpaceStation_3");
				NPCWatcher.EconomyStationNames.Add("Economy_SpaceStation_4");
				NPCWatcher.EconomyStationNames.Add("Economy_SpaceStation_5");

				/*
				var ecoStationsDefs = new List<MyStationsListDefinition>(MyDefinitionManager.Static.GetDefinitionsOfType<MyStationsListDefinition>().ToList());
			   
				foreach(var station in ecoStationsDefs) {

					foreach(var stationPrefabName in station.StationNames) {

						var stationNameString = stationPrefabName.ToString();

						if(string.IsNullOrWhiteSpace(stationNameString) == true) {

							continue;

						}

						if(NPCWatcher.EconomyStationNames.Contains(stationNameString) == false) {

							NPCWatcher.EconomyStationNames.Add(stationNameString);
							Logger.AddMsg("Economy Station: " + stationNameString);

						}

					}

				}
				*/

			} catch(Exception exc) {

				Logger.AddMsg("Something Failed While Building List Of Economy Station Prefab Names. See Below:");
				Logger.AddMsg(dropContainerErrorLog.ToString());

			}

			Logger.AddMsg("Registering Mod Message Handlers.");
			Logger.AddMsg("Mod Channel: " + MyAPIGateway.Utilities.GamePaths.ModScopeName);
			//MyAPIGateway.Utilities.RegisterMessageHandler(1521905890, ModMessages.ModMessageHandler);
			MyAPIGateway.Utilities.RegisterMessageHandler(1521905890001, ModMessages.ModMessageReceiverBlockReplace);
			MyAPIGateway.Utilities.RegisterMessageHandler(1521905890002, ModMessages.ModMessageReceiverSpawnRequest);
			MyAPIGateway.Utilities.RegisterMessageHandler(1521905890003, ModMessages.ModMessageReceiverRivalAISpawnRequest);
			Logger.AddMsg("Initiating Main Settings.");
			Settings.InitSettings();
			NPCWatcher.InitFactionData();
			SpawnResources.PopulateNpcFactionLists();
			TerritoryManager.TerritoryRefresh();
			SpawnGroupManager.CreateSpawnLists();
			
			string[] uniqueSpawnedArray = new string[0];
			if(MyAPIGateway.Utilities.GetVariable<string[]>("MES-UniqueGroupsSpawned", out uniqueSpawnedArray) == true){
				
				SpawnGroupManager.UniqueGroupsSpawned = new List<string>(uniqueSpawnedArray.ToList());
				
			}else{
				
				Logger.AddMsg("Failed To Retrieve Previously Spawned Unique Encounters List or No Unique Encounters Have Spawned Yet.");
				
			}
			
			//Setup Existing Boss Encounters
			string storedBossData = "";
			
			if(MyAPIGateway.Utilities.GetVariable<string>("MES-ActiveBossEncounters", out storedBossData) == true){
				
				if(storedBossData != ""){

					try{
						
						var byteArray = Convert.FromBase64String(storedBossData);
						var storedData = MyAPIGateway.Utilities.SerializeFromBinary<BossEncounter[]>(byteArray);
						NPCWatcher.BossEncounters = new List<BossEncounter>(storedData.ToList());
						bool listChange = false;
						
						if(NPCWatcher.BossEncounters.Count > 0){
							
							for(int i = NPCWatcher.BossEncounters.Count - 1; i >= 0; i--){
								
								var bossEncounter = NPCWatcher.BossEncounters[i];
								bossEncounter.SpawnGroup = SpawnGroupManager.GetSpawnGroupByName(bossEncounter.SpawnGroupName);
								
								if(bossEncounter.SpawnGroup == null){
									
									bossEncounter.RemoveGpsForPlayers();
									listChange = true;
									NPCWatcher.BossEncounters.RemoveAt(i);
									
								}
								
							}
						
						}
						
						if(listChange == true){
							
							if(NPCWatcher.BossEncounters.Count > 0){
								
								BossEncounter[] encounterArray = NPCWatcher.BossEncounters.ToArray();
								byteArray = MyAPIGateway.Utilities.SerializeToBinary<BossEncounter[]>(encounterArray);
								storedBossData = Convert.ToBase64String(byteArray);
								MyAPIGateway.Utilities.SetVariable<string>("MES-ActiveBossEncounters", storedBossData);
								
							}else{
								
								MyAPIGateway.Utilities.SetVariable<string>("MES-ActiveBossEncounters", "");
								
							}

						}
						
					}catch(Exception e){
						
						Logger.AddMsg("Something went wrong while getting Boss Encounter Data from Storage.");
						Logger.AddMsg(e.ToString(), true);
						
					}

				}
				
			}
			
			//Get Active Mods
			Logger.AddMsg("Getting Active Mods.");
			foreach(var mod in MyAPIGateway.Session.Mods){
				
				if(mod.PublishedFileId != 0){
					
					ActiveMods.Add(mod.PublishedFileId);
					
				}
				
				/*if(mod.PublishedFileId == 1135484377 || mod.PublishedFileId == 973528334){
					
					string msgA = "Conflicting Mod Detected: " + mod.FriendlyName;
					MyVisualScriptLogicProvider.ShowNotificationToAll(msgA, 15000, "Red");
					Logger.AddMsg(msgA);
					conflictingSettings = true;
					
				}*/
				
			}
			
			if(ActiveMods.Contains(1555044803) == true){
				
				Logger.AddMsg("NPC Weapon Upgrades Mod Detected. Enabling Weapon Randomization.");
				NPCWeaponUpgradesModDetected = true;
				
			}
			
			if(ActiveMods.Contains(1773965697) == true){
				
				Logger.AddMsg("Wave Spawner (Space) Mod Detected. Enabling Wave Spawning for SpaceCargoShips.");
				SpaceWaveSpawnerModDetected = true;
				
			}
			
			bool suppressCargo = false;
			bool suppressEncounter = false;
			
			if(ActiveMods.Contains(888457124) == true){
				
				suppressCargo = true;
				
			}
			
			if(ActiveMods.Contains(888457381) == true){
				
				suppressEncounter = true;
				
			}

			NPCShieldManager.DefenseShieldModLoaded = ActiveMods.Contains(1365616918) || ShieldMod;
			NPCShieldManager.NPCShieldProviderModLoaded = ActiveMods.Contains(2043339470);
			NPCShieldManager.InitializeArmorBlockList();

			SuppressGroups.ApplySuppression(suppressCargo, suppressEncounter);
			
			//Init Timers
			PlayerWatcherTimer = Settings.General.PlayerWatcherTimerTrigger;
			NPCWatcher.NpcDistanceCheckTimer = Settings.General.NpcDistanceCheckTimerTrigger;
			NPCWatcher.NpcOwnershipCheckTimer = Settings.General.NpcOwnershipCheckTimerTrigger;
			NPCWatcher.NpcCleanupCheckTimer = Settings.General.NpcCleanupCheckTimerTrigger;
			NPCWatcher.SpawnedVoxelCheckTimer = Settings.General.SpawnedVoxelCheckTimerTrigger;
			SpawnResources.RefreshEntityLists();
			
			//Setup Watchers and Handlers
			MyAPIGateway.Entities.OnEntityAdd += NPCWatcher.NewEntityDetected;
			
			//Get Initial Players
			PlayerList.Clear();
			MyAPIGateway.Players.GetPlayers(PlayerList);

			//Get Existing NPCs
			Logger.AddMsg("Check For Existing NPC Grids");
			NPCWatcher.StartupScan();
			
			//Setup Wave Spawners
			SpaceCargoShipWaveSpawner = new WaveSpawner("SpaceCargoShip");

			//Init Economy Stuff
			Logger.AddMsg("Initializing Economy Resources");
			EconomyHelper.Setup();
			
			//Get Spawned Voxels From Save
			try{
				
				string[] tempSpawnedVoxels = new string[0];
				
				if(MyAPIGateway.Utilities.GetVariable<string[]>("MES-SpawnedVoxels", out tempSpawnedVoxels) == true){
					
					foreach(var voxelId in tempSpawnedVoxels){
						
						long tempId = 0;
						
						if(long.TryParse(voxelId, out tempId) == false){
							
							continue;
							
						}
						
						IMyEntity voxelEntity = null;
						
						if(MyAPIGateway.Entities.TryGetEntityById(tempId, out voxelEntity) == false){
							
							continue;
							
						}
						
						if(NPCWatcher.SpawnedVoxels.ContainsKey(voxelId) == false){
							
							NPCWatcher.SpawnedVoxels.Add(voxelId, voxelEntity);
							
						}
		
					}
					
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("Something went wrong while trying to retrieve previously spawned voxels.");
				
			}
				
			
		}
		
		public void ProcessPlayerWatchList(){
			
			PlayerList.Clear();
			MyAPIGateway.Players.GetPlayers(PlayerList);
			
			foreach(var player in PlayerList){
				
				if(player.IsBot == true || player.Character == null){
					
					continue;
					
				}
				
				if(playerWatchList.ContainsKey(player) == true){

					//Regular Timers
					
					if(Settings.General.EnableSpaceCargoShips == true){
						
						playerWatchList[player].SpaceCargoShipTimer -= Settings.General.PlayerWatcherTimerTrigger;
						
					}
					
					if(Settings.General.EnablePlanetaryCargoShips == true){
						
						playerWatchList[player].AtmoCargoShipTimer -= Settings.General.PlayerWatcherTimerTrigger;
						
					}
					
					if(Settings.General.EnableRandomEncounters == true){
						
						//CoolDown Timers
						if(playerWatchList[player].RandomEncounterCoolDownTimer > 0){
							
							playerWatchList[player].RandomEncounterCoolDownTimer -= Settings.General.PlayerWatcherTimerTrigger;
							
						}else{
							
							playerWatchList[player].RandomEncounterCheckTimer -= Settings.General.PlayerWatcherTimerTrigger;
							
						}
						
						if(playerWatchList[player].RandomEncounterDistanceCoordCheck == Vector3D.Zero){
						
							playerWatchList[player].RandomEncounterDistanceCoordCheck = player.GetPosition();
							
						}
						
					}
					
					if(Settings.General.EnablePlanetaryInstallations  == true){
						
						if(playerWatchList[player].PlanetaryInstallationCooldownTimer > 0){
							
							playerWatchList[player].PlanetaryInstallationCooldownTimer -= Settings.General.PlayerWatcherTimerTrigger;
							
						}else{
							
							playerWatchList[player].PlanetaryInstallationCheckTimer -= Settings.General.PlayerWatcherTimerTrigger;
							
						}
						
					}
					
					if(Settings.General.EnableBossEncounters == true){
						
						if(BossEncounterSpawner.IsPlayerInBossEncounter(player.IdentityId) == false){
							
							if(playerWatchList[player].BossEncounterCooldownTimer > 0){
							
								playerWatchList[player].BossEncounterCooldownTimer -= Settings.General.PlayerWatcherTimerTrigger;
								
							}else{
								
								playerWatchList[player].BossEncounterCheckTimer -= Settings.General.PlayerWatcherTimerTrigger;
								
							}
							
						}
						
					}
					
					//Apply Increment to Timers and Engage Spawners When Appropriate
					if(playerWatchList[player].SpaceCargoShipTimer <= 0 && NPCWatcher.PendingNPCs.Count == 0){
					
						playerWatchList[player].SpaceCargoShipTimer = rnd.Next(Settings.SpaceCargoShips.MinSpawnTime, Settings.SpaceCargoShips.MaxSpawnTime);
						Logger.SkipNextMessage = true;
						Logger.AddMsg("Attempting Space/Lunar Cargo Ship Spawn Near Player: " + player.DisplayName);
						Logger.SkipNextMessage = true;
						var spawnResult = SpaceCargoShipSpawner.AttemptSpawn(player.GetPosition());
						Logger.AddMsg(spawnResult);
					
					}
					
					if(playerWatchList[player].AtmoCargoShipTimer <= 0 && NPCWatcher.PendingNPCs.Count == 0){
						
						playerWatchList[player].AtmoCargoShipTimer = rnd.Next(Settings.PlanetaryCargoShips.MinSpawnTime, Settings.PlanetaryCargoShips.MaxSpawnTime);
						Logger.SkipNextMessage = true;
						Logger.AddMsg("Attempting Planetary Cargo Ship Spawn Near Player: " + player.DisplayName);
						Logger.SkipNextMessage = true;
						var spawnResult = PlanetaryCargoShipSpawner.AttemptSpawn(player.GetPosition());
						Logger.AddMsg(spawnResult);
						
					}
					
					if(playerWatchList[player].RandomEncounterCheckTimer <= 0 && playerWatchList[player].RandomEncounterCoolDownTimer <= 0 && NPCWatcher.PendingNPCs.Count == 0){
						
						playerWatchList[player].RandomEncounterCheckTimer = Settings.RandomEncounters.SpawnTimerTrigger;
						
						if(Vector3D.Distance(player.GetPosition(), playerWatchList[player].RandomEncounterDistanceCoordCheck) >= Settings.RandomEncounters.PlayerTravelDistance){
							
							playerWatchList[player].RandomEncounterDistanceCoordCheck = player.GetPosition();
							Logger.SkipNextMessage = true;
							Logger.AddMsg("Attempting Random Encounter Spawn Near Player: " + player.DisplayName);
							Logger.SkipNextMessage = true;
							var spawnResult = RandomEncounterSpawner.AttemptSpawn(player.GetPosition());
							Logger.AddMsg(spawnResult);
							
							if(spawnResult.StartsWith("Spawning Group - ") == true){
								
								playerWatchList[player].RandomEncounterCoolDownTimer = Settings.RandomEncounters.PlayerSpawnCooldown;
								
							}
							
						}
						
					}
					
					if(playerWatchList[player].PlanetaryInstallationCheckTimer <= 0 && playerWatchList[player].PlanetaryInstallationCooldownTimer <= 0 && NPCWatcher.PendingNPCs.Count == 0){
						
						playerWatchList[player].PlanetaryInstallationCheckTimer = Settings.PlanetaryInstallations.SpawnTimerTrigger;
						Logger.SkipNextMessage = true;
						Logger.AddMsg("Attempting Planetary Installation Spawn Near Player: " + player.DisplayName);
						Logger.SkipNextMessage = true;
						var spawnResult = PlanetaryInstallationSpawner.AttemptSpawn(player.GetPosition(), player);
						Logger.AddMsg(spawnResult);
						
						if(spawnResult.StartsWith("Spawning Group - ") == true){
							
							playerWatchList[player].PlanetaryInstallationCooldownTimer = Settings.PlanetaryInstallations.PlayerSpawnCooldown;
							
						}
						
					}
					
					if(playerWatchList[player].BossEncounterCheckTimer <= 0 && NPCWatcher.PendingNPCs.Count == 0){
						
						playerWatchList[player].BossEncounterCheckTimer = Settings.BossEncounters.SpawnTimerTrigger;
						Logger.SkipNextMessage = true;
						Logger.AddMsg("Attempting Boss Encounter Spawn Near Player: " + player.DisplayName);
						Logger.SkipNextMessage = true;
						var spawnResult = BossEncounterSpawner.AttemptSpawn(player.GetPosition());
						Logger.AddMsg(spawnResult);
						
						if(spawnResult.StartsWith("Boss Encounter GPS Created") == true){
							
							playerWatchList[player].BossEncounterCooldownTimer = Settings.BossEncounters.PlayerSpawnCooldown;
							
						}
								
					}
					
				}else{
					
					var newPlayerWatcher = new PlayerWatcher();
					playerWatchList.Add(player, newPlayerWatcher);
					
				}
				
			}
			
		}
		
		public static void PlayerConnected(long identityId){
			
			
			
		}
		
		public static void CutVoxelsAtAirtightPositions(MatrixD cutMatrix){

			var sphere = new BoundingSphereD(cutMatrix.Translation, 1000);
			var mapList = new List<IMyVoxelBase>();
			MyAPIGateway.Session.VoxelMaps.GetInstances(mapList);
			
			for(int i = mapList.Count - 1; i >= 0; i--){
				
				if(mapList[i].PositionComp.WorldAABB.Intersects(sphere) == false){
					
					mapList.RemoveAt(i);
					
				}
				
			}
			
			var voxelTool = MyAPIGateway.Session.VoxelMaps.GetBoxVoxelHand();
			voxelTool.Boundaries = new BoundingBoxD(new Vector3D(-1.35, -1.35, -1.35), new Vector3D(1.35, 1.35, 1.35));
			voxelTool.Transform = cutMatrix;
			
			foreach(var voxel in mapList){
						
				MyAPIGateway.Session.VoxelMaps.CutOutShape(voxel, voxelTool);
				//Logger.AddMsg("Cut At: " + cellWorldSpace.ToString(), true);

			}
			
		}
		
		public void ModMessageReceiver(object payload){
			
			var payloadString = payload as string;
			
			if(payloadString == null){
				
				return;
				
			}
			
		}
		
		
		protected override void UnloadData(){
			
			MyAPIGateway.Utilities.MessageEntered -= ChatCommand.MESChatCommand;
			MyAPIGateway.Multiplayer.UnregisterMessageHandler(8877, ChatCommand.MESMessageHandler);
			
			if(MyAPIGateway.Multiplayer.IsServer == false){
				
				return;
				
			}
			
			MyAPIGateway.Utilities.UnregisterMessageHandler(1521905890002, ModMessages.ModMessageReceiverBlockReplace);
			MyAPIGateway.Utilities.UnregisterMessageHandler(1521905890001, ModMessages.ModMessageReceiverBlockReplace);
			MyAPIGateway.Utilities.UnregisterMessageHandler(1521905890, ModMessages.ModMessageHandler);
			
			if (WeaponCoreLoaded)
				WeaponCore.Unload();

			if (ShieldApiLoaded)
				DefenseShields.Unload();

			MyAPIGateway.Entities.OnEntityAdd -= NPCWatcher.NewEntityDetected;
			
		}
		
	}
	
}