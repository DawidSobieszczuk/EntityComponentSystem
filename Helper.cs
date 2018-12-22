using System.Collections.Generic;
using System.Diagnostics;
namespace ECS {
    static class Helper {
        private static long UID = 0;

        public static long CreateUID() {
            return UID++;
        }

        public static bool CheckExistUID<T, K>(List<T> list, T item, K owner) where T : ECSObject where K : ECSObject {
            if(list.Count == 0)
                return false;

            if(list.Exists(x => x.UID == item.UID)) {
                Debug.WriteLine(string.Format("UID | {0} already exist in {1}", item, owner));
                return true;
            }

            return false;
        }

        public static bool CheckExistName<T, K>(List<T> list, T item, K owner) where T : ECSObject where K : ECSObject {
            if(list.Count == 0)
                return false;

            if(list.Exists(x => x.Name == item.Name)) {
                Debug.WriteLine(string.Format("Name | {0} already exist in {1}", item, owner));
                return true;
            }

            return false;
        }
    }
}
