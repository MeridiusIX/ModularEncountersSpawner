﻿using ModularEncountersSpawner.Templates;
using Sandbox.Common.ObjectBuilders;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game;
using VRage.ObjectBuilders;

namespace ModularEncountersSpawner.Manipulation {

	public struct ArmorForReplacement {

		public List<MyObjectBuilder_CubeBlock> Blocks;
		public MyObjectBuilder_CubeBlock Block;

		public ArmorForReplacement(List<MyObjectBuilder_CubeBlock> blocks, MyObjectBuilder_CubeBlock block) {

			Blocks = blocks;
			Block = block;

		}

	}

	public static class ArmorModuleReplacement {

		public static List<MyDefinitionId> SmallArmor = new List<MyDefinitionId>();
		public static List<MyDefinitionId> LargeArmor = new List<MyDefinitionId>();

		public static List<MyDefinitionId> SmallModules = new List<MyDefinitionId>();
		public static List<MyDefinitionId> LargeModules = new List<MyDefinitionId>();

		public static List<string> ModuleSubtypes = new List<string>();

		private static Random _rnd = new Random();
		private static bool _setupComplete = false;

		public static void Setup() {

			_setupComplete = true;
			SmallArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorBlock"));
			SmallArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorBlock"));
			LargeArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorBlock"));
			LargeArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorBlock"));

			SmallModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-Nanobots-Small"));
			SmallModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-JumpDrive-Small"));
			SmallModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-Jetpack-Small"));
			SmallModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-Drill-Small"));

			LargeModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-Nanobots-Large"));
			LargeModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-JumpDrive-Large"));
			LargeModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-Jetpack-Large"));
			LargeModules.Add(new MyDefinitionId(typeof(MyObjectBuilder_RadioAntenna), "MES-Suppressor-Drill-Large"));

			foreach (var id in SmallModules)
				ModuleSubtypes.Add(id.SubtypeName);

			foreach (var id in LargeModules)
				ModuleSubtypes.Add(id.SubtypeName);
		}

		public static void ProcessGridForModules(MyObjectBuilder_CubeGrid[] grids, ImprovedSpawnGroup spawnGroup) {

			if (!_setupComplete)
				Setup();

			bool setArmor = false;
			List<MyDefinitionId> allowedArmor = null;
			List<MyDefinitionId> allowedModules = null;
			List<MyDefinitionId> usedModules = new List<MyDefinitionId>();
			var availableArmor = new List<ArmorForReplacement>();


			foreach (var grid in grids) {

				if (grid?.CubeBlocks == null)
					return;

				if (!setArmor) {

					setArmor = true;
					allowedArmor = grid.GridSizeEnum == MyCubeSize.Large ? LargeArmor : SmallArmor;
					allowedModules = grid.GridSizeEnum == MyCubeSize.Large ? LargeModules : SmallModules;

				}
		
				foreach (var block in grid.CubeBlocks) {

					if (allowedArmor.Contains(block.GetId()))
						availableArmor.Add(new ArmorForReplacement(grid.CubeBlocks, block));

				}

			}

			for (int i = 0; i < spawnGroup.ModulesForArmorReplacement.Count; i++) {

				if (availableArmor.Count == 0) {

					break;

				}

				var armorIndex = availableArmor.Count == 1 ? 0 : _rnd.Next(0, availableArmor.Count);

				if (usedModules.Contains(spawnGroup.ModulesForArmorReplacement[i]) || !allowedModules.Contains(spawnGroup.ModulesForArmorReplacement[i]) || !ReplaceArmorWithModule(availableArmor[armorIndex].Blocks, availableArmor[armorIndex].Block, spawnGroup.ModulesForArmorReplacement[i])) {

					continue;

				}

				availableArmor.RemoveAt(armorIndex);
				usedModules.Add(spawnGroup.ModulesForArmorReplacement[i]);

			}

			if (MES_SessionCore.InhibitorModDetected) {

				for (int i = 0; i < allowedModules.Count; i++) {

					if (availableArmor.Count == 0) {

						break;

					}

					var armorIndex = availableArmor.Count == 1 ? 0 : _rnd.Next(0, availableArmor.Count);

					if (usedModules.Contains(allowedModules[i]) || !allowedModules.Contains(allowedModules[i]) || !ReplaceArmorWithModule(availableArmor[armorIndex].Blocks, availableArmor[armorIndex].Block, allowedModules[i])) {

						continue;

					}

					availableArmor.RemoveAt(armorIndex);
					usedModules.Add(allowedModules[i]);

				}

			}

		}

		public static bool ReplaceArmorWithModule(List<MyObjectBuilder_CubeBlock> blocks, MyObjectBuilder_CubeBlock oldBlock, SerializableDefinitionId newBlockId) {

			var newBlock = MyObjectBuilderSerializer.CreateNewObject(newBlockId) as MyObjectBuilder_CubeBlock;

			if (newBlock == null) {

				Logger.AddMsg("Could Not Add Module To Prefab, It Does Not Exist: " + newBlockId.ToString());
				return false;

			}
			
			newBlock.BlockOrientation = oldBlock.BlockOrientation;
			newBlock.Min = oldBlock.Min;
			newBlock.ColorMaskHSV = oldBlock.ColorMaskHSV;
			newBlock.Owner = oldBlock.Owner;

			blocks.Remove(oldBlock);
			blocks.Add(newBlock);

			SetDefaultInhibitorRanges(newBlock);

			return true;
		
		}

		public static void SetDefaultInhibitorRanges(MyObjectBuilder_CubeBlock block) {

			var antenna = block as MyObjectBuilder_RadioAntenna;

			if (antenna == null)
				return;

			if (antenna.SubtypeName == "MES-Suppressor-Nanobots-Large")
				antenna.BroadcastRadius = 1000;

			if (antenna.SubtypeName == "MES-Suppressor-JumpDrive-Large")
				antenna.BroadcastRadius = 6000;

			if (antenna.SubtypeName == "MES-Suppressor-Jetpack-Large")
				antenna.BroadcastRadius = 1000;

			if (antenna.SubtypeName == "MES-Suppressor-Drill-Large")
				antenna.BroadcastRadius = 500;

			if (antenna.SubtypeName == "MES-Suppressor-Nanobots-Small")
				antenna.BroadcastRadius = 1000;

			if (antenna.SubtypeName == "MES-Suppressor-JumpDrive-Small")
				antenna.BroadcastRadius = 6000;

			if (antenna.SubtypeName == "MES-Suppressor-Jetpack-Small")
				antenna.BroadcastRadius = 1000;

			if (antenna.SubtypeName == "MES-Suppressor-Drill-Small")
				antenna.BroadcastRadius = 500;

		}

	}

}
