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
using ModularEncountersSpawner;
using ModularEncountersSpawner.Configuration;


namespace ModularEncountersSpawner.BlockLogic{
	
	//Change MyObjectBuilder_LargeGatlingTurret to the matching ObjectBuilder for your block
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "SuppressorLargeId", "SuppressorSmallId")]
	 
	public class SuppressorBlockLogic : MyGameLogicComponent{

		bool AffectBlocksAttachedToOwnGrid = false;

		IMyRadioAntenna Antenna;
		bool IsWorking = false;

		List<MyDefinitionId> SuppressedBlockIds = new List<MyDefinitionId>();
		List<IMyFunctionalBlock> SuppressedBlocksInWorld = new List<IMyFunctionalBlock>();
		List<IMyCubeGrid> AllGridsRegistered = new List<IMyCubeGrid>();

		List<IMyFunctionalBlock> BlocksToSuppress = new List<IMyFunctionalBlock>();

		bool SetupDone = false;
		bool IsServer = false;
		
		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{
				
				Antenna = Entity as IMyRadioAntenna;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public override void UpdateBeforeSimulation100(){

			if (!SetupDone && !MyAPIGateway.Multiplayer.IsServer) {

				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;

			}

			if (!SetupDone)
				Setup();

			MyAPIGateway.Parallel.Start(() => {

				for (int i = SuppressedBlocksInWorld.Count - 1; i >= 0; i--) {

					var block = SuppressedBlocksInWorld[i];

					if (block?.SlimBlock == null) {

						SuppressedBlocksInWorld.RemoveAt(i);
						continue;

					}

					if (CheckBlockForSuppression(block))
						BlocksToSuppress.Add(block);

				}

			}, () => {

				for (int i = BlocksToSuppress.Count - 1; i >= 0; i--) {

					var block = BlocksToSuppress[i];

					if (block?.SlimBlock == null) {

						continue;

					}

					block.Enabled = false;

				}

				BlocksToSuppress.Clear();

			});

		}

		void Setup() {

			SetupDone = true;
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

				terminalBlock.IsWorkingChanged += SuppressedBlockChanged;
				SuppressedBlocksInWorld.Add(terminalBlock);

			}

		}

		void SuppressedBlockChanged(IMyCubeBlock block) {

			if (!block.IsWorking || !block.IsFunctional)
				return;

			if (CheckBlockForSuppression(block as IMyFunctionalBlock))
				(block as IMyFunctionalBlock).Enabled = false;

		}

		bool CheckBlockForSuppression(IMyFunctionalBlock block) {

			if (block == null || !IsWorking || !Antenna.IsBroadcasting)
				return false;

			if (AffectBlocksAttachedToOwnGrid) {

				if (Antenna.SlimBlock.CubeGrid.IsSameConstructAs(block.SlimBlock.CubeGrid))
					return false;

			}

			return (Vector3D.Distance(Antenna.GetPosition(), block.GetPosition()) < Antenna.Radius);

		}

		void OnWorkingChange(IMyCubeBlock block) {

			if(block.IsWorking == false || block.IsFunctional == false) {

				IsWorking = false;
				return;

			}

			IsWorking = true;
			
		}

		public override void OnRemovedFromScene(){
			
			base.OnRemovedFromScene();
			
			var Block = Entity as IMyBeacon;
			
			if(Block == null){
				
				return;
				
			}

			Block.IsWorkingChanged += OnWorkingChange;

		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}