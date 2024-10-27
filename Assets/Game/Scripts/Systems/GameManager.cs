using System;
using System.Collections;
using Circuits.UI;
using Circuits.Utility;
using UnityEngine;
using UnityEngine.Events;
using Logger = Circuits.Utility.Logger;

namespace Circuits
{
    public partial class GameManager : Singleton<GameManager>
    {
        [Header("Misc")]
        public UIControllerGameManager UI;
        private readonly Logger _logger = new Logger("GameManager");
        public override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            InitializeInput();
            LoadPlayerData();
            StartCoroutine(ILoadScreen(IFirstSetup()));
        }

        public virtual void Update()
        {
            UpdateInput();
            #if UNITY_EDITOR
                UpdateDebug();
            #endif
        }
        
        public IEnumerator IFirstSetup()
        {
            //TODO: Move to GameManager.Quality?
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1; 
            yield return new WaitForSeconds(.1f);
        }

        public void SaveData()
        {
            SavePlayerData();
        }
        
        public void Quit()
        {
            SaveData();
            
            // Quit from application
            Application.Quit();
            #if UNITY_EDITOR
            // We are in the editor so stop playmode.
            UnityEditor.EditorApplication.isPlaying = false;
            #endif  
        }

        // THIS METHOD IS UNRELIABLE.
        public void OnApplicationQuit()
        {
            SaveData();
        }

        // Save on losing focus.
        public void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                SaveData();
        }
    }
}