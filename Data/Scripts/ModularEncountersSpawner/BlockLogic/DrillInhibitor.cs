using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using System;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

namespace ModularEncountersSpawner.BlockLogic {

	//Change MyObjectBuilder_LargeGatlingTurret to the matching ObjectBuilder for your block
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "MES-Suppressor-Drill-Large", "MES-Suppressor-Drill-Small")]
	 
	public class DrillInhibitor : MyGameLogicComponent{
		
		IMyRadioAntenna Antenna;
		bool IsWorking = false;
		bool IsNpcOwned = false;
		bool IsValid = false;
		bool IsDedicated = false;

		bool SetupDone = false;
		bool InDisableRange = false;

		int toolbarIndex = 0;

		float defaultRange = 500;
		string lastCustomData = "";
		int tickCount = 0;

		DateTime LastToolbarChange = MyAPIGateway.Session.GameDateTime;

		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{
				
				Antenna = Entity as IMyRadioAntenna;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public override void UpdateBeforeSimulation10() {

			if (SetupDone == false) {

				SetupDone = true;

				Antenna = Entity as IMyRadioAntenna;

				if (Antenna == null) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				IsDedicated = MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated;
				Antenna.IsWorkingChanged += OnWorkingChange;
				Antenna.CustomName = "[Drill Inhibitor Field]";
				SetRange();
				IsWorking = Antenna?.Enabled ?? false;
				MyVisualScriptLogicProvider.ToolbarItemChanged += ToolbarItemChanged;
				Antenna.OwnershipChanged += OnOwnerChange;
				IsValid = Antenna?.SlimBlock?.CubeGrid?.Physics != null;
				OnWorkingChange(Antenna);
				OnOwnerChange(Antenna);

			}

			if (Antenna == null) {

				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;

			}

			if (!IsWorking)
				return;

			tickCount += 10;

			if (tickCount >= 100) {

				tickCount = 0;
				SetRange();
			
			}

			if (IsDedicated)
				return;

			var character = MyAPIGateway.Session?.LocalHumanPlayer?.Character as IMyCharacter;
			var controlledEntity = MyAPIGateway.Session?.LocalHumanPlayer?.Controller?.ControlledEntity?.Entity;

			if (character == null || controlledEntity == null || character.EntityId != controlledEntity.EntityId) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Controller Character Mismatch", 4000);
				return;

			}
				

			if (character.IsDead)
				return;

			var distance = Vector3D.Distance(character.GetPosition(), Antenna.GetPosition());
			//MyVisualScriptLogicProvider.ShowNotificationLocal(distance.ToString(), 4000, "White");
			//MyVisualScriptLogicProvider.ShowNotificationLocal("Chara" + character.GetPosition().ToString(), 4000, "White");
			//MyVisualScriptLogicProvider.ShowNotificationLocal("Block" + Antenna.GetPosition().ToString(), 4000, "White");

			var disable = distance <= defaultRange && IsNpcOwned;

			if (disable && !InDisableRange && character.EquippedTool != null) {

				MyVisualScriptLogicProvider.ShowNotificationLocal("WARNING: Inhibitor Field Has Disabled Hand Drill Use!", 4000, "Red");
				//MyVisualScriptLogicProvider.PlayHudSoundLocal(VRage.Audio.MyGuiSounds.HudUnable, MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

			}

			InDisableRange = disable;
			ToolEquipped(MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

		}

		public void ToolEquipped(long playerId, string typeId = "", string subtypeId = "") {

			if (IsDedicated || MyAPIGateway.Session?.LocalHumanPlayer == null) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Player Null", 4000, "White");
				return;

			}

			if (playerId != MyAPIGateway.Session.LocalHumanPlayer.IdentityId) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Identity Not Matched", 4000, "White");
				return;

			}

			if (!InDisableRange || !IsNpcOwned || !IsValid) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Outside Disable Range", 4000, "White");
				return;

			}

			if (MyAPIGateway.Session.LocalHumanPlayer.Character?.EquippedTool == null) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Tool Null", 4000, "White");
				return;

			}

			var timeSpan = MyAPIGateway.Session.GameDateTime - LastToolbarChange;

			if (timeSpan.TotalMilliseconds < 250) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Toolbar Change Timer", 4000, "White");
				return;

			}

			var drill = MyAPIGateway.Session.LocalHumanPlayer.Character.EquippedTool as IMyHandDrill;

			if (drill == null) {

				//MyVisualScriptLogicProvider.ShowNotificationLocal("Drill Null", 4000, "White");
				return;

			}

			if (drill.IsShooting) {

				MyVisualScriptLogicProvider.SetPlayersHealth(MyAPIGateway.Session.LocalHumanPlayer.IdentityId, MyVisualScriptLogicProvider.GetPlayersHealth(MyAPIGateway.Session.LocalHumanPlayer.IdentityId) - 6);
			
			}

			toolbarIndex++;

			if (toolbarIndex > 8)
				toolbarIndex = 0;

			MyVisualScriptLogicProvider.SwitchToolbarToSlot(toolbarIndex, MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

		}

		void ToolbarItemChanged(long entityId, string typeId, string subtypeId, int page, int slot) {

			if (MyAPIGateway.Session?.LocalHumanPlayer?.Character == null || MyAPIGateway.Session.LocalHumanPlayer.Character.EntityId != entityId)
				return;

			LastToolbarChange = MyAPIGateway.Session.GameDateTime;

		}

		void OnOwnerChange(IMyTerminalBlock block) {

			if (block.OwnerId == 0 || MyAPIGateway.Players.TryGetSteamId(block.OwnerId) > 0) {

				IsNpcOwned = false;
				return;

			}

			IsNpcOwned = true;

		}

		void OnWorkingChange(IMyCubeBlock block) {

			if(block.IsWorking == false || block.IsFunctional == false) {

				IsWorking = false;
				return;

			}

			IsWorking = true;
			
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

			Block.IsWorkingChanged -= OnWorkingChange;
			//MyVisualScriptLogicProvider.ToolEquipped -= ToolEquipped;
			Block.OwnershipChanged -= OnOwnerChange;
			MyVisualScriptLogicProvider.ToolbarItemChanged -= ToolbarItemChanged;

		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}