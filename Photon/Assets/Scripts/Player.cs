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
        //�г���
        NickNameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        NickNameText.color = pv.IsMine ? Color.red : Color.green;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
