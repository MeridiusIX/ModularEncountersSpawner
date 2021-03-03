using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;


namespace ModularEncountersSpawner.BlockLogic {

	//Change MyObjectBuilder_LargeGatlingTurret to the matching ObjectBuilder for your block
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "MES-Suppressor-Nanobots-Large")]
	 
	public class NanobotSuppressorLogic : MyGameLogicComponent{

		bool AffectBlocksAttachedToOwnGrid = false;

		IMyRadioAntenna Antenna;
		bool IsWorking = false;
		bool IsNpcOwned = false;
		bool InRange = false;
		bool IsValid = false;

		List<MyDefinitionId> SuppressedBlockIds = new List<MyDefinitionId>();
		List<IMyFunctionalBlock> SuppressedBlocksInWorld = new List<IMyFunctionalBlock>();
		List<IMyCubeGrid> AllGridsRegistered = new List<IMyCubeGrid>();

		List<IMyFunctionalBlock> BlocksToSuppress = new List<IMyFunctionalBlock>();

		bool SetupDone = false;

		DateTime OverheatTimer = MyAPIGateway.Session.GameDateTime;
		int OverheatInstanceCounter = 0;

		float defaultRange = 800;
		string lastCustomData = "";
		int tickCount = 0;

		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{

				//MyVisualScriptLogicProvider.ShowNotificationToAll("Suppressor Init", 3000);
				Antenna = Entity as IMyRadioAntenna;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public override void UpdateBeforeSimulation100(){

			if (!SetupDone)
				Setup();

			if (!SetupDone) {

				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;

			}

			if (!MyAPIGateway.Utilities.IsDedicated && MyAPIGateway.Session.LocalHumanPlayer != null) {

				if (IsWorking && IsNpcOwned) {

					var distance = Vector3D.Distance(Antenna.GetPosition(), MyAPIGateway.Session.LocalHumanPlayer.GetPosition());
					var inRange = distance <= Antenna.Radius;

					if (inRange && !InRange) {

						MyVisualScriptLogicProvider.ShowNotificationLocal("WARNING: Inhibitor Field Has Disable Jump Drive Functionality!", 4000, "Red");
						//MyVisualScriptLogicProvider.PlayHudSoundLocal(VRage.Audio.MyGuiSounds.HudUnable, MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

					}

					InRange = inRange;

				}

			}

			tickCount += 100;

			if (tickCount >= 100) {

				tickCount = 0;
				SetRange();

			}

			if (MyAPIGateway.Multiplayer.IsServer) {


				for (int i = SuppressedBlocksInWorld.Count - 1; i >= 0; i--) {

					var block = SuppressedBlocksInWorld[i];

					if (block?.SlimBlock == null) {

						SuppressedBlocksInWorld.RemoveAt(i);
						continue;

					}

					if (IsNpcOwned && CheckBlockForSuppression(block)) {

						block.Enabled = false;

						var timespan = MyAPIGateway.Session.GameDateTime - OverheatTimer;

						if (timespan.TotalMilliseconds <= 250) {

							OverheatInstanceCounter++;

							if (OverheatInstanceCounter >= 100) {

								OverheatInstanceCounter += 10;
								block.SlimBlock.DoDamage(OverheatInstanceCounter * 10, MyStringHash.GetOrCompute("Overheat"), true, null, Antenna.OwnerId);

							}

						} else {

							OverheatInstanceCounter = 0;

						}

						OverheatTimer = MyAPIGateway.Session.GameDateTime;

					}

				}

			}

		}

		void Setup() {

			Antenna = Entity as IMyRadioAntenna;

			if (Antenna == null) {

				//MyVisualScriptLogicProvider.ShowNotificationToAll("Suppressor Null Or Not Server", 3000);
				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;

			}

			SetupDone = true;

			SetRange();

			if (!MyAPIGateway.Multiplayer.IsServer)
				return;

			if (Antenna.Storage == null) {

				Antenna.Storage = new MyModStorageComponent();
				Antenna.CustomName = "[Nanobot Inhibitor Field]";

			}

			SuppressedBlockIds.Add(new MyDefinitionId(typeof(MyObjectBuilder_ShipWelder), "SELtdLargeNanobotBuildAndRepairSystem"));
			SuppressedBlockIds.Add(new MyDefinitionId(typeof(MyObjectBuilder_ShipWelder), "SELtdSmallNanobotBuildAndRepairSystem"));
			Antenna.IsWorkingChanged += OnWorkingChange;
			Antenna.OwnershipChanged += OnOwnerChange;
			IsValid = Antenna?.SlimBlock?.CubeGrid?.Physics != null;
			OnOwnerChange(Antenna);
			OnWorkingChange(Antenna);

			var entities = new HashSet<IMyEntity>();
			MyAPIGateway.Entities.GetEntities(entities, x => x as IMyCubeGrid != null);

			foreach(var entity in entities){

				OnEntityAdd(entity);

			}

		}

		void OnEntityAdd(IMyEntity entity) {

			var cubeGrid = entity as IMyCubeGrid;

			if (cubeGrid != null)
				ScanGrid(cubeGrid);

		}

		void ScanGrid(IMyCubeGrid cubeGrid) {

			cubeGrid.OnBlockAdded += OnBlockAdd;
			AllGridsRegistered.Add(cubeGrid);

			var blockList = new List<IMySlimBlock>();
			cubeGrid.GetBlocks(blockList, x => x.FatBlock != null);

			foreach (var block in blockList) {

				OnBlockAdd(block);

			}

		}

		void OnBlockAdd(IMySlimBlock block) {

			if (SuppressedBlockIds.Contains(block.BlockDefinition.Id)) {

				var terminalBlock = block.FatBlock as IMyFunctionalBlock;

				if (SuppressedBlocksInWorld.Contains(terminalBlock))
					return;

				//MyVisualScriptLogicProvider.ShowNotificationToAll("Added Suppressable Block", 3000);
				terminalBlock.IsWorkingChanged += SuppressedBlockChanged;
				SuppressedBlocksInWorld.Add(terminalBlock);

			}

		}

		void SuppressedBlockChanged(IMyCubeBlock block) {

			if (!block.IsWorking || !block.IsFunctional || !IsValid)
				return;

			if (Antenna == null || Antenna.MarkedForClose) {

				//MyVisualScriptLogicProvider.ShowNotificationToAll("Suppressor Null or Closed", 3000);
				block.IsWorkingChanged -= SuppressedBlockChanged;
				return;

			}

			if (IsNpcOwned && CheckBlockForSuppression(block as IMyFunctionalBlock))
				(block as IMyFunctionalBlock).Enabled = false;

		}

		bool CheckBlockForSuppression(IMyFunctionalBlock block) {

			if (block == null || !IsWorking || !Antenna.IsBroadcasting)
				return false;

			if (AffectBlocksAttachedToOwnGrid) {

				if (Antenna.SlimBlock.CubeGrid.IsSameConstructAs(block.SlimBlock.CubeGrid))
					return false;

			}

			return (Vector3D.Distance(Antenna.GetPosition(), block.GetPosition()) < defaultRange);

		}

		void OnWorkingChange(IMyCubeBlock block) {

			if(block.IsWorking == false || block.IsFunctional == false) {

				IsWorking = false;
				return;

			}

			IsWorking = true;
			
		}

		void OnOwnerChange(IMyTerminalBlock block) {

			if (block.OwnerId == 0 || MyAPIGateway.Players.TryGetSteamId(block.OwnerId) > 1) {

				IsNpcOwned = false;
				return;
			
			}

			IsNpcOwned = true;

		}

		void SetRange() {

			if (string.IsNullOrWhiteSpace(Antenna.CustomData)) {

				Antenna.CustomData = defaultRange.ToString();
				lastCustomData = defaultRange.ToString();
				Antenna.Radius = defaultRange;
				return;

			}

			if (Antenna.CustomData == lastCustomData)
				return;

			lastCustomData = Antenna.CustomData;
			float result = 0;

			if (!float.TryParse(Antenna.CustomData, out result))
				return;

			Antenna.Radius = result;
			defaultRange = result;

		}

		public override void OnRemovedFromScene(){
			
			base.OnRemovedFromScene();
			
			var Block = Entity as IMyRadioAntenna;
			
			if(Block == null){
				
				return;
				
			}

			Block.OwnershipChanged -= OnOwnerChange;
			Block.IsWorkingChanged -= OnWorkingChange;

		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}