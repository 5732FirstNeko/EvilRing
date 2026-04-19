using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Swordman_core_GameStart))]
public class Swordman_core_GameStart : UnitSkillDataSo
{
    [SerializeField] private GameObject flySwordRoundPrefab;

    [SerializeField] private string flySwordAnimationName;

    private LinkedList<GameObject> flySwordRoundEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        flySwordRoundEffects = new LinkedList<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            GameObject effectround = Instantiate(flySwordRoundPrefab, Vector3.zero, Quaternion.identity);
            effectround.SetActive(false);
            flySwordRoundEffects.AddLast(effectround);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.unit.OnHPChange != null) return;

        user.unit.OnHPChange += OnHpChange;
        user.OnFirstValueChange += RecoverFlySword;
    }

    private void OnHpChange(int Hp, UnitPlat user)
    {
        if (Hp == 0) return;

        user.costumvalue_first++;
        GameObject effect = null;
        LinkedList<GameObject> activeEffect = new LinkedList<GameObject>();
        foreach (var eff in flySwordRoundEffects)
        {
            if (!eff.activeSelf)
            {
                effect = eff;
                activeEffect.AddLast(effect);
            }
            else
            {
                activeEffect.AddLast(eff);
            }
        }

        if (effect == null)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject effectround = Instantiate(flySwordRoundPrefab, Vector3.zero, Quaternion.identity);
                effectround.SetActive(false);
                flySwordRoundEffects.AddLast(effectround);
                if (effect == null)
                { 
                    effect = effectround; 
                    activeEffect.AddLast(effectround);
                }
            }
        }

        float rotateAngle = 360 / activeEffect.Count;

        float rotateOffest = 0;
        foreach (var eff in activeEffect)
        {
            effect.transform.position = user.transform.position;
            effect.transform.Rotate(Vector3.up, rotateOffest, Space.Self);
            effect.transform.localScale = Vector3.zero;
            effect.transform.DOScale(Vector3.one, 0.5f);
            effect.SetActive(true);
            rotateOffest += rotateAngle;

            effect.GetComponentInChildren<Animator>().CrossFade(flySwordAnimationName, 0);
        }
    }

    public void RecoverFlySword(int value)
    {
        if (value >= 0) return;

        foreach (var sword in flySwordRoundEffects)
        {
            sword.SetActive(false);
        }
    }
}
