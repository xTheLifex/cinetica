using Cinetica.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Cinetica
{
    public partial class GameManager : Singleton<GameManager>
    {
        public static PlayerData playerData = new PlayerData();
        
        public static UnityEvent OnPlayerDataChanged = new UnityEvent();
        public static void LoadPlayerData()
        {
            playerData = new PlayerData();
            playerData.Load();
        }

        public static void SavePlayerData()
        {
            playerData.Save();
        }
    }

    public class PlayerData
    {
        public bool tutorialComplete = false;

        public bool IsLevelComplete(string levelName) => PlayerPrefs.GetInt($"LevelCompletion-{levelName}", 0) == 1;
        public void SetLevelComplete(string levelname, bool completed = true) { PlayerPrefs.SetInt($"LevelCompletion-{levelname}", completed ? 1 : 0); }
        
        public static PlayerData Full()
        {
            var data = new PlayerData
            {
                tutorialComplete = true
            };

            foreach (var level in GameManager.GetLevelEntries())
                data.SetLevelComplete(level.levelName);
            
            GameManager.OnPlayerDataChanged?.Invoke();
            return data;
        }

        public void Load()
        {
            tutorialComplete = PlayerPrefs.GetInt("tutorialComplete", 0) == 1;
            GameManager.OnPlayerDataChanged?.Invoke();
        }

        public void Save()
        {
            PlayerPrefs.SetInt("tutorialComplete", tutorialComplete ? 1 : 0);
            GameManager.OnPlayerDataChanged?.Invoke();
            PlayerPrefs.Save();
        }
    }
}