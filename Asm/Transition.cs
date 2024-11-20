using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YellowPanda.Transition {
    public class Transition : MonoBehaviour {

        public static Transition defaultTransitionPrefab;
        public static Transition ST;
        public static Transition overrideTransitionPrefab;

        // Parameters
        public bool autoFadeIn = true;

        // Components
        Animator myAnimator;

        // Internal
        bool sameScene, sameSceneAutoIn;
        AsyncOperation loadingScene;
        TransitionCallback sameSceneCallback;

        // Delegates
        public delegate void TransitionCallback();

        public static void Goto(string scene, Transition transitionPrefab = null) {
            if (defaultTransitionPrefab == null) {
                Debug.LogWarning("No default transition set, use Transition.defaultTransitionPrefab to set a default transition");
            }
            overrideTransitionPrefab = transitionPrefab;

            // Create if isn't instantiated already
            if (ST == null) {
                if (overrideTransitionPrefab == null)
                    ST = Instantiate(defaultTransitionPrefab);
                else
                    ST = Instantiate(overrideTransitionPrefab);
            }
            // Disable auto fade in for this instance
            ST.autoFadeIn = false;
            ST.sameScene = false;

            // Load requested scene
            ST.loadingScene = SceneManager.LoadSceneAsync(scene);
            ST.loadingScene.allowSceneActivation = false;
            TransitionManager.Instance.PrepareSceneIntro(scene);

            // Fade out
            ST.myAnimator.Play("Out");
        }

        public static void SameSceneTransition(TransitionCallback callback = null, bool autoIn = true) {
            // Create if isn't instantiated already
            if (ST == null) {
                ST = Instantiate(defaultTransitionPrefab);
            }

            // Disable auto fade in for this instance
            ST.autoFadeIn = false;

            // Assign callback
            ST.sameSceneCallback = callback;
            ST.sameSceneAutoIn = autoIn;
            ST.sameScene = true;

            // Fade out
            ST.myAnimator.Play("Out");
        }

        void OnDestroy() {
            // Release reference
            ST = null;
        }

        public static void FadeIn() {
            if (ST == null) return;

            ST.myAnimator.Play("In");
        }

        public void FadeInDone() {
            Destroy(gameObject);
        }

        public void FadeOutDone() {
            if (sameScene) {
                sameSceneCallback?.Invoke();
                if (sameSceneAutoIn) StartCoroutine(FadeInSkipFrame());
            }
            else {
                loadingScene.allowSceneActivation = true;
            }
        }

        IEnumerator FadeInSkipFrame() {
            // Skip 2 frames
            yield return null;
            yield return null;

            // Fade in
            myAnimator.Play("In");
        }

        void Start() {
            if (autoFadeIn) {
                myAnimator.Play("In", 0, 0);
            }
        }

        void Awake() {
            if (ST != null && ST != this) {
                Destroy(ST.gameObject);
            }

            ST = this;

            // Components
            myAnimator = GetComponent<Animator>();
        }
    }
}