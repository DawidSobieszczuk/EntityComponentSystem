using System.Collections.Generic;

namespace ECS {
    public class World : ECSObject {
        internal readonly List<Entity> Entities;
        internal readonly List<System> Systems;

        public World() {
            Entities = new List<Entity>();
            Systems = new List<System>();
        }

        // Entity Add/Remove //
        public void AddEntity(Entity entity) {
            if(Helper.CheckExistUID(Entities, entity, this))
                return;

            Entities.Add(entity);
            entity.World = this;
        }

        public void AddEntities(params Entity[] entities) {
            foreach(Entity entity in entities)
                AddEntity(entity);
        }

        public bool RemoveEntity(Entity entity) {
            return Entities.Remove(entity);
        }

        public bool RemoveEntity(string name) {
            return Entities.RemoveAll(x => x.Name == name) > 0;
        }

        // System Add/Remove //
        public void AddSystem(System system) {
            if(Helper.CheckExistName(Systems, system, this))
                return;

            Systems.Add(system);
        }

        public void AddSystems(params System[] systems) {
            foreach(System system in systems)
                AddSystem(system);
        }

        public bool RemoveSystem(System system) {
            return Systems.Remove(system);
        }

        public bool RemoveSystem(string name) {
            return Systems.RemoveAll(x => x.Name == name) > 0;
        }

        // System Loop //
        enum ForEachType { Load, Update, Draw }
        private void ForEach(ForEachType type, float dt) {
            Systems.ForEach(s => {
                Entities.ForEach(e => {
                    if(s.Match(e)) {
                        switch(type) {
                            case ForEachType.Load:
                                s.Load(e);
                                break;
                            case ForEachType.Update:
                                s.Update(e, dt);
                                break;
                            case ForEachType.Draw:
                                s.Draw(e);
                                break;
                        }
                    }
                });
            });
        }

        public void Load() {
            ForEach(ForEachType.Load, 0);
        }

        public void Update(float dt) {
            ForEach(ForEachType.Update, dt);
        }

        public void Draw() {
            ForEach(ForEachType.Draw, 0);
        }

        //▼▾ Override ToString  ▾▼//
        public override string ToString() {
            return string.Format("World<{0}:{1}>", Name, UID);
        }
    }
}
