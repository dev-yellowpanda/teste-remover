using UnityEngine;
using UnityEngine.SceneManagement;

namespace YellowPanda.Transition {
    public class TransitionManager : MonoBehaviour {

        public static TransitionManager Instance;
        private string sceneToPrepare;

        [SerializeField] Transition defaultTransition;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                if (Instance != this) {
                    Destroy(gameObject);
                    return;
                }
            }
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Transition.defaultTransitionPrefab = defaultTransition;
        }

        public void PrepareSceneIntro(string scene) {
            sceneToPrepare = scene;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
            if (sceneToPrepare == null) return;
            if (sceneToPrepare.Length == 0) return;

            if (scene.name == sceneToPrepare) {
                if (Transition.overrideTransitionPrefab == null)
                    Instantiate(Transition.defaultTransitionPrefab);
                else
                    Instantiate(Transition.overrideTransitionPrefab);

                Transition.overrideTransitionPrefab = null;
                sceneToPrepare = "";
            }
        }
    }
}