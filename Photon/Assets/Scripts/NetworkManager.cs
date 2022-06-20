using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviourPunCallbacks
{

    public InputField NickNameInput;    //�̸�
    public GameObject DisconnectPanel;  //���� ���� ���� �� UI
    public GameObject RespawnPanel;     //���� �Ҷ��� UI

    void Awake()
    {
        ///�ʱ�ȭ
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    /// <summary>
    /// ���� ����
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    /// <summary>
    /// �濡 ����
    /// </summary>
    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
        StartCoroutine("DestroyBullet");
        Spawn();
    }

    /// <summary>
    /// �Ѿ� ����
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject Go in GameObject.FindGameObjectsWithTag("Bullet")) Go.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
    }

    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f,19f),4,0), Quaternion.identity);
        RespawnPanel.SetActive(false);
    }

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();  //ESCŰ�� ������ �� ���� ����
    }
    
    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }
}
