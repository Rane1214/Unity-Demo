using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus_act : MonoBehaviour
{
    public int ID;
    public int Level = 0;
    public int ColorIndex;
    public bool isSpecial;

    private GameObject[] VirusStyles;

    private Game_Manager Script_General_data;
    private Virus_Manager Script_Virus_Manager;

    private void Start()
    {
        Script_General_data = Game_Manager.GameManager_Script;
        Script_Virus_Manager = Game_Manager.GameManager.GetComponent<Virus_Manager>();
        ID = Script_General_data.GetNewVirusID();
        VirusStyles = Script_General_data.VirusStyles;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision?.gameObject != null &&
            collision.gameObject.TryGetComponent<Virus_act>(out var Oppo_Act) &&
            !isSpecial && !collision.gameObject.GetComponent<Virus_act>().isSpecial &&
            gameObject.tag == Script_General_data.tag_MatureVirus &&
            Script_General_data.Dic_Virus[ColorIndex].Count > Level &&
            Oppo_Act.Level == Level &&
            Oppo_Act.ColorIndex == ColorIndex)
        {
            if (ID < Oppo_Act.ID)
            {
                Script_General_data.int_Player_Score += Level * Level * 5;

                var virus = Instantiate(Script_General_data.Dic_Virus[ColorIndex][Level++], Vector3.zero,
                    Quaternion.identity, Script_General_data.Go_CanvasPlayGround.transform);

                virus.transform.localPosition = gameObject.transform.localPosition;
                virus.tag = Script_General_data.tag_MatureVirus;
                var act = virus.GetComponent<Virus_act>();
                act.Level = Level;

                bool isSpecial = Level == Script_General_data.Dic_Virus[ColorIndex].Count || (Level > 2 && Random.Range(0, 99) < Level * 2);

                if (isSpecial)
                {
                    var effect = Instantiate(VirusStyles[ColorIndex], new Vector3(), Quaternion.identity, virus.transform);
                    effect.transform.localPosition = new Vector3(-0.1f, 0, 0);
                    act.isSpecial = isSpecial;
                }

                Virus_Manager.Viruses.Add(virus);

                Virus_Manager.Viruses.Remove(collision.gameObject);
                Destroy(collision.gameObject);

                Virus_Manager.Viruses.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }

    private bool isDoubleClick;
    private IEnumerator CheckDoubleClick()
    {
        isDoubleClick = true;
        yield return new WaitForSeconds(1);
        isDoubleClick = false;
    }

    private static object locker = new object();
    private void OnMouseDown()
    {
        lock (locker)
        {

#if DEBUG
            isSpecial = true;
#endif

            if (isSpecial && !Virus_Manager.isSkilling && !Game_Manager.DontMove && CompareTag(Script_General_data.tag_MatureVirus))
            {
                if (!isDoubleClick)
                    StartCoroutine(nameof(CheckDoubleClick));
                else
                {
                    Virus_Manager.isSkilling = true;
                    Script_Virus_Manager.ActivateSkill(gameObject);
                }
            }
        }
    }
}
