using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Bullet : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    int dir;

    void Start() => Destroy(gameObject, 3.5f);  //3.5초 후에 총알 삭제

    void Update() => transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);

    void OnTriggerEnter2D(Collider2D col) //col을 RPC의 매개변수로 넣어줄 수 있다
    {
        if (col.gameObject.CompareTag("Ground")) pv.RPC("DestroyRPC", RpcTarget.AllBuffered);   //땅에 충돌시 총알 파괴
        if(!pv.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine)  //느린쪽에 맞춰 Hit판정
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
