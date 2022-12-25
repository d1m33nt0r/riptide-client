using Multiplayer;
using RiptideNetworking;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;

        private bool[] inputs;

        private void Start()
        {
            inputs = new bool[6];
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
                inputs[0] = true;
            if (Input.GetKey(KeyCode.A))
                inputs[1] = true;
            if (Input.GetKey(KeyCode.S))
                inputs[2] = true;
            if (Input.GetKey(KeyCode.D))
                inputs[3] = true;
            if (Input.GetKey(KeyCode.Space))
                inputs[4] = true;
            if (Input.GetKey(KeyCode.LeftShift))
                inputs[5] = true;
        }

        private void FixedUpdate()
        {
            SendInputs();

            for (var i = 0; i < inputs.Length; i++)
                inputs[i] = false;
        }

        #region Messages

        private void SendInputs()
        {
            var message = Message.Create(MessageSendMode.unreliable, ClientToServerID.inputs);
            message.AddBools(inputs);
            message.AddVector3(cameraTransform.forward);
            NetworkManager.Singleton.Client.Send(message);
        }

        #endregion
    }
}