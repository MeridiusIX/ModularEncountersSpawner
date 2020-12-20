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
	
	public static class ChatCommand{

		public static List<MyCubeBlockDefinition> BlockDefinitionList = new List<MyCubeBlockDefinition>();

		public static void MESChatCommand(string messageText, ref bool sendToOthers){
			
			var thisPlayer = MyAPIGateway.Session.LocalHumanPlayer;
			bool isAdmin = false;
			
			if(thisPlayer == null){
				
				return;
				
			}
			
			if(thisPlayer.PromoteLevel == MyPromoteLevel.Admin || thisPlayer.PromoteLevel == MyPromoteLevel.Owner){
				
				isAdmin = true;
				
			}
			
			if(isAdmin == false){
				
				MyVisualScriptLogicProvider.ShowNotification("Access Denied. Spawner Chat Commands Only Available To Admin Players.", 5000, "Red", thisPlayer.IdentityId);
				return;
				
			}
			
			if(messageText.StartsWith("/MES.") == true){
				
				sendToOthers = false;
				var syncData = new SyncData();
				syncData.Instruction = "MESChatMsg";
				syncData.PlayerId = thisPlayer.IdentityId;
				syncData.SteamUserId = thisPlayer.SteamUserId;
				syncData.ChatMessage = messageText;
				syncData.PlayerPosition = thisPlayer.GetPosition();
				var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
				var sendMsg = MyAPIGateway.Multiplayer.SendMessageToServer(8877, sendData);
				
			}
			
		}
		
		public static void MESMessageHandler(byte[] data){
			
			var receivedData = MyAPIGateway.Utilities.SerializeFromBinary<SyncData>(data);
			
			if(receivedData.Instruction.StartsWith("MESChatMsg") == true){
				
				ServerChatProcessing(receivedData);
				
			}
			
			if(receivedData.Instruction.StartsWith("MESClipboard") == true){
				
				ClipboardProcessing(receivedData);
				
			}
			
			if(receivedData.Instruction.StartsWith("MESBossGPS") == true){
				
				BossEncounterGpsManager(receivedData);
				
			}
			
			if(receivedData.Instruction.StartsWith("ClientGetBossGPS") == true){
				
				/*ClientGetBossGPS(receivedData);*/
				
			}
			
			
		}
		
		public static void BossEncounterGpsManager(SyncData receivedData){
			
			var player = MyAPIGateway.Session.LocalHumanPlayer;
			
			if(player == null){
			
				return;
				
			}
			
			if(receivedData.Instruction == "MESBossGPSRemove"){
								
				if(MES_SessionCore.BossEncounterGps != null){
					
					try{
						
						MyAPIGateway.Session.GPS.RemoveLocalGps(MES_SessionCore.BossEncounterGps);
						MES_SessionCore.BossEncounterGps = null;
						
					}catch(Exception exc){
						
						
						
					}
					
				}
				
				return;
				
			}
			
			if(receivedData.Instruction == "MESBossGPSCreate"){
				
				var gpsCoords = receivedData.GpsCoords;
				
				if(gpsCoords == Vector3D.Zero){
					
					return;
					
				}
				
				foreach(var gps in MyAPIGateway.Session.GPS.GetGpsList(player.IdentityId)){
					
					if(gps.Coords == gpsCoords){
						
						Logger.AddMsg("Boss Encounter GPS Or Other GPS Already Exist At Coordinates.", true);
						return;
						
					}
					
				}
				
				Logger.AddMsg("Boss Encounter GPS Created.", true);
				MES_SessionCore.BossEncounterGps = MyAPIGateway.Session.GPS.Create(receivedData.GpsName/* + player.IdentityId.ToString()*/, "", gpsCoords, true);
				
				try{
					
					MyAPIGateway.Session.GPS.AddLocalGps(MES_SessionCore.BossEncounterGps);
					var syncData = receivedData;
					syncData.Instruction = "MESBossGPSColorServer";
					MyVisualScriptLogicProvider.SetGPSColor(receivedData.GpsName, Color.Magenta, 0);
					//syncData.GpsName += player.IdentityId.ToString();
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageToServer(8877, sendData);
					
				}catch(Exception exp){
					
					
					
				}
				
			}
			
			if(receivedData.Instruction == "MESBossGPSColorServer"){
				
				MyVisualScriptLogicProvider.SetGPSColor(receivedData.GpsName, Color.Magenta, receivedData.PlayerId);
				//var syncData = receivedData;
				//syncData.Instruction = "MESBossGPSClientRename";
				//var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
				//bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
				
			}
			
			if(receivedData.Instruction == "MESBossGPSClientRename"){
				
				var gpsList = MyAPIGateway.Session.GPS.GetGpsList(receivedData.PlayerId);
				
				foreach(var gps in gpsList){
					
					if(gps.Name == receivedData.GpsName){
						
						MyVisualScriptLogicProvider.ShowNotificationToAll("Found Local GPS", 5000);
						MES_SessionCore.BossEncounterGps = gps;
						MyAPIGateway.Session.GPS.RemoveLocalGps(gps);
						MES_SessionCore.BossEncounterGps.Name = receivedData.GpsName.Replace(receivedData.PlayerId.ToString(), "");
						MyAPIGateway.Session.GPS.AddLocalGps(MES_SessionCore.BossEncounterGps);
						
					}
					
				}
				
			}

		}
		
		/*
		public static void ClientGetBossGPS(SyncData receivedData){
			
			foreach(var boss in NPCWatcher.BossEncounters){
				
				if(boss.PlayersInEncounter.Contains(receivedData.PlayerId) == true){
					
					var syncData = new SyncData();
					syncData.Instruction = "MESBossGPSCreate";
					syncData.PlayerId = receivedData.PlayerId;
					syncData.SteamUserId = receivedData.SteamUserId;
					syncData.GpsName = boss.GpsTemplate.Name;
					syncData.GpsCoords = boss.GpsTemplate.Coords;
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					
				}
				
			}
			
		}
		*/
		
		public static void ClipboardProcessing(SyncData receivedData){
			
			var player = MyAPIGateway.Session.LocalHumanPlayer;
			
			if(player == null || string.IsNullOrEmpty(receivedData.ClipboardContents) == true){
				
				return;
				
			}
			
			VRage.Utils.MyClipboardHelper.SetClipboard(receivedData.ClipboardContents);
			//MyAPIGateway.Utilities.ShowMissionScreen("Chat Command Results", "Details Saved To Game Log", "", receivedData.ClipboardContents, null, "Close");
			
		}
		
		public static void ServerChatProcessing(SyncData receivedData){

			if(receivedData.ChatMessage.StartsWith("/MES.SSCS") == true) {

				receivedData.ChatMessage = receivedData.ChatMessage.Replace("/MES.SSCS", "/MES.Spawn.SpaceCargoShip");

			}

			if(receivedData.ChatMessage.StartsWith("/MES.SRE") == true) {

				receivedData.ChatMessage = receivedData.ChatMessage.Replace("/MES.SRE", "/MES.Spawn.RandomEncounter");

			}

			if(receivedData.ChatMessage.StartsWith("/MES.SPCS") == true) {

				receivedData.ChatMessage = receivedData.ChatMessage.Replace("/MES.SPCS", "/MES.Spawn.PlanetaryCargoShip");

			}

			if(receivedData.ChatMessage.StartsWith("/MES.SPI") == true) {

				receivedData.ChatMessage = receivedData.ChatMessage.Replace("/MES.SPI", "/MES.Spawn.PlanetaryInstallation");

			}

			if(receivedData.ChatMessage.StartsWith("/MES.SBE") == true) {

				receivedData.ChatMessage = receivedData.ChatMessage.Replace("/MES.SBE", "/MES.Spawn.BossEncounter");

			}

			//Debug Commands
			if(receivedData.ChatMessage.StartsWith("/MES.") == true){

				//ChangeCounter
				if (receivedData.ChatMessage.StartsWith("/MES.ChangeCounter.")) {

					var msgSplit = receivedData.ChatMessage.Split('.');

					if (msgSplit.Length != 4) {

						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received", 5000, "White", receivedData.PlayerId);
						return;

					}

					int existingAmount = 0;
					int newAmount = 0;

					bool amountGot = int.TryParse(msgSplit[3], out newAmount);
					bool existingGot = MyAPIGateway.Utilities.GetVariable<int>(msgSplit[2], out existingAmount);

					if (!amountGot) {

						MyVisualScriptLogicProvider.ShowNotification("Could not parse amount to modify counter", 5000, "White", receivedData.PlayerId);
						return;

					}

					MyVisualScriptLogicProvider.ShowNotification("Value for Counter: " + msgSplit[2] + " ::: " + existingAmount, 5000, "White", receivedData.PlayerId);

					if((existingAmount + newAmount) != existingAmount)
						MyVisualScriptLogicProvider.ShowNotification("New Value for Counter: " + msgSplit[2] + " ::: " + (existingAmount + newAmount), 5000, "White", receivedData.PlayerId);

					MyAPIGateway.Utilities.SetVariable<int>(msgSplit[2], existingAmount + newAmount);
					return;

				}

				//ResetReputation
				if (receivedData.ChatMessage.StartsWith("/MES.ResetReputation.")) {

					var msgSplit = receivedData.ChatMessage.Split('.');

					if (msgSplit.Length != 3) {

						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received", 5000, "White", receivedData.PlayerId);
						return;

					}

					var result = RelationManager.ResetFactionReputation(msgSplit[2]);
					MyVisualScriptLogicProvider.ShowNotification("Faction [" + msgSplit[2] + "] Reputation Reset Result: " + result, 5000, "White", receivedData.PlayerId);
					return; 

				}

				//CreateKPL
				if (receivedData.ChatMessage.StartsWith("/MES.CreateKPL.")) {

					var msgSplit = receivedData.ChatMessage.Split('.');

					if (msgSplit.Length < 3) {

						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received", 5000, "White", receivedData.PlayerId);
						return;

					}

					string faction = msgSplit[2];
					double radius = 10000;
					int duration = -1;
					int maxEncounters = -1;
					int minThreat = -1;

					if (msgSplit.Length >= 4)
						double.TryParse(msgSplit[3], out radius);

					if (msgSplit.Length >= 5)
						int.TryParse(msgSplit[4], out duration);

					if (msgSplit.Length >= 6)
						int.TryParse(msgSplit[5], out maxEncounters);

					if (msgSplit.Length >= 7)
						int.TryParse(msgSplit[6], out minThreat);

					KnownPlayerLocationManager.AddKnownPlayerLocation(receivedData.PlayerPosition, faction, radius, duration, maxEncounters, minThreat);
					return;

				}

				//Prefab Spawn
				if (receivedData.ChatMessage.StartsWith("/MES.PrefabSpawn.")) {

					var msgSplit = receivedData.ChatMessage.Split('.');

					if (msgSplit.Length != 3) {

						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received", 5000, "White", receivedData.PlayerId);
						return;

					}

					var prefab = MyDefinitionManager.Static.GetPrefabDefinition(msgSplit[2]);

					if (prefab == null) {

						MyVisualScriptLogicProvider.ShowNotification("Could Not Find Prefab With Name: " + msgSplit[2], 5000, "White", receivedData.PlayerId);
						return;

					}

					var matrix = MatrixD.Identity;
					matrix.Translation = receivedData.PlayerPosition;
					var player = SpawnResources.GetPlayerById(receivedData.PlayerId);

					if (player?.Character != null)
						matrix = player.Character.WorldMatrix;

					Vector3D coords = (prefab.BoundingSphere.Radius * 1.2) * matrix.Forward + matrix.Translation;

					var dummyList = new List<IMyCubeGrid>();
					MyAPIGateway.PrefabManager.SpawnPrefab(dummyList, msgSplit[2], coords, matrix.Backward, matrix.Up, Vector3.Zero, Vector3.Zero, null, SpawningOptions.RotateFirstCockpitTowardsDirection, receivedData.PlayerId);

					return;

				}

				//Enable Debug Mode
				if (receivedData.ChatMessage.StartsWith("/MES.EnableDebugMode.") == true){
					
					var msgSplit = receivedData.ChatMessage.Split('.');
					
					if(msgSplit.Length != 3){
						
						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received", 5000, "White", receivedData.PlayerId);
						return;
						
					}
					
					bool mode = false;
					
					if(bool.TryParse(msgSplit[2], out mode) == false){
						
						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received", 5000, "White", receivedData.PlayerId);
						return;
						
					}
					
					Logger.LoggerDebugMode = mode;
					MyVisualScriptLogicProvider.ShowNotification("Debug Mode Enabled: " + mode.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}

				//Debug: InitStoreBlocks
				if(receivedData.ChatMessage.StartsWith("/MES.Debug.InitStoreBlocks") == true) {

					Debug.InitStoreBlocks();

				}

				if(receivedData.ChatMessage.StartsWith("/MES.Debug.Reputation.") == true) {

					Debug.ReputationCheck(receivedData);

				}

				if(receivedData.ChatMessage.StartsWith("/MES.Debug.MoreEcMad") == true) {

					var player = SpawnResources.GetPlayerById(receivedData.PlayerId);

					if(player != null) {

						foreach(var faction in MyAPIGateway.Session.Factions.Factions.Keys) {

							var npcFaction = MyAPIGateway.Session.Factions.Factions[faction];
							MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(player.IdentityId, npcFaction.FactionId, -501);



						}

					}

				}

				if(receivedData.ChatMessage.StartsWith("/MES.Debug.MoreEcHostile") == true) {

					var player = SpawnResources.GetPlayerById(receivedData.PlayerId);

					if(player != null) {

						foreach(var faction in MyAPIGateway.Session.Factions.Factions.Keys) {

							var npcFaction = MyAPIGateway.Session.Factions.Factions[faction];
							MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(player.IdentityId, npcFaction.FactionId, -1500);

						}

					}

				}

				if(receivedData.ChatMessage.StartsWith("/MES.Debug.MoreEcNeutral") == true) {

					var player = SpawnResources.GetPlayerById(receivedData.PlayerId);

					if(player != null) {

						foreach(var faction in MyAPIGateway.Session.Factions.Factions.Keys) {

							var npcFaction = MyAPIGateway.Session.Factions.Factions[faction];
							MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(player.IdentityId, npcFaction.FactionId, 0);



						}

					}

				}

				//Debug.CheckNight
				if (receivedData.ChatMessage.StartsWith("/MES.Debug.CheckNight") == true) {

					MyVisualScriptLogicProvider.ShowNotification("Is Night: " + SpawnResources.IsNight(MyAPIGateway.Session.LocalHumanPlayer.GetPosition()).ToString(), 5000, "White", receivedData.PlayerId);
					MyVisualScriptLogicProvider.ShowNotification("Time: " + MyAPIGateway.Session.GameDateTime.ToString(), 5000, "White", receivedData.PlayerId);
					var gameTime = MyAPIGateway.Session.GameDateTime - new DateTime(2081, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					MyVisualScriptLogicProvider.ShowNotification("Seconds: " + gameTime.TotalSeconds.ToString(), 5000, "White", receivedData.PlayerId);

				}

				//Debug.CheckRemovalStatus
				if (receivedData.ChatMessage.StartsWith("/MES.Debug.CheckRemovalStatus") == true) {

					MyVisualScriptLogicProvider.ShowNotification("Number Of Pending Deletions: " + NPCWatcher.DeleteGridList.Count.ToString(), 5000, "White", receivedData.PlayerId);
					MyVisualScriptLogicProvider.ShowNotification("Deletion Process Status: " + NPCWatcher.DeleteGrids.ToString(), 5000, "White", receivedData.PlayerId);

				}

				//Debug.Debug.CreateKPL
				if(receivedData.ChatMessage.StartsWith("/MES.Debug.CreateKPL") == true) {

					KnownPlayerLocationManager.AddKnownPlayerLocation(MyAPIGateway.Session.LocalHumanPlayer.GetPosition(), "SPRT", 200, 1, -1);

				}

				//Joke: ComeAtMeBro
				if(receivedData.ChatMessage.StartsWith("/MES.ComeAtMeBro") == true) {

					var playerList = new List<IMyPlayer>();
					MyAPIGateway.Players.GetPlayers(playerList);
					IMyPlayer thisPlayer = null;

					foreach(var player in playerList) {

						if(player.IdentityId == receivedData.PlayerId) {

							thisPlayer = player;
							break;

						}

					}

					if(thisPlayer == null) {

						return;

					}

					int bros = 0;

					foreach(var cubeGrid in NPCWatcher.ActiveNPCs.Keys.ToList()) {

						if(cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false) {

							continue;

						}

						if(cubeGrid.Physics == null) {

							continue;

						}

						if(NPCWatcher.ActiveNPCs[cubeGrid].SpawnType == "SpaceCargoShip") {

							cubeGrid.Physics.LinearVelocity = Vector3D.Normalize(thisPlayer.GetPosition() - cubeGrid.GetPosition()) * 100;
							bros++;

						}

					}

					MyVisualScriptLogicProvider.ShowNotification(bros.ToString() + " Bros Coming At You!", 5000, "Red", thisPlayer.IdentityId);

				}

				//RemoveAllNPCs
				if(receivedData.ChatMessage.StartsWith("/MES.RemoveAllNPCs") == true) {

					foreach(var cubeGrid in NPCWatcher.ActiveNPCs.Keys.ToList()) {

						if(cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false) {

							continue;

						}

						if(cubeGrid.Physics == null) {

							continue;

						}

						if(NPCWatcher.ActiveNPCs[cubeGrid].CleanupIgnore == false) {

							NPCWatcher.ActiveNPCs[cubeGrid].FlagForDespawn = true;

						}

					}

					MyVisualScriptLogicProvider.ShowNotification("All Eligible NPC Grids Processed For Removal", 5000, "White", receivedData.PlayerId);

				}

				//Settings
				if(receivedData.ChatMessage.StartsWith("/MES.Settings.") == true){
					
					var result = SettingsEditor.EditSettings(receivedData.ChatMessage);
					MyVisualScriptLogicProvider.ShowNotification(result, 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//TryRandomSpawn
				if(receivedData.ChatMessage.StartsWith("/MES.Spawn.") == true){
					
					var playerList = new List<IMyPlayer>();
					MyAPIGateway.Players.GetPlayers(playerList);
					IMyPlayer thisPlayer = null;
					
					foreach(var player in playerList){
						
						if(player.IdentityId == receivedData.PlayerId){
							
							thisPlayer = player;
							break;
							
						}
						
					}
					
					if(thisPlayer == null){
						
						MyVisualScriptLogicProvider.ShowNotification("Could Not Spawn Encounter: Player Not In Watch List", 5000, "White", receivedData.PlayerId);
						return;
						
					}
					
					bool success = false;
					
					if(receivedData.ChatMessage.Contains("SpaceCargoShip") == true || receivedData.ChatMessage.Contains("AllSpawns") == true){
						
						if(MES_SessionCore.playerWatchList.ContainsKey(thisPlayer) == true){
							
							SpawnGroupManager.AdminSpawnGroup = SpecificSpawnGroupRequest(receivedData.ChatMessage, "SpaceCargoShip");
							MES_SessionCore.PlayerWatcherTimer = 0;
							MES_SessionCore.playerWatchList[thisPlayer].SpaceCargoShipTimer = 0;
							MyVisualScriptLogicProvider.ShowNotification("Attempting Random Spawn: Space Cargo Ship", 5000, "White", receivedData.PlayerId);
							success = true;
							
						}
						
					}
					
					if(receivedData.ChatMessage.Contains("PlanetaryCargoShip") == true){
						
						if(MES_SessionCore.playerWatchList.ContainsKey(thisPlayer) == true){
							
							SpawnGroupManager.AdminSpawnGroup = SpecificSpawnGroupRequest(receivedData.ChatMessage, "PlanetaryCargoShip");
							MES_SessionCore.playerWatchList[thisPlayer].AtmoCargoShipTimer = 0;
							MES_SessionCore.PlayerWatcherTimer = 0;
							MyVisualScriptLogicProvider.ShowNotification("Attempting Random Spawn: Planetary Cargo Ship", 5000, "White", receivedData.PlayerId);
							success = true;
							
						}
						
					}
					
					if(receivedData.ChatMessage.Contains("RandomEncounter") == true){
						
						if(MES_SessionCore.playerWatchList.ContainsKey(thisPlayer) == true){
							
							SpawnGroupManager.AdminSpawnGroup = SpecificSpawnGroupRequest(receivedData.ChatMessage, "RandomEncounter");
							
							MES_SessionCore.PlayerWatcherTimer = 0;
							MES_SessionCore.playerWatchList[thisPlayer].RandomEncounterCheckTimer = 0;
							MES_SessionCore.playerWatchList[thisPlayer].RandomEncounterCoolDownTimer = 0;
							var fakeDistance = Settings.RandomEncounters.PlayerTravelDistance + 1000;
							MES_SessionCore.playerWatchList[thisPlayer].RandomEncounterDistanceCoordCheck = fakeDistance * Vector3D.Up + thisPlayer.GetPosition();
							MyVisualScriptLogicProvider.ShowNotification("Attempting Random Spawn: Random Encounter", 5000, "White", receivedData.PlayerId);
							success = true;
							
						}
						
					}
					
					if(receivedData.ChatMessage.Contains("PlanetaryInstallation") == true){
						
						if(MES_SessionCore.playerWatchList.ContainsKey(thisPlayer) == true){
							
							SpawnGroupManager.AdminSpawnGroup = SpecificSpawnGroupRequest(receivedData.ChatMessage, "PlanetaryInstallation");
							
							MES_SessionCore.PlayerWatcherTimer = 0;
							MES_SessionCore.playerWatchList[thisPlayer].PlanetaryInstallationCheckTimer = 0;
							MES_SessionCore.playerWatchList[thisPlayer].PlanetaryInstallationCooldownTimer = 0;
							
							var fakeDistance = Settings.PlanetaryInstallations.PlayerDistanceSpawnTrigger + 1000;
							var randomDir = SpawnResources.GetRandomCompassDirection(thisPlayer.GetPosition(), SpawnResources.GetNearestPlanet(thisPlayer.GetPosition()));
							
							MES_SessionCore.playerWatchList[thisPlayer].InstallationDistanceCoordCheck = fakeDistance * randomDir + thisPlayer.GetPosition();
							MyVisualScriptLogicProvider.ShowNotification("Attempting Random Spawn: Planetary Installation", 5000, "White", receivedData.PlayerId);
							success = true;
							
						}
						
					}
					
					if(receivedData.ChatMessage.Contains("BossEncounter") == true){
						
						if(BossEncounterSpawner.IsPlayerInBossEncounter(thisPlayer.IdentityId) == true){
							
							MyVisualScriptLogicProvider.ShowNotification("Boss Encounter Already Active", 5000, "White", receivedData.PlayerId);
							
						}
						
						if(MES_SessionCore.playerWatchList.ContainsKey(thisPlayer) == true){
							
							SpawnGroupManager.AdminSpawnGroup = SpecificSpawnGroupRequest(receivedData.ChatMessage, "BossEncounter");
							
							MES_SessionCore.PlayerWatcherTimer = 0;
							MES_SessionCore.playerWatchList[thisPlayer].BossEncounterCooldownTimer = 0;
							MES_SessionCore.playerWatchList[thisPlayer].BossEncounterCheckTimer = 0;
							MyVisualScriptLogicProvider.ShowNotification("Attempting Random Spawn: Boss Encounter", 5000, "White", receivedData.PlayerId);
							success = true;
							
						}
						
					}
					
					if(success == false){
						
						MyVisualScriptLogicProvider.ShowNotification("Could Not Spawn Encounter: Player Not In Watch List", 5000, "White", receivedData.PlayerId);
						
					}
					
					return;
					
				}
				
				//WaveSpawner
				if(receivedData.ChatMessage.StartsWith("/MES.WaveSpawner.") == true){
					
					bool success = false;
					
					if(receivedData.ChatMessage.Contains("SpaceCargoShip") == true && Settings.SpaceCargoShips.EnableWaveSpawner == true){
						
						MyVisualScriptLogicProvider.ShowNotification("Wave Spawner (Space Cargo Ship) Activated.", 5000, "White", receivedData.PlayerId);
						MES_SessionCore.SpaceCargoShipWaveSpawner.CurrentWaveTimer = Settings.SpaceCargoShips.MaxWaveSpawnTime;
						success = true;
						
					}
					
					if(success == false){
						
						MyVisualScriptLogicProvider.ShowNotification("Wave Spawner Could Not Be Triggered. Please Enable In Configuration.", 5000, "White", receivedData.PlayerId);
						
					}
					
				}
				
				//Enable Territory
				if(receivedData.ChatMessage.StartsWith("/MES.EnableTerritory.") == true){
					
					var messageReplace = receivedData.ChatMessage.Replace("/MES.EnableTerritory.", "");
					
					if(messageReplace == ""){
						
						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received: No Territory Name Provided", 5000, "White", receivedData.PlayerId);
						return;
						
					}
					
					MyAPIGateway.Utilities.SetVariable<bool>("MES-Territory-" + messageReplace, true);
					TerritoryManager.TerritoryRefresh();
					MyVisualScriptLogicProvider.ShowNotification("Territory Enabled: " + messageReplace, 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//Disable Territory
				if(receivedData.ChatMessage.StartsWith("/MES.DisableTerritory.") == true){
					
					var messageReplace = receivedData.ChatMessage.Replace("/MES.DisableTerritory.", "");
					
					if(messageReplace == ""){
						
						MyVisualScriptLogicProvider.ShowNotification("Invalid Command Received: No Territory Name Provided", 5000, "White", receivedData.PlayerId);
						return;
						
					}
					
					MyAPIGateway.Utilities.SetVariable<bool>("MES-Territory-" + messageReplace, false);
					TerritoryManager.TerritoryRefresh();
					MyVisualScriptLogicProvider.ShowNotification("Territory Disabled: " + messageReplace, 5000, "White", receivedData.PlayerId);
					return;
					
				}
								
				//Get SpawnGroups
				if(receivedData.ChatMessage.StartsWith("/MES.GetSpawnGroups") == true){
					
					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					syncData.ClipboardContents = Logger.SpawnGroupResults();
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("Spawn Group Data To Clipboard. Success: " + sendStatus.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//Get Active NPCs
				if(receivedData.ChatMessage.StartsWith("/MES.GetActiveNPCs") == true){
					
					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					syncData.ClipboardContents = Logger.GetActiveNPCs();
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("Active NPC Data To Clipboard. Success: " + sendStatus.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}

				//Get Block Definition Info
				if(receivedData.ChatMessage.StartsWith("/MES.GetBlockDefinitions") == true) {

					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					syncData.ClipboardContents = Logger.GetBlockDefinitionInfo();
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("Block Data To Clipboard. Success: " + sendStatus.ToString(), 5000, "White", receivedData.PlayerId);
					return;

				}

				//Get Block Definition Info
				if(receivedData.ChatMessage.StartsWith("/MES.GetColorsFromGrid") == true) {

					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					syncData.ClipboardContents = Logger.GetColorListFromGrid(SpawnResources.GetPlayerById(syncData.PlayerId));
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					return;

				}

				//Get Player Watch Lists
				if(receivedData.ChatMessage.StartsWith("/MES.GetPlayerWatchList") == true){
					
					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					syncData.ClipboardContents = Logger.GetPlayerWatcherData();
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("Player Watch Data To Clipboard. Success: " + sendStatus.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//Get Local Threat Score
				if(receivedData.ChatMessage.StartsWith("/MES.GetThreatScore") == true || receivedData.ChatMessage.StartsWith("/MES.GTS") == true) {
					
					var messageReplace = receivedData.ChatMessage.Replace("/MES.GetThreatScore.", "");
					ImprovedSpawnGroup selectedSpawnGroup = null;
					
					if(messageReplace == ""){
						
						MyVisualScriptLogicProvider.ShowNotification("Default Threat Check Range Of 5000 Used. Spawngroup Not Provided or Detected.", 5000, "White", receivedData.PlayerId);
						//return;
						selectedSpawnGroup = new ImprovedSpawnGroup();
						
					}
					
					var playerList = new List<IMyPlayer>();
					MyAPIGateway.Players.GetPlayers(playerList);
					IMyPlayer thisPlayer = null;
					
					foreach(var player in playerList){
						
						if(player.IdentityId == receivedData.PlayerId){
							
							thisPlayer = player;
							break;
							
						}
						
					}
					
					if(thisPlayer == null){
						
						MyVisualScriptLogicProvider.ShowNotification("Command Failed: Apparently you don't exist?", 5000, "White", receivedData.PlayerId);
						return;
						
					}
					
					if(selectedSpawnGroup == null){
						
						foreach(var spawnGroup in SpawnGroupManager.SpawnGroups){
						
							if(spawnGroup.SpawnGroup.Id.SubtypeName == messageReplace){
								
								selectedSpawnGroup = spawnGroup;
								break;
								
							}
							
						}
						
					}

					if(selectedSpawnGroup == null){
						
						MyVisualScriptLogicProvider.ShowNotification("Default Threat Check Range Of 5000 Used. Spawngroup Not Provided or Detected.", 5000, "White", receivedData.PlayerId);
						//return;
						selectedSpawnGroup = new ImprovedSpawnGroup();
						
					}
					
					SpawnResources.RefreshEntityLists();
					SpawnResources.LastThreatRefresh = SpawnResources.GameStartTime;
					var threatLevel = SpawnResources.GetThreatLevel(selectedSpawnGroup, thisPlayer.GetPosition());
					
					MyVisualScriptLogicProvider.ShowNotification("Threat Level Score Near You: " + threatLevel.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//Reset Active Territories
				if(receivedData.ChatMessage.StartsWith("/MES.ResetActiveTerritories") == true){
					
					TerritoryManager.TerritoryRefresh(true);
					
					var syncData = receivedData;
					syncData.Instruction = "MESResetActiveTerritories";
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("Active Territories Reset To Default Values.", 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//Get Spawned Unique Encounters
				if(receivedData.ChatMessage.StartsWith("/MES.GetSpawnedUniqueEncounters") == true){
					
					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					syncData.ClipboardContents = Logger.GetSpawnedUniqueEncounters();
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("Spawned Unique Encounters List Sent To Clipboard. Success: " + sendStatus.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//Get Spawn Group Eligibility At Position
				if(receivedData.ChatMessage.StartsWith("/MES.GetEligibleSpawnsAtPosition") == true || receivedData.ChatMessage.StartsWith("/MES.GESAP") == true) {
					
					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					syncData.ClipboardContents = Logger.EligibleSpawnGroupsAtPosition(receivedData.PlayerPosition);
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("List of Eligible Spawn Groups At Position Sent To Clipboard. Success: " + sendStatus.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
				//Get Planet Direction At Position
				if(receivedData.ChatMessage.StartsWith("/MES.GetDirectionFromPlanetCore") == true){
					
					var syncData = receivedData;
					syncData.Instruction = "MESClipboard";
					
					var planet = SpawnResources.GetNearestPlanet(receivedData.PlayerPosition);
					
					if(planet != null){
						
						var planetEntity = planet as IMyEntity;
						var dir = Vector3D.Normalize(receivedData.PlayerPosition - planetEntity.GetPosition());
						var sb = new StringBuilder();
						sb.Append("Direction Vector For ").Append(planet.Generator.Id.SubtypeName).Append(" At Position ").Append(planetEntity.GetPosition().ToString()).AppendLine();
						sb.Append("X: ").Append(dir.X.ToString()).AppendLine();
						sb.Append("Y: ").Append(dir.Y.ToString()).AppendLine();
						sb.Append("Z: ").Append(dir.Z.ToString()).AppendLine();
						syncData.ClipboardContents = sb.ToString();
						
						
					}else{
						
						syncData.ClipboardContents = "No Planets In Game World.";
						
					}
					
					
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, receivedData.SteamUserId);
					MyVisualScriptLogicProvider.ShowNotification("Direction From Planet Core Values Sent To Clipboard. Success: " + sendStatus.ToString(), 5000, "White", receivedData.PlayerId);
					return;
					
				}
				
			}
			
			//Settings Commands
			
		}
		
		public static string SpecificSpawnGroupRequest(string msg, string spawnType){
			
			if(msg.Contains(spawnType + ".") == false){
				
				return "";
				
			}
			
			var result = msg.Replace("/MES.Spawn." + spawnType + ".", "");
			
			if(string.IsNullOrEmpty(result) == true){
				
				return "";
				
			}

			return result;
			
		}
		
	}
	
}