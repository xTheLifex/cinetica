using System;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using static Cinetica.Utility.Utils; 
namespace Cinetica.UI
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
            GameManager.SavePlayerData();
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
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
            foreach (LevelEntry entry in GameManager.GetLevelEntries())
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
            SetWindowState(_tutorialWindow, false, 0f);
            SetWindowState(_levelSelectWindow, false, 0f);
        }
        
        private void SetActiveWindow(VisualElement window)
        {
            CloseAllWindows();
            SetWindowState(window, true);
        }

        private void SetWindowState(VisualElement window, bool state, float time = 1f)
        {
            var from = window.style.top.value.value;
            var to = state ? 5f : 105f;
            
            if (time <= 0f)
            {
                Set(to);
                return;
            }
            
            LeanTween.value(gameObject, from, to, time)
                .setOnUpdate(Set)
                .setEaseSpring();

            void Set(float x)
            {
                window.style.top = new StyleLength(new Length(x, LengthUnit.Percent));
            }
        }
    }
}