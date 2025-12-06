using System;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Numerics;
using Avalonia.Input;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Digger.Architecture;
using ReactiveUI;

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
                if (y == 0 || IsCreatureHere(x, y - 1, new Sack()))
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaY = -1 };
            case Key.Down:
                if (y == Game.MapHeight - 1 || IsCreatureHere(x, y + 1, new Sack()))
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaY = 1 };
            case Key.Left:
                if (x == 0 || IsCreatureHere(x - 1, y, new Sack()))
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaX = -1 };
            case Key.Right:
                if (x == Game.MapWidth - 1 || IsCreatureHere(x + 1, y, new Sack()))
                    return new CreatureCommand() {};
                return new CreatureCommand() { DeltaX = 1 };
            default:
                return new CreatureCommand();
        }
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        return conflictedObject is Sack || conflictedObject is Monster;
    }

    public int GetDrawingPriority()
    {
        return (int)Priority.Digger;
    }

    public string GetImageFileName()
    {
        return "Digger.png";
    }

    public bool IsCreatureHere(int x, int y, ICreature obj)
    {
        var creature = Game.Map[x, y];
        return creature != null && creature.GetType() == obj.GetType();
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
            if (IsCreatureHere(x, y + 1, null) ||
            WasFalling && IsCreatureHere(x, y + 1, new Monster()) ||
            WasFalling && IsCreatureHere(x, y + 1, new Player()))
            {
                WasFalling = true;
                CellsCount++;
                return new CreatureCommand() { DeltaX = 0, DeltaY = 1};
            } else if (CellsCount > 1 && !(
                IsCreatureHere(x, y + 1, null) || 
                IsCreatureHere(x, y + 1, new Player())))
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

    public bool IsCreatureHere(int x, int y, ICreature obj)
    {
        var creature = Game.Map[x, y];
        if (obj == null)
            return creature is null;
        return creature.GetType() == obj.GetType();
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
        return conflictedObject is Monster;
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

class Monster : ICreature
{
    public int PlayerCordX;
    public int PlayerCordY;
    public CreatureCommand Act(int x, int y)
    {
        PlayerCordX = FoundPlayerCord()[0];
        PlayerCordY = FoundPlayerCord()[1];

        if (PlayerCordX != -1 && PlayerCordY != -1)
        {
            if (PlayerCordX > x && !(
                IsCreatureHere(x + 1, y, new Terrain()) || 
                IsCreatureHere(x + 1, y, new Sack()) || 
                IsCreatureHere(x + 1, y, new Monster())))
                return new CreatureCommand() { DeltaX = 1 };
            else if (PlayerCordX < x && !(
                IsCreatureHere(x - 1, y, new Terrain()) || 
                IsCreatureHere(x - 1, y, new Sack()) || 
                IsCreatureHere(x - 1, y, new Monster())))
                return new CreatureCommand() { DeltaX = -1 };
            else if (PlayerCordY > y && !(
                IsCreatureHere(x, y + 1, new Terrain()) || 
                IsCreatureHere(x, y + 1, new Sack()) || 
                IsCreatureHere(x, y + 1, new Monster())))
                return new CreatureCommand() { DeltaY = 1 };
            else if (PlayerCordY < y && !(
                IsCreatureHere(x, y - 1, new Terrain()) || 
                IsCreatureHere(x, y - 1, new Sack()) || 
                IsCreatureHere(x, y - 1, new Monster())))
                return new CreatureCommand() { DeltaY = -1 };
        }
        return new CreatureCommand();
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        return conflictedObject is Sack || conflictedObject is Monster;
    }

    public int GetDrawingPriority()
    {
        return (int)Priority.Monster;
    }

    public string GetImageFileName()
    {
        return "Monster.png";
    }

    public bool IsCreatureHere(int x, int y, ICreature obj)
    {
        var creature = Game.Map[x, y];
        return creature != null && creature.GetType() == obj.GetType();
    }

    public int[] FoundPlayerCord()
    {
        for (var playerCordX = 0; playerCordX < Game.MapWidth; playerCordX++)
            for (var playerCordY = 0; playerCordY < Game.MapHeight; playerCordY++)
                if (Game.Map[playerCordX, playerCordY] is Player)
                {
                    return new[] { playerCordX, playerCordY };
                }
        return new[] { -1, -1 };
    }
}