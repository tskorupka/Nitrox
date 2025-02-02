﻿using NitroxModel.DataStructures;
using NitroxModel.DataStructures.GameLogic.Entities;
using NitroxModel.Packets;
using NitroxServer.Communication.Packets.Processors.Abstract;
using NitroxServer.GameLogic;
using NitroxServer.GameLogic.Entities;

namespace NitroxServer.Communication.Packets.Processors
{
    class EntitySpawnedByClientProcessor : AuthenticatedPacketProcessor<EntitySpawnedByClient>
    {
        private readonly PlayerManager playerManager;
        private readonly WorldEntityManager worldEntityManager;
        private readonly EntitySimulation entitySimulation;

        public EntitySpawnedByClientProcessor(PlayerManager playerManager, WorldEntityManager worldEntityManager, EntitySimulation entitySimulation)
        {
            this.playerManager = playerManager;
            this.worldEntityManager = worldEntityManager;
            this.entitySimulation = entitySimulation;
        }

        public override void Process(EntitySpawnedByClient packet, Player playerWhoSpawned)
        {
            WorldEntity entity = packet.Entity;
            worldEntityManager.RegisterNewEntity(entity);

            SimulatedEntity simulatedEntity = entitySimulation.AssignNewEntityToPlayer(entity, playerWhoSpawned);

            SimulationOwnershipChange ownershipChangePacket = new SimulationOwnershipChange(simulatedEntity);
            playerManager.SendPacketToAllPlayers(ownershipChangePacket);

            foreach (Player player in playerManager.GetConnectedPlayers())
            {
                bool isOtherPlayer = player != playerWhoSpawned;
                if (isOtherPlayer && player.CanSee(entity))
                {
                    CellEntities cellEntities = new CellEntities(entity);
                    player.SendPacket(cellEntities);
                }
            }
        }
    }
}
