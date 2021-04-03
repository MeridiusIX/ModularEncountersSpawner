using ModularEncountersSpawner.Spawners;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.World;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace ModularEncountersSpawner.Admin {

    public static class Debug {

        public static bool DebugMode = true;

        public static void SpawnProgramBlockForControls() {

            var randomDir = MyUtils.GetRandomVector3D();
            var randomSpawn = randomDir * 1100000;
            var cubeGridOb = new MyObjectBuilder_CubeGrid();
            cubeGridOb.PersistentFlags = MyPersistentEntityFlags2.InScene;
            cubeGridOb.IsStatic = false;
            cubeGridOb.GridSizeEnum = MyCubeSize.Small;
            cubeGridOb.LinearVelocity = new Vector3(0, 0, 0);
            cubeGridOb.AngularVelocity = new Vector3(0, 0, 0);
            cubeGridOb.PositionAndOrientation = new MyPositionAndOrientation(randomSpawn, Vector3.Forward, Vector3.Up);
            var cubeBlockOb = new MyObjectBuilder_MyProgrammableBlock();
            cubeBlockOb.Min = new Vector3I(0, 0, 0);
            cubeBlockOb.SubtypeName = "SmallProgrammableBlock";
            cubeBlockOb.EntityId = 0;
            cubeBlockOb.Owner = 0;
            cubeBlockOb.BlockOrientation = new SerializableBlockOrientation(Base6Directions.Direction.Forward, Base6Directions.Direction.Up);
            cubeGridOb.CubeBlocks.Add(cubeBlockOb);
            MyAPIGateway.Entities.RemapObjectBuilder(cubeGridOb);
            var entitySmall = MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(cubeGridOb);

            var customControlBool = MyAPIGateway.TerminalControls.CreateProperty<bool, IMyProgrammableBlock>("MES-SendChatCommand");
            customControlBool.Enabled = Block => true;
            customControlBool.Getter = Block => {

                var steamId = MyAPIGateway.Players.TryGetSteamId(Block.OwnerId);

                if (steamId > 0) {

                    if (MyAPIGateway.Session.IsUserAdmin(steamId)) {

                        var cubeBlock = Block as MyCubeBlock;

                        if (cubeBlock?.IDModule != null && cubeBlock.IDModule.ShareMode == MyOwnershipShareModeEnum.None) {

                            var data = new SyncData();
                            data.ChatMessage = Block.CustomData.Trim();
                            data.PlayerId = Block.OwnerId;
                            data.PlayerPosition = Block.GetPosition();
                            data.SteamUserId = steamId;
                            data.Instruction = "MESChatMsg";
                            data.BlockSource = true;
                            data.Block = Block;
                            ChatCommand.ServerChatProcessing(data);
                            return true;

                        } else
                            Block.CustomData = "Block Sharing Must Be 'No Share'.";

                    } else
                        Block.CustomData = "Block Owner Not Admin.";

                } else
                    Block.CustomData = "Could Not Get ID From Block Owner.";

                return false;

            };
            MyAPIGateway.TerminalControls.AddControl<IMyProgrammableBlock>(customControlBool);

            entitySmall.Close();

        }

        public static void CheckZone() {

            if (DebugMode == false) {

                return;

            }

            var player = MyAPIGateway.Session.LocalHumanPlayer;
            SpawnResources.RefreshEntityLists();
            var check = SpawnResources.IsPositionInSafeZone(player.GetPosition());

            if (check == true) {

                Logger.AddMsg("In Zone!", true);

            }

        }

        public static void InitStoreBlocks() {

            var entityList = new HashSet<IMyEntity>();
            MyAPIGateway.Entities.GetEntities(entityList);

            foreach (var entity in entityList) {

                var cubeGrid = entity as IMyCubeGrid;

                if (cubeGrid == null) {

                    continue;

                }

                //EconomyHelper.InitNpcStoreBlock(cubeGrid);

            }

        }

        public static void ReputationCheck(SyncData receivedData) {

            var stringSplit = receivedData.ChatMessage.Split('.');

            if (stringSplit.Length != 4) {

                return;

            }

            var npcFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(stringSplit[3]);

            if (npcFaction == null) {

                return;

            }

            var myFaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(receivedData.PlayerId);
            int rep = 0;

            if (myFaction != null) {

                rep = MyAPIGateway.Session.Factions.GetReputationBetweenFactions(myFaction.FactionId, npcFaction.FactionId);

            } else {

                rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(receivedData.PlayerId, npcFaction.FactionId);

            }

            MyVisualScriptLogicProvider.ShowNotification("Reputation To Faction: " + rep.ToString(), 5000, "White", receivedData.PlayerId);

        }

    }

}