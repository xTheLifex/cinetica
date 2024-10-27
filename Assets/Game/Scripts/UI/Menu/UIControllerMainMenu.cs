using System;
using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = Circuits.Utility.Logger;

namespace Circuits.UI
{
    public class UIControllerMainMenu : UIController
    {
        private Button _buttonTutorial, 
            _buttonPlay, 
            _buttonSettings, 
            _buttonCredits, 
            _buttonQuit,
            _buttonTutorialConfirm;

        private VisualElement _tutorialWindow;

        public override void Awake()
        {
            base.Awake();

            _buttonTutorial = _document.rootVisualElement.Q<Button>("ButtonTutorial");
            _buttonPlay = _document.rootVisualElement.Q<Button>("ButtonPlay");
            _buttonSettings = _document.rootVisualElement.Q<Button>("ButtonSettings");
            _buttonCredits = _document.rootVisualElement.Q<Button>("ButtonCredits");
            _buttonQuit = _document.rootVisualElement.Q<Button>("ButtonQuit");
            _buttonTutorialConfirm = _document.rootVisualElement.Q<Button>("ButtonTutorialConfirm");

            _tutorialWindow = _document.rootVisualElement.Q<VisualElement>("BasicTutorial");
            _tutorialWindow.style.top = new StyleLength(new Length(105f, LengthUnit.Percent));
            _buttonTutorial.clicked += TutorialButton;
            _buttonPlay.clicked += PlayButton;
            _buttonSettings.clicked += SettingsButton;
            _buttonCredits.clicked += CreditsButton;
            _buttonQuit.clicked += QuitButton;
            _buttonTutorialConfirm.clicked += TutorialButtonConfirm;

            GameManager.OnPlayerDataChanged.AddListener(SetupButtons);
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            _buttonPlay.SetEnabled(GameManager.playerData.tutorialComplete);
        }

        private void TutorialButton()
        {
            SetTutorialWindow(true);
        }

        private void PlayButton()
        {
            StartCoroutine(GameManager.Instance.ILoadLevel(1));
        }
        private void SettingsButton() {}
        private void CreditsButton() {}
        private void QuitButton() {}
        private void TutorialButtonConfirm()
        { 
            SetTutorialWindow(false);
            GameManager.playerData.tutorialComplete = true;
            GameManager.SavePlayerData();
        }

        private void SetTutorialWindow(bool state, float time = 1f)
        {
            var from = state ? 105f : 5f;
            var to = state ? 5f : 105f;

            LeanTween.value(gameObject, from, to, time)
                .setOnUpdate(Set)
                .setEaseSpring();

            void Set(float x)
            {
                _tutorialWindow.style.top = new StyleLength(new Length(x, LengthUnit.Percent));
            }
        }
    }
}