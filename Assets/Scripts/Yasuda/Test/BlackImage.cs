using UnityEngine;

public class BlackImage : MonoBehaviour
{


    //スキル使えない時は暗くする。制作中

    [SerializeField]
    private SkillPointBox m_point;

    [SerializeField]
    private Skill_Kaihuku m_kaihuku;

    bool test = true;


    void Update()
    {
        if(test)
        {

        }
        else
        {

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            test ^= true;
        }


    }
}
