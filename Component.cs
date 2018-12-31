namespace ECS {
    public abstract class Component : ECSObject {
        public new string Name { get => base.Name; }

        public Component() { }

        public override string ToString() {
            return string.Format("Component<{0}:{1}>", Name, UID);
        }
    }
}
