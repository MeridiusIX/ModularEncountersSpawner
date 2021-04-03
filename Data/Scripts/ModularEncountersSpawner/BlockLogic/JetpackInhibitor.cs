using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.ModAPI;
using System;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;


namespace ModularEncountersSpawner.BlockLogic {

	//Change MyObjectBuilder_LargeGatlingTurret to the matching ObjectBuilder for your block
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "MES-Suppressor-Jetpack-Large", "MES-Suppressor-Jetpack-Small")]
	 
	public class JetpackInhibitor : MyGameLogicComponent{
		
		IMyRadioAntenna Antenna;
		bool IsWorking = false;
		bool IsNpcOwned = false;
		bool IsValid = false;
		bool IsDedicated = false;

		bool SetupDone = false;
		bool InDampenerRange = false;
		bool InDisableRange = false;

		float defaultRange = 1200;
		string lastCustomData = "";
		int tickCount = 0;

		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{
				
				Antenna = Entity as IMyRadioAntenna;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public override void UpdateBeforeSimulation(){

			if(SetupDone == false) {

				SetupDone = true;

				Antenna = Entity as IMyRadioAntenna;

				if (Antenna == null) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				IsDedicated = MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated;
				Antenna.IsWorkingChanged += OnWorkingChange;
				Antenna.OwnershipChanged += OnOwnerChange;
				IsValid = Antenna?.SlimBlock?.CubeGrid?.Physics != null;
				Antenna.CustomName = "[Jetpack Inhibitor Field]";
				SetRange();
				IsWorking = Antenna?.Enabled ?? false;

				OnWorkingChange(Antenna);
				OnOwnerChange(Antenna);

			}

			if(Antenna == null){
				
				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;
				
			}

			if (IsDedicated)
				return;

			if (!IsWorking || !IsNpcOwned || !IsValid)
				return;

			if (!InDisableRange && !InDampenerRange)
				return;



			var character = MyAPIGateway.Session?.LocalHumanPlayer?.Character as IMyCharacter;
			var controlledEntity = MyAPIGateway.Session?.LocalHumanPlayer?.Controller?.ControlledEntity?.Entity;

			if (character == null || character.IsDead || character != controlledEntity || !character.EnabledThrusts)
				return;



			if (InDisableRange) {

				character.SwitchThrusts();

			} else {

				if (character.EnabledDamping)
					character.SwitchDamping();
			
			}

		}

		public override void UpdateBeforeSimulation100() {

			if (!IsWorking)
				return;


			tickCount += 100;

			if (tickCount >= 100) {

				tickCount = 0;
				SetRange();

			}

			if (IsDedicated)
				return;

			if (MyAPIGateway.Session?.LocalHumanPlayer?.Character == null)
				return;

			if (MyAPIGateway.Session.LocalHumanPlayer.Character.IsDead)
				return;

			var distance = Vector3D.Distance(MyAPIGateway.Session.LocalHumanPlayer.Character.GetPosition(), Antenna.GetPosition());

			var dampeners = distance <= defaultRange && IsNpcOwned;
			var disable = distance <= defaultRange / 3 && IsNpcOwned;

			if (dampeners && !InDampenerRange) {

				MyVisualScriptLogicProvider.ShowNotificationLocal("WARNING: Inhibitor Field Has Disabled Jetpack Dampeners!", 4000, "Red");
				//MyVisualScriptLogicProvider.PlayHudSoundLocal(VRage.Audio.MyGuiSounds.HudUnable, MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

			}

			if (disable && !InDisableRange) {

				MyVisualScriptLogicProvider.ShowNotificationLocal("WARNING: Inhibitor Field Has Disabled Jetpack!", 4000, "Red");
				//MyVisualScriptLogicProvider.PlayHudSoundLocal(VRage.Audio.MyGuiSounds.HudUnable, MyAPIGateway.Session.LocalHumanPlayer.IdentityId);

			}

			InDampenerRange = dampeners;
			InDisableRange = disable;

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

		void OnWorkingChange(IMyCubeBlock block) {

			if(block.IsWorking == false || block.IsFunctional == false) {

				IsWorking = false;
				return;

			}

			IsWorking = true;
			
		}

		void OnOwnerChange(IMyTerminalBlock block) {

			if (block.OwnerId == 0 || MyAPIGateway.Players.TryGetSteamId(block.OwnerId) > 0) {

				IsNpcOwned = false;
				return;

			}

			IsNpcOwned = true;

		}

		public override void OnRemovedFromScene(){
			
			base.OnRemovedFromScene();
			
			var Block = Entity as IMyRadioAntenna;
			
			if(Block == null){
				
				return;
				
			}

			Block.IsWorkingChanged -= OnWorkingChange;
			Block.OwnershipChanged -= OnOwnerChange;

		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}