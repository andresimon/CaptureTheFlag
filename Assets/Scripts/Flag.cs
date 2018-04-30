using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Flag : NetworkBehaviour {

    public enum State
    {
        Available,
        Dropped,
        Possessed
    };

    public float droppedTime = 1.0f;

    private float m_droppedTimer = 1.0f;

    [SyncVar]
    State m_state;

    public State GetState() {
        return m_state;
    }

    // Use this for initialization
    void Start () {
        //Vector3 spawnPoint;
        //ObjectSpawner.RandomPoint(this.transform.position, 10.0f, out spawnPoint);
        //this.transform.position = spawnPoint;
        //GetComponent<MeshRenderer> ().enabled = false;
        m_state = State.Available;
        m_droppedTimer = droppedTime;

    }

    [ClientRpc]
    public void RpcPickUpFlag(GameObject player)
    {
        AttachFlagToGameObject(player);
    }

    [Command]
    public void CmdDropFlag()
    {
        //this.transform.parent = null;
        m_state = State.Dropped;
       // this.transform.position = new Vector3(0f, 4f, 0f);
    }

    public void AttachFlagToGameObject(GameObject obj)
    {
        PlayerController101 pc = obj.GetComponent<PlayerController101>();
        if (pc) {
            this.transform.parent = obj.transform;
            pc.CmdPickUpFlag ();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(!isServer || other.tag != "Player")
        {
            return;
        }

        //make this player drop the flag, start a cooldown for pickup
        if (m_state == State.Available) {

            m_state = State.Possessed;
            AttachFlagToGameObject (other.gameObject);
            RpcPickUpFlag (other.gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
        if (m_state == State.Dropped)
        {
            this.transform.parent = null;
        }

        if (!isServer) {    
            return;
        }

        if (m_state == State.Dropped) {
            this.transform.parent = null;
            m_droppedTimer -= Time.deltaTime;
            if (m_droppedTimer < 0.0f) {
                m_state = State.Available;
                m_droppedTimer = droppedTime;
            }
        }
    }
}


























/*


    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(handlePickup());
        other.GetComponent<HealthAndDamage>().health += 10;
    }

    [Server]
    IEnumerator handlePickup()
    {
        visible = false;
        yield return new WaitForSeconds(15);
        visible = true;
    }
}

public class HealthPack : NetworkBehaviour {
    [SyncVar(hook = "OnVisibleChanged")]
    bool visible;

    public override void OnStartServer ()
    {
        visible = true;
    }

    public override void OnStartClient ()
    {
        OnVisibleChanged(visible);
    }

   

    void OnVisibleChanged(bool newValue)
    {
        GetComponent<Renderer>().enabled = newValue;
        GetComponent<Collider>().enabled = newValue;
    }

*/