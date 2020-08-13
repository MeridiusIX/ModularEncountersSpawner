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
using ProtoBuf;
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

namespace ModularEncountersSpawner.Templates{
	
	[ProtoContract]
	public class BossEncounter{
		
		[ProtoIgnore]
		public ImprovedSpawnGroup SpawnGroup;
		
		[ProtoMember(1)]
		public string SpawnGroupName;
		
		[ProtoMember(2)]
		public string Type;
		
		[ProtoMember(3)]
		public Vector3D Position;
		
		[ProtoMember(4)]
		public List<long> PlayersInEncounter;
		
		[ProtoMember(5)]
		public int Timer;
		
		[ProtoMember(6)]
		public int SpawnAttempts;
		
		[ProtoMember(7)]
		public Dictionary<long, int> PlayerGPSHashes;
		
		public BossEncounter(){
			
			SpawnGroup = new ImprovedSpawnGroup();
			SpawnGroupName = "";
			Type = "";
			Position = new Vector3D(0,0,0);
			PlayersInEncounter = new List<long>();
			Timer = Settings.BossEncounters.SignalActiveTimer;
			SpawnAttempts = 0;
			PlayerGPSHashes = new Dictionary<long, int>();
			
		}
		
		public bool CreateGpsForPlayers(){
			
			this.SpawnGroup = SpawnGroupManager.GetSpawnGroupByName(this.SpawnGroupName);
			
			if(this.SpawnGroup == null){
				
				return false;
				
			}
			
			var playerList = new List<IMyPlayer>();
			MyAPIGateway.Players.GetPlayers(playerList);
			var bossGps = MyAPIGateway.Session.GPS.Create(this.SpawnGroup.BossCustomGPSLabel, "", this.Position, true);
            
            

			foreach(var player in playerList){
				
				if(player.IsBot == true){
					
					continue;
					
				}
				
				if(this.PlayersInEncounter.Contains(player.IdentityId) == false){
					
					continue;
					
				}
				
				MyAPIGateway.Session.GPS.AddGps(player.IdentityId, bossGps);
				MyVisualScriptLogicProvider.SetGPSColor(this.SpawnGroup.BossCustomGPSLabel, new Color(255,0,255), player.IdentityId);
				
				if(PlayerGPSHashes.ContainsKey(player.IdentityId) == true){

                    PlayerGPSHashes[player.IdentityId] = bossGps.Hash;

                } else{

                    PlayerGPSHashes.Add(player.IdentityId, bossGps.Hash);

                }

			}

            return true;
		
		}
		
		public bool CheckPlayerDistance(IMyPlayer player){
			
			if(this.PlayersInEncounter.Contains(player.IdentityId) == false || player == null){
				
				return false;
				
			}
			
			var distance = Vector3D.Distance(this.Position, player.GetPosition());
			
			if(distance <= Settings.BossEncounters.TriggerDistance){
				
				return true;
				
			}
			
			return false;
			
		}
		
		public void RemoveGpsForPlayers(){

			foreach(var identityId in this.PlayerGPSHashes.Keys){
				
				try{
					
					MyAPIGateway.Session.GPS.RemoveGps(identityId, PlayerGPSHashes[identityId]);
					
				}catch(Exception e){
					
					Logger.AddMsg("Something went wrong while removing Boss Encounter GPS from IdentityId: " + identityId.ToString());
					Logger.AddMsg(e.ToString(), true);
					
				}

			}
           
		}
		
		public void UseExistingSettings(BossEncounter newData){
			
			this.SpawnGroupName = newData.SpawnGroupName;
			this.SpawnGroup = SpawnGroupManager.GetSpawnGroupByName(this.SpawnGroupName);
			this.Type = newData.Type;
			this.Position = newData.Position;
			this.PlayersInEncounter = newData.PlayersInEncounter;
			this.Timer = newData.Timer;
			this.SpawnAttempts = newData.SpawnAttempts;
			this.PlayerGPSHashes = newData.PlayerGPSHashes;
			
		}
		
	}
	
}