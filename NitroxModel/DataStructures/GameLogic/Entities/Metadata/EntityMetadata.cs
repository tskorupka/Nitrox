using System;
using ProtoBufNet;

namespace NitroxModel.DataStructures.GameLogic.Entities.Metadata
{
    [Serializable]
    [ProtoContract]
    [ProtoInclude(50, typeof(KeypadMetadata))]
    [ProtoInclude(60, typeof(SealedDoorMetadata))]
    [ProtoInclude(70, typeof(PrecursorDoorwayMetadata))]
    [ProtoInclude(80, typeof(PrecursorTeleporterMetadata))]
    [ProtoInclude(90, typeof(PrecursorKeyTerminalMetadata))]
    [ProtoInclude(100, typeof(PrecursorTeleporterActivationTerminalMetadata))]
    [ProtoInclude(110, typeof(StarshipDoorMetadata))]
    [ProtoInclude(120, typeof(WeldableWallPanelGenericMetadata))]
    [ProtoInclude(130, typeof(IncubatorMetadata))]
    [ProtoInclude(140, typeof(EntitySignMetadata))]
    [ProtoInclude(150, typeof(ConstructorMetadata))]
    [ProtoInclude(160, typeof(FlashlightMetadata))]
    [ProtoInclude(170, typeof(BatteryMetadata))]
    [ProtoInclude(180, typeof(RepairedComponentMetadata))]
    public abstract class EntityMetadata
    {
    }
}
