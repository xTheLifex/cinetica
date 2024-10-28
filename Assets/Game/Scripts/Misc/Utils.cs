using UnityEngine;
using UnityEngine.SceneManagement;

namespace Circuits.Utility
{
    public static class Utils
    {
        public static bool SceneExists(string name) => SceneUtility.GetBuildIndexByScenePath(name) >= 0;
    }
}