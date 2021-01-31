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


namespace ModularEncountersSpawner.BlockLogic {

	//Change MyObjectBuilder_LargeGatlingTurret to the matching ObjectBuilder for your block
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_InteriorLight), false, "MESThrustTrailLarge", "MESThrustTrailSmall")]

	public class ThrustTrailBlock : MyGameLogicComponent {

		IMyInteriorLight Block = null;

		bool SetupDone = false;
		bool IsDedicated = false;
		bool Enabled = false;

		List<IMyThrust> Thrusters = new List<IMyThrust>();

		public override void Init(MyObjectBuilder_EntityBase objectBuilder) {

			base.Init(objectBuilder);

			try {

				Block = Entity as IMyInteriorLight;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;

			} catch (Exception exc) {



			}

		}

		public override void UpdateBeforeSimulation() {

			if (SetupDone == false) {

				SetupDone = true;
				IsDedicated = MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated;

				if (IsDedicated == true) {

					NeedsUpdate = MyEntityUpdateEnum.NONE;
					return;

				}

				Block = Entity as IMyInteriorLight;
				Block.IsWorkingChanged += OnWorkingChange;
				Enabled = Block.Enabled;

				var allBlocks = new List<IMySlimBlock>();
				Block.SlimBlock.CubeGrid.GetBlocks(allBlocks);

				foreach (var block in allBlocks.Where(b => b.FatBlock != null)) {

					if (block.FatBlock as IMyThrust != null)
						Thrusters.Add(block.FatBlock as IMyThrust);

				}

			}

			if (Block == null) {

				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;

			}



		}

		void OnWorkingChange(IMyCubeBlock block) {

			if (block.IsWorking == false || block.IsFunctional == false) {

				Enabled = false;
				return;

			}

			Enabled = true;

		}

		public override void OnRemovedFromScene() {

			base.OnRemovedFromScene();

			var Block = Entity as IMyInteriorLight;

			if (Block == null) {

				return;

			}

			Block.IsWorkingChanged -= OnWorkingChange;

		}

		public override void OnBeforeRemovedFromContainer() {

			base.OnBeforeRemovedFromContainer();

			if (Entity.InScene == true) {

				OnRemovedFromScene();

			}

		}



	}

	public class ThrusterTrailProfile{

		public IMyThrust Thruster;

		public bool ProfileValid;
		public bool Enabled;

		public ThrusterTrailProfile() {
		
			
		
		}

	}

	public class TrailProfile {

		public Vector3D Start;
		public Vector3D End;
		public Vector4 Color;

		public TrailProfile() {

			Start = Vector3D.Zero;
			End = Vector3D.Zero;
			Color = Vector4.Zero;
 
		}

	}
	
}