using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;


public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D rigid;
    public Animator anim;
    public SpriteRenderer sr;
    public PhotonView pv;
    public Text NickNameText;
    public Image HealthImage;

    bool isWalk;
    bool isGround;
    Vector3 curPos;

    void Awake()
    {
        //닉네임
        NickNameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        NickNameText.color = pv.IsMine ? Color.green : Color.red;

        if (pv.IsMine)
        {
            //2D 카메라
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }
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
                isWalk = true;
                pv.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);    //재접속시 filpx를 동기화 시켜주기 위해서 AllBuffed
            }
            else isWalk = false;

            anim.SetBool("walk", isWalk);


            //점프, 바닥체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.5f, 1 << LayerMask.NameToLayer("Ground"));

            anim.SetBool("jump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround)
            {
                pv.RPC("JumpRPC", RpcTarget.All);
            }

            //총알발사
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(sr.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity)
                    .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, sr.flipX ? -1 : 1);
                anim.SetTrigger("shot");
            }
        }

        //IsMine이 아닌 것들은 부드럽게 위치 동기화
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);

    }

    /// <summary>
    /// 플레이어의 이동 방향에 따라 스프라이트를 뒤집는다
    /// </summary>
    /// <param name="axis">이동 방향</param>
    [PunRPC]
    void FlipXRPC(float axis) => sr.flipX = axis == -1;

    /// <summary>
    /// 점프
    /// </summary>
    [PunRPC]
    void JumpRPC()
    {
        isWalk = false;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 700);
    }

    /// <summary>
    /// 총알과 충돌 했을 때
    /// </summary>
    public void Hit()
    {
        HealthImage.fillAmount -= 0.1f;
        if (HealthImage.fillAmount <= 0 )
        {
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            pv.RPC("DestroyRPC", RpcTarget.AllBuffered);    //AllBuffered로 해야 제대로 사라져 복제 버그가 안 생긴다
        }
    }

    /// <summary>
    /// 플레이어를 삭제 시킬때
    /// </summary>
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    /// <summary>
    /// 플레이어의 위치와 채력상태를 동기화
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("DeadLine")) //맵 밖으로 나갔을 때
        {
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            pv.RPC("DestroyRPC",RpcTarget.AllBuffered);
        }
    }

}
