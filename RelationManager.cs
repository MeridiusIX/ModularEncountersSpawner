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
using Sandbox.Game.Lights;
using Sandbox.Game.Weapons;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI;
using ProtoBuf;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Interfaces;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Spawners;

namespace ModularEncountersSpawner {
	
	public static class RelationManager{
		
		public static bool RunAgain = false;
		public static bool SetupDone = false;
		
		public static List<long> NeutralNpcFactions = new List<long>();
		public static List<long> FriendsNpcFactions = new List<long>();
		public static Dictionary<long, long> SetNeutralRelations = new Dictionary<long, long>();
		public static Dictionary<long, long> SetFriendsRelations = new Dictionary<long, long>();
		public static List<string> PreviouslySetRelations = new List<string>();
		
		public static void Setup(){
			
			var factionList = MyDefinitionManager.Static.GetDefaultFactions();
			
			foreach(var factionDef in factionList){
				
				var faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(factionDef.Tag);
				
				if(faction == null){
					
					continue;
					
				}

                if(factionDef.Context != null) {
                    
                    if(string.IsNullOrWhiteSpace(factionDef.Context.ModId) == false) {

                        //EFM-Wico
                        if(factionDef.Context.ModId.Contains("1301917772") == true) {

                            continue;

                        }

                        //EEM
                        if(factionDef.Context.ModId.Contains("531659576") == true) {

                            continue;

                        }

                        //EEM-Unstable
                        if(factionDef.Context.ModId.Contains("1508213460") == true) {

                            continue;

                        }

                    }

                }

                if(factionDef.DefaultRelation == MyRelationsBetweenFactions.Enemies || factionDef.DefaultRelationToPlayers == MyRelationsBetweenFactions.Enemies) {

                    continue;

                }
				
				if(factionDef.DefaultRelation == MyRelationsBetweenFactions.Neutral || factionDef.DefaultRelationToPlayers == MyRelationsBetweenFactions.Neutral) {
					
					NeutralNpcFactions.Add(faction.FactionId);
					
				}
				
				if(factionDef.DefaultRelation == MyRelationsBetweenFactions.Friends && factionDef.DefaultRelationToPlayers == MyRelationsBetweenFactions.Friends) {
					
					FriendsNpcFactions.Add(faction.FactionId);
					
				}
				
			}
			
		}
		
		public static void InitialReputationFixer(){

            if(SetupDone == false) {

                SetupDone = true;
                Setup();

            }

            MyAPIGateway.Parallel.Start(() => {

                PreviouslySetRelations.Clear();
                string previousRelationsArray = "";

                if(MyAPIGateway.Utilities.GetVariable<string>("MES-FixedDefaultNpcRelations", out previousRelationsArray) == true) {

                    var bytes = Convert.FromBase64String(previousRelationsArray);
                    PreviouslySetRelations = MyAPIGateway.Utilities.SerializeFromBinary<List<string>>(bytes);
                    

                }
                var playerList = new List<IMyPlayer>();
				MyAPIGateway.Players.GetPlayers(playerList);
				
				foreach(var player in playerList){
					
					if(player.IsBot == true || player.Character == null){
						
						continue;
						
					}
					
					foreach(var neutral in NeutralNpcFactions){
						
						string identityString = player.IdentityId.ToString() + "-" + neutral.ToString();

                        if(PreviouslySetRelations.Contains(identityString) == true){
							
							continue;
							
						}
						
						if(SetNeutralRelations.ContainsKey(player.IdentityId) == false){
							
							SetNeutralRelations.Add(player.IdentityId, neutral);
							PreviouslySetRelations.Add(identityString);
							RunAgain = true;
							break;
							
						}

					}
					
					foreach(var friends in FriendsNpcFactions){
						
						string identityString = player.IdentityId.ToString() + "-" + friends.ToString();

                        if(PreviouslySetRelations.Contains(identityString) == true){
							
							continue;
							
						}
						
						if(SetFriendsRelations.ContainsKey(player.IdentityId) == false){
							
							SetFriendsRelations.Add(player.IdentityId, friends);
							PreviouslySetRelations.Add(identityString);
							RunAgain = true;
							break;
							
						}

					}
					
				}

                var newbytes = MyAPIGateway.Utilities.SerializeToBinary<List<string>>(PreviouslySetRelations);
                string storage = Convert.ToBase64String(newbytes);
				MyAPIGateway.Utilities.SetVariable<string>("MES-FixedDefaultNpcRelations", storage);
				
			}, () => {
				
				MyAPIGateway.Utilities.InvokeOnGameThread(() => {
					
					foreach(var player in SetNeutralRelations.Keys.ToList()){

                        if(MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player, SetNeutralRelations[player]) < -499) {

                            MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(player, SetNeutralRelations[player], 0);

                        }
						
					}
					
					foreach(var player in SetFriendsRelations.Keys.ToList()){

                        if(MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player, SetFriendsRelations[player]) < -499) {

                            MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(player, SetFriendsRelations[player], 500);

                        }

                    }
					
					SetNeutralRelations.Clear();
					SetFriendsRelations.Clear();
					
					if(RunAgain == true){
						
						RunAgain = false;
						InitialReputationFixer();
						
					}

                });
				
			});
			
		}
		
	}
	
}