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
using ModularEncountersSpawner.Templates;

namespace ModularEncountersSpawner.Templates{
	
	[ProtoContract]
	public class SpawnRequestMES{
		
		[ProtoMember(1)]
		public string SpawnGroupName {get; set;}

		[ProtoMember(2)]
		public string FactionTagOverride {get; set;}
		
		[ProtoMember(3)]
		public Vector3D SpawnCoordinates {get; set;}
		
		[ProtoMember(4)]
		public Vector3D SpawnDirectionForward {get; set;}
		
		[ProtoMember(5)]
		public Vector3D SpawnDirectionUp {get; set;}
		
		[ProtoMember(6)]
		public Vector3D LinearVelocity {get; set;}
		
		public SpawnRequestMES(){
			
			SpawnGroupName = "";
			FactionTagOverride = "";
			SpawnCoordinates = Vector3D.Zero;
			SpawnDirectionForward = Vector3D.Forward;
			SpawnDirectionUp = Vector3D.Up;
			LinearVelocity = Vector3D.Zero;

		}
		
	}
	
}