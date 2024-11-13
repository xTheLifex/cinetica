#if UNITY_EDITOR || DEVEL
using System;
using Cinetica.Utility;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Cinetica
{
    public partial class GameManager : Singleton<GameManager>
    {
        public void UpdateDebug()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                _logger.Log("Cleared all player preferences!");
                PlayerPrefs.DeleteAll();
                playerData = new PlayerData();
                SavePlayerData();
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                _logger.Log("Giving platinum player preferences!");
                playerData = PlayerData.Full();
                SavePlayerData();
            }
        }
        
        // For inspector events.
        public void Print(string msg)
        {
            Debug.Log(msg);
        }
    }
}


#endif