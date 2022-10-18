using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Volume_Manager : MonoBehaviour
{
    public enum Profile
    {
        Default,
        SettingPage,
    }

    [SerializeField]
    GameObject Go_Volume;

    public static bool locker;
    private Volume Script_Volume;
    private Bloom Profile_Bloom;
    private Vignette Profile_Vignette;
    private DepthOfField Profile_DepthOfField;
    private Dictionary<Profile, VolumeProfile> Dic_Profiles;

    private void Start()
    {
        Script_Volume = Go_Volume.GetComponent<Volume>();
        Script_Volume.profile.TryGet(out Profile_Bloom);
        Script_Volume.profile.TryGet(out Profile_Vignette);
        Script_Volume.profile.TryGet(out Profile_DepthOfField);
        SetupProfile();
    }

    private void SetupProfile()
    {
        Dic_Profiles = new Dictionary<Profile, VolumeProfile>
        {
            [Profile.Default] = new VolumeProfile
            {
                wait = 0.1f,
                depthOfField = new depthOfField
                {
                    com = new mutual { interval = -30, update = true },
                    focalLength = 1
                },
                vignette = new vignette
                {
                    com = new mutual { interval = -0.1f, update = true },
                    center = new Vector2(0.5f, 0.5f),
                    intensity = 0
                }
            },
            [Profile.SettingPage] = new VolumeProfile
            {
                wait = 0.1f,
                depthOfField = new depthOfField
                {
                    com = new mutual { interval = 30, update = true },
                    focalLength = 60,
                },
                vignette = new vignette
                {
                    com = new mutual { interval = 0.1f, update = true },
                    center = new Vector2(0.5f, 0),
                    intensity = 0.3f
                }
            }
        };
    }

    public bool Magic(Profile _profile)
    {
        if (!locker)
            StartCoroutine(_Magic(_profile));
        return !locker;
    }

    private IEnumerator _Magic(Profile _profile)
    {
        locker = true;

        var profile = Dic_Profiles[_profile];

        Profile_Vignette.center.value = profile.vignette.center;

        bool conti;

        do
        {
            conti = false;

            if (profile.depthOfField.com.update &&
                Profile_DepthOfField.focalLength.value.ToString("0.00") != profile.depthOfField.focalLength.ToString("0.00"))
            {
                if ((profile.depthOfField.com.interval > 0 && (Profile_DepthOfField.focalLength.value > profile.depthOfField.focalLength)) ||
                    (profile.depthOfField.com.interval < 0 && (Profile_DepthOfField.focalLength.value < profile.depthOfField.focalLength)))
                    Profile_DepthOfField.focalLength.value = profile.depthOfField.focalLength;
                else
                    Profile_DepthOfField.focalLength.value += profile.depthOfField.com.interval;
                conti = true;
            }

            if (profile.vignette.com.update &&
               Profile_Vignette.intensity.value.ToString("0.00") != profile.vignette.intensity.ToString("0.00"))
            {
                if ((profile.vignette.com.interval > 0 && (Profile_Vignette.intensity.value > profile.vignette.intensity)) ||
                    (profile.vignette.com.interval < 0 && (Profile_Vignette.intensity.value < profile.vignette.intensity)))
                    Profile_Vignette.intensity.value = profile.vignette.intensity;
                else
                    Profile_Vignette.intensity.value += profile.vignette.com.interval;
                conti = true;
            }

            yield return new WaitForSeconds(profile.wait);
        }
        while (conti);

        locker = false;
    }

    private struct VolumeProfile
    {
        public float wait;
        public depthOfField depthOfField;
        public vignette vignette;
    }

    private struct depthOfField
    {
        public mutual com;
        public float focalLength;
    }
    private struct vignette
    {
        public mutual com;
        public Vector2 center;
        public float intensity;
    }
    private struct mutual
    {
        public bool update;
        public float interval;
    }

}
