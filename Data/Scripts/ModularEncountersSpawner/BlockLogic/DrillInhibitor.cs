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
using Sandbox.ModAPI.Weapons;

namespace ModularEncountersSpawner.BlockLogic{
	
	//Change MyObjectBuilder_LargeGatlingTurret to the matching ObjectBuilder for your block
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "MES-Suppressor-Drill-Large")]
	 
	public class DrillInhibitor : MyGameLogicComponent{
		
		IMyRadioAntenna Antenna;
		bool IsWorking = false;
		bool IsValid = false;

		bool SetupDone = false;
		bool InDisableRange = false;

		int toolbarIndex = 0;

		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{
				
				Antenna = Entity as IMyRadioAntenna;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public void ToolEquipped(long playerId, string typeId = "", string subtypeId = "") {

			if (MyAPIGateway.Session.LocalHumanPlayer == null) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Player Null", 4000, "White");
				return;

			}

			if (playerId != MyAPIGateway.Session.LocalHumanPlayer.IdentityId) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Identity Not Matched", 4000, "White");
				return;

			}

			if (!InDisableRange) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Outside Disable Range", 4000, "White");
				return;

			}

			if (MyAPIGateway.Session.LocalHumanPlayer.Character?.EquippedTool == null) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Tool Null", 4000, "White");
				return;

			}
				

			var drill = MyAPIGateway.Session.LocalHumanPlayer.Character.EquippedTool as IMyHandDrill;

			if (drill == null) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Drill Null", 4000, "White");
				return;

			}
			
			toolbarIndex++;

			if (toolbarIndex > 8)
				toolbarIndex = 0;

			MyVisualScriptLogicProvider.SwitchToolbarToSlot(toolbarIndex, MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

		}

		public override void UpdateBeforeSimulation10() {

			if (SetupDone == false) {

				SetupDone = true;

				if (MyAPIGateway.Utilities.IsDedicated && MyAPIGateway.Multiplayer.IsServer) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				Antenna = Entity as IMyRadioAntenna;

				if (Antenna == null) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				Antenna.IsWorkingChanged += OnWorkingChange;
				Antenna.CustomName = "[Drill Inhibitor Field]";
				Antenna.Radius = 500;
				IsWorking = Antenna?.Enabled ?? false;
				MyVisualScriptLogicProvider.ToolEquipped += ToolEquipped;

			}

			if (Antenna == null) {

				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;

			}

			if (!IsWorking)
				return;

			var character = MyAPIGateway.Session?.LocalHumanPlayer?.Character;

			if (character == null)
				return;

			if (character.IsDead)
				return;

			var distance = Vector3D.Distance(character.GetPosition(), Antenna.GetPosition());

			var disable = distance <= Antenna.Radius;

			if (disable && !InDisableRange && character.EquippedTool != null) {

				InDisableRange = disable;
				ToolEquipped(MyAPIGateway.Session.LocalHumanPlayer.IdentityId);
				MyVisualScriptLogicProvider.ShowNotificationLocal("WARNING: Inhibitor Field Has Disabled Hand Drill Use!", 4000, "Red");
				//MyVisualScriptLogicProvider.PlayHudSoundLocal(VRage.Audio.MyGuiSounds.HudUnable, MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

			}

			InDisableRange = disable;

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
			
			var Block = Entity as IMyRadioAntenna;
			
			if(Block == null){
				
				return;
				
			}

			Block.IsWorkingChanged -= OnWorkingChange;
			MyVisualScriptLogicProvider.ToolEquipped -= ToolEquipped;

		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}