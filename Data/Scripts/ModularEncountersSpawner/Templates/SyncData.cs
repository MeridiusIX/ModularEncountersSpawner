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
	public class SyncData{
		
		[ProtoMember(1)]
		public string Instruction {get; set;}
		
		[ProtoMember(2)]
		public long PlayerId {get; set;}
		
		[ProtoMember(3)]
		public ulong SteamUserId {get; set;}
		
		[ProtoMember(4)]
		public string ChatMessage {get; set;}
		
		[ProtoMember(5)]
		public string GpsName {get; set;}
		
		[ProtoMember(6)]
		public Vector3D GpsCoords {get; set;}
		
		[ProtoMember(7)]
		public string ClipboardContents {get; set;}
		
		[ProtoMember(8)]
		public Vector3D PlayerPosition {get; set;}
		
		public SyncData(){
			
			Instruction = "";
			PlayerId = 0;
			SteamUserId = 0;
			ChatMessage = "";
			GpsName = "";
			GpsCoords = Vector3D.Zero;
			ClipboardContents = "";
			PlayerPosition = Vector3D.Zero;
			
		}
		
	}
	
}