using System.Collections;
using UnityEngine;

public class BadVirus_Act : MonoBehaviour
{
    private Game_Manager Script_General_data;
    private Virus_Manager Script_Virus_Manager;

    void Start()
    {
        Script_General_data = Game_Manager.GameManager_Script;
        Script_Virus_Manager = Game_Manager.GameManager.GetComponent<Virus_Manager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         StartCoroutine(nameof(ColorBomb));
    }

    public IEnumerator ColorBomb()
    {
        yield return new WaitForSeconds(2);
        for (int n = 0; n < Script_General_data.Dic_Virus.Count; n++)
        {
            var Go_VirusNow = Instantiate(Script_General_data.Dic_Virus[n][0], Vector3.zero, Quaternion.identity, Script_General_data.Go_CanvasPlayGround.transform);
            Go_VirusNow.GetComponent<Rigidbody2D>().gravityScale = 1;
            var difloc = gameObject.transform.localPosition;
            difloc.x += Random.Range(-1f, 1f);
            difloc.y += Random.Range(-1f, 1f);
            Go_VirusNow.transform.localPosition = difloc;
            var act = Go_VirusNow.GetComponent<Virus_act>();
            act.tag = Script_General_data.tag_MatureVirus;
            Virus_Manager.Viruses.Add(Go_VirusNow);
        }

        Virus_Manager.Viruses.Remove(gameObject);
        Destroy(gameObject);
    }
}
