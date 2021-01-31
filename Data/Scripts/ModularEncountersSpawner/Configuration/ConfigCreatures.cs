using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI;
using VRage.Game;

namespace ModularEncountersSpawner.Configuration {

	//Creatures

	/*
	  
	  Hello Stranger!
	 
	  If you are in here because you want to change settings
	  for how this mod behaves, you are in the wrong place.

	  All the settings in this file, along with the other
	  configuration files, are created as XML files in the
	  \Storage\1521905890.sbm_ModularEncountersSpawner folder
	  of your Save File. This means you do not need to edit
	  the mod files here to tune the settings to your liking.

	  The workshop page for this mod also has a link to a
	  guide that explains what all the configuration options
	  do, along with how to activate them in-game via chat
	  commands if desired.
	  
	  If you plan to edit the values here anyway, I ask that
	  you do not reupload this mod to the Steam Workshop. If
	  this is not respected and I find out about it, I'll
	  exercise my rights as the creator and file a DMCA
	  takedown on any infringing copies. This warning can be
	  found on the workshop page for this mod as well.

	  Thank you.
		 
	*/

	public class ConfigCreatures{

		public float ModVersion;

		public bool OverrideVanillaCreatureSpawns;

		public int MinCreatureSpawnTime;
		public int MaxCreatureSpawnTime;

		public double MaxPlayerAltitudeForSpawn;

		public string[] CreatureBlacklist;
		public string[] CreaturePlanetBlacklist;

		public ConfigCreatures(){
			
			ModVersion = MES_SessionCore.ModVersion;

			OverrideVanillaCreatureSpawns = false;

			MinCreatureSpawnTime = 900;
			MaxCreatureSpawnTime = 1200;

			MaxPlayerAltitudeForSpawn = 150;

			CreatureBlacklist = new string[] { "BotSubtypeIdHere", "AnotherBotSubtypeId" };
			CreaturePlanetBlacklist = new string[] { "PlanetSubtypeIdHere", "AnotherPlanetSubtypeId" };



		}
		
		public ConfigCreatures LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-Creatures.xml", typeof(ConfigCreatures)) == true){
				
				try{
					
					ConfigCreatures config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-Creatures.xml", typeof(ConfigCreatures));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigCreatures>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-Creatures.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-Creatures.xml. Using Default Configuration.");
					var defaultSettings = new ConfigCreatures();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigCreatures();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-Creatures.xml", typeof(ConfigCreatures))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigCreatures>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-Creatures.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigCreatures settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-Creatures.xml", typeof(ConfigCreatures))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigCreatures>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-Creatures.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-Creatures.xml. Changes Will Be Lost On World Reload.");
				
			}
			
			return "Settings Changed, But Could Not Be Saved To XML. Changes May Be Lost On Session Reload.";
			
		}
		
	}
	
}