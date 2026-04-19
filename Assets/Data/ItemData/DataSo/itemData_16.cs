using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_16))]
public class itemData_16 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += OnAction;
    }

    private float OnAction()
    {

        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            switch (unit.unit.speed)
            {
                case 1:
                    unit.unit.speed = 9;
                    break;
                case 2:
                    unit.unit.speed = 8;
                    break;
                case 3:
                    unit.unit.speed = 7;
                    break;
                case 4:
                    unit.unit.speed = 6;
                    break;
                case 5:
                    unit.unit.speed = 5;
                    break;
                case 6:
                    unit.unit.speed = 4;
                    break;
                case 7:
                    unit.unit.speed = 3;
                    break;
                case 8:
                    unit.unit.speed = 2;
                    break;
                case 9:
                    unit.unit.speed = 1;
                    break;
                default:
                    unit.unit.speed = 1;
                    break;
            }
        }

        foreach (var unit in
            BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            switch (unit.unit.speed)
            {
                case 1:
                    unit.unit.speed = 9;
                    break;
                case 2:
                    unit.unit.speed = 8;
                    break;
                case 3:
                    unit.unit.speed = 7;
                    break;
                case 4:
                    unit.unit.speed = 6;
                    break;
                case 5:
                    unit.unit.speed = 5;
                    break;
                case 6:
                    unit.unit.speed = 4;
                    break;
                case 7:
                    unit.unit.speed = 3;
                    break;
                case 8:
                    unit.unit.speed = 2;
                    break;
                case 9:
                    unit.unit.speed = 1;
                    break;
                default:
                    unit.unit.speed = 1;
                    break;
            }
        }

        BattleSystem.instance.OnGameStart -= OnAction;

        return 0;
    }
}