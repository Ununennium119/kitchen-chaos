using UnityEngine;

namespace Common.Utility {
    /// <summary>
    /// A utility class for logging different types of repeating messages.
    /// </summary>
    public static class Logger {
        public static void LogMultipleInstancesError(Object obj) {
            Debug.LogError($"There is more than one {GetClassFullName(obj)} in the scene! Destroying this one...");
        }

        public static void LogInitializingInstance(Object obj) {
            Debug.Log($"Setting up {GetClassFullName(obj)}...");
        }

        public static void LogInstanceInitialized(Object obj) {
            Debug.Log($"{GetClassFullName(obj)} is initialized.");
        }


        private static string GetClassFullName(Object obj) {
            return obj.GetType().FullName;
        }
    }
}
