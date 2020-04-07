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

namespace ModularEncountersSpawner.Templates{
	
	[ProtoContract]
	public class ActiveNPC{
		
		[ProtoMember(1)]
		public string SpawnGroupName;
		
		[ProtoMember(2)]
		public string Name;
		
		[ProtoMember(3)]
		public string GridName;
		
		[ProtoMember(4)]
		public string InitialFaction;
		
		[ProtoMember(5)]
		public Vector3D StartCoords;
		
		[ProtoMember(6)]
		public Vector3D EndCoords;
		
		[ProtoMember(7)]
		public Vector3D CurrentCoords;
		
		[ProtoMember(8)]
		public float AutoPilotSpeed;
		
		[ProtoMember(9)]
		public bool CleanupIgnore;
		
		[ProtoMember(10)]
		public int CleanupTime;
		
		[ProtoMember(11)]
		public bool KeenBehaviorCheck;

		[ProtoMember(12)]
		public string SpawnType;

		[ProtoIgnore]
		public string KeenAiName;
		
		[ProtoIgnore]
		public float KeenAiTriggerDistance;
		
		[ProtoIgnore]
		public bool FullyNPCOwned;
		
		[ProtoIgnore]
		public bool FlagForDespawn;
		
		[ProtoIgnore]
		public bool FixTurrets;
		
		[ProtoIgnore]
		public bool StartupScanValid;
		
		[ProtoIgnore]
		public ImprovedSpawnGroup SpawnGroup;
		
		[ProtoIgnore]
		public IMyCubeGrid CubeGrid;
		
		[ProtoIgnore]
		public MyPlanet Planet;
		
		[ProtoIgnore]
		public IMyRemoteControl RemoteControl;
		
		[ProtoIgnore]
		public List<IMyGasTank> HydrogenTanks;
		
		[ProtoIgnore]
		public List<IMyGasGenerator> GasGenerators;
		
		[ProtoIgnore]
		public bool ForceStaticGrid;
		
		[ProtoIgnore]
		public bool DisabledBlocks;
		
		[ProtoIgnore]
		public bool VoxelCut;
		
		[ProtoIgnore]
		public bool ReplacedWeapons;
		
		[ProtoIgnore]
		public bool AddedCrew;
		
		[ProtoIgnore]
		public bool CheckedBlockCount;
		
		[ProtoIgnore]
		public bool ReplenishedSystems;

		[ProtoIgnore]
		public bool StoreBlocksInit;

		[ProtoIgnore]
		public bool ModStorageRetrieveFail;

		[ProtoIgnore]
		public IMyFaction faction;

		[ProtoIgnore]
		public bool EconomyStationCheck;

		[ProtoIgnore]
		public bool NonPhysicalAmmoCheck;

		[ProtoIgnore]
		public bool EmptyInventoryCheck;

		[ProtoIgnore]
		public bool DefenseShieldActivationCheck;


		public ActiveNPC(){
			
			SpawnGroupName = "";
			Name = "";
			GridName = "";
			InitialFaction = "";
			
			StartCoords = Vector3D.Zero;
			EndCoords = Vector3D.Zero;
			CurrentCoords = Vector3D.Zero;
			
			AutoPilotSpeed = 0;
			
			SpawnType = "Other";
			CleanupIgnore = false;
			CleanupTime = 0;
			KeenBehaviorCheck = false;
			KeenAiName = "";
			KeenAiTriggerDistance = 0;
			FullyNPCOwned = true;
			FlagForDespawn = false;
			FixTurrets = false;
			
			StartupScanValid = false;

			SpawnGroup = new ImprovedSpawnGroup();
			CubeGrid = null;
			Planet = null;
			RemoteControl = null;
			HydrogenTanks = new List<IMyGasTank>();
			GasGenerators = new List<IMyGasGenerator>();
			ForceStaticGrid = false;
			DisabledBlocks = false;
			ReplacedWeapons = false;
			AddedCrew = false;
			VoxelCut = false;
			CheckedBlockCount = false;
			ReplenishedSystems = true;
			StoreBlocksInit = false;
			ModStorageRetrieveFail = false;
			faction = null;
			EconomyStationCheck = false;
			NonPhysicalAmmoCheck = false;
			EmptyInventoryCheck = false;
			DefenseShieldActivationCheck = false;

		}

		public ActiveNPC(string dataStorage) {

			try {

				var byteData = Convert.FromBase64String(dataStorage);
				var npcData = MyAPIGateway.Utilities.SerializeFromBinary<ActiveNPC>(byteData);

				if(npcData != null) {

					this.SpawnGroupName = npcData.SpawnGroupName;
					this.Name = npcData.Name;
					this.GridName = npcData.GridName;
					this.InitialFaction = npcData.InitialFaction;
					this.StartCoords = npcData.StartCoords;
					this.EndCoords = npcData.EndCoords;
					this.CurrentCoords = npcData.CurrentCoords;
					this.AutoPilotSpeed = npcData.AutoPilotSpeed;
					this.CleanupIgnore = npcData.CleanupIgnore;
					this.CleanupTime = npcData.CleanupTime;
					this.KeenBehaviorCheck = npcData.KeenBehaviorCheck;
					this.SpawnType = npcData.SpawnType;
					this.SpawnGroup = new ImprovedSpawnGroup();

					foreach(var spawnGroup in SpawnGroupManager.SpawnGroups) {

						if(spawnGroup.SpawnGroupName == this.SpawnGroupName) {

							this.SpawnGroup = spawnGroup;
							break;

						}

					}

					this.HydrogenTanks = new List<IMyGasTank>();
					this.GasGenerators = new List<IMyGasGenerator>();
					this.faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(npcData.InitialFaction);

				}

			} catch(Exception exc) {

				this.ModStorageRetrieveFail = true;
				Logger.AddMsg("Failed To Load ActiveNPC Data from ModStorageComponent");

			}

		}

		public string ToString() {

			try {

				var byteData = MyAPIGateway.Utilities.SerializeToBinary<ActiveNPC>(this);
				var stringData = Convert.ToBase64String(byteData);
				return stringData;

			} catch(Exception exc) {

				Logger.AddMsg("Failed To Save ActiveNPC Data to ModStorageComponent for " + GridName);

			}

			return "";

		}
		
	}
	
}