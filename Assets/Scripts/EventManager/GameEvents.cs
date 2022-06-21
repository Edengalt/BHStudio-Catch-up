
using System.Threading;

public class GameEvent { }

/// <summary>
/// Adding points to scoreboard 
/// </summary>
public class AddPoints : GameEvent
{
    public string ID;
    public int scoreDelta;

    public AddPoints(string id, int scoreDelta)
    {
        ID = id;
        this.scoreDelta = scoreDelta;
    }
}


/// <summary>
/// Adding new player to scoreboard
/// </summary>
public class AddPlayer : GameEvent
{
    public string ID;
    public PlayerInitedNetworkBehaviour player;

    public AddPlayer(string id,PlayerInitedNetworkBehaviour _player)
    {
        ID = id;
        player = _player;
    }
}

public class SessionStarted : GameEvent
{
}

public class SessionEnded : GameEvent
{
    public string winnerName;

    public SessionEnded(string _winnerName)
    {
        winnerName = _winnerName;
    }
}
