using System;
using System.Data;
using System.IO.Compression;
using Avalonia.Input;
using Digger.Architecture;

namespace Digger;

//Напишите здесь классы Player, Terrain и другие.

public enum Priority
{
    Terrain = 3,//Местность
    Digger = 2,//Копатель (Игрок)
    Sack = 0,//Мешок (с золотом)
    Gold = 4,//Золото
    Monster = 1//Монстр
};

class Player : ICreature
{
    public CreatureCommand Act(int x, int y)
    {
        switch (Game.KeyPressed)
        {
            case Key.Up:
                if (y == 0)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaY = -1 };
            case Key.Down:
                if (y == Game.MapHeight - 1)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaY = 1 };
            case Key.Left:
                if (x == 0)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaX = -1 };
            case Key.Right:
                if (x == Game.MapWidth - 1)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaX = 1 };
            default:
                return new CreatureCommand();
        }
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        return false;
    }

    public int GetDrawingPriority()
    {
        return 1;
    }

    public string GetImageFileName()
    {
        return "Digger.png";
    }
}

class Terrain : ICreature
{
    public CreatureCommand Act(int x, int y)
    {
        return new CreatureCommand();
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        return conflictedObject is Player;
    }

    public int GetDrawingPriority()
    {
        return 4;
    }

    public string GetImageFileName()
    {
        return "Terrain.png";
    }
}