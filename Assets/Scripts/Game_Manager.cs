using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;

public class Game_Manager : MonoBehaviour
{
    //Test Component
    public bool AutoDrop = false;
    [Range(0.1f, 1)] public float AutoDropInterval = 0.1f;
    public float flt_TimeRemaining { get; set; } = 0;
    //End Of Test Component

    [SerializeField]
    private int score = 0;
    public int int_Player_Score
    {
        get { return score; }
        set
        {
            if (score != value)
            {
                score = value;
                SetScore(value);
            }
        }

    }

    public Dictionary<int, List<GameObject>> Dic_Virus;
    public Dictionary<int, List<GameObject>> Dic_Virus_Special;

    public GameObject[] VirusStyles;

    [SerializeField]
    List<GameObject> Lst_C1 = new List<GameObject>();
    [SerializeField]
    List<GameObject> Lst_C2 = new List<GameObject>();
    [SerializeField]
    List<GameObject> Lst_C3 = new List<GameObject>();
    [SerializeField]
    List<GameObject> Lst_C4 = new List<GameObject>();

    public List<GameObject> Lst_Go_Flame_Ball;

    [HideInInspector]
    public float Flt_LeftBorder;
    [HideInInspector]
    public float Flt_RightBorder;

    public static int PrPf_Int_HighScore = 0;
    public static int PrPf_Int_Music = 1;
    public static int PrPf_Int_Sound = 1;
    public static int PrPf_Int_Vibrate = 0;
    public static int PrPf_Int_AdsRemoved = 0;

    public int VirusID = 1;
    public string tag_MatureVirus;
    public string tag_FreshVirus;

    public GameObject Go_UILoseLine;
    public GameObject Go_UIPreLoseLine;
    public GameObject Go_GlobalVolume;
    public GameObject Go_LWall;
    public GameObject Go_RWall;
    public GameObject Go_Floor;
    public GameObject Go_FloorLine;
    public GameObject Go_FloorBloom_Light;
    public GameObject Go_FloorBloom_LightBugs;
    public GameObject Go_Scorebox;
    public GameObject Go_HighScorebox;
    public GameObject Go_Camera;
    public GameObject Go_CanvasPlayGround;
    public GameObject Go_SpawnPosition;
    public GameObject Go_SettingPage;
    public GameObject Go_SettingButton;
    public GameObject Go_MusicButton;
    public GameObject Go_SoundButton;
    public GameObject Go_VibrateButton;
    public GameObject Go_PowerupButton;
    public GameObject Go_PurchaseButton;
    public GameObject Go_Dollar;
    public static GameObject Go_Static_PurchaseButton;
    public static GameObject Go_Static_Dollar;

    public GameObject Go_BadVirus;

    public Material Mtr_Fire;
    public AudioClip Ac_SpawnCharge;
    public AudioClip Ac_SameColorBoom;
    public AudioClip Ac_FlameBall;
    public AudioClip Ac_Smallen;
    public AudioClip Ac_FloorBoom;
    public AudioClip Ac_FloorBoom2;

    public static bool DontMove;
    public static bool DontDrop;
    public static Game_Manager GameManager_Script;
    public static Volume_Manager Volumer;
    public static GameObject GameManager;

    private AudioSource As_SoundManager;
    private TextMeshProUGUI tmp_Score;
    private TextMeshProUGUI tmp_HighScore;
    private AdMober adMober;
    private AdMober.RewardedAds Ads_PowerUp;
    private AdMober.RewardedAds Ads_Skip;
    private Virus_Manager Script_VirusManager;
    private int PowerUpChance = 2;


    private void Awake()
    {
        GameManager_Script = this;
        GameManager = gameObject;
        Go_Static_PurchaseButton = Go_PurchaseButton;
        Go_Static_Dollar = Go_Dollar;
    }

    private void Start()
    {

#if DEBUG
        PlayerPrefs.SetInt(nameof(PrPf_Int_AdsRemoved), PrPf_Int_AdsRemoved);
#endif

        Volumer = GetComponent<Volume_Manager>();
        As_SoundManager = gameObject.AddComponent<AudioSource>();

        tmp_Score = Go_Scorebox.GetComponent<TextMeshProUGUI>();

        if (!PlayerPrefs.HasKey(nameof(PrPf_Int_HighScore))) PlayerPrefs.SetInt(nameof(PrPf_Int_HighScore), PrPf_Int_HighScore);
        if (!PlayerPrefs.HasKey(nameof(PrPf_Int_Music))) PlayerPrefs.SetInt(nameof(PrPf_Int_Music), PrPf_Int_Music);
        if (!PlayerPrefs.HasKey(nameof(PrPf_Int_Sound))) PlayerPrefs.SetInt(nameof(PrPf_Int_Sound), PrPf_Int_Sound);
        if (!PlayerPrefs.HasKey(nameof(PrPf_Int_Vibrate))) PlayerPrefs.SetInt(nameof(PrPf_Int_Vibrate), PrPf_Int_Vibrate);
        if (!PlayerPrefs.HasKey(nameof(PrPf_Int_AdsRemoved))) PlayerPrefs.SetInt(nameof(PrPf_Int_AdsRemoved), PrPf_Int_AdsRemoved);

        PrPf_Int_Music = PlayerPrefs.GetInt(nameof(PrPf_Int_Music));
        PrPf_Int_Sound = PlayerPrefs.GetInt(nameof(PrPf_Int_Sound));
        PrPf_Int_Vibrate = PlayerPrefs.GetInt(nameof(PrPf_Int_Vibrate));
        PrPf_Int_AdsRemoved = PlayerPrefs.GetInt(nameof(PrPf_Int_AdsRemoved));
        AdsRemoved(AdMober.AdsRemoved = PrPf_Int_AdsRemoved == 1);

        GetComponent<AudioSource>().volume = PrPf_Int_Music;
        FadeBtnAColor(Go_MusicButton, PrPf_Int_Music == 0);

        FadeBtnAColor(Go_SoundButton, PrPf_Int_Sound == 0);
        FadeBtnAColor(Go_VibrateButton, PrPf_Int_Vibrate == 0);

        PrPf_Int_HighScore = PlayerPrefs.GetInt(nameof(PrPf_Int_HighScore));
        tmp_HighScore = Go_HighScorebox.GetComponent<TextMeshProUGUI>();
        tmp_HighScore.SetText(PrPf_Int_HighScore.ToString("D6"));

        Flt_LeftBorder = Go_LWall.transform.localPosition.x;
        Flt_RightBorder = Go_RWall.transform.localPosition.x - 30;

        Dic_Virus = new Dictionary<int, List<GameObject>>()
        {
            [0] = Lst_C1,
            [1] = Lst_C2,
            [2] = Lst_C3,
            [3] = Lst_C4,
        };

        adMober = new AdMober();
        Ads_PowerUp = new AdMober.RewardedAds();
        Ads_PowerUp.rewardedAd.OnAdClosed += DropControl;
        Ads_PowerUp.rewardedAd.OnUserEarnedReward += Proceed_PowerUp;

        Ads_Skip = new AdMober.RewardedAds();
        Ads_Skip.rewardedAd.OnAdClosed += DropControl;
        Ads_Skip.rewardedAd.OnUserEarnedReward += Proceed_Skip;
    }

    public void SetVirusManager(Virus_Manager vm)
    {
        Script_VirusManager = vm;
    }

    public int GetNewVirusID()
    {
        return VirusID++;
    }

    public int GetVirusIDNow()
    {
        return VirusID;
    }

    private void SetScore(int n)
    {
        if (n > PrPf_Int_HighScore)
        {
            PlayerPrefs.SetInt(nameof(PrPf_Int_HighScore), n);
            PlayerPrefs.Save();
            tmp_HighScore.SetText(n.ToString("D6"));
        }
        tmp_Score.SetText(n.ToString("D6"));
    }

    public void ShowSetting()
    {
        if (Volume_Manager.locker || Virus_Manager.isSkilling) return;
        DontMove = true;
        Go_SettingButton.SetActive(false);
        Go_SettingPage.SetActive(true);
        Volumer.Magic(Volume_Manager.Profile.SettingPage);
        adMober.BannerShow();
    }

    public static void AdsRemoved(bool removed = true)
    {
        PlayerPrefs.SetInt(nameof(PrPf_Int_AdsRemoved), PrPf_Int_AdsRemoved = removed ? 1 : 0);

        Button btn = Go_Static_PurchaseButton.GetComponent<Button>();
        btn.interactable = !removed;
        Go_Static_Dollar.SetActive(!removed);
    }

    public void HideSetting()
    {
        HideSetting(false);
    }

    public void HideSetting(bool force)
    {
        if (Volume_Manager.locker && !force) return;
        DontMove = false;
        Go_SettingButton.SetActive(true);
        Go_SettingPage.SetActive(false);
        Volumer.Magic(Volume_Manager.Profile.Default);
        adMober.BannerHide();
    }

    private void FadeBtnAColor(GameObject button, bool fade)
    {
        var color = button.GetComponent<Image>().color;
        color.a = fade ? 0.2f : 1;
        button.GetComponent<Image>().color = color;
    }

    public void MusicOn()
    {
        PrPf_Int_Music = PrPf_Int_Music == 1 ? 0 : 1;
        PlayerPrefs.SetInt(nameof(PrPf_Int_Music), PrPf_Int_Music);
        FadeBtnAColor(Go_MusicButton, PrPf_Int_Music == 0);

        GetComponent<AudioSource>().volume = PrPf_Int_Music;
    }
    public void SoundOn()
    {
        PrPf_Int_Sound = PrPf_Int_Sound == 1 ? 0 : 1;
        PlayerPrefs.SetInt(nameof(PrPf_Int_Sound), PrPf_Int_Sound);
        FadeBtnAColor(Go_SoundButton, PrPf_Int_Sound == 0);
    }

    public void VibrateOn()
    {
        PrPf_Int_Vibrate = PrPf_Int_Vibrate == 1 ? 0 : 1;
        PlayerPrefs.SetInt(nameof(PrPf_Int_Vibrate), PrPf_Int_Vibrate);
        FadeBtnAColor(Go_VibrateButton, PrPf_Int_Vibrate == 0);
    }

    public void PlaySound(float volume, AudioClip clip)
    {
        if (PrPf_Int_Sound == 1)
        {
            As_SoundManager.volume = volume;
            As_SoundManager.PlayOneShot(clip);
        }
    }

    public void PlayVibrate()
    {
        if (PrPf_Int_Vibrate == 1) Handheld.Vibrate();
    }

    public void ManualGameOver()
    {
        if (!isDoubleClick)
            StartCoroutine(nameof(CheckDoubleClick));
        else
        {
            adMober.InterstitialShow();
            GameOver();
        }
    }


    private bool isDoubleClick;
    private IEnumerator CheckDoubleClick()
    {
        isDoubleClick = true;
        yield return new WaitForSeconds(1);
        isDoubleClick = false;
    }

    public void GameOver()
    {
        HideSetting(force: true);
        FadeBtnAColor(Go_PowerupButton, false);
        int_Player_Score = 0;
        VirusID = 0;
        PowerUpChance = 2;
        Virus_Manager.PreGameOver();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void DropControl(object sender, EventArgs args)
    {
        DontDrop = false;
    }

    public void Skill_PowerUp()
    {
        if (PowerUpChance <= 0) return;
        DontDrop = true;
        Ads_PowerUp.Show(out bool proceed);
        if (proceed) Proceed_PowerUp(null, null);
        DontDrop = false;
    }

    public void Skill_Skip()
    {
        DontDrop = true;
        Ads_Skip.Show(out bool proceed);
        if (proceed) Proceed_Skip(null, null);
        DontDrop = false;
    }

    private void Proceed_PowerUp(object sender, EventArgs args)
    {
        if (--PowerUpChance <= 0) FadeBtnAColor(Go_PowerupButton, true);
        Script_VirusManager.PowerUp();
    }

    private void Proceed_Skip(object sender, EventArgs args)
    {
        Script_VirusManager.Skip();
    }
}
