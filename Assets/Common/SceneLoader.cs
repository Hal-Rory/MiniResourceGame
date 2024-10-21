using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Utility
{
    public class SceneLoader : MonoBehaviour
    {
        public static Action OnPreUnload;
        public static Action OnPostUnload;
        public static Action OnPreLoad;
        public static Action OnPostLoad;
        public static void LoadScene(string room)
        {
            UnloadCurrentScene();
            OnPreLoad?.Invoke();
            SceneManager.LoadScene(room);
            OnPostLoad?.Invoke();
        }

        public static void UnloadCurrentScene()
        {
            OnPreUnload?.Invoke();
            OnPostUnload?.Invoke();
        }
        public static void UnloadScene(string room)
        {
            OnPreUnload?.Invoke();
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            OnPostUnload?.Invoke();
        }
    }
}
