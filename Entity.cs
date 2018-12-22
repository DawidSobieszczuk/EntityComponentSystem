using System.Collections.Generic;
using System.Diagnostics;

namespace ECS {
    public class Entity : ECSObject {
        private readonly List<Component> _Components;
        internal World World;

        public Entity Parent;
        public Entity[] Children { get { if(World == null) return new Entity[0]; return World.Entities.FindAll(x => x.Parent == this).ToArray(); } }

        public Entity() : base() {
            _Components = new List<Component>();
        }

        public void AddChild(Entity child) {
            if(child == this)
                return;
            child.Parent = this;
        }

        public void AddComponent(Component component) {
            if(Helper.CheckExistName(_Components, component, this))
                return;

            _Components.Add(component);
        }

        public void AddComponents(params Component[] components) {
            foreach(Component component in components)
                AddComponent(component);
        }

        public Component FindComponent(string name){
            return _Components.Find(x => x.Name == name);
        }

        public T FindComponent<T>(string name) where T : Component {
            return (T)_Components.Find(x => x.Name == name);
        }

        public bool HasComponent(string name) {
            return _Components.Exists(x => x.Name == name);
        }

        public bool RemoveComponent(string name) {            
            return _Components.RemoveAll(x => x.Name == name) > 0;
        }

        public override string ToString() {
            return string.Format("Entity<{0}:{1}>", Name, UID);
        }
    }
}
