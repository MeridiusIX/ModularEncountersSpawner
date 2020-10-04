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

namespace ModularEncountersSpawner.Templates{
	
	public class NPCTerritory{

		public string Name;
		public string TagOld;
		public bool Active;
		public Vector3D Position;
		public string Type;
		public double Radius;
		public bool ScaleRadiusWithPlanetSize;
		public bool NoSpawnZone;
		public bool StrictTerritory;
		public List<string> FactionTagWhitelist;
		public List<string> FactionTagBlacklist;
		public bool AnnounceArriveDepart;
		public string CustomArriveMessage;
		public string CustomDepartMessage;
		public string PlanetGeneratorName;
		public bool BadTerritory;
		
		public NPCTerritory(){
			
			Name = "";
			TagOld = "TerritoryTagNotUsed";
			Active = true;
			Position = new Vector3D(0,0,0);
			Type = "Static";
			Radius = 0;
			ScaleRadiusWithPlanetSize = false;
			NoSpawnZone = false;
			StrictTerritory = false;
			FactionTagWhitelist = new List<string>();
			FactionTagBlacklist = new List<string>();
			AnnounceArriveDepart = false;
			CustomArriveMessage = "";
			CustomDepartMessage = "";
			PlanetGeneratorName = "";
			BadTerritory = false;
			
		}
		
	}
	
}