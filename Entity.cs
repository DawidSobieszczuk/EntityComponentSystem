using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ECS {
    public class Entity : ECSObject {
        private readonly List<Component> Components = new List<Component>();
        
        internal World World;

        public Entity Parent;
        public Entity[] Children { get { if(World == null) return new Entity[0]; return World.Entities.FindAll(x => x.Parent == this).ToArray(); } }

        public Entity(){ }
        public Entity(string name) {
            Name = name;
        }
        
        public void AddChild(Entity child) {
            if(child == this)
                return;
            child.Parent = this;
        }

        public void AddComponent(Component component) {
            if(Helper.CheckExistName(Components, component, this))
                return;

            Components.Add(component);
        }

        public void AddComponents(params Component[] components) {
            foreach(Component component in components)
                AddComponent(component);
        }

        public Component GetComponent(string name){
            try {
                return Components.Find(x => x.Name == name);
            } catch(ArgumentNullException e) {
                Debug.WriteLine("Component not exist: " + e);
            }

            return null;
        }

        public T GetComponent<T>() where T : Component {
            return (T)GetComponent(typeof(T).Name);
        }

        public bool HasComponent(string name) {
            return Components.Exists(x => x.Name == name);
        }

        public bool RemoveComponent(string name) {            
            return Components.RemoveAll(x => x.Name == name) > 0;
        }

        public override string ToString() {
            return string.Format("Entity<{0}:{1}>", Name, UID);
        }
    }
}
