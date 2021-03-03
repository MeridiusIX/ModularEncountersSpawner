using ModularEncountersSpawner.Templates;
using Sandbox.Game;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace ModularEncountersSpawner.World {

	public static class CargoShipWatcher {

		public static Dictionary<IMyCubeGrid, Vector3> LastGridSpeed = new Dictionary<IMyCubeGrid, Vector3>();
		public static List<IMyCubeGrid> NoLongerMonitorGrids = new List<IMyCubeGrid>();
		public static List<IMyCubeGrid> RestoreGridSpeeds = new List<IMyCubeGrid>();
		public static List<IMyCubeGrid> NewlySpawnedGridSpeedCheck = new List<IMyCubeGrid>();

		public static void ProcessCargoShipSpeedWatcher() {

			//Process NewlySpawnedGridSpeedCheck List
			if (NewlySpawnedGridSpeedCheck.Count > 0) {

				foreach (var grid in NewlySpawnedGridSpeedCheck) {

					if (grid == null || MyAPIGateway.Entities.Exist(grid) == false) {

						continue;

					}

					if (grid.Physics == null || grid.IsStatic == true) {

						continue;

					}

					ActiveNPC npc = new ActiveNPC();

					if (NPCWatcher.ActiveNPCs.TryGetValue(grid, out npc) == false) {

						continue;

					}

					if (npc.SpawnType != "SpaceCargoShip") {

						continue;

					}

					if (grid.Physics.LinearVelocity.Length() <= 0.1 && npc.AutoPilotSpeed > 0.1) {

						var velocityVector = Vector3D.Normalize(npc.EndCoords - npc.StartCoords) * npc.AutoPilotSpeed;
						grid.Physics.LinearVelocity = velocityVector;

					}

				}

				NewlySpawnedGridSpeedCheck.Clear();

			}

			//Process RestoreGridSpeed List
			if (RestoreGridSpeeds.Count > 0) {

				foreach (var grid in RestoreGridSpeeds) {

					if (grid == null || MyAPIGateway.Entities.Exist(grid) == false) {

						continue;

					}

					if (grid.Physics == null || grid.IsStatic == true) {

						continue;

					}

					Vector3 velocity = Vector3.Zero;

					if (LastGridSpeed.TryGetValue(grid, out velocity) == false) {

						continue;

					}

					grid.Physics.LinearVelocity = velocity;
					Logger.AddMsg("Stopped Cargo Ship Resumed: " + grid.CustomName, true);

				}

				RestoreGridSpeeds.Clear();

			}

			//Start Parallel Process
			MyAPIGateway.Parallel.Start(() => {

				try {

					var players = new List<IMyPlayer>();
					var entities = new HashSet<IMyEntities>();

					var activeCargoShips = new List<IMyCubeGrid>();

					foreach (var grid in NPCWatcher.ActiveNPCs.Keys.ToList()) {

						if (grid == null || MyAPIGateway.Entities.Exist(grid) == false) {

							continue;

						}

						if (NoLongerMonitorGrids.Contains(grid) == true) {

							continue;

						}

						if (grid.Physics == null || grid.IsStatic == true) {

							continue;

						}

						ActiveNPC npc = new ActiveNPC();

						if (NPCWatcher.ActiveNPCs.TryGetValue(grid, out npc) == false) {

							continue;

						}

						if (npc.SpawnType != "SpaceCargoShip") {

							continue;

						}

						var line = new LineD(npc.StartCoords, npc.EndCoords);

						if (!grid.WorldAABB.Intersects(ref line)) {

							Logger.AddMsg("Grid Has Left Path: " + grid.CustomName, true);
							NoLongerMonitorGrids.Add(grid);
							continue;

						}

						if (CheckGridBlocksForAutopilot(grid) == false) {

							Logger.AddMsg("Grid Has AutoPilot: " + grid.CustomName, true);
							NoLongerMonitorGrids.Add(grid);
							continue;

						}

						if (grid.Physics.LinearVelocity.Length() <= 0.1) {

							Vector3 previousSpeed = Vector3.Zero;

							if (LastGridSpeed.TryGetValue(grid, out previousSpeed) == true) {

								RestoreGridSpeeds.Add(grid);

							} else {

								Logger.AddMsg("Stopped Cargo Ship Had No Previous Speed: " + grid.CustomName, true);
								NoLongerMonitorGrids.Add(grid);

							}

						} else {

							Vector3 previousSpeed = Vector3.Zero;

							if (LastGridSpeed.TryGetValue(grid, out previousSpeed) == true) {

								var diff = previousSpeed.Length() / grid.Physics.LinearVelocity.Length();

								if (diff <= 0.7f || diff >= 1.3f) {

									Logger.AddMsg("Cargo Ship Speed Reduced By Other Factors: " + grid.CustomName, true);
									NoLongerMonitorGrids.Add(grid);

								} else {

									LastGridSpeed[grid] = grid.Physics.LinearVelocity;

								}

							} else {

								Logger.AddMsg("Cargo Ship Speed Registered: " + grid.CustomName, true);
								LastGridSpeed.Add(grid, grid.Physics.LinearVelocity);

							}

						}

					}

				} catch (Exception e) {

					if (Logger.LoggerDebugMode == true) {

						Logger.AddMsg(e.ToString(), true);

					}

				}

			});


		}

		public static bool CheckGridBlocksForAutopilot(IMyCubeGrid grid) {

			var blockList = new List<IMySlimBlock>();
			grid.GetBlocks(blockList, x => x.FatBlock != null);
			bool hasAutopilot = false;

			foreach (var block in blockList.Where(x => x.FatBlock as IMyRemoteControl != null)) {

				var remote = block.FatBlock as IMyRemoteControl;

				if (remote.IsAutoPilotEnabled == true) {

					Logger.AddMsg("Cargo Ship Has Autopilot: " + grid.CustomName, true);
					hasAutopilot = true;
					break;

				}

				if (string.IsNullOrWhiteSpace(remote.Name) == true) {

					MyVisualScriptLogicProvider.SetName(remote.EntityId, remote.EntityId.ToString());

				}

				if (string.IsNullOrWhiteSpace(MyVisualScriptLogicProvider.DroneGetCurrentAIBehavior(remote.Name)) == false) {

					Logger.AddMsg("Cargo Ship Has Behavior: " + grid.CustomName, true);
					hasAutopilot = true;
					break;

				}

				if (remote.EnabledDamping == true) {

					Logger.AddMsg("Cargo Ship Has Dampeners: " + grid.CustomName, true);
					hasAutopilot = true;
					break;

				}

			}

			if (hasAutopilot == true) {

				NoLongerMonitorGrids.Add(grid);
				return false;

			}

			return true;

		}

	}

}
