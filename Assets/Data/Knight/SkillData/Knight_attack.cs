using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Knight_attack))]
public class Knight_attack : UnitSkillDataSo
{
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private GameObject slashHitPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject slashEffect;
    private List<GameObject> slashHitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        slashEffect = Instantiate(slashPrefab, Vector3.zero, Quaternion.identity);
        slashEffect.SetActive(false);

        slashHitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(slashHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            slashHitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        Vector3 slashPosition = Vector3.zero;
        foreach (var unit in unitPlats)
        {
            slashPosition += unit.transform.position;
        }
        slashPosition /= unitPlats.Count;

        PlayableDirector director = slashEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "slashEffect", 0.6f, 
            () =>
            {
                slashEffect.transform.position = slashPosition;
                slashEffect.SetActive(true);

                director.Play();
            });

        TimerManager.instance.StartTimer(name + "slashHitEffect",0.6f + (float)director.duration * 0.2f, 
            () => 
            {
                int i = 0;
                foreach (var unit in unitPlats)
                {
                    int index = i;
                    slashHitEffects[i].transform.position = unit.transform.position;
                    slashHitEffects[i].SetActive(true);
                    slashHitEffects[i].GetComponent<PlayableDirector>().Play();

                    unit.UnitPlatHurtAnimation();
                    unit.unit.HP -= Mathf.RoundToInt(Damage * (1 + user.costumvalue_first * 0.5f));

                    i++;
                }
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + (float)director.duration * 0.2f +
            (float)slashHitEffects[0].GetComponent<PlayableDirector>().duration, 
            () =>
            {
                slashEffect.SetActive(false);
                for (int i = 0; i < slashHitEffects.Count; i++)
                {
                    slashHitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 
            0.6f + (float)director.duration * 0.1f + 
            (float)slashHitEffects[0].GetComponent<PlayableDirector>().duration + 0.6f;
    }
}
