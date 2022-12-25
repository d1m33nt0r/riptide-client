using System.Collections.Generic;
using Multiplayer;
using RiptideNetworking;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new ();
    
    public ushort ID { get; private set; }
    public bool IsLocal { get; private set; }

    private string username;

    private void OnDestroy()
    {
        list.Remove(ID);
    }

    public static void Spawn(ushort ID, string username, Vector3 position)
    {
        Player player;
        if (ID == NetworkManager.Singleton.Client.Id)
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity)
                .GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity)
                .GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {ID} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.ID = ID;
        player.username = username;
        
        list.Add(ID, player);
    }

    [MessageHandler((ushort)ServerToClientID.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
}