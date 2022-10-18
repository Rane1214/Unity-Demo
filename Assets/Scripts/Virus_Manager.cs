using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using EZCameraShake;

public class Virus_Manager : MonoBehaviour
{
    public static List<GameObject> Viruses = new List<GameObject>();

    private static GameObject Go_VirusNow;
    public static bool isSkilling;
    private static bool isSpawning;
    private static bool isDroping;
    private static bool BadNow;
    private static int whenBad = 1;
    private int BadNowCountMax = 15;
    private int BadNowCount = 15;

    private int int_Color_Count;
    private Game_Manager Script_General_data;

    private GameObject Go_Floor;
    private Collider2D Cld_Floor;

    // SameColorBoom
    private Volume Script_Volume;
    private Bloom Profile_Bloom;
    private Vignette Profile_Vignette;

    // FloorBoom
    private GameObject Go_Particle;
    private Light2D L2d_Bugs;

    private GameObject Go_Dummy;

    void Start()
    {
        Script_General_data = Game_Manager.GameManager_Script;
        Script_General_data.SetVirusManager(this);

        Go_Dummy = new GameObject() { name = "DropLocator" };

        Go_Floor = Script_General_data.Go_Floor;
        Cld_Floor = Go_Floor.GetComponent<Collider2D>();

        Script_Volume = Script_General_data.Go_GlobalVolume.GetComponent<Volume>();
        Script_Volume.profile.TryGet(out Profile_Bloom);
        Script_Volume.profile.TryGet(out Profile_Vignette);

        Go_Particle = Script_General_data.Go_FloorBloom_LightBugs;
        L2d_Bugs = Script_General_data.Go_FloorBloom_Light.GetComponent<Light2D>();

        int_Color_Count = Script_General_data.Dic_Virus.Count;

        StartCoroutine(nameof(Spawn));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!Game_Manager.DontMove && CanDrop())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
                if (hit.collider != null)
                    if (hit.transform.gameObject.TryGetComponent<Virus_act>(out var act) && act.isSpecial) return;

                Drop();
            }
        }
        else if (!Game_Manager.DontMove && Script_General_data.AutoDrop)
        {
            if (Script_General_data.flt_TimeRemaining > 0)
                Script_General_data.flt_TimeRemaining -= Time.deltaTime;
            else
            {
                Script_General_data.flt_TimeRemaining = Script_General_data.AutoDropInterval;
                Drop(isAuto: true);
            }
        }
    }

    private bool CanDrop()
    {
        var mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        return mousepos > 0 ?
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < Script_General_data.Go_UILoseLine.transform.position.y :
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > Script_General_data.Go_FloorLine.transform.position.y;
    }

    private IEnumerator Spawn()
    {
        isSpawning = true;

        yield return new WaitForSeconds(0.15f);

        int whichColor = Random.Range(0, int_Color_Count);

        bool SpawnedBad = SpawnBad();

        Go_VirusNow = Instantiate(SpawnedBad ?
            Script_General_data.Go_BadVirus :
            Script_General_data.Dic_Virus[whichColor][0], Vector3.zero, Quaternion.identity, Script_General_data.Go_CanvasPlayGround.transform);

        float finalScale = Go_VirusNow.transform.localScale.x;
        float perScale = finalScale / 10;
        float nowScale = 0;
        Go_VirusNow.transform.localPosition = Script_General_data.Go_SpawnPosition.transform.localPosition;
        Go_VirusNow.transform.localScale = Vector3.zero;
        Go_VirusNow.GetComponent<Rigidbody2D>().gravityScale = 0;

        Viruses.Add(Go_VirusNow);

        while (finalScale >= nowScale)
        {
            Go_VirusNow.transform.localScale = new Vector3(nowScale, nowScale, 0);
            nowScale += perScale;
            nowScale = Mathf.Round(nowScale * 100f) / 100f;
            yield return new WaitForSeconds(0.05f);
        }

        isSpawning = false;

        if (SpawnedBad) Drop(true);

    }

    private bool SpawnBad()
    {
        whenBad = whenBad == BadNowCountMax ? 1 : whenBad + 1;

        if (BadNow)
        {
            if (BadNowCount-- != 0) return true;
            else
            {
                whenBad = 1;
                BadNowCount = BadNowCountMax;
                return BadNow = false;
            }
        }

        if (whenBad == 10 ||
            whenBad == 11 && Script_General_data.GetVirusIDNow() > 150 ||
            whenBad == 12 && Script_General_data.GetVirusIDNow() > 250 ||
            whenBad == 13 && Script_General_data.GetVirusIDNow() > 350 ||
            whenBad == 14 && Script_General_data.GetVirusIDNow() > 450 ||
            whenBad == 15 && Script_General_data.GetVirusIDNow() > 550
            ) return true;

        return false;
    }

    private void Drop(bool isAuto = false)
    {
        if (isDroping || isSpawning || (Game_Manager.DontDrop && !isAuto)) return;

        isDroping = true;

        Script_General_data.PlaySound(0.5f, Script_General_data.Ac_SpawnCharge);

        float x;
        if (isAuto)
        {
            x = Random.Range(Script_General_data.Flt_LeftBorder, Script_General_data.Flt_RightBorder);
        }
        else
        {
            Go_Dummy.transform.parent = null;
            Go_Dummy.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Go_Dummy.transform.SetParent(Script_General_data.Go_CanvasPlayGround.transform);
            x = Go_Dummy.transform.localPosition.x;
            if (x < Script_General_data.Flt_LeftBorder) x = Script_General_data.Flt_LeftBorder;
            else if (x > Script_General_data.Flt_RightBorder) x = Script_General_data.Flt_RightBorder;
        }

        Vector3 Pos = new Vector3(x, Go_VirusNow.transform.localPosition.y, 0);

        Go_VirusNow.transform.localPosition = Pos;
        Go_VirusNow.GetComponent<Rigidbody2D>().gravityScale = 1;

        StartCoroutine(nameof(Spawn));

        isDroping = false;
    }

    public void ActivateSkill(GameObject virus)
    {
        var virus_act = virus.GetComponent<Virus_act>();
        switch (virus_act.ColorIndex)
        {
            case 0:
                StartCoroutine(FloorBoom(virus));
                break;
            case 1:
                StartCoroutine(Smallen(virus));
                break;
            case 2:
                StartCoroutine(FlameBall(virus));
                break;
            case 3:
                StartCoroutine(SameColorBoom(virus_act.ColorIndex));
                break;
            default:
                isSkilling = false;
                break;
        }
    }

    private IEnumerator Smallen(GameObject virusOri)
    {
        yield return StartCoroutine(nameof(Smallen_BlmNVig), true);

        Script_General_data.PlaySound(0.3f, Script_General_data.Ac_Smallen);
        yield return new WaitForSeconds(0.5f);

        Virus_act act = null;
        var viruses = Viruses.Where(v => v != null && v.TryGetComponent(out act) && act.Level > 1 && !act.isSpecial && v != Go_VirusNow);

        List<GameObject> temp_viruses_toRemove = new List<GameObject>() { virusOri };
        List<GameObject> temp_viruses_toAdd = new List<GameObject>();
        foreach (var virus in viruses)
        {
            var smaller_virus = Instantiate(Script_General_data.Dic_Virus[act.ColorIndex][0], Vector3.zero, Quaternion.identity, Script_General_data.Go_CanvasPlayGround.transform);
            smaller_virus.transform.localPosition = virus.transform.localPosition;
            smaller_virus.tag = Script_General_data.tag_MatureVirus;

            temp_viruses_toRemove.Add(virus);
            temp_viruses_toAdd.Add(smaller_virus);
        }

        foreach (var v in temp_viruses_toRemove)
        {
            Viruses.Remove(v);
            Destroy(v);
        }

        foreach (var v in temp_viruses_toAdd)
            Viruses.Add(v);

        CameraShaker.Instance.ShakeOnce(1, 1, 0.1f, 0.2f);
        yield return StartCoroutine(nameof(Smallen_BlmNVig), false);

        yield return new WaitForSeconds(0.5f);
        isSkilling = false;
    }

    private IEnumerator Smallen_BlmNVig(bool on)
    {
        const float b = 1;
        const float v = 0.2f;

        if (on)
        {
            while (Profile_Bloom.intensity.value < b || Profile_Vignette.intensity.value < v)
            {
                if (Profile_Bloom.intensity.value < b)
                    Profile_Bloom.intensity.value += 0.1f;
                if (Profile_Vignette.intensity.value < v)
                    Profile_Vignette.intensity.value += 0.01f;

                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (Profile_Bloom.intensity.value > 0 || Profile_Vignette.intensity.value > 0)
            {
                if (Profile_Bloom.intensity.value > 0)
                    Profile_Bloom.intensity.value -= 0.1f;
                if (Profile_Vignette.intensity.value > 0)
                    Profile_Vignette.intensity.value -= 0.01f;

                yield return new WaitForSeconds(0.02f);

            }
        }
    }

    float[] ary_flameRadius = new float[] { 0.3f, 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f };
    private IEnumerator FlameBall(GameObject virusOri)
    {
        var act = virusOri.GetComponent<Virus_act>();
        var Go_SizedFlameBall = Script_General_data.Lst_Go_Flame_Ball[act.Level - 1];
        var flameBall = Instantiate(Go_SizedFlameBall, new Vector3(), Quaternion.identity, virusOri.transform);
        flameBall.transform.localPosition = Vector3.zero;

        yield return StartCoroutine(nameof(Flame_BlmNVig), true);

        yield return new WaitForSeconds(0.5f);

        var GetBurnRadius = ary_flameRadius[act.Level];

        for (int i = 0; i < 3; i++)
        {
            CameraShaker.Instance.ShakeOnce(0.3f, 5, 0.5f, 0.5f);
            Script_General_data.PlayVibrate();

            var viruses = Physics2D.OverlapCircleAll(virusOri.transform.position, GetBurnRadius);
            foreach (var virus in viruses)
            {
                if (virus.gameObject != virusOri && virus.gameObject.tag == Script_General_data.tag_MatureVirus)
                {
                    Viruses.Remove(virus.gameObject);
                    Destroy(virus.gameObject);
                }
            }
            Script_General_data.PlaySound(1, Script_General_data.Ac_FlameBall);
            yield return new WaitForSeconds(1);
        }

        yield return StartCoroutine(nameof(Flame_BlmNVig), false);

        yield return new WaitForSeconds(0.5f);

        CameraShaker.Instance.ShakeOnce(0.3f, 5, 0.5f, 0.5f);
        Viruses.Remove(virusOri);
        Destroy(virusOri);
        isSkilling = false;
    }

    private IEnumerator Flame_BlmNVig(bool on)
    {
        const float b = 1;
        const float v = 0.2f;
        float fade_start = 2;
        float fade_end = 0.3f;

        if (on)
        {
            while (Profile_Bloom.intensity.value < b || Profile_Vignette.intensity.value < v ||
                fade_start > fade_end)
            {
                if (Profile_Bloom.intensity.value < b)
                    Profile_Bloom.intensity.value += 0.1f;
                if (Profile_Vignette.intensity.value < v)
                    Profile_Vignette.intensity.value += 0.01f;
                if (fade_start > fade_end)
                {
                    Script_General_data.Mtr_Fire.SetFloat("_Fade", fade_start);
                    fade_start -= 0.1f;
                }

                yield return new WaitForSeconds(0.025f);
            }
        }
        else
        {
            while (Profile_Bloom.intensity.value > 0 || Profile_Vignette.intensity.value > 0 ||
                fade_start > fade_end)
            {
                if (Profile_Bloom.intensity.value > 0)
                    Profile_Bloom.intensity.value -= 0.1f;
                if (Profile_Vignette.intensity.value > 0)
                    Profile_Vignette.intensity.value -= 0.01f;
                if (fade_start > fade_end)
                {
                    Script_General_data.Mtr_Fire.SetFloat("_Fade", fade_end);
                    fade_end += 0.1f;
                }
                yield return new WaitForSeconds(0.02f);

            }
        }
    }

    private IEnumerator SameColorBoom(int colorindex)
    {
        var SameColor = Viruses.Where(x => x.TryGetComponent<Virus_act>(out var v) && v.ColorIndex == colorindex && x.CompareTag(Script_General_data.tag_MatureVirus)).ToArray();

        CameraShaker.Instance.ShakeOnce(0.1f, 5, 2, 2);
        yield return StartCoroutine(nameof(BlmNVig), true);

        foreach (var virus in SameColor)
        {
            Viruses.Remove(virus);
            Destroy(virus);
        }
        Script_General_data.PlayVibrate();
        Script_General_data.PlaySound(0.3f, Script_General_data.Ac_SameColorBoom);

        yield return StartCoroutine(nameof(BlmNVig), false);

        isSkilling = false;
    }


    private IEnumerator BlmNVig(bool on)
    {
        float b = 0.8f;
        float v = 0.4f;

        if (on)
        {
            while (Profile_Bloom.intensity.value < b || Profile_Vignette.intensity.value < v)
            {
                if (Profile_Bloom.intensity.value < b)
                    Profile_Bloom.intensity.value += 0.1f;
                if (Profile_Vignette.intensity.value < v)
                    Profile_Vignette.intensity.value += 0.02f;

                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            while (Profile_Bloom.intensity.value > 0 || Profile_Vignette.intensity.value > 0)
            {
                if (Profile_Bloom.intensity.value > 0)
                    Profile_Bloom.intensity.value -= 0.1f;
                if (Profile_Vignette.intensity.value > 0)
                    Profile_Vignette.intensity.value -= 0.02f;

                yield return new WaitForSeconds(0.02f);

            }
        }
    }

    private RaycastHit2D[] hits = new RaycastHit2D[30];
    private ContactFilter2D filter = new ContactFilter2D();
    private IEnumerator FloorBoom(GameObject virusOri)
    {
        CameraShaker.Instance.ShakeOnce(0.1f, 10, 2, 1);
        Script_General_data.PlaySound(0.7f, Script_General_data.Ac_FloorBoom);
        yield return StartCoroutine(nameof(FloorLight), true);

        int numHit = Cld_Floor.Cast(Vector2.up, filter, hits, 0.2f);
        for (int i = 0; i < numHit; i++)
        {
            var virus = hits[i].transform?.gameObject;
            if (virus != null && virus.tag == Script_General_data.tag_MatureVirus)
            {
                Viruses.Remove(virus);
                Destroy(virus);
            }
        }

        Viruses.Remove(virusOri);
        Destroy(virusOri);
        Script_General_data.PlayVibrate();
        Script_General_data.PlaySound(1, Script_General_data.Ac_FloorBoom2);

        yield return StartCoroutine(nameof(FloorLight), false);

        isSkilling = false;
    }

    private IEnumerator FloorLight(bool on)
    {
        float lightInten = 2;
        float v = 0.2f;

        Go_Particle.SetActive(on);

        if (on)
        {
            while (L2d_Bugs.intensity < lightInten || Profile_Vignette.intensity.value < v)
            {
                if (L2d_Bugs.intensity < lightInten)
                    L2d_Bugs.intensity += 0.1f;
                if (Profile_Vignette.intensity.value < v)
                    Profile_Vignette.intensity.value += 0.01f;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            while (L2d_Bugs.intensity > 0 || Profile_Vignette.intensity.value > 0)
            {
                if (L2d_Bugs.intensity > 0)
                    L2d_Bugs.intensity -= 0.1f;
                if (Profile_Vignette.intensity.value > 0)
                    Profile_Vignette.intensity.value -= 0.01f;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public static void PreGameOver()
    {
        foreach (var virus in Viruses)
            if (virus != null && virus != Go_VirusNow) Destroy(virus);

        bool addback = Viruses.Contains(Go_VirusNow);
        Viruses.Clear();
        if (addback) Viruses.Add(Go_VirusNow);
        if (Go_VirusNow.TryGetComponent<Virus_act>(out var act)) act.ID = 0;
        whenBad = 1;
    }

    public void Skip()
    {
        BadNow = true;
    }

    public void PowerUp()
    {
        StartCoroutine(nameof(PowerUp_Action));
    }

    public IEnumerator PowerUp_Action()
    {
    check:
        while (isSpawning) yield return new WaitForSeconds(0.1f);

        if (Go_VirusNow.TryGetComponent<Virus_act>(out var act))
        {
            var effect = Instantiate(Script_General_data.VirusStyles[act.ColorIndex], new Vector3(), Quaternion.identity, Go_VirusNow.transform);
            effect.transform.localPosition = new Vector3(-0.1f, 0, 0);
            act.isSpecial = true;
        }
        else
            goto check;
    }
}
