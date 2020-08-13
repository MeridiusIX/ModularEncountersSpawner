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
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Beacon), false, "DisposableNpcBeaconLarge", "DisposableNpcBeaconSmall")]
	 
	public class DisposableBeaconLogic : MyGameLogicComponent{
		
		IMyBeacon Beacon;
		bool IsWorking = false;

		float TicksSincePlayerNearby = 0;
		float TicksSinceWorking = 0;

		bool SetupDone = false;
		bool IsServer = false;
		
		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{
				
				Beacon = Entity as IMyBeacon;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public override void UpdateBeforeSimulation100(){

			if(SetupDone == false) {

				SetupDone = true;
				IsServer = MyAPIGateway.Multiplayer.IsServer;

				if(IsServer == false) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				Beacon = Entity as IMyBeacon;
				Beacon.IsWorkingChanged += OnWorkingChange;

			}

			if(Beacon == null){
				
				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;
				
			}

			if(Settings.CustomBlocks.UseDisposableBeaconInactivity == true) {

				if(IsWorking == false) {

					TicksSinceWorking += 100;

				} else {

					TicksSinceWorking = 0;

				}

				if((TicksSinceWorking / 60) / 60 >= Settings.CustomBlocks.DisposableBeaconRemovalTimerMinutes) {

					Beacon.CubeGrid.RazeBlock(Beacon.SlimBlock.Min);
					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

			}

			if(Settings.CustomBlocks.UseDisposableBeaconPlayerDistance == true) {

				double closestDistance = -1;

				foreach(var player in MES_SessionCore.PlayerList) {

					if(player.IsBot == true || player.Character == null) {

						continue;

					}

					if(player.Character.IsDead == true || player.Character.IsPlayer == false) {

						continue;

					}

					var thisDist = Vector3D.Distance(player.GetPosition(), Beacon.GetPosition());

					if(thisDist < closestDistance || closestDistance == -1) {

						closestDistance = thisDist;

					}

				}

				if(closestDistance >= Settings.CustomBlocks.DisposableBeaconPlayerDistanceTrigger) {

					TicksSincePlayerNearby += 100;

					if((TicksSincePlayerNearby / 60) / 60 >= Settings.CustomBlocks.DisposableBeaconRemovalTimerMinutes) {

						Beacon.CubeGrid.RazeBlock(Beacon.SlimBlock.Min);
						NeedsUpdate = MyEntityUpdateEnum.NONE;
						return;

					}

				} else {

					TicksSincePlayerNearby = 0;

				}

			}

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