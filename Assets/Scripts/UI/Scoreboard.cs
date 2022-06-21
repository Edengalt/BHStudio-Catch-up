using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class Scoreboard : NetworkBehaviour
{
  
    [SerializeField] private Transform placeHolder;
    [SerializeField] private ScoreboardItem scoreboardItem;
    private List<ScoreboardItem> Items = new List<ScoreboardItem>();
    private List<string> localNames = new List<string>();
    [SerializeField]private List<PlayerInitedNetworkBehaviour> storedPlayers = new List<PlayerInitedNetworkBehaviour>();
    private SyncList<string> names = new SyncList<string>();
    private SyncList<int> scores = new SyncList<int>();
    private void Awake()
    {
        EventManager.Instance.AddListener<AddPoints>(OnAddPoints);
        EventManager.Instance.AddListener<AddPlayer>(OnAddPlayer);
        EventManager.Instance.AddListener<SessionEnded>(OnSessionEnded);
        names.Callback += ScoreboardNamesChanged;
        scores.Callback += ScoreboardScoresChanged;
    }

    private void OnSessionEnded(SessionEnded e)
    {
        SessionEnded();
    }
    
    [Command(requiresAuthority = false)]
    public void SessionEnded()
    {
        for (int s = 0; s < scores.Count; s++)
        {
            scores[s] = 0;
        }
    }

    private string playersID = "";
    

    private void OnAddPlayer(AddPlayer e)
    {
        AddPlayer(e.ID, e.player);
        playersID = e.ID;
        Highlight(playersID);
    }
    
    [Command(requiresAuthority = false)]
    public void AddPlayer(string ID, PlayerInitedNetworkBehaviour pl)
    {
        storedPlayers.Add(pl);
        names.Add(ID);
        scores.Add(0);
    }


    public void Highlight(string ID)
    {
        foreach (var item in Items)
        {
            if(item.ID == ID)
                item.Highlight();
        }
    }
    
   
    
    private void OnAddPoints(AddPoints e)
    {
        AddPoints(e.ID, e.scoreDelta);
    }

    [Command(requiresAuthority = false)]
    public void AddPoints(string ID, int scoreDelta)
    {
        foreach (var item in Items)
        {
            if (item.ID == ID)
            {
                Debug.Log("Setting score to " + ID);
                item.SetScore(scoreDelta);
                break;
            }
        }

        scores[names.IndexOf(ID)] += scoreDelta;
        SortScore();
    }
    
    public void ScoreboardScoresChanged(SyncList<int>.Operation op, int index, int oldItem, int newItem)
    {
        Debug.Log("SyncList changed " + index + " " + newItem);
        
        for (int i = 0; i < scores.Count; i++)
        {
            foreach (var item in Items)
            {
                if (item.ID == localNames[i])
                {
                    item.SetScore(scores[i]);
                }
            }
        }
        CheckLostPlayers();
        SortScore();
    }

    public void CheckLostPlayers()
    {
        if(isServer) return;
        List<string> todel = new List<string>();
            foreach (var _name in localNames)
            {
                if (!names.Contains(_name))
                {
                    ScoreboardItem item = Items.Find(x => x.ID == _name);
                    Items.Remove(item);
                    item.Rollout();
                    todel.Add(_name);
                }
                
            }
            for (int i = 0; i < todel.Count; i++)
            {
                localNames.Remove(todel[i]);
            }
            for (int j = 0; j < storedPlayers.Count; j++)
            {
                if (storedPlayers[j] == null)
                    storedPlayers.RemoveAt(j);
            }
        
    }

    public void ScoreboardNamesChanged(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        Debug.Log("SyncList changed " + newItem);
        foreach (var syncItem in names)
        {
            if (!localNames.Contains(syncItem))
            {
                ScoreboardItem newPlayer = Instantiate(scoreboardItem, placeHolder);
                
                newPlayer.Setup(syncItem);
                Items.Add(newPlayer);
                localNames.Add(syncItem);
                
            }
        }
       // CheckLostedPlayers();

        Highlight(playersID);
    }

    private void Update()
    {
       if(isServer) DeleteUnused();
    }


    [Server]
    public void DeleteUnused()
    {
        List<ScoreboardItem> toDel = new List<ScoreboardItem>();
        foreach (var pl in storedPlayers)
        {
            if (pl == null)
            {
                string name = Items[storedPlayers.IndexOf(pl)].ID;
                Debug.Log("try to delete " + name);
                scores.RemoveAt(names.IndexOf(name));
                names.Remove(name);
                //RemovePlayer(Items[storedPlayers.IndexOf(pl)].ID);
                ScoreboardItem item = Items[storedPlayers.IndexOf(pl)];
                toDel.Add(item);
                Debug.Log("DELETED " + name);
            }
        }

        foreach (var del in toDel)
        {
            Items.Remove(del);
            del.Rollout();
        }

        for (int i = 0; i < storedPlayers.Count; i++)
        {
            if (storedPlayers[i] == null)
                storedPlayers.RemoveAt(i);
        }
    }
    
    public void SortScore()
    {
        ScoreboardItem winner = Items.Find(x => x.Score >= GlobalGameSettings.Instance.MaxScore);
        if(winner)
            EventManager.Instance.Raise(new SessionEnded(winner.ID));
        IOrderedEnumerable<ScoreboardItem> itemsClone = Items.OrderBy(x => x.Score);
        foreach (var item in itemsClone)
        {
            Items.Find(x => x.ID == item.ID).transform.SetAsFirstSibling();
        }
       
    }
    
}
