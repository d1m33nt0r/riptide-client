using System.Collections.Generic;
using DefaultNamespace;
using Multiplayer;
using RiptideNetworking;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new ();
    
    public ushort ID { get; private set; }
    public bool IsLocal { get; private set; }

    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private Transform cameraTransform;
    
    private string username;

    private void OnDestroy()
    {
        list.Remove(ID);
    }

    private void Move(Vector3 newPosition, Vector3 forward)
    {
        transform.position = newPosition;

        if (!IsLocal)
        {
            cameraTransform.forward = forward;
            animationManager.AnimateBasedOnSpeed();
        }
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

    #region Messages

    [MessageHandler((ushort)ServerToClientID.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
    
    [MessageHandler((ushort)ServerToClientID.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Move(message.GetVector3(), message.GetVector3());
    }

    #endregion
    
}