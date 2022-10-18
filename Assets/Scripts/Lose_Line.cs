using System.Collections;
using UnityEngine;

public class Lose_Line : MonoBehaviour
{
    private Game_Manager Script_General_data;
    private SpriteRenderer Sr_Lose;

    private bool isGameOvering = true;
    private Collider2D Cld_LastVirus;

    private void Start()
    {
        Script_General_data = Game_Manager.GameManager_Script;
        Sr_Lose = Script_General_data.Go_UILoseLine.GetComponent<SpriteRenderer>();
    }

    public void Warning(bool isWarn)
    {
        StopCoroutine(nameof(FadeLoseLine));
        StartCoroutine(FadeLoseLine(isWarn));
    }

    private IEnumerator FadeLoseLine(bool isWarn)
    {
        var temp = Sr_Lose.color;
        if (isWarn)
        {
            while (temp.a < 0.1f)
            {
                temp.a += 0.02f;
                Sr_Lose.color = temp;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            while (temp.a > 0)
            {
                temp.a -= 0.02f;
                Sr_Lose.color = temp;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Script_General_data.tag_FreshVirus))
        {
            collision.gameObject.tag = Script_General_data.tag_MatureVirus;
            isGameOvering = true;
            Cld_LastVirus = collision;
            StartCoroutine(nameof(GameOver));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == Cld_LastVirus)
        {
            Cld_LastVirus = null;
            isGameOvering = false;
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.5f);
        if (isGameOvering) Script_General_data.GameOver();
        isGameOvering = false;
    }

}
