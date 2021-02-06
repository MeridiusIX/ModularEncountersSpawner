using ModularEncountersSpawner.Templates;
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

		private static Random _rnd = new Random();
		private static bool _setupComplete = false;

		public static void ProcessGridForModules(MyObjectBuilder_CubeGrid[] grids, ImprovedSpawnGroup spawnGroup) {

			if (!_setupComplete) {

				_setupComplete = true;
				SmallArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorBlock"));
				SmallArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorBlock"));
				LargeArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorBlock"));
				LargeArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorBlock"));

			}

			bool setArmor = false;
			List<MyDefinitionId> allowedArmor = null;
			var availableArmor = new List<ArmorForReplacement>();

			foreach (var grid in grids) {

				if (grid?.CubeBlocks == null)
					return;

				if (!setArmor) {

					setArmor = true;
					allowedArmor = grid.GridSizeEnum == MyCubeSize.Large ? LargeArmor : SmallArmor;

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

				if (!ReplaceArmorWithModule(availableArmor[armorIndex].Blocks, availableArmor[armorIndex].Block, spawnGroup.ModulesForArmorReplacement[i])) {

					continue;

				}

				availableArmor.RemoveAt(armorIndex);

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

		}

	}

}
