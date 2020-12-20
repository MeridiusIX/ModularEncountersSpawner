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
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Utils;
using VRageMath;


namespace ModularEncountersSpawner.BlockLogic{

	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Thrust), false, 
		"MES-NPC-Thrust-Ion-LargeGrid-Large", 
		"MES-NPC-Thrust-Ion-LargeGrid-Small", 
		"MES-NPC-Thrust-Ion-SmallGrid-Large", 
		"MES-NPC-Thrust-Ion-SmallGrid-Small", 
		"MES-NPC-Thrust-Hydro-LargeGrid-Large", 
		"MES-NPC-Thrust-Hydro-LargeGrid-Small", 
		"MES-NPC-Thrust-Hydro-SmallGrid-Large", 
		"MES-NPC-Thrust-Hydro-SmallGrid-Small", 
		"MES-NPC-Thrust-Atmo-LargeGrid-Large", 
		"MES-NPC-Thrust-Atmo-LargeGrid-Small", 
		"MES-NPC-Thrust-Atmo-SmallGrid-Large", 
		"MES-NPC-Thrust-Atmo-SmallGrid-Small",
		"MES-NPC-Thrust-IonSciFi-LargeGrid-Large",
		"MES-NPC-Thrust-IonSciFi-LargeGrid-Small",
		"MES-NPC-Thrust-IonSciFi-SmallGrid-Large",
		"MES-NPC-Thrust-IonSciFi-SmallGrid-Small",
		"MES-NPC-Thrust-AtmoSciFi-LargeGrid-Large",
		"MES-NPC-Thrust-AtmoSciFi-LargeGrid-Small",
		"MES-NPC-Thrust-AtmoSciFi-SmallGrid-Large",
		"MES-NPC-Thrust-AtmoSciFi-SmallGrid-Small")
		]
	 
	public class NpcThrusterLogic : MyGameLogicComponent{
		
		public IMyThrust Thruster;
		public ThrustSettings Settings;
		public string SettingsString;

		public bool RestrictThrustUse;
		public float ThrustForceMultiplier;
		public float ThrustPowerMultiplier;

		public IMyCubeGrid OriginalGrid;

		public bool NpcOwned;

		public static Guid StorageKey = new Guid("DA22D750-3262-4C1E-A898-7FB7A7F58182");


		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{

				NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public override void UpdateBeforeSimulation(){
			
			if(Thruster == null && Entity != null){

				//Logger.AddMsg("NPC Thruster Setup", true);
				Thruster = Entity as IMyThrust;

				if (Thruster == null) {

					//Logger.AddMsg("NPC Thruster Is Null", true);
					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				OriginalGrid = Thruster.SlimBlock.CubeGrid;

				if (OriginalGrid == null) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				bool gotData = false;

				if (OriginalGrid.Storage != null) {

					if (OriginalGrid.Storage.TryGetValue(StorageKey, out SettingsString))
						gotData = GetSettings(SettingsString);

				}

				if (!gotData && Thruster.Storage != null && Thruster.Storage.TryGetValue(StorageKey, out SettingsString))
					gotData = GetSettings(SettingsString);

				if (!gotData) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				var thrustDef = Thruster.SlimBlock.BlockDefinition as MyThrustDefinition;

				if (thrustDef?.ThrusterType == null) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				var thrustType = thrustDef.ThrusterType.ToString();

				if (thrustType == "Atmospheric") {

					RestrictThrustUse = Settings.RestrictNpcAtmoThrust;
					ThrustForceMultiplier = Settings.NpcAtmoThrustForceMultiply;
					ThrustPowerMultiplier = Settings.NpcAtmoThrustPowerMultiply;

				}

				if (thrustType == "Hydrogen") {

					RestrictThrustUse = Settings.RestrictNpcHydroThrust;
					ThrustForceMultiplier = Settings.NpcHydroThrustForceMultiply;
					ThrustPowerMultiplier = Settings.NpcHydroThrustPowerMultiply;

				}

				if (thrustType == "Ion") {

					RestrictThrustUse = Settings.RestrictNpcIonThrust;
					ThrustForceMultiplier = Settings.NpcIonThrustForceMultiply;
					ThrustPowerMultiplier = Settings.NpcIonThrustPowerMultiply;

				}

				Thruster.SlimBlock.CubeGrid.OnBlockOwnershipChanged += GridOwnershipChange;
				Thruster.IsWorkingChanged += WorkingChanged;
				Thruster.SlimBlock.CubeGrid.OnGridSplit += GridSplit;
				GridOwnershipChange(Thruster.SlimBlock.CubeGrid);
				WorkingChanged(Thruster);

				Logger.AddMsg("NPC Thruster Configured For: " + Thruster.SlimBlock.CubeGrid.CustomName, true);
				NeedsUpdate = MyEntityUpdateEnum.EACH_100TH_FRAME;

			}

		}

		public override void UpdateBeforeSimulation100() {



		}

		public void GridOwnershipChange(IMyCubeGrid grid) {

			NpcOwned = false;

			if (Thruster.MarkedForClose)
				return;

			if (grid.BigOwners != null && grid.BigOwners.Count > 0) {

				foreach (var owner in grid.BigOwners) {

					if (owner == 0)
						continue;

					if (!(MyAPIGateway.Players.TryGetSteamId(owner) > 0)) {

						NpcOwned = true;
						break;
					
					}
				
				}

			}

			if (NpcOwned) {

				Logger.AddMsg(string.Format("[{0}] Using Custom Thrust Multipliers: Force: {1} // Power: {2}", Thruster.CustomName, ThrustForceMultiplier, ThrustPowerMultiplier), true);
				Thruster.ThrustMultiplier = ThrustForceMultiplier;
				Thruster.PowerConsumptionMultiplier = ThrustPowerMultiplier;


			} else {

				Logger.AddMsg(string.Format("[{0}] Using Default Thrust Multipliers", Thruster.CustomName), true);
				Thruster.ThrustMultiplier = 1;
				Thruster.PowerConsumptionMultiplier = 1;

				if (RestrictThrustUse)
					Thruster.Enabled = false;

			}

		}

		public void WorkingChanged(IMyCubeBlock block) {

			if (Thruster.Enabled) {

				if (RestrictThrustUse && !NpcOwned)
					Thruster.Enabled = false;

			}

		}

		public void GridSplit(IMyCubeGrid a, IMyCubeGrid b) {

			if (Thruster.MarkedForClose || Thruster.SlimBlock.CubeGrid == OriginalGrid)
				return;

			a.OnBlockOwnershipChanged -= GridOwnershipChange;
			b.OnBlockOwnershipChanged -= GridOwnershipChange;

			a.OnGridSplit -= GridSplit;
			b.OnGridSplit -= GridSplit;

			if (Thruster.MarkedForClose)
				return;

			if (Thruster.Storage == null)
				Thruster.Storage = new MyModStorageComponent();

			if (Thruster.Storage.ContainsKey(StorageKey))
				Thruster.Storage[StorageKey] = SettingsString;
			else
				Thruster.Storage.Add(StorageKey, SettingsString);

			Thruster.SlimBlock.CubeGrid.OnBlockOwnershipChanged += GridOwnershipChange;
			GridOwnershipChange(Thruster.SlimBlock.CubeGrid);
			WorkingChanged(Thruster);

		}

		public bool GetSettings(string data) {

			try {

				var bytes = Convert.FromBase64String(data);

				if (bytes == null)
					return false;

				Settings = MyAPIGateway.Utilities.SerializeFromBinary<ThrustSettings>(bytes);

				if (Settings != null)
					return true;

				return false;

			} catch (Exception e) {
			
				return false;
			
			}

		}
		
		public override void OnRemovedFromScene(){
			
			base.OnRemovedFromScene();
			
			var Block = Entity as IMyThrust;
			
			if(Block?.SlimBlock?.CubeGrid == null){
				
				return;
				
			}

			Block.SlimBlock.CubeGrid.OnBlockOwnershipChanged -= GridOwnershipChange;
			Block.SlimBlock.CubeGrid.OnGridSplit -= GridSplit;

		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}