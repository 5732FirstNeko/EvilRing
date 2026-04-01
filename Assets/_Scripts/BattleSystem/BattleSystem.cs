using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DG.Tweening;
using PriorityQueue;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static int unitPlatQueueCount = 4;

    public static BattleSystem Instance;
    public static BattleSystem instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(BattleSystem).Name);
                Instance = Object.AddComponent<BattleSystem>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    [Header("Friendly")]
    public List<UnitSiteFlag> friendlyUnitSiteFlag;

    [Header("Hostitly")]
    public List<UnitSiteFlag> hostilityUnitSiteFlag;

    [Header("UnitPlayMoveTime")]
    [SerializeField] private float unitPlatMoveTime;

    /// <summary>
    /// All State use Value
    /// </summary>
    public UnitPlatQueue FriendlyUnitPlatsQueue { get; private set; }

    /// <summary>
    /// All State use Value
    /// </summary>
    public UnitPlatQueue HostilityUnitPlatsQueue { get; private set; }

    private PriorityQueue<UnitPlat> battleQueue;
    private Dictionary<Unit, UnitSkill> OnGameStartSkills;
    private Dictionary<Unit, UnitSkill> OnRoundStartSkills;
    private Dictionary<Unit, UnitSkill> OnRoundEndSkills;
    private Dictionary<Unit, UnitSkill> OnStrikeBackSkills;

    private Dictionary<Unit, byte> OnStrikeBackMap;
    private Dictionary<UnitBuff, List<UnitPlat>> UnitBuffMap;

    private int friendlyDeadCount;
    private int hostilityDeadCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        battleQueue = new PriorityQueue<UnitPlat>(true);
        OnGameStartSkills = new Dictionary<Unit, UnitSkill>();
        OnRoundStartSkills = new Dictionary<Unit, UnitSkill>();
        OnRoundEndSkills = new Dictionary<Unit, UnitSkill>();
        OnStrikeBackSkills = new Dictionary<Unit, UnitSkill>();

        OnStrikeBackMap = new Dictionary<Unit, byte>();
        UnitBuffMap = new Dictionary<UnitBuff, List<UnitPlat>>();

        FriendlyUnitPlatsQueue = new UnitPlatQueue();
        HostilityUnitPlatsQueue = new UnitPlatQueue();

        friendlyDeadCount = 0;
        hostilityDeadCount = 0;
    }

    public void UnitSiteChange(Faction faction, UnitSite fromSite, UnitSite toSite)
    {
        //TODO : UnitMove Animation make

        switch (faction)
        {
            case Faction.Friendly:
                List<UnitPlat> friendlyplats = FriendlyUnitPlatsQueue.UnitPlatSiteChange(fromSite, toSite);

                friendlyplats[0].transform.DOMoveX(friendlyUnitSiteFlag[0].transform.position.x, unitPlatMoveTime);
                friendlyplats[1].transform.DOMoveX(friendlyUnitSiteFlag[1].transform.position.x, unitPlatMoveTime);
                friendlyplats[2].transform.DOMoveX(friendlyUnitSiteFlag[2].transform.position.x, unitPlatMoveTime);
                friendlyplats[3].transform.DOMoveX(friendlyUnitSiteFlag[3].transform.position.x, unitPlatMoveTime);

                break;
            case Faction.Hostility:
                List<UnitPlat> hostitlyplats = HostilityUnitPlatsQueue.UnitPlatSiteChange(fromSite, toSite);

                hostitlyplats[0].transform.DOMoveX(hostilityUnitSiteFlag[0].transform.position.x, unitPlatMoveTime);
                hostitlyplats[1].transform.DOMoveX(hostilityUnitSiteFlag[1].transform.position.x, unitPlatMoveTime);
                hostitlyplats[2].transform.DOMoveX(hostilityUnitSiteFlag[2].transform.position.x, unitPlatMoveTime);
                hostitlyplats[3].transform.DOMoveX(hostilityUnitSiteFlag[3].transform.position.x, unitPlatMoveTime);

                break;
        }
    }

    public void BattleEnd()
    {
        UnitBuffMap.Clear();
        foreach (var unitplat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            unitplat.UnitPlatClear();
        }
        foreach (var unitplat in HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            unitplat.UnitPlatClear();
        }
    }

    public void BattleInit()
    {
        //TODO : Battle Start UI Animation

        FriendlyUnitPlatsQueue.Clear();
        HostilityUnitPlatsQueue.Clear();

        List<UnitPlat> hostitlyPlats = UnitCardSystem.instance.GetHostitlyUnitPlats();
        List<UnitPlat> friendlyPlats = UnitCardSystem.instance.GetFinalFriendlyUnitPlats();
        {
            for (int i = 0; i < 4; i++)
            {
                HostilityUnitPlatsQueue.unitPlatQueue.Add(hostitlyPlats[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                FriendlyUnitPlatsQueue.unitPlatQueue.Add(friendlyPlats[i]);
            }
        }

        OnGameStartSkills.Clear();
        OnRoundStartSkills.Clear();
        OnRoundEndSkills.Clear();
        OnStrikeBackSkills.Clear();
        OnStrikeBackMap.Clear();
        friendlyDeadCount = 0;
        hostilityDeadCount = 0;


        foreach (var plat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            battleQueue.Enqueue(plat, plat.unit.speed);
            foreach (var skill in plat.unit.unitSkills)
            {
                if (skill.TriggerTiming == TriggerTiming.OnGameStart)
                {
                    OnGameStartSkills.Add(plat.unit,skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
                {
                    OnRoundStartSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
                {
                    OnRoundEndSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
                {
                    OnStrikeBackSkills.Add(plat.unit, skill);
                    OnStrikeBackMap.Add(plat.unit, plat.unit.HP);
                }
            }
        }
        foreach (var plat in HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            battleQueue.Enqueue(plat, plat.unit.speed);
            foreach (var skill in plat.unit.unitSkills)
            {
                if (skill.TriggerTiming == TriggerTiming.OnGameStart)
                {
                    OnGameStartSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
                {
                    OnRoundStartSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
                {
                    OnRoundEndSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
                {
                    OnStrikeBackSkills.Add(plat.unit, skill);
                    OnStrikeBackMap.Add(plat.unit, plat.unit.HP);
                }
            }
        }
    }

    public void BattleFunction()
    {
        StartCoroutine(Battle());
    }

    private IEnumerator Battle()
    {
        yield return HPCheck(FriendlyUnitPlatsQueue.GetAllUnitPlat());
        yield return HPCheck(HostilityUnitPlatsQueue.GetAllUnitPlat());

        List<Inventory> removeInventory = new List<Inventory>();
        foreach (var inventory in InventoryManager.instance.globalInventoryList)
        {
            inventory.Action(HostilityUnitPlatsQueue.GetAllUnitPlat());
            inventory.roundContinue--;
            if (inventory.roundContinue <= 0)
            {
                removeInventory.Add(inventory);
            }
            yield return HPCheck(HostilityUnitPlatsQueue.GetAllUnitPlat());
            yield return new WaitForSeconds(inventory.itemData.itemBuff.BuffActionTime);
        }

        for (int i = removeInventory.Count - 1; i >= 0; i--)
        {
            InventoryManager.instance.RemoveInventoryfromGlobal(removeInventory[i]);
        }

        yield return LevelStartBattle();
        yield return AllBuffAction(TriggerTiming.OnGameStart);

        int safecount = 0;
        while (friendlyDeadCount < 4 && hostilityDeadCount < 4)
        {
            safecount++;
            if (safecount > 1000)
            {
                yield break;
            }

            while (battleQueue.Count > 0)
            {
                safecount++;
                if (safecount > 1000)
                {
                    yield break;
                }

                foreach (var unit in OnStrikeBackMap.Keys)
                {
                    OnStrikeBackMap[unit] = unit.HP;
                }
                yield return RoundStartBattle();
                yield return AllBuffAction(TriggerTiming.OnRoundStart);

                UnitPlat unitPlat = battleQueue.Dequeue();
                if (!unitPlat.isDead)
                {
                    UnitSkill OnRoundSkill = unitPlat.unit.UnitSkillChoice();
                    ICollection<UnitPlat> targetPlats = GetActionTargetPlat(unitPlat.unit, OnRoundSkill);
                    OnRoundSkill.Action(targetPlats);

                    Debug.Log(unitPlat.name);
                    Debug.Log(unitPlat.unit.faction + " " + unitPlat.site + " Attack ");
                    foreach (var tar in targetPlats)
                    {
                        Debug.Log(tar.unit.faction + " " + tar.site);
                    }
                    Debug.Log("----------------");

                    foreach (var buff in OnRoundSkill.UnitBuffs)
                    {
                        AddBuffToUnitPlat(buff, targetPlats);
                    }
                    yield return AllBuffAction(TriggerTiming.OnRound);
                    yield return new WaitForSeconds(OnRoundSkill.SkillTime);
                }
                
                yield return RoundEndBattle();
                yield return AllBuffAction(TriggerTiming.OnRoundEnd);

                yield return StrikeBackBattle();
                yield return AllBuffAction(TriggerTiming.OnStrikeBack);
            }

            foreach (var plat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                battleQueue.Enqueue(plat, plat.unit.speed);
            }
            foreach (var plat in HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                battleQueue.Enqueue(plat, plat.unit.speed);
            }
        }
    }

    private IEnumerator AllBuffAction(TriggerTiming timing)
    {
        List<UnitBuff> removeBuff = new List<UnitBuff>();
        foreach (var (buff, plats) in UnitBuffMap)
        {
            if (buff.TriggerTiming != timing)
                continue;

            ICollection<UnitPlat> units = plats;
            buff.Action(units);
            foreach (var plat in plats)
            {
                int continueRound = plat.BuffConsumption(buff);
                if (continueRound <= 0)
                {
                    removeBuff.Add(buff);
                }
            }

            yield return HPCheck(plats);
            yield return new WaitForSeconds(buff.BuffActionTime);
        }

        for (int i = removeBuff.Count - 1; i >= 0; i--)
        {
            UnitBuffMap.Remove(removeBuff[i]);
        }

        List<Inventory> removeInventory = new List<Inventory>();
        foreach (var (inventory, plat) in InventoryManager.instance.InventoryTargetMap)
        {
            Debug.Log("Inventory : " + inventory.name + " " + plat.name);
            if (inventory.itemData.itemBuff.TriggerTiming != timing)
                continue;

            UnitPlat[] actionPlat = new UnitPlat[] { plat };
            Debug.Log("actionPlatCount : " +  actionPlat.Length);
            inventory.Action(actionPlat);
            inventory.roundContinue--;
            if (inventory.roundContinue <= 0)
            {
                removeInventory.Add(inventory);
            }
            yield return HPCheck(actionPlat);
            yield return new WaitForSeconds(inventory.itemData.itemBuff.BuffActionTime);
        }

        for (int i = removeInventory.Count - 1; i >= 0; i--)
        {
            InventoryManager.instance.RemoveInventoryfromUnit(removeInventory[i]);
        }
    }

    private IEnumerator LevelStartBattle()
    {
        foreach (var (unit, gameStartSkill) in OnGameStartSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, gameStartSkill);
            if (unitPlats != null)
            {
                gameStartSkill.Action(unitPlats);
                yield return HPCheck(unitPlats);

                foreach (var buff in gameStartSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff,unitPlats);
                }

                yield return new WaitForSeconds(gameStartSkill.SkillTime);
            }

        }
    }

    private IEnumerator RoundStartBattle()
    {
        foreach (var (unit, roundStartSkill) in OnRoundStartSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, roundStartSkill);
            if (unitPlats != null)
            {
                roundStartSkill.Action(unitPlats);
                yield return HPCheck(unitPlats);

                foreach (var buff in roundStartSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, unitPlats);
                }

                yield return new WaitForSeconds(roundStartSkill.SkillTime);
            }

        }
    }

    private IEnumerator RoundEndBattle()
    {
        foreach (var (unit, roundEndSkill) in OnRoundEndSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, roundEndSkill);
            if (unitPlats != null)
            {
                roundEndSkill.Action(unitPlats);
                yield return HPCheck(unitPlats);

                foreach (var buff in roundEndSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, unitPlats);
                }
                yield return new WaitForSeconds(roundEndSkill.SkillTime);
            }

        }
    }

    private IEnumerator StrikeBackBattle()
    {
        foreach (var (unit, HP) in OnStrikeBackMap)
        {
            if (unit.HP < HP)
            {
                UnitSkill strikeBackSkill = OnStrikeBackSkills[unit];
                ICollection<UnitPlat> targetPlats = GetActionTargetPlat(unit, strikeBackSkill);
                strikeBackSkill.Action(targetPlats);
                yield return HPCheck(targetPlats);

                foreach (var buff in strikeBackSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, targetPlats);
                }

                yield return new WaitForSeconds(strikeBackSkill.SkillTime);
            }
        }
    }

    private IEnumerator HPCheck(ICollection<UnitPlat> unitPlats)
    {
        float maxDeadTIme = 0;
        foreach (var plat in unitPlats)
        {
            if (plat.isDead) continue;

            if (plat.unit.HP <= 0)
            {
                plat.isDead = true;
                if (plat.unit.faction == Faction.Friendly)
                {
                    battleQueue.Remove(plat);
                    friendlyDeadCount++;
                }
                else
                {
                    hostilityDeadCount++;
                }


                if (hostilityDeadCount >= unitPlatQueueCount)
                {
                    //TODO : Game Win Logic
                    StopAllCoroutines();
                    BattleEnd();
                    GameManager.instance.GameBattleEnd(true);
                }

                if (friendlyDeadCount >= unitPlatQueueCount)
                {
                    //TODO : Game Lose Logic
                    StopAllCoroutines();
                    BattleEnd();
                    GameManager.instance.GameBattleEnd(false);
                }

                if (maxDeadTIme < plat.unit.DeadAnimationTime)
                {
                    maxDeadTIme = plat.unit.DeadAnimationTime;
                }
            }
        }

        yield return maxDeadTIme;
    }

    private void AddBuffToUnitPlat(UnitBuff buff, ICollection<UnitPlat> plats)
    {
        foreach (var plat in plats)
        {
            plat.AddBuff(buff);
        }

        foreach (var (buf, plat) in UnitBuffMap)
        {
            if (buf.Equals(buff))
            {
                plat.AddRange(plats);
                return;
            }
        }

        UnitBuffMap.Add(buff, plats.ToList());
    }

    private ICollection<UnitPlat> GetActionTargetPlat(Unit self, UnitSkill skill)
    {
        Debug.Log(self.faction + " " + skill.SkillTarget);

        if (self.faction == Faction.Friendly)
        {
            if (skill.SkillTarget == Faction.Friendly)
            {
                return FriendlyUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
            else
            {
                return HostilityUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
        }
        else if(self.faction == Faction.Hostility)
        {
            if (skill.SkillTarget == Faction.Friendly)
            {
                return HostilityUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
            else
            {
                return FriendlyUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
        }

        return null;
    }

    private ICollection<Unit> GetActionTarget(ICollection<UnitPlat> unitCollection)
    {
        List<Unit> units = new List<Unit>();
        foreach (var plat in unitCollection)
        {
            units.Add(plat.unit);
        }

        return units;
    }

    private ICollection<Unit> GetActionTarget(Unit self, UnitSkill skill)
    {
        List<Unit> units = new List<Unit>();
        foreach (var plat in GetActionTargetPlat(self, skill))
        {
            units.Add(plat.unit);
        }

        return units;
    }

    public ICollection<UnitPlat> GetActionPlats(Faction faction, ICollection<UnitSite> range)
    {
        List<UnitPlat> reslutes = new List<UnitPlat>();
        switch (faction)
        {
            case Faction.Friendly:
                foreach (var ran in range)
                {
                    int index = GetIndexByUnitSite(ran);
                    reslutes.Add(FriendlyUnitPlatsQueue.GetUnitPlatByUnitSite(ran).plat);
                }
                break;
            case Faction.Hostility:
                foreach (var ran in range)
                {
                    int index = BattleSystem.GetIndexByUnitSite(ran);
                    reslutes.Add(HostilityUnitPlatsQueue.GetUnitPlatByUnitSite(ran).plat);
                }
                break;
        }

        return reslutes;
    }

    public static int GetIndexByUnitSite(UnitSite site)
    {
        switch (site)
        {
            case UnitSite.first:
                return 0;
            case UnitSite.second:
                return 1;
            case UnitSite.third:
                return 2;
            case UnitSite.fourth:
                return 3;
            default:
                return -1;
        }
    }

    public static UnitSite GetUnitSiteByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return UnitSite.first;
            case 1:
                return UnitSite.second;
            case 2:
                return UnitSite.third;
            case 3:
                return UnitSite.fourth;
            default :
                return UnitSite.under;
        }
    }

    public UnitSite GetUnitSiteByUnit(Faction faction, Unit unit)
    {
        switch (faction)
        {
            case Faction.Friendly:
                return FriendlyUnitPlatsQueue.GetUnitSite(unit);
            case Faction.Hostility:
                return HostilityUnitPlatsQueue.GetUnitSite(unit);
            default:
                return UnitSite.under;
        }
    }

    public class UnitPlatQueue
    {
        public List<UnitPlat> unitPlatQueue;

        public List<UnitPlat> UnitPlatSiteChange(UnitSite fromUnitSite, UnitSite toUnitSite)
        {
            UnitPlat fromUnit = GetUnitPlatByUnitSite(fromUnitSite).plat;
            int insertIndex = GetUnitPlatByUnitSite(toUnitSite).index;

            unitPlatQueue.Remove(fromUnit);
            unitPlatQueue.Insert(insertIndex, fromUnit);

            for (int i = 0; i < 4; i++)
            {
                unitPlatQueue[i].site = GetUnitSiteByIndex(i);
            }
            return unitPlatQueue;
        }

        public (int index, UnitPlat plat) GetUnitPlatByUnitSite(UnitSite unitSite)
        {
            switch (unitSite)
            {
                case UnitSite.first:
                    return (0, unitPlatQueue[0]);
                case UnitSite.second:
                    return (1, unitPlatQueue[1]);
                case UnitSite.third:
                    return (2, unitPlatQueue[2]);
                case UnitSite.fourth:
                    return (3, unitPlatQueue[3]);
                default:
                    return (-1, null);
            }
        }

        public ICollection<UnitPlat> GetAllUnitPlat()
        {
            return unitPlatQueue;
        }

        public ICollection<UnitPlat> GetUnitPlats(ICollection<UnitSite> sites)
        {
            List<UnitPlat> unitPlats = new List<UnitPlat>();
            foreach (var site in sites)
            {
                UnitPlat plat = GetUnitPlatByUnitSite(site).plat;
                unitPlats.Add(plat);
            }

            return unitPlats;
        }

        public UnitSite GetUnitSite(Unit unit)
        {
            foreach (var unitPlat in unitPlatQueue)
            {
                if (unitPlat.unit == unit)
                {
                    return unitPlat.site;
                }
            }

            return UnitSite.under;
        }

        public void Clear()
        {
            unitPlatQueue.Clear();
        }

        public UnitPlatQueue()
        {
            unitPlatQueue = new List<UnitPlat>(unitPlatQueueCount);
        }

        public UnitPlatQueue(List<UnitPlat> unitPlatQueue)
        {
            this.unitPlatQueue = new List<UnitPlat>();
            unitPlatQueue.AddRange(unitPlatQueue);
        }
    }
}

public enum UnitSite
{
    first,
    second, 
    third, 
    fourth,
    under
}