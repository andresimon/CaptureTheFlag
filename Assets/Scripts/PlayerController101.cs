using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomMsgType
{
    public static short Transform = MsgType.Highest + 1;
};

public class PlayerController101 : NetworkBehaviour
{
    public float m_linearSpeed = 5.0f;
    public float m_angularSpeed = 3.0f;
	public float m_jumpSpeed = 5.0f;

    private Rigidbody m_rb = null;

    [SyncVar(hook = "OnPlayerIDChanged")] public string playerID;
    [SyncVar(hook = "OnPlayerNumChanged")] public int playerNum;

    Camera playerCam;
    Transform labelHolder;

  
    [SyncVar]
    public Color color;
    [SyncVar]
    public string playerName;

    [SyncVar]
    public bool m_hasFlag = false;

    public bool HasFlag() {
        return m_hasFlag;
    }


    [Command]
    public void CmdPickUpFlag()
    {
        m_hasFlag = true;
    }

    [Command]
    public void CmdDropFlag()
    {
        m_hasFlag = false;
    }


    bool IsHost()
    {
        return isServer && isLocalPlayer;
    }

    void Awake()
    {
        playerCam = GetComponentInChildren<Camera>();
        playerCam.gameObject.SetActive(false);

        labelHolder = transform.Find("LabelHolder");
    }

    // Use this for initialization
    void Start () {
        m_rb = GetComponent<Rigidbody>();
       Debug.Log("Start()");
        Vector3 spawnPoint;
      //  ObjectSpawner.RandomPoint(this.transform.position, 10.0f, out spawnPoint);
      //  this.transform.position = spawnPoint;

      Renderer rends = GetComponentInParent<Renderer>();
       
       // /Renderer[] rends = GetComponentsInChildren<Renderer>();
      //  foreach (Renderer r in rends)
          rends.material.color = color;
        


	}

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        //Debug.Log("OnStartAuthority()");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Debug.Log("OnStartClient()");
        OnPlayerIDChanged(playerID);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
      Debug.Log("OnStartLocalPlayer()");
        GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 0.0f);

        CmdGetPlayerNum();
        Debug.Log("OI: " + playerNum);
        //    string myPlayerID = string.Format("Player {0}", netId.Value);
        string myPlayerID = string.Format("Player {0}", playerNum);
        CmdSetPlayerID(myPlayerID);
      //  CmdSetPlayerNum(playerNum);

       // playerCam.gameObject.SetActive(true);
    }

    [Command]
    public void CmdGetPlayerNum()
    {
        playerNum = CTFGameManager.AssignPlayerNum();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer()");
    }

    public void Jump()
    {
		Vector3 jumpVelocity = Vector3.up * m_jumpSpeed;
        m_rb.velocity += jumpVelocity;
        TrailRenderer tr = GetComponent<TrailRenderer>();
        tr.enabled = true;
    }

    [ClientRpc]
    public void RpcJump()
    {
        Jump();
    }

    [Command]
    public void CmdJump()
    {
        Jump();
        RpcJump();
    }

    void Update () 
    {
        if(!isLocalPlayer) { return; }

        playerCam.transform.rotation = Quaternion.Euler(new Vector3(70,0,0));

        labelHolder.rotation = Quaternion.identity;

        float rotationInput = Input.GetAxis("Horizontal");
        float forwardInput = Input.GetAxis("Vertical");

        Vector3 linearVelocity = this.transform.forward * (forwardInput * m_linearSpeed) * (HasFlag()?0.5f:1f);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdJump();
        }

        float yVelocity = m_rb.velocity.y;


        linearVelocity.y = yVelocity;
        m_rb.velocity = linearVelocity;

        Vector3 angularVelocity = this.transform.up * (rotationInput * m_angularSpeed);
        m_rb.angularVelocity = angularVelocity;
    }

    [Command]
    void CmdSetPlayerID(string newID)
    {
        playerID = newID;
    }

    void OnPlayerIDChanged(string newValue)
    {
        playerID = newValue;
        var textMesh = labelHolder.Find("Label").GetComponent<TextMesh>();
        textMesh.text = newValue;
    }

    [Command]
    void CmdSetPlayerNum(int num)
    {
        playerNum = num;
    }

    void OnPlayerNumChanged(int newValue)
    {
        playerNum = newValue;
        var textMesh = labelHolder.Find("Label").GetComponent<TextMesh>();
        textMesh.text = newValue.ToString();
    }

    [Command]
    public void CmdPlayerDropFlag()
    {
        Transform childTran = this.transform.GetChild(this.transform.childCount - 1);
        Flag flag = childTran.gameObject.GetComponent<Flag>();
        if (flag) {
            flag.CmdDropFlag();
            CmdDropFlag();
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if(!isLocalPlayer || other.collider.tag != "Player")
        {
            return;
        }

        if (HasFlag()) 
        {
            Transform childTran = this.transform.GetChild (this.transform.childCount - 1);
            if (childTran.gameObject.tag == "Flag") 
            {
                CmdPlayerDropFlag();
            }
        }
    }
}
    