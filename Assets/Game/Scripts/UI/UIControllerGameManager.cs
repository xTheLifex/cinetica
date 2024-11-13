using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static LeanTween;
using Logger = Cinetica.Utility.Logger;

namespace Cinetica.UI
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
            yield return IToggleBlockers(state, 0.5f, instant);
        }
        
        public IEnumerator IToggleBlockers(bool state, float time = 0.5f, bool instant = false)
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
            
            LeanTween.value(gameObject, from, to, time)
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
            yield return IToggleLoadingScreen(state, 0.25f, instant);
        }
        
        public IEnumerator IToggleLoadingScreen(bool state, float time, bool instant)
        {
            Color og = _loadingOverlay.style.backgroundColor.value;
            Debug.Log("Toggling Loading Screen to: " + state);
            if (!state)
                _loadingIcon.visible = false;
            else
                _loadingOverlay.pickingMode = PickingMode.Position;
            var completed = false;
            // Value here is transparency
            var from = state ? 0f : .6f;
            var to = state ? .6f : 0f;
            if (instant)
            {
                Set(to);
                yield break;
            }
            Set(from);
            
            LeanTween.value(gameObject, from, to, time)
                .setOnUpdate(Set)
                .setOnComplete(() => completed = true);

            yield return new WaitUntil(() => completed);
            if (state)
                _loadingIcon.visible = true;
            else
                _loadingOverlay.pickingMode = PickingMode.Ignore;
            Debug.Log("Loading Screen set to: " + state);
            Set(to);
            yield break;
            
            void Set(float a)
            {
                _loadingOverlay.style.backgroundColor = new StyleColor(new Color(og.r, og.g, og.b, a));
            }
        }
    }
}