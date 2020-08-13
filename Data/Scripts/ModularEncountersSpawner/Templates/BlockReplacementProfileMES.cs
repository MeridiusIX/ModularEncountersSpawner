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
	public class BlockReplacementProfileMES{
		
		[ProtoMember(1)]
		public string ReplacementReferenceName {get; set;}
		
		[ProtoMember(2)]
		public Dictionary<SerializableDefinitionId, SerializableDefinitionId> ReplacementReferenceDict {get; set;}
		
		public BlockReplacementProfileMES(){
			
			ReplacementReferenceName = "";
			ReplacementReferenceDict = new Dictionary<SerializableDefinitionId, SerializableDefinitionId>();
			
		}
		
	}
	
}