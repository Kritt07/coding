using System;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Numerics;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
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
                if (y == 0 || Game.Map[x, y - 1] is Sack)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaY = -1 };
            case Key.Down:
                if (y == Game.MapHeight - 1 || Game.Map[x, y + 1] is Sack)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaY = 1 };
            case Key.Left:
                if (x == 0 || Game.Map[x - 1, y] is Sack)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaX = -1 };
            case Key.Right:
                if (x == Game.MapWidth - 1 || Game.Map[x + 1, y] is Sack)
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaX = 1 };
            default:
                return new CreatureCommand();
        }
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        return conflictedObject is Sack;
    }

    public int GetDrawingPriority()
    {
        return (int)Priority.Digger;
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
        return (int)Priority.Terrain;
    }

    public string GetImageFileName()
    {
        return "Terrain.png";
    }
}

class Sack : ICreature
{
    public int CellsCount = 0;
    public bool WasFalling = false;
    public CreatureCommand Act(int x, int y)
    {
        if (y != Game.MapHeight - 1)
        {
            if (Game.Map[x, y + 1] is null || WasFalling && Game.Map[x, y + 1] is Player)
            {
                WasFalling = true;
                CellsCount++;
                return new CreatureCommand() { DeltaX = 0, DeltaY = 1};
            } else if (CellsCount > 1 && !(Game.Map[x, y + 1] is null || Game.Map[x, y + 1] is Player))
                return new CreatureCommand() { TransformTo = new Gold() };

            CellsCount = 0;
            return new CreatureCommand();
        } else if (CellsCount > 1) 
            return new CreatureCommand() { TransformTo = new Gold() };

        CellsCount = 0;
        return new CreatureCommand();
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        return false;
    }

    public int GetDrawingPriority()
    {
        return (int)Priority.Sack;
    }

    public string GetImageFileName()
    {
        return "Sack.png";
    }
}

class Gold : ICreature
{
    public CreatureCommand Act(int x, int y)
    {
        return new CreatureCommand();
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        if (conflictedObject is Player)
        {
            Game.Scores += 10;
            return true;
        }
        return false;
    }

    public int GetDrawingPriority()
    {
        return (int)Priority.Gold;
    }

    public string GetImageFileName()
    {
        return "Gold.png";
    }
}