using System;
using System.Collections.Generic;
using BinaryPack.Attributes;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata;
using ProtoBufNet;

namespace NitroxModel.DataStructures.GameLogic.Entities
{
    [Serializable]
    [ProtoContract]
    public class PrefabPlaceholderEntity : Entity
    {
        [ProtoMember(1)]
        public string ClassId { get; set; }

        [IgnoreConstructor]
        protected PrefabPlaceholderEntity()
        {
            // Constructor for serialization. Has to be "protected" for json serialization.
        }

        public PrefabPlaceholderEntity(NitroxId id, NitroxTechType techType, NitroxId parentId)
        {
            Id = id;
            TechType = techType;
            ParentId = parentId;
            ChildEntities = new List<Entity>();
        }

        /// <remarks>Used for deserialization</remarks>
        public PrefabPlaceholderEntity(NitroxId id, string classId, NitroxTechType techType, EntityMetadata metadata, NitroxId parentId, List<Entity> childEntities)
        {
            Id = id;
            ClassId = classId;
            TechType = techType;
            Metadata = metadata;
            ParentId = parentId;
            ChildEntities = childEntities;
        }
    }
}
