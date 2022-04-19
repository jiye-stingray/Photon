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
            // 이동
            float axis = Input.GetAxisRaw("Horizontal");
            rigid.velocity = new Vector2(4 * axis, rigid.velocity.y);

            if (axis != 0)
            {
                anim.SetBool("walk", true);
                pv.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);    //재접속시 filpx를 동기화 시켜주기 위해서 AllBuffed
            }
            else anim.SetBool("walk", false);

            //점프, 바닥체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
            anim.SetBool("jump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow)&& isGround)
            {
                pv.RPC("JumpRPC", RpcTarget.All);
            }

            //총알발사
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(sr.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity);
                anim.SetTrigger("shot");
            }
        }
    }

    [PunRPC]
    void FlipXRPC(float axis) => sr.flipX = axis == -1;

    [PunRPC]
    void JumpRPC()
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 700);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


}
