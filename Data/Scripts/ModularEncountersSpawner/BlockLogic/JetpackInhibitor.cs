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
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "MES-Suppressor-Jetpack-Large")]
	 
	public class JetpackInhibitor : MyGameLogicComponent{
		
		IMyRadioAntenna Antenna;
		bool IsWorking = false;
		bool IsValid = false;

		bool SetupDone = false;
		bool InDampenerRange = false;
		bool InDisableRange = false;

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

				if(MyAPIGateway.Utilities.IsDedicated && MyAPIGateway.Multiplayer.IsServer) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				Antenna = Entity as IMyRadioAntenna;

				if (Antenna == null) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				Antenna.IsWorkingChanged += OnWorkingChange;
				Antenna.CustomName = "[Jetpack Inhibitor Field]";
				Antenna.Radius = 1200;
				IsWorking = Antenna?.Enabled ?? false;

			}

			if(Antenna == null){
				
				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;
				
			}

			if (!IsWorking)
				return;

			if (!InDisableRange && !InDampenerRange)
				return;

			var character = MyAPIGateway.Session?.LocalHumanPlayer?.Character;

			if (character == null || character.IsDead || !character.EnabledThrusts)
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

			if (MyAPIGateway.Session?.LocalHumanPlayer?.Character == null)
				return;

			if (MyAPIGateway.Session.LocalHumanPlayer.Character.IsDead)
				return;

			var distance = Vector3D.Distance(MyAPIGateway.Session.LocalHumanPlayer.Character.GetPosition(), Antenna.GetPosition());

			var dampeners = distance <= Antenna.Radius;
			var disable = distance <= Antenna.Radius / 3;

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

		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}