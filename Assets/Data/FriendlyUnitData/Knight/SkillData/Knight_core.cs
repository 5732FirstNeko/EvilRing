using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Knight_core))]
public class Knight_core : UnitSkillDataSo
{
    [SerializeField] private GameObject buffPrefab;
    [SerializeField] private GameObject particalPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject buffEffect;
    private GameObject particalEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        buffEffect = Instantiate(buffPrefab, Vector3.zero, Quaternion.identity);
        buffEffect.SetActive(false);

        //particalEffect = Instantiate(particalPrefab, Vector3.zero, Quaternion.identity);
        //particalEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        buffEffect.transform.position = user.transform.position;
        buffEffect.SetActive(true);
        PlayableDirector director = buffEffect.GetComponent<PlayableDirector>();
        director.Play();

        TimerManager.instance.StartTimer(name + "buffEffect",(float)director.duration + 0.1f, 
            () => 
            {
                buffEffect.SetActive(false);
            });

        user.costumvalue_first = 1;
        user.unit.unitSkills[skillListIndex].SkillTime = (float)director.duration + 0.2f;
    }
}
