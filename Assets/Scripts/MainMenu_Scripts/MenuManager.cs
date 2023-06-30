using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class MenuManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject loginScreen;
    public GameObject lobbyScreen;
    public GameObject createRoomScreen;
    public GameObject roomScreen;
    public GameObject roomBrowserScreen;

    [Header("Login Screen")]
    public Button loginInButton;

    [Header("Create Room Screen")]
    public Button completeRoomCreationButton;
    public TMP_InputField maxNumberPlayerInputField;
    public TMP_InputField roomNameInputField;

    [Header("Room Screen")]
    public GameObject playerNickPrefab;
    public RectTransform playerNickContainer;
    public Button startGameButton;
    public TextMeshProUGUI roomInfoText;
    public TextMeshProUGUI roomPlayerCounterText;
    private List<GameObject> playerNickObjectsList = new List<GameObject>();
    public Image missionImage;
    public TextMeshProUGUI missionText;
    public Button nextMissionButton;
    public Button previousMissionButton;
    public string[] missions;
    private int index = 0;

    [Header("Room Browser Screen")]
    public GameObject roomButtonPrefab;
    public RectTransform roomListContainer;
    public Dictionary<string, GameObject> roomButtonsList;
    private Dictionary<string, RoomInfo> roomInfoList;


    public Button joinRoomButton;
    public TMP_InputField findRoomNameInputField;

    [Header("Warning Panels Screen")]
    public GameObject shortNickNamePanel;
    public GameObject shortRoomNamePanel;
    public GameObject sameRoomNamePanel;
    public GameObject fullRoomPanel;
    public GameObject invalidCharacterPanel;
    public GameObject playerLimitExeedPanel;
    public GameObject roomNotFoundPanel;
    public GameObject playerNumberNeedToBeSetPanel;
    public GameObject disconectedFromServerPanel;

    private void Start()
    {
        loginInButton.interactable = false;

        roomButtonsList = new Dictionary<string, GameObject>();
        roomInfoList = new Dictionary<string, RoomInfo>();

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        loginInButton.interactable = true;
        Debug.Log("Connected To Master Server");
    }

    public void SetScreen(GameObject screen)
    {
        loginScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);

        screen.SetActive(true);
        ResetInputFieldsText();
    }

    void ResetInputFieldsText()
    {
        findRoomNameInputField.text = "";
        maxNumberPlayerInputField.text = "";
        roomNameInputField.text = "";
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    //LOGIN SCREEN
    public void LogInButton(GameObject screen)
    {
        if (PhotonNetwork.NickName.Length < 4)
        {
            shortNickNamePanel.SetActive(true);
            return;
        }
        else
        {
            SetScreen(lobbyScreen);
        }
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    //CREATE ROOM SCREEN
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        if (maxNumberPlayerInputField.text.Length <= 0)
        {
            playerNumberNeedToBeSetPanel.SetActive(true);
            return;
        }


        foreach (var entry in roomInfoList)
        {
            if (entry.Value.Name == roomNameInput.text)
            {
                sameRoomNamePanel.SetActive(true);
                return;
            }
        }

        if (maxNumberPlayerInputField.text.Length == 1 && maxNumberPlayerInputField.text[0] == '0')
        {
            playerNumberNeedToBeSetPanel.SetActive(true);
            return;
        }

        foreach (char item in maxNumberPlayerInputField.text)
        {
            if (item == '-')
            {
                invalidCharacterPanel.SetActive(true);
                return;
            }
        }

        int maxPlayerCout = int.Parse(maxNumberPlayerInputField.text);

        if (roomNameInput.text.Length < 4)
        {
            shortRoomNamePanel.SetActive(true);
            return;
        }
        else if (maxPlayerCout > NetworkManager.instance.maxPlayers)
        {
            playerLimitExeedPanel.SetActive(true);
            return;
        }

        NetworkManager.instance.CreateRoom(roomNameInput.text, maxPlayerCout);
        createRoomScreen.SetActive(false);
        roomNameInput.text = "";
        maxNumberPlayerInputField.text = "";
    }

    //ROOM BROWSER SCREEN
    public void FindRoomButton(TMP_InputField roomNameInput)
    {
        if (roomNameInput.text.Length < 4)
        {
            shortRoomNamePanel.SetActive(true);
            return;
        }

        bool hasRoom = false;
        foreach (var entry in roomInfoList)
        {
            if (entry.Value.Name == roomNameInput.text)
            {
                if (entry.Value.PlayerCount == entry.Value.MaxPlayers)
                {
                    fullRoomPanel.SetActive(true);
                    return;
                }

                hasRoom = true;
                break;
            }
        }

        if (!hasRoom)
        {
            roomNotFoundPanel.SetActive(true);
            return;
        }

        NetworkManager.instance.JoinRoom(roomNameInput.text);
        joinRoomButton.interactable = false;
        roomNameInput.text = "";
    }

    GameObject PopulateObjectList(GameObject prefab, Transform container, List<GameObject> returnList)
    {
        GameObject obj = Instantiate(prefab, container);
        returnList.Add(obj);
        return obj;
    }

    public void OnJoinRoomButton(string roomName, RoomInfo info)
    {
        if (info.PlayerCount == info.MaxPlayers)
        {
            fullRoomPanel.SetActive(true);
            return;
        }

        NetworkManager.instance.JoinRoom(roomName);
        roomBrowserScreen.SetActive(false);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomButtonsList();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        roomInfoList.Clear();
        ClearRoomButtonsList();
    }

    public override void OnLeftLobby()
    {
        roomInfoList.Clear();
        ClearRoomButtonsList();
    }

    private void ClearRoomButtonsList()
    {
        foreach (GameObject entry in roomButtonsList.Values)
        {
            Destroy(entry.gameObject);
        }

        roomButtonsList.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (roomInfoList.ContainsKey(info.Name))
                {
                    roomInfoList.Remove(info.Name);
                }
                continue;
            }

            if (roomInfoList.ContainsKey(info.Name))
            {
                roomInfoList[info.Name] = info;
            }
            else
            {
                roomInfoList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in roomInfoList.Values)
        {
            GameObject entry = Instantiate(roomButtonPrefab, roomListContainer.transform);
            entry.GetComponent<RoomButton>().SetRoomInfo(info);
            Button buttonEvent = entry.GetComponent<Button>();
            buttonEvent.onClick.AddListener(() => { OnJoinRoomButton(info.Name, info); });
            roomButtonsList.Add(info.Name, entry);
        }
    }

    //ROOM SCREEN
    public override void OnJoinedRoom()
    {
        SetScreen(roomScreen);
        ClearRoomButtonsList();
        photonView.RPC("UpdateRoomInformationsUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInformationsUI();
    }

    [PunRPC]
    public void UpdateRoomInformationsUI()
    {
        foreach (GameObject item in playerNickObjectsList)
        {
            Destroy(item);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject obj = PopulateObjectList(playerNickPrefab, playerNickContainer, playerNickObjectsList);
            obj.GetComponent<TextMeshProUGUI>().text = player.NickName;
        }

        roomInfoText.text = PhotonNetwork.CurrentRoom.Name;
        roomPlayerCounterText.text = "(" + PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString() + ")";

        startGameButton.interactable = PhotonNetwork.IsMasterClient;
        nextMissionButton.interactable = PhotonNetwork.IsMasterClient;
        previousMissionButton.interactable = PhotonNetwork.IsMasterClient;

        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("ChangeMission", RpcTarget.All, index);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        disconectedFromServerPanel.SetActive(true);
        disconectedFromServerPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Desconectado do Servidor: Erro: " + cause.ToString();
    }

    public void OnLeaveLobbyButton()
    {
        NetworkManager.instance.LeaveRoom();
        SetScreen(lobbyScreen);
    }

    public void ChangeMissionButton(int value)
    {
        index += value;

        if (index >= missions.Length)
            index = 0;
        else if (index < 0)
            index = missions.Length - 1;

        photonView.RPC("ChangeMission", RpcTarget.All, index);
    }

    [PunRPC]
    public void ChangeMission(int _index)
    {
        index = _index;
        missionImage.sprite = Resources.Load<Sprite>("MissionsImage/" + missions[_index]);
        missionText.text = missions[_index];
    }

    public void OnStartGameButton()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, missions[index]);
    }
}
