using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static LeanTween;
using Logger = Circuits.Utility.Logger;

namespace Circuits.UI
{
    public class UIControllerGameManager : UIController
    {
        private VisualElement _blockerRight;
        private VisualElement _blockerLeft;
        private VisualElement _loadingIcon;
        private VisualElement _loadingOverlay;

        public override void Awake()
        {
            base.Awake();
            _blockerRight = _document.rootVisualElement.Q<VisualElement>("BlockerRight");
            _blockerLeft = _document.rootVisualElement.Q<VisualElement>("BlockerLeft");
            _loadingIcon = _document.rootVisualElement.Q<VisualElement>("Icon");
            _loadingOverlay = _document.rootVisualElement.Q<VisualElement>("LoadingOverlay");
        }

        public void Update()
        {
            _loadingIcon.transform.rotation = Quaternion.Euler(0f, 0f, _loadingIcon.transform.rotation.eulerAngles.z +
                                                                       (Time.deltaTime * 100f));
        }

        public IEnumerator IToggleBlockers(bool state, bool instant = false)
        {
            Debug.Log("Toggling Blockers to: " + state);
            var completed = false;
            var from = state ? -100f : 0f;
            var to = state ? 0f : -100f;
            if (instant)
            {
                Set(to);
                yield break;
            }
            Set(from);
            
            LeanTween.value(gameObject, from, to, 1f)
                .setOnUpdate(Set)
                .setOnComplete(() => completed = true);

            yield return new WaitUntil(() => completed);
            Debug.Log("Blockers set to: " + state);
            Set(to);
            yield break;

            void Set(float x)
            {
                _blockerRight.style.right = new StyleLength(new Length(x, LengthUnit.Percent));
                _blockerLeft.style.left = new StyleLength(new Length(x, LengthUnit.Percent));
            }
        }

        public IEnumerator IToggleLoadingScreen(bool state, bool instant = false)
        {
            Debug.Log("Toggling Loading Screen to: " + state);
            if (!state)
                _loadingIcon.visible = false;
            var completed = false;
            // Value here is transparency, when turning on it goes from 100 to 0.
            var from = state ? 0f : 1f;
            var to = state ? 1f : 0f;
            if (instant)
            {
                Set(to);
                yield break;
            }
            Set(from);
            
            LeanTween.value(gameObject, from, to, 1f)
                .setOnUpdate(Set)
                .setOnComplete(() => completed = true);

            yield return new WaitUntil(() => completed);
            if (state)
                _loadingIcon.visible = true;
            Debug.Log("Loading Screen set to: " + state);
            Set(to);
            yield break;
            
            void Set(float a)
            {
                _loadingOverlay.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, a));
            }
        }
    }
}