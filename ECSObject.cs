using System.Collections.Generic;

namespace ECS {
    public abstract class ECSObject {
        static protected List<Entity> _Entities;

        public readonly long UID;
        public string Name;

        public ECSObject() {
            UID = Helper.CreateUID();
            Name = GetType().Name;
        }
    }
}
