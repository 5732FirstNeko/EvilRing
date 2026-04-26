using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Element/" + nameof(Element_fire))]
public class Element_fire : UnitSkillDataSo
{
    [SerializeField] private GameObject fireAttackPrefab;
    [SerializeField] private GameObject fireHitPrefab;

    [SerializeField] private Vector3 spwanPos;

    private List<GameObject> fireAttackEffect;
    private List<GameObject> fireHitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        fireAttackEffect = new List<GameObject>();
        fireHitEffect = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            GameObject attack = Instantiate(fireHitPrefab, Vector3.zero, Quaternion.identity);
            GameObject hit = Instantiate(fireHitPrefab, Vector3.zero, Quaternion.identity);

            attack.SetActive(false);
            hit.SetActive(false);

            fireAttackEffect.Add(attack);
            fireHitEffect.Add(hit);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            Attack(user);
        }
        else
        {
            Defend(user);
        }
    }

    private void Attack(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "AttackEfect", 0.6f, 
            () =>
            {
                int i = 0;
                foreach (var unit in
                    BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        int index = i;

                        fireAttackEffect[index].transform.position = user.transform.position + 
                            UnitPlat.topDistance + spwanPos;

                        Vector3 direction = (unit.transform.position - 
                            fireAttackEffect[index].transform.position).normalized;
                        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                        Quaternion axisCorrection = Quaternion.FromToRotation(Vector3.forward, Vector3.left);
                        fireAttackEffect[index].transform.rotation = targetRot * axisCorrection;

                        fireAttackEffect[index].SetActive(true);
                        fireAttackEffect[index].transform.DOMove(unit.transform.position, 1.5f).
                            OnComplete(() =>
                            {
                                fireHitEffect[index].transform.position = unit.transform.position;
                                fireHitEffect[index].SetActive(true);
                                fireHitEffect[index].GetComponent<PlayableDirector>().Play();


                                unit.UnitPlatHurtAnimation();
                                unit.unit.HP -= Damage + user.costumvalue_first == 1 ? 3 : 0;
                                if (user.costumvalue_first == 1)
                                {
                                    unit.DamageTextJump("灼烧", Color.white);
                                }
                            });

                        i++;
                    }

                }
            });

        TimerManager.instance.StartTimer(name + "EffectClose",0.6f + 1.5f + 0.5f + 0.1f, 
            () => 
            {
                for (int i = 0; i < 4; i++)
                {
                    fireAttackEffect[i].transform.position = spwanPos;
                    fireAttackEffect[i].SetActive(false);
                    fireHitEffect[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.spCount -= 2;
        user.unit.unitSkills[0].SkillTime = 0.6f + 1.5f + 0.5f + 0.1f + 0.6f;
    }

    private void Defend(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "DefendEffect", 0.6f,
            () =>
            {
                user.DamageTextJump("火焰保护", Color.white);
                user.unit.OnDefend += DefendAction;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1f,
            () =>
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 1f + 0.6f;
    }

    private int DefendAction(int hpcahnge, UnitPlat user)
    {
        if (hpcahnge < 0)
        {
            user.DamageTextJump("火焰保护", Color.white);
            user.unit.OnDefend -= DefendAction;
            return 0;
        }

        return hpcahnge;
    }
}
