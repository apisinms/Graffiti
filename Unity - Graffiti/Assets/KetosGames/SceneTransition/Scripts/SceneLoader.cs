using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace KetosGames.SceneTransition
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader SceneLoaderInstance;


        public GameObject LoadingScreen;
        public Image FadeImage;
        public Material FadeMaterial;

        [Tooltip("Use with VR.")]
        public bool VRMode = false;
        [Tooltip("When checked, use the Loading scene as the Loading screen (instead of the Loading UI).")]
        public bool UseSceneForLoadingScreen = true;
        [Tooltip("The name of the Loading scene to load.")]
        public string LoadingSceneName = "Loading";
        [Tooltip("When checked, fade in the loading screen.")]
        public bool FadeInLoadingScreen = true;
        [Tooltip("When checked, fade out the loading screen.")]
        public bool FadeOutLoadingScreen = true;
        [Tooltip("The number of seconds to animate the fade.")]
        public float FadeSeconds = 1f;
        [Tooltip("The number of seconds to show the loading screen after fade in. Set it to 0 to go to the new scene as soon as it's ready.")]
        public float MinimumLoadingScreenSeconds = 1f;
        [Tooltip("The color to use in the fade animation.")]
        public Color FadeColor = Color.black;

        // Get the progress of the load
        [HideInInspector]
        public float Progress = 0;

        private AsyncOperation SceneLoadingOperation;
        public bool waitOtherPlayer { get; set; }
        private bool FadingIn = true;
        private bool FadingOut = false;
        private float FadeTime = 0;
        private Color FadeClearColor;
        private bool Loading = false;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static SceneLoader Instance
        {
            get
            {
                if (SceneLoaderInstance == null)
                {
                    SceneLoader sceneLoader = (SceneLoader)GameObject.FindObjectOfType(typeof(SceneLoader));
                    if (sceneLoader != null)
                    {
                        SceneLoaderInstance = sceneLoader;
                    }
                    else
                    {
                        GameObject SceneLoaderPrefab = Resources.Load<GameObject>("KetosGames/SceneTransition/Prefabs/SceneLoader");
                        SceneLoaderInstance = (GameObject.Instantiate(SceneLoaderPrefab)).GetComponent<SceneLoader>();
                    }
                }
                return SceneLoaderInstance;
            }
        }

        /// <summary>
        /// Loads a scene.
        /// </summary>
        /// <param name="name">Name of the scene to load</param>
        public static void LoadScene(string name, bool _waitOther = false)
        {
            Instance.Load(name, _waitOther);
        }

        /// <summary>
        /// Loads a scene.
        /// </summary>
        /// <param buildIndex="buildIndex">Build index of the scene to load</param>
        public static void LoadScene(int buildIndex, bool _waitOther = false)
        {
            Instance.Load(buildIndex, _waitOther);
        }

        /// <summary>
        /// Awake
        /// </summary>
        public void Awake()
        {
            Object.DontDestroyOnLoad(this.gameObject);

            // Get rid of any old SceneLoaders
            if (SceneLoaderInstance != null && SceneLoaderInstance != this)
            {
                Destroy(SceneLoaderInstance.gameObject);
                SceneLoaderInstance = this;
            }

            if (VRMode && FadeMaterial == null)
            {
                throw new System.Exception("Fade Material is required for VR Support!");
            }
            if (!VRMode && FadeImage == null)
            {
                throw new System.Exception("Fade Image is required!");
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            SetFadersEnabled(true);
            if (LoadingScreen != null)
            {
                LoadingScreen.SetActive(false);
            }
            if (!Loading)
            {
                BeginFadeIn();
            }
        }

        /// <summary>
        /// Update 
        /// </summary>
        void Update()
        {
            // 로딩 전 강종 시
            if (NetworkManager.instance.CheckSomeoneQuitBeforeGameLoad() == true)
            {
                Destroy(gameObject);

                MessageBox.Show("다른 플레이어가 종료했습니다.");

                // 3초 뒤에 그냥 바로 로비로 감
                YieldInstructionCache.WaitForSeconds(5.0f);
                SceneManager.LoadScene("Lobby");
                NetworkManager.instance.SendGotoLobby();
            }

            // 최대 로딩 대기시간 넘어감
            if (NetworkManager.instance.CheckMaxLoadingTime() == true)
            {
                Destroy(gameObject);

                MessageBox.Show("최대 로딩 대기시간을 초과하였습니다.");

                // 3초 뒤에 그냥 바로 로비로 감
                YieldInstructionCache.WaitForSeconds(5.0f);
                SceneManager.LoadScene("Lobby");
                NetworkManager.instance.SendGotoLobby();
            }

            if (FadingIn)
            {
                UpdateFadeIn();
            }
            else if (FadingOut)
            {
                UpdateFadeOut();
            }
        }

        /// <summary>
        /// Begins the fade out.
        /// </summary>
        public void BeginFadeOut()
        {
            UpdateCamera();
            if (FadingIn && FadeTime > 0)
            {
                FadeTime = 1 / FadeTime; // Reverse fade
            }
            else
            {
                FadeTime = 0;
                setFadeColor(Color.clear);
            }
            SetFadersEnabled(true);
            FadingIn = false;
            FadingOut = true;
            FadeClearColor = FadeColor;
            FadeClearColor.a = 0;
        }

        /// <summary>
        /// Begins the fade in.
        /// </summary>
        public void BeginFadeIn()
        {
            UpdateCamera();
            if (FadingOut && FadeTime > 0)
            {
                FadeTime = 1 / FadeTime; // Reverse fade
            }
            else
            {
                FadeTime = 0;
                setFadeColor(FadeColor);
            }
            SetFadersEnabled(true);
            FadingIn = true;
            FadingOut = false;
            FadeClearColor = FadeColor;
            FadeClearColor.a = 0;
        }

        /// <summary>
        /// Ends the fade in.
        /// </summary>
        private void EndFadeIn()
        {
            setFadeColor(Color.clear);
            SetFadersEnabled(false);
            FadingIn = false;

			// 만약 인게임으로 들어가는 FadeIn이었다면 FadeIn 끝나고 초기 인게임 패킷 보내준다.
			if(waitOtherPlayer == true)
			{
				NetworkManager.instance.SendIngamePacket(true);
			}
        }

        /// <summary>
        /// Ends the fade out.
        /// </summary>
        private void EndFadeOut()
        {
            setFadeColor(FadeColor);
            FadingOut = false;
        }

        /// <summary>
        /// Fade in as a scene is starting
        /// </summary>
        private void UpdateFadeIn()
        {
            FadeTime += Time.unscaledDeltaTime / FadeSeconds;
            setFadeColor(Color.Lerp(FadeColor, FadeClearColor, FadeTime));

            if (FadeTime > 1)
            {
                EndFadeIn();
            }
        }

        /// <summary>
        /// Fade out as a scene is ending
        /// </summary>
        private void UpdateFadeOut()
        {
            FadeTime += Time.unscaledDeltaTime / FadeSeconds;
            setFadeColor(Color.Lerp(FadeClearColor, FadeColor, FadeTime));

            if (FadeTime > 1)
            {
                EndFadeOut();
            }
        }

        /// <summary>
        /// Loads a scene
        /// </summary>
        /// <param name="name">Name of the scene to load</param>
        public void Load(string name, bool _waitOtehr)
        {
            if (!Loading)
            {
                var scene = new Scene
                {
                    SceneName = name
                };
                StartCoroutine(InnerLoad(scene, _waitOtehr));
            }
        }

        /// <summary>
        /// Loads a scene
        /// </summary>
        /// <param buildIndex="buildIndex">Build index of the scene to load</param>
        public void Load(int buildIndex, bool _waitOtehr)
        {
            if (!Loading)
            {
                var scene = new Scene
                {
                    BuildIndex = buildIndex
                };
                StartCoroutine(InnerLoad(scene, _waitOtehr));
            }
        }

        /// <summary>
        /// Coroutine for loading the scene
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="name">Name of the scene to load</param>
        IEnumerator InnerLoad(Scene scene, bool _waitOther)
        {
            Loading = true;
            Progress = 0;

            // Fade out
            BeginFadeOut();
            while (FadingOut)
            {
                yield return null;
            }

            if (UseSceneForLoadingScreen)
            {
                //Show loading scene
                SceneManager.LoadScene(LoadingSceneName);
            }
            else
            {
                if (!VRMode && LoadingScreen != null)
                {
                    LoadingScreen.SetActive(true);
                }
            }

            // Fade in
            if (UseSceneForLoadingScreen || !VRMode)
            {
                if (FadeInLoadingScreen)
                {
                    BeginFadeIn();
                    while (FadingIn)
                    {
                        if (MinimumLoadingScreenSeconds == 0f && SceneLoadingOperation.progress >= 0.9f)
                        {
                            break; // the scene is finished loading, lets get out of here
                        }
                        yield return null;
                    }
                }
                else
                {
                    EndFadeIn();
                }
            }

            var startTime = Time.unscaledTime;

            //Start to load the level we want in the background
            if (!string.IsNullOrEmpty(scene.SceneName))
            {
                SceneLoadingOperation = SceneManager.LoadSceneAsync(scene.SceneName);
            }
            else
            {
                SceneLoadingOperation = SceneManager.LoadSceneAsync(scene.BuildIndex);
            }
            SceneLoadingOperation.allowSceneActivation = false;

            //Wait for the level to finish loading
            while (SceneLoadingOperation.progress < 0.9f)
            {
                Progress = SceneLoadingOperation.progress;
                yield return null;
            }
            Progress = 1f;

            // Wait for MinimumLoadingScreenSeconds
            if (UseSceneForLoadingScreen || !VRMode)
            {
                while (Time.unscaledTime - startTime < MinimumLoadingScreenSeconds)
                {
                    yield return null;
                }
            }

            SetFadersEnabled(true); // Enable Faders in new scene before switching to it

            // Fade out
            if (UseSceneForLoadingScreen || !VRMode)
            {
                if (FadeOutLoadingScreen)
                {
                    BeginFadeOut();
                    while (FadingOut)
                    {
                        yield return null;
                    }
                }
                else
                {
                    EndFadeOut();
                }
            }

            // 다른 플레이어를 기다려야하는 씬일 경우에만 로딩 패킷 보낸다.
            if (_waitOther == true)
            {
                // 다음 씬 로딩시키고
                SceneLoadingOperation.allowSceneActivation = true;

                // 다 로딩 될 때까지 기다린다
                while (!SceneLoadingOperation.isDone)
                {
                    yield return null;
                }

                // 다음 씬이 모두 로딩 됐다면 로딩완료 패킷을 보낸다.
                NetworkManager.instance.SendLoadingComplete();

                // 아직 다른 플레이어 로딩이 안됐다면 모두 될 때까지 기다려준다.
                while (waitOtherPlayer == false)
                {
                    yield return null;
                }

            }
            else if (_waitOther == false)
            {
                SceneLoadingOperation.allowSceneActivation = true;
            }

            while (!SceneLoadingOperation.isDone)
            {
                yield return null;
            }
            if (LoadingScreen != null)
            {
                LoadingScreen.SetActive(false);
            }

            // Fade in
            BeginFadeIn();

            Loading = false; // At this point is should be safe to start a new load even though it's still fading in
        }

        /// <summary>
        /// Setup the sceneLoader canvas based on the VR Support
        /// </summary>
        private void UpdateCamera()
        {
            if (VRMode && FadeMaterial != null)
            {
                // Find all cameras and add fade material to them (initially disabled)
                foreach (Camera c in Camera.allCameras)
                {
                    if (c.gameObject.GetComponent<ScreenFadeControl>() == null)
                    {
                        var fadeControl = c.gameObject.AddComponent<ScreenFadeControl>();
                        fadeControl.fadeMaterial = FadeMaterial;
                    }
                }
            }
        }

        /// <summary>
        /// Sets if the faders are enabled.
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        private void SetFadersEnabled(bool value)
        {
            if (VRMode)
            {
                // Find all cameras and set enabled
                foreach (Camera c in Camera.allCameras)
                {
                    if (c.gameObject.GetComponent<ScreenFadeControl>() != null)
                    {
                        c.gameObject.GetComponent<ScreenFadeControl>().enabled = value;
                    }
                }
            }
            else
            {
                FadeImage.gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// Sets the color of the fade.
        /// </summary>
        /// <param name="value">Value.</param>
        private void setFadeColor(Color value)
        {
            if (VRMode)
            {
                FadeMaterial.color = value;
            }
            else
            {
                FadeImage.color = value;
            }
        }
    }

    class Scene
    {
        public string SceneName;
        public int BuildIndex;
    }
}