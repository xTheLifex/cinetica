using System;
using System.Collections;
using System.Collections.Generic;
using Circuits.Utility;
using static Circuits.Utility.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Circuits
{
    public partial class GameManager : Singleton<GameManager>
    {
        public bool IsLoadingLevel { get; private set; }
        
        /// <summary>
        /// Fades the screen to a black overlay with loading icon showing up.
        /// </summary>
        public IEnumerator IFadeScreen()
        {
            UI.gameObject.SetActive(true);
            yield return UI.IToggleLoadingScreen(true);
        }

        /// <summary>
        /// Hides the dark overlay and the loading icon.
        /// </summary>
        public IEnumerator IShowScreen()
        {
            UI.gameObject.SetActive(true);
            yield return UI.IToggleLoadingScreen(false);
        }
        
        /// <summary>
        /// Blocks the player view with effects.
        /// </summary>
        public IEnumerator IObstructView(bool instant = false)
        {
            UI.gameObject.SetActive(true);
            yield return UI.IToggleBlockers(true, instant);
        }

        /// <summary>
        /// Clears the player view from the blocker, letting the player see again.
        /// </summary>
        public IEnumerator IClearView(bool instant = false)
        {
            UI.gameObject.SetActive(true);
            yield return UI.IToggleBlockers(false, instant);
        }

        /// <summary>
        /// Shows a loading screen, before hiding it after completing an operation.
        /// </summary>
        /// <param name="Task">The operation to await.</param>
        public IEnumerator ILoadScreen(IEnumerator Task, bool obstruct = false)
        {
            UI.gameObject.SetActive(true); // Enable GameObject
            if (obstruct) yield return IObstructView(); // Obstruct the view if requested
            yield return IFadeScreen(); // Fade screen into a loading state
            yield return Task; // Execute loading tasks
            yield return IShowScreen(); // Clear the loading overlay 
            yield return IClearView(); // Clear any obstruction.
        }

        /// <summary>
        /// Shows a loading screen, before hiding it after completing an operation.
        /// </summary>
        /// <param name="tasks">The operations to await.</param>
        /// <param name="obstruct"></param>
        public IEnumerator ILoadScreen(IEnumerator[] tasks, bool obstruct = false)
        {
            UI.gameObject.SetActive(true);
            if (obstruct) yield return IObstructView();
            yield return IFadeScreen();
            foreach (IEnumerator task in tasks)
                yield return task;
            yield return IShowScreen();
            if (obstruct) yield return IClearView();
        }

        /// <summary>
        /// Calls necessary methods to load a new scene.
        /// </summary>
        /// <param name="index">The build index of the level to load.</param>
        /// <param name="additionalTask">An additional task to execute and await after the scene is loaded.</param>
        private IEnumerator ILoadLevel(string index, [CanBeNull] IEnumerator additionalTask)
        {
            _logger.Log("Preparing to load a new scene.");
            IsLoadingLevel = true;
            UI.gameObject.SetActive(true);
            yield return IObstructView();
            yield return IFadeScreen();
            _logger.Log("Loading a new scene...");
            //SceneManager.LoadScene(index, LoadSceneMode.Single);
            var loading = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
            loading.allowSceneActivation = false;
            while (!loading.isDone)
            {
                if (loading.progress >= 0.9f)
                {
                    _logger.Log("Scene appears to be loaded. Enabling scene activation and continuing.");
                    loading.allowSceneActivation = true;
                }
                yield return null;
            }
            _logger.Log("Waiting additional task...");
            if (additionalTask != null) yield return additionalTask;
            _logger.Log("Level loaded!");
            IsLoadingLevel = false;
            yield return IShowScreen();
            yield return IClearView();
        }

        private IEnumerator ILoadLevel(string index)
        {
            yield return ILoadLevel(index, null);
        }

        public void LoadLevel(string index)
        {
            if (!SceneExists(index))
            {
                _logger.LogError($"Trying to load invalid scene: {index}");
                return;
            }
            StartCoroutine(ILoadLevel(index));
        }

        public void LoadLevel(string index, IEnumerator additionalTask)
        {
            if (!SceneExists(index))
            {
                _logger.LogError($"Trying to load invalid scene: {index}");
                return;
            }
            StartCoroutine(ILoadLevel(index, additionalTask));
        }
    }
}