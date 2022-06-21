using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Bullet : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    int dir;

    AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        audio.Play();
        Destroy(gameObject, 3.5f);  //3.5�� �Ŀ� �Ѿ� ����

    }

    void Update() => transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);

    void OnTriggerEnter2D(Collider2D col) //col�� RPC�� �Ű������� �־��� �� �ִ�
    {
        if (col.gameObject.CompareTag("Ground")) pv.RPC("DestroyRPC", RpcTarget.AllBuffered);   //���� �浹�� �Ѿ� �ı�
        if(!pv.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine)  //�����ʿ� ���� Hit����
        {
            col.GetComponent<Player>().Hit();
            pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DirRPC(int dir) => this.dir = dir;

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
