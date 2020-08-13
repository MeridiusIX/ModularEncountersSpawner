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
		
		public string Name {get; set;}
		public string TagOld {get; set;}
		public bool Active {get; set;}
		public Vector3D Position {get; set;}
		public string Type {get; set;}
		public double Radius {get; set;}
		public bool ScaleRadiusWithPlanetSize {get; set;}
		public bool NoSpawnZone {get; set;}
		public bool StrictTerritory {get; set;}
		public List<string> FactionTagWhitelist {get; set;}
		public List<string> FactionTagBlacklist {get; set;}
		public bool AnnounceArriveDepart {get; set;}
		public string CustomArriveMessage {get; set;}
		public string CustomDepartMessage {get; set;}
		public string PlanetGeneratorName {get; set;}
		public bool BadTerritory {get; set;}
		
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