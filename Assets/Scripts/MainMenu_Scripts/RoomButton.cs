using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI roomPlayerCounterText;
    public RoomInfo roomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo _roomInfo)
    {
        roomInfo = _roomInfo;
        roomPlayerCounterText.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
        roomNameText.text = roomInfo.Name;
    }
}
