using System;
using System.Collections;
using Circuits.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Circuits
{
    public partial class GameManager : Singleton<GameManager>
    {
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
        public IEnumerator ILoadLevel(int index, [CanBeNull] IEnumerator additionalTask)
        {
            UI.gameObject.SetActive(true);
            yield return IObstructView();
            yield return IFadeScreen();
            yield return SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
            if (additionalTask != null)
                yield return additionalTask;
            yield return IShowScreen();
            yield return IClearView();
        }
    }
}