using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Wizard_auxiliary))]
public class Wizard_auxiliary : UnitSkillDataSo
{
    [SerializeField] private UnitDataSo wizardCard_core;

    [SerializeField] private GameObject magicColllectPathPrefab;
    [SerializeField] private GameObject magicCollectPrefab;

    [SerializeField] private Vector3 curveOffest;
    [SerializeField] private int skillListIndex;

    private GameObject magicCollectPathEffect;
    private GameObject magicCollectEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        magicCollectPathEffect = Instantiate(magicColllectPathPrefab, Vector3.zero, Quaternion.identity);
        magicCollectPathEffect.SetActive(false);

        magicCollectEffect = Instantiate(magicCollectPrefab, Vector3.zero, Quaternion.identity);
        magicCollectEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (unit.unitData == wizardCard_core)
            {
                target = unit;
                break;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        Debug.Log(target.name);

        magicCollectPathEffect.transform.position = user.transform.position + UnitPlat.topDistance;
        magicCollectPathEffect.SetActive(true);
        Vector3[] cubicPath = new Vector3[]
        {
            magicCollectPathEffect.transform.position,
            magicCollectPathEffect.transform.position - curveOffest,
            target.transform.position,
        };
        PlayableDirector director = magicCollectEffect.GetComponent<PlayableDirector>();
        magicCollectPathEffect.transform.DOPath(cubicPath, 2f, PathType.CatmullRom).SetOptions(false).SetDelay(0.6f).
            OnComplete(() => 
            {
                magicCollectPathEffect.SetActive(false);

                magicCollectEffect.transform.position = target.transform.position;
                magicCollectEffect.SetActive(true);
                director.Play();

                BattleSystem.instance.UnitReEnqueue(target);
            });

        TimerManager.instance.StartTimer(name + "auxiliaryClose",(float)director.duration + 3f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
                magicCollectEffect.SetActive(true);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = (float)director.duration + 3f;
    }
}
