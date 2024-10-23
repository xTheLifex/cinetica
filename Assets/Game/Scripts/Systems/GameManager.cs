using System;
using System.Collections;
using Circuits.UI;
using Circuits.Utility;
using UnityEngine;
using Logger = Circuits.Utility.Logger;

namespace Circuits
{
    public partial class GameManager : Singleton<GameManager>
    {
        public UIControllerGameManager UI;
        
        private readonly Logger _logger = new Logger("GameManager");
        public override void Awake()
        {
            base.Awake();
            StartCoroutine(ILoadScreen(IFirstSetup()));
        }

        public IEnumerator IFirstSetup()
        {
            yield return new WaitForSeconds(.1f);
        }

        public void SaveData()
        {
            // TODO: Save data here.
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