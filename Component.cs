namespace ECS {
    public abstract class Component : ECSObject {
        public Component() : base() { }

        public override string ToString() {
            return string.Format("Component<{0}:{1}>", Name, UID);
        }
    }
}
