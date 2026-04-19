using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(LycorisSkill_core_Damage))]
public class LycorisSkill_core_Damage : UnitSkillDataSo
{
    [SerializeField] private GameObject LycorisHitprefab;
    [SerializeField] private GameObject attackprefab;
    [SerializeField] private GameObject friDeadprefab;

    [SerializeField] private float friDeadStateTime;
    [SerializeField] private float hitStateTIme;

    private GameObject attackinstance;
    private GameObject friDeadinstance;
    private PlayableDirector lycorisHitassest;

    public override void GameStartInit()
    {
        base.GameStartInit();
        attackinstance = Instantiate(attackprefab, Vector3.zero, Quaternion.identity);
        friDeadinstance = Instantiate(friDeadprefab, Vector3.zero, Quaternion.identity);
        lycorisHitassest = Instantiate(LycorisHitprefab, Vector3.zero, Quaternion.identity).
            GetComponent<PlayableDirector>();

        attackinstance.SetActive(false);
        friDeadinstance.SetActive(false);
        lycorisHitassest.gameObject.SetActive(false);

        SkillTime = friDeadStateTime + hitStateTIme + 0.5f + 0.5f + 1f;
    }


    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            target = unit;
        }
        if (target == null)
        {
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);

        UnitPlat deadTarget = null;
        foreach (var unitPlat in
                BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (FactorySystem.instance.LycorisCards.Contains(unitPlat.unitData))
            {
                deadTarget = unitPlat;
                break;
            }
        }
        if (deadTarget == null)
        {
            foreach (var unitPlat in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (unitPlat != user)
                {
                    deadTarget = unitPlat;
                }
            }
        }

        Sequence sequence = DOTween.Sequence();

        Vector3 originScale = user.transform.localScale;

        Tween userScaletween = user.transform.DOScale(originScale * attackScale, 0.5f).Pause().OnComplete(() => 
        {
            if (deadTarget == null)
            {
                user.unit.HP = 1;
                user.UnitPlatHurtAnimation();
                friDeadinstance.SetActive(true);
                friDeadinstance.transform.position = user.transform.position;
                foreach (var partical in friDeadinstance.GetComponentsInChildren<ParticleSystem>())
                {
                    partical.Play();
                }
            }
            else
            {
                deadTarget.unit.HP = 1;
                deadTarget.UnitPlatHurtAnimation();
                friDeadinstance.SetActive(true);
                friDeadinstance.transform.position = deadTarget.transform.position;
                foreach (var partical in friDeadinstance.GetComponentsInChildren<ParticleSystem>())
                {
                    partical.Play();
                }
            }
        });

        Tween attackinitTween = attackinstance.transform.DOMove(user.transform.position, 0f).
            SetEase(Ease.InQuart).Pause().
            OnComplete(() => { attackinstance.SetActive(true); friDeadinstance.SetActive(false); });

        Vector3[] path = new Vector3[]
        {
            user.transform.position,
            user.transform.position + new Vector3(-1.5f, 0.25f),
            target.transform.position
        };

        Tween hitAnimationTween = attackinstance.transform.DOPath(path, 1f,PathType.CatmullRom).
            SetEase(Ease.InQuart).Pause().
            OnComplete(() => 
            { 
                attackinstance.SetActive(false);
                lycorisHitassest.transform.position = target.transform.position;
                lycorisHitassest.gameObject.SetActive(true);
                lycorisHitassest.Play();
                TimerManager.instance.StartTimer(user.name + "LycorisUnit_coreSkill1",
                    3.14f, () =>
                    {
                        target.UnitPlatHurtAnimation(1, 0.1f);
                        target.unit.HP -= 1;
                    });
                TimerManager.instance.StartTimer(user.name + "LycorisUnit_coreSkill2",
                    5.4f, () =>
                    {
                        target.UnitPlatHurtAnimation(1, 0.1f);
                        target.unit.HP -= Mathf.RoundToInt(target.unit.MaxHP * 0.9f);
                    });
            });

        Tween userrecoverTween = user.transform.DOScale(originScale, 0.5f).Pause().
            OnComplete(
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                lycorisHitassest.gameObject.SetActive(false);
            });

        sequence.Append(userScaletween);
        sequence.AppendInterval(friDeadStateTime);
        sequence.Append(attackinitTween);
        sequence.Append(hitAnimationTween);
        sequence.AppendInterval(hitStateTIme);
        sequence.Append(userrecoverTween);

        sequence.Play();
    }
}
