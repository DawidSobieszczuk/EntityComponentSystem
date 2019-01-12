using Newtonsoft.Json;

namespace ECS {
    public abstract class System : ECSObject {
        private readonly string[] RequireComponentNames;
        protected Entity CurrentEntity { get; private set; }

        public System(params string[] requireComponentNames) {
            RequireComponentNames = requireComponentNames;
        }

        protected T Get<T>() where T : Component {
            return CurrentEntity.GetComponent<T>();
        }

        public bool Match(Entity entity) {
            if(RequireComponentNames.Length < 1)
                return false;
            foreach(string s in RequireComponentNames) {
                if(!entity.HasComponent(s)) return false;
            }

            CurrentEntity = entity;
            return true;
        }

        public virtual void Init() { }
        public virtual void Update(float dt) { }
        public virtual void Draw() { }

        public override string ToString() {
            return string.Format("System<{0}:{1}>", Name, UID);
        }
    }
}
