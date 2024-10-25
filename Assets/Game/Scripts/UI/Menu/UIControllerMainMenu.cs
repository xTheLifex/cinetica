using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = Circuits.Utility.Logger;

namespace Circuits.UI
{
    public class UIControllerMainMenu : UIController
    {
        private Button _buttonTutorial, 
            _buttonPlay, 
            _buttonSandbox,
            _buttonSettings, 
            _buttonCredits, 
            _buttonQuit;

        public override void Awake()
        {
            base.Awake();

            _buttonTutorial = _document.rootVisualElement.Q<Button>("ButtonTutorial");
            _buttonPlay = _document.rootVisualElement.Q<Button>("ButtonPlay");
            _buttonSandbox = _document.rootVisualElement.Q<Button>("ButtonSandbox");
            _buttonSettings = _document.rootVisualElement.Q<Button>("ButtonSettings");
            _buttonCredits = _document.rootVisualElement.Q<Button>("ButtonCredits");
            _buttonQuit = _document.rootVisualElement.Q<Button>("ButtonQuit");

            _buttonTutorial.clicked += TutorialButton;
            _buttonPlay.clicked += PlayButton;
            _buttonSandbox.clicked += SandboxButton;
            _buttonSettings.clicked += SettingsButton;
            _buttonCredits.clicked += CreditsButton;
            _buttonQuit.clicked += QuitButton;

            GameManager.OnPlayerDataChanged.AddListener(SetupButtons);
            SetupButtons();
        }

        private void SetupButtons()
        {
            _buttonPlay.SetEnabled(GameManager.playerData.tutorialComplete);
            _buttonSandbox.SetEnabled(GameManager.playerData.tutorialComplete);
        }
        
        private void TutorialButton() {}
        private void PlayButton() {}
        private void SandboxButton() {}
        private void SettingsButton() {}
        private void CreditsButton() {}
        private void QuitButton() {}
    }
}