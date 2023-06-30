using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;

public class ChatBox : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI chatLogText;
    public TMP_InputField chatInput;

    public static ChatBox instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnChatInputSend()
    {
        if (chatInput.text.Length > 0)
        {
            photonView.RPC("Log", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatInput.text);
            chatInput.text = string.Empty;
        }
    }

    public override void OnLeftRoom()
    {
        chatLogText.text = "";
        chatInput.text = "";
    }

    [PunRPC]
    void Log(string playerName, string message)
    {
        chatLogText.text += string.Format("<b>{0}:</b> {1}\n", playerName, message);
        chatLogText.rectTransform.sizeDelta = new Vector2(chatLogText.rectTransform.sizeDelta.x, chatLogText.bounds.size.y + 20);
    }
}
