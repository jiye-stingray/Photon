using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D rigid;
    public Animator anim;
    public SpriteRenderer sr;
    public PhotonView pv;
    public Text NickNameText;
    public Image HealthImage;

    bool isGround;
    Vector3 curPos;

    void Awake()
    {
        //닉네임
        NickNameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        NickNameText.color = pv.IsMine ? Color.green : Color.red;
    }
    void Update()
    {
        if (pv.IsMine)
        {
            // <- -> 이동
            float axis = Input.GetAxisRaw("Horizontal");
            rigid.velocity = new Vector2(4 * axis, rigid.velocity.y);

            if (axis != 0)
            {
                anim.SetBool("walk", true);
                pv.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);    //재접속시 filpx를 동기화 시켜주기 위해서 AllBuffed
            }
            else anim.SetBool("walk", false);
        }
    }

    [PunRPC]
    void FlipXRPC(float axis) => sr.flipX = axis == -1;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


}
