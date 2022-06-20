using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviourPunCallbacks
{

    public InputField NickNameInput;    //이름
    public GameObject DisconnectPanel;  //접속 해제 상태 때 UI
    public GameObject RespawnPanel;     //접속 할때의 UI

    void Awake()
    {
        ///초기화
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    /// <summary>
    /// 접속
    /// </summary>
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    /// <summary>
    /// 방장 접속
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    /// <summary>
    /// 방에 참여
    /// </summary>
    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
        StartCoroutine("DestroyBullet");
        Spawn();
    }

    /// <summary>
    /// 총알 삭제
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject Go in GameObject.FindGameObjectsWithTag("Bullet")) Go.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
    }

    /// <summary>
    /// 플레이어 스폰
    /// </summary>
    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f,19f),4,0), Quaternion.identity);
        RespawnPanel.SetActive(false);
    }

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();  //ESC키를 눌렀을 시 접속 해제
    }
    
    /// <summary>
    /// 접속 해제 함수
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }
}
