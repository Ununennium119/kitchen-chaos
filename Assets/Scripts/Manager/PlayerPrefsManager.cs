using UnityEngine;

namespace Manager {
    /// <summary>
    /// This class is responsible for accessing player preferences.
    /// </summary>
    /// <seealso cref="PlayerPrefs"/>
    public static class PlayerPrefsManager {
        private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
        private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
        private const string PLAYER_PREFS_BINDINGS = "BINDINGS";


        public static float GetMusicVolume(float defaultValue) {
            return PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, defaultValue);
        }

        public static void SetMusicVolume(float volume) {
            PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
            PlayerPrefs.Save();
        }


        public static float GetSoundEffectsVolume(float defaultValue) {
            return PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, defaultValue);
        }

        public static void SetSoundEffectsVolume(float volume) {
            PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
            PlayerPrefs.Save();
        }


        public static string GetPlayerBindingsJsonString() {
            return PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS);
        }

        public static void SetPlayerBindingsJsonString(string bindingJsonString) {
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, bindingJsonString);
            PlayerPrefs.Save();
        }

        public static bool HasPlayerBindingsJsonString() {
            return PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS);
        }
    }
}
