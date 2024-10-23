using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = Circuits.Utility.Logger;

namespace Circuits.UI
{
    public class UIControllerMainMenu : UIController
    {
        private Button _buttonTutorial;
        private Button _buttonPlay;
        private Button _buttonSettings;
        private Button _buttonCredits;
        private Button _buttonQuit;

        public override void Awake()
        {
            base.Awake();

            _buttonTutorial = _document.rootVisualElement.Q<Button>("ButtonTutorial");
            _buttonPlay = _document.rootVisualElement.Q<Button>("ButtonPlay");
            _buttonSettings = _document.rootVisualElement.Q<Button>("ButtonSettings");
            _buttonCredits = _document.rootVisualElement.Q<Button>("ButtonCredits");
            _buttonQuit = _document.rootVisualElement.Q<Button>("ButtonQuit");

            _buttonTutorial.clicked += TutorialButton;
            _buttonPlay.clicked += PlayButton;
            _buttonSettings.clicked += SettingsButton;
            _buttonCredits.clicked += CreditsButton;
            _buttonQuit.clicked += QuitButton;
        }
        
        private void TutorialButton() {}

        private void PlayButton()
        {
            StartCoroutine(Test());
        }
        private void SettingsButton() {}
        private void CreditsButton() {}
        private void QuitButton() {}

        private IEnumerator Test()
        {
            yield return GameManager.Instance.UI.IToggleLoadingScreen(true);
            yield return new WaitForSeconds(2.5f);
            yield return GameManager.Instance.UI.IToggleLoadingScreen(false);
        }
    }
}