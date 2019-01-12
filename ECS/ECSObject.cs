namespace ECS {
    public abstract class ECSObject {
        public readonly string UID;
        public string Name;

        public ECSObject() {
            UID = Helper.CreateUID();
            Name = GetType().Name;
        }
    }
}
