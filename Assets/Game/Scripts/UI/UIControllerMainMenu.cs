using System;
using System.Collections;
using System.Collections.Generic;
using log4net.Core;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Logger = Circuits.Utility.Logger;
using static Circuits.Utility.Utils; 
namespace Circuits.UI
{
    public class UIControllerMainMenu : UIController
    {
        private VisualTreeAsset levelSelectTemplateAsset;
        private VisualElement _levelTemplateContainer;
        
        private Button _buttonTutorial, 
            _buttonPlay, 
            _buttonSettings, 
            _buttonCredits, 
            _buttonQuit,
            _buttonTutorialConfirm,
            _buttonLevelSelectConfirm;

        private VisualElement _tutorialWindow, _levelSelectWindow;

        private LevelEntry _selectedLevel;

        private readonly List<LevelEntry> _levelEntries = new List<LevelEntry>()
        {
            new LevelEntry("Tutorial0", "Tutorial - O básico"),
            new LevelEntry("Tutorial1", "Tutorial - Interruptores", new string[] {"Tutorial0"}),
            new LevelEntry("Tutorial2", "Tutorial - Capacitores", new string[] {"Tutorial1"}),
            new LevelEntry("Main1", "Um pequeno desafio...", new string[] {"Tutorial2"}),
        };
        
        public override void Awake()
        {
            base.Awake();

            levelSelectTemplateAsset = Resources.Load<VisualTreeAsset>("UI/LevelTemplate");
            _levelTemplateContainer = _document.rootVisualElement.Q<Button>("LevelTemplate").parent;
            
            
            _buttonTutorial = _document.rootVisualElement.Q<Button>("ButtonTutorial");
            _buttonPlay = _document.rootVisualElement.Q<Button>("ButtonPlay");
            _buttonSettings = _document.rootVisualElement.Q<Button>("ButtonSettings");
            _buttonCredits = _document.rootVisualElement.Q<Button>("ButtonCredits");
            _buttonQuit = _document.rootVisualElement.Q<Button>("ButtonQuit");
            
            _tutorialWindow = _document.rootVisualElement.Q<VisualElement>("BasicTutorial");
            _tutorialWindow.style.top = new StyleLength(new Length(105f, LengthUnit.Percent));
            _buttonTutorialConfirm = _document.rootVisualElement.Q<Button>("ButtonTutorialConfirm");
            
            _levelSelectWindow = _document.rootVisualElement.Q<VisualElement>("LevelSelect");
            _buttonLevelSelectConfirm = _document.rootVisualElement.Q<Button>("ButtonPlayLevel");
            
            _buttonTutorial.clicked += TutorialButton;
            _buttonPlay.clicked += PlayButton;
            _buttonSettings.clicked += SettingsButton;
            _buttonCredits.clicked += CreditsButton;
            _buttonQuit.clicked += QuitButton;
            
            _buttonTutorialConfirm.clicked += TutorialButtonConfirm;
            _buttonLevelSelectConfirm.clicked += LevelSelectButtonConfirm;

            GameManager.OnPlayerDataChanged.AddListener(RepopulateUI);
            RepopulateUI();
        }
        
        // Update all the stuff we need that is based on data or progress.
        private void RepopulateUI()
        {
            UpdateButtons();
            PopulateLevels();
        }

        // Some buttons have conditions to be clickable so we will update them more frequently than other stuff.
        private void UpdateButtons()
        {
            _buttonPlay.SetEnabled(GameManager.playerData.tutorialComplete);
            _buttonLevelSelectConfirm.SetEnabled(_selectedLevel != null && (_selectedLevel.RequirementsMet()) && !GameManager.Instance.IsLoadingLevel);
        }

        // ==================== MAIN MENU =========================================================== 
        
        private void TutorialButton()
        {
            SetActiveWindow(_tutorialWindow);
        }

        private void PlayButton()
        {
            SetActiveWindow(_levelSelectWindow);
        }

        private void SettingsButton()
        {
            CloseAllWindows();
        }

        private void CreditsButton()
        {
            CloseAllWindows();
        }

        private void QuitButton()
        {
            CloseAllWindows();
        }

        // ==================== TUTORIAL ===========================================================
        
        private void TutorialButtonConfirm()
        { 
            CloseAllWindows();
            GameManager.playerData.tutorialComplete = true;
            GameManager.SavePlayerData();
        }

        // ==================== LEVEL SELECT ===========================================================

        private void PopulateLevels()
        {
            _levelTemplateContainer.Clear();
            foreach (LevelEntry entry in _levelEntries)
            {
                Button button = levelSelectTemplateAsset.CloneTree().Q<Button>("LevelTemplate");
                button.SetEnabled(entry.RequirementsMet());
                button.text = entry.displayName;
                button.clicked += () =>
                {
                    _selectedLevel = entry;
                    UpdateButtons();
                };
                
                _levelTemplateContainer.Add(button);
            }
        }
        
        private void LevelSelectButtonConfirm()
        {
            if (_selectedLevel == null)
                return;
            
            GameManager.Instance.LoadLevel(_selectedLevel.levelName);
            UpdateButtons();
        }
        
        // ==================== METHODS ===========================================================
        
        private void CloseAllWindows()
        {
            SetWindowState(_tutorialWindow, false);
        }
        
        private void SetActiveWindow(VisualElement window)
        {
            if (window != _tutorialWindow) SetWindowState(_tutorialWindow, false);
            if (window != _levelSelectWindow) SetWindowState(_levelSelectWindow, false);
            SetWindowState(window, true);
        }

        private void SetWindowState(VisualElement window, bool state, float time = 1f)
        {
            var from = window.style.top.value.value;
            var to = state ? 5f : 105f;
            
            LeanTween.value(gameObject, from, to, time)
                .setOnUpdate(Set)
                .setEaseSpring();

            void Set(float x)
            {
                window.style.top = new StyleLength(new Length(x, LengthUnit.Percent));
            }
        }
    }

    public class LevelEntry
    {
        public string displayName = "Fase";
        public string levelName = "Main1";
        public string[] levelRequirements = Array.Empty<string>();

        public LevelEntry(string levelName, string displayName, string[] levelRequirements = null)
        {
            this.displayName = displayName;
            this.levelName = levelName;
            this.levelRequirements = levelRequirements ?? Array.Empty<string>();
        }

        public bool RequirementsMet()
        {
            if (levelRequirements == null || levelRequirements.Length == 0) return true;
            bool allCompleted = true;

            if (!SceneExists(levelName))
                return false;
            
            foreach (var level in levelRequirements)
            {
                if (!GameManager.playerData.IsLevelComplete(level))
                {
                    allCompleted = false;
                }
            }

            return allCompleted;
        }
    }
}