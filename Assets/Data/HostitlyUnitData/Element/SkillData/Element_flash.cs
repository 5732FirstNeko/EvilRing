using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Element/" + nameof(Element_flash))]
public class Element_flash : UnitSkillDataSo
{
    [SerializeField] private GameObject flashAttackPrefab;
    [SerializeField] private GameObject flashHitPrefab;

    [SerializeField] private float hitEffectRate = 1;

    private List<GameObject> flashAttackEffects;
    private List<GameObject> flashHitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        flashAttackEffects = new List<GameObject>();
        flashHitEffects = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject attackEffect = Instantiate(flashAttackPrefab, Vector3.zero, Quaternion.identity);
            attackEffect.SetActive(false);

            GameObject hitEffect = Instantiate(flashHitPrefab, Vector3.zero, Quaternion.identity);
            hitEffect.SetActive(false);

            flashAttackEffects.Add(attackEffect);
            flashHitEffects.Add(hitEffect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat first = null;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                first = unit;
                break;
            }
        }

        if (first == null)
        {
            user.unit.unitSkills[0].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        UnitPlat second = null;
        PlayableDirector attackDirector = flashAttackEffects[0].GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "FlashEffect", 0.6f, 
            () => 
            {
                flashAttackEffects[0].transform.position = first.transform.position;
                flashAttackEffects[0].SetActive(true);
                flashAttackEffects[0].GetComponent<PlayableDirector>().Play();

                if (user.costumvalue_first != 1) return;

                int index = Random.Range(0, 11);
                if (index < 5)
                {
                    return;
                }

                foreach (var unit in
                    BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && 
                        unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && unit != first)
                    {
                        second = unit;
                        break;
                    }
                }

                flashAttackEffects[1].transform.position = second.transform.position;
                flashAttackEffects[1].SetActive(true);
                flashAttackEffects[1].GetComponent<PlayableDirector>().Play();
            });


        TimerManager.instance.StartTimer(name + "HitEffect",
            0.6f + (float)attackDirector.duration * hitEffectRate, 
            () => 
            {
                flashHitEffects[0].transform.position = first.transform.position;
                flashHitEffects[0].SetActive(true);
                flashHitEffects[0].GetComponent<PlayableDirector>().Play();

                first.unit.HP -= Damage;
                first.UnitPlatHurtAnimation();

                if (second == null) return;

                flashHitEffects[1].transform.position = second.transform.position;
                flashHitEffects[1].SetActive(true);
                flashHitEffects[1].GetComponent<PlayableDirector>().Play();

                second.unit.HP -= Damage;
                second.UnitPlatHurtAnimation();
            });

        TimerManager.instance.StartTimer(name + "EffectClose",
            0.6f + 0.6f + (float)attackDirector.duration + 0.6f, 
            () => 
            {
                for (int i = 0; i < flashAttackEffects.Count; i++)
                {
                    flashAttackEffects[i].SetActive(false);
                    flashHitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 0.6f + (float)attackDirector.duration + 0.6f + 0.6f;
    }
}
