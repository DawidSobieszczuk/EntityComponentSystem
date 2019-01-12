using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace ECS {
    public class World : ECSObject {
        #region static
        private static JsonSerializerSettings _jss = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        private static byte[] _DefaultSalt {
            get => new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };
        }

        private static void _WriteBytes(string path, byte[] bytes) {
            using(BinaryWriter bw = new BinaryWriter(File.OpenWrite(path))) {
                bw.Write(bytes.Length);
                bw.Write(bytes);
            }
        }

        private static void _CreateAndSetAesKeyIV(string key, byte[] salt, Aes aes) {
            if(salt == null)
                salt = _DefaultSalt;

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);
        }

        public static void SaveToFile(World world) {
            SaveToFile(world, world.Name);
        }

        /// <summary>
        /// Save World to file
        /// </summary>
        /// <param name="world">World To Save</param>
        /// <param name="path">World Path</param>
        /// <param name="key">No Encrypt when null</param>
        /// <param name="salt">Generate when null</param>
        public static void SaveToFile(World world, string path, string key = null, byte[] salt = null) {
            byte[] worldData;
            
            using(MemoryStream ms = new MemoryStream()) { 
                using(BsonDataWriter writer = new BsonDataWriter(ms)) {
                    JsonSerializer serializer = JsonSerializer.Create(_jss);

                    serializer.Serialize(writer, world);
                }
                worldData = ms.ToArray();
            }

            if(key == null) {
                _WriteBytes(path, worldData);
            } else {
                using(Aes encryptor = Aes.Create()) {
                    _CreateAndSetAesKeyIV(key, salt, encryptor);
                    using(MemoryStream ms = new MemoryStream()) {
                        using(CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)) {
                            cs.Write(worldData, 0, worldData.Length);
                        }
                        _WriteBytes(path, ms.ToArray());
                    }
                }
            }
        }


        /// <summary>
        /// Create World From File.
        /// </summary>
        /// <param name="path">World Path</param>
        /// <param name="key">If encrypted</param>
        /// <returns></returns>
        public static World LoadFromFile(string path, string key = null, byte[] salt = null) {
            World world = null;
            byte[] worldData = null;

            using(BinaryReader br = new BinaryReader(File.OpenRead(path))) {
                int length = br.ReadInt32();

                if(key == null) {
                    worldData = br.ReadBytes(length);
                } else {
                    using(Aes decryptor = Aes.Create()) {
                        _CreateAndSetAesKeyIV(key, salt, decryptor);
                        using(CryptoStream cs = new CryptoStream(br.BaseStream, decryptor.CreateDecryptor(), CryptoStreamMode.Read)) {
                            int offset = sizeof(int);
                            byte[] data = new byte[length + offset];
                            cs.Read(data, offset, length);
                            worldData = data.Skip(offset).ToArray();
                        }
                    }
                }
            }

            using(MemoryStream ms = new MemoryStream(worldData)) {
                using(BsonDataReader reader = new BsonDataReader(ms)) {
                    JsonSerializer serializer = JsonSerializer.Create(_jss);
                    world = serializer.Deserialize<World>(reader);
                }
            }


            return world;
        }
        #endregion

        [JsonProperty()]
        internal readonly List<Entity> Entities;
        [JsonProperty()]
        internal readonly List<System> Systems;

        public World() {
            Entities = new List<Entity>();
            Systems = new List<System>();
        }

        public void SaveToFile(string path, string key = null, byte[] salt = null) {
            SaveToFile(this, path, key, salt);
        }

        public void CreateEntity(string name, params Component[] components) {
            Entity entity = new Entity(name);
            entity.AddComponents(components);
            AddEntity(entity);
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
        enum ForEachType { Init, Update, Draw }
        private void ForEach(ForEachType type, float dt) {
            Systems.ForEach(s => {
                Entities.ForEach(e => {
                    if(s.Match(e)) { // <- this set CurrentEntity
                        switch(type) {
                            case ForEachType.Init:
                                s.Init();
                                break;
                            case ForEachType.Update:
                                s.Update(dt);
                                break;
                            case ForEachType.Draw:
                                s.Draw();
                                break;
                        }
                    }
                });
            });
        }

        public void Init() {
            ForEach(ForEachType.Init, 0);
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
