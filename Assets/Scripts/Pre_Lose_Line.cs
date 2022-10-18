using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pre_Lose_Line : MonoBehaviour
{
    public static Dictionary<GameObject, bool> Dic_EnteredVirus = new Dictionary<GameObject, bool>();
    private Game_Manager Script_General_data;
    private Lose_Line Script_Lose_Line;
    private bool isWarned;

    private void Start()
    {
        Script_General_data = Game_Manager.GameManager_Script;
        Script_Lose_Line = Script_General_data.Go_UILoseLine.GetComponent<Lose_Line>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Script_General_data.tag_MatureVirus))
        {
            Dic_EnteredVirus.Add(collision.gameObject, false);
            StartCoroutine(WarnCountDown(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Dic_EnteredVirus.Remove(collision.gameObject);
        CheckWarn();
    }

    private IEnumerator WarnCountDown(Collider2D collision)
    {
        yield return new WaitForSeconds(0.5f);
        if (collision != null && Dic_EnteredVirus.ContainsKey(collision.gameObject))
            Dic_EnteredVirus[collision.gameObject] = true;
        CheckWarn();
    }

    private void CheckWarn()
    {
        bool Warn = Dic_EnteredVirus.Where(item => item.Key != null && item.Value).Count() > 0;
        if (Warn != isWarned)
        {
            isWarned = Warn;
            Script_Lose_Line.Warning(Warn);
        }
    }
}
