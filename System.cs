using System.Collections.Generic;
using System.Diagnostics;

namespace ECS {
    public abstract class System : ECSObject {
        protected readonly List<string> __RequireComponetNames;

        public System() {
            __RequireComponetNames = new List<string>();
        }

        public bool Match(Entity entity) {
            if(__RequireComponetNames.Count < 1)
                return false;
            foreach(string s in  __RequireComponetNames) {
                if(!entity.HasComponent(s)) return false;
            }

            return true;
        }

        public virtual void Load(Entity entity) { }
        public virtual void Update(Entity entity, float dt) { }
        public virtual void Draw(Entity entity) { }

        public override string ToString() {
            return string.Format("System<{0}:{1}>", Name, UID);
        }
    }
}
