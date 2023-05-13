using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MessageBroker : MonoBehaviour
{
    #region Config

    private const float waitForAcknowledgementInterval = 0.2f;
    public static readonly List<OnlineMessageCode> messagesNotToBeAcknowledged = new() { OnlineMessageCode.METADATA };

    #endregion

    #region Helper classes
    public class MessageQueue
    {
        public Queue<Letter> queue = new();
        public Letter currentMessageToAcknowledge = null;
    }

    public class Letter
    {
        public OnlineMessage msg;
        public Receiver receiver;

        public Letter(OnlineMessage msg, Receiver receiver)
        {
            this.msg = msg;
            this.receiver = receiver;
        }
    }
    public class Receiver
    {
        public NetworkConnection networkConnection;
        public int lobbyId;

        public Receiver(NetworkConnection networkConnection, int lobbyId)
        {
            this.networkConnection = networkConnection;
            this.lobbyId = lobbyId;
        }
    }
    #endregion

    private bool isActive = false;
    private bool IsActive { get { return isActive && ((OnlineClient.Instance && OnlineClient.Instance.IsActive) || (OnlineServer.Instance && OnlineServer.Instance.IsActive)); } }
    private NetworkDriver driver;
    public NetworkDriver Driver { set { driver = value; isActive = true; } }

    private readonly Dictionary<NetworkConnection, MessageQueue> connections = new();
    private readonly Dictionary<NetworkConnection, LimitedList<string>> receivedMessages = new();

    private void Awake()
    {
        OnlineMessageEvents.OnMessageReceive += OnMessageReceive;
    }

    private float delta = 0;
    private void Update()
    {
        if (!IsActive)
            return;

        foreach(var cnnQueue in connections)
        {
            MessageQueue msgQueue = cnnQueue.Value;

            if(msgQueue.currentMessageToAcknowledge != null)
            {
                delta += Time.deltaTime;
                if(delta >= waitForAcknowledgementInterval)
                {
                    delta = 0;
                    SendMessage(msgQueue.currentMessageToAcknowledge);
                    return;
                }
            } else
            {
                if (msgQueue.queue.Count > 0)
                {
                    msgQueue.currentMessageToAcknowledge = msgQueue.queue.Dequeue();
                    SendMessage(msgQueue.currentMessageToAcknowledge);
                }
            }
        }
    }

    public void RemoveConnection(NetworkConnection cnn)
    {
        if (connections.ContainsKey(cnn))
            connections.Remove(cnn);

        if (receivedMessages.ContainsKey(cnn))
            receivedMessages.Remove(cnn);
    }

    public void SendMessage(OnlineMessage msg, NetworkConnection networkConnection, int lobbyId)
    {
        SendMessage(msg, new Receiver(networkConnection, lobbyId));
    }

    private void SendMessage(OnlineMessage msg, Receiver receiver)
    {
        if(!connections.ContainsKey(receiver.networkConnection))
        {
            connections.Add(receiver.networkConnection, new MessageQueue());
        }

        Letter letter = new Letter(msg, receiver);
        if(messagesNotToBeAcknowledged.Contains(msg.Code))
        {
            SendMessage(letter);
            return;
        }

        connections[receiver.networkConnection].queue.Enqueue(letter);
    }

    private void SendMessage(Letter letter)
    {
        driver.BeginSend(letter.receiver.networkConnection, out DataStreamWriter writer);
        letter.msg.Serialize(ref writer, letter.receiver.lobbyId);
        driver.EndSend(writer);
    }

    // If msg is an acknowledgement, confirm acknowledgement, else acknowledge received msg and read msg
    public void OnMessageReceive(OnlineMessage msg, NetworkConnection sender)
    {
        if (msg.GetType() == typeof(MsgAcknowledgement))
        {
            ConfirmAcknowledgement(sender, ((MsgAcknowledgement)msg).msgId);
            return;
        }
        
        if (!messagesNotToBeAcknowledged.Contains(msg.Code))
        {
            // Debug.Log("Received msg " + msg + " with id " + msg.Id);

            // Send MsgAcknowledgement
            SendMessage(new Letter(new MsgAcknowledgement { msgId = msg.Id }, new Receiver(sender, msg.LobbyId)));

            if (IgnoreMessage(msg, sender))
                return;

            receivedMessages[sender].Add(msg.Id);
        }

        if (OnlineServer.Instance && OnlineServer.Instance.IsActive)
            msg.ReceivedOnServer(sender);
        else
            msg.ReceivedOnClient();
    }

    private void ConfirmAcknowledgement(NetworkConnection sender, string msgId)
    {
        MessageQueue msgQueue = connections[sender];
        if (msgQueue.currentMessageToAcknowledge == null)
            return;

        if (msgQueue.currentMessageToAcknowledge.msg.Id == msgId)
        {
            msgQueue.currentMessageToAcknowledge = null;
            // Debug.Log("ACKNOWLEDGE: Connection confirmed reception of msg with id " + msgId);
        } 
    }

    private bool IgnoreMessage(OnlineMessage msg, NetworkConnection sender)
    {
        if(!receivedMessages.ContainsKey(sender))
        {
            receivedMessages.Add(sender, new(5));
        }

        return receivedMessages[sender].Contains(msg.Id);
    }

    private void OnDestroy()
    {
        OnlineMessageEvents.OnMessageReceive -= OnMessageReceive;
    }
}
