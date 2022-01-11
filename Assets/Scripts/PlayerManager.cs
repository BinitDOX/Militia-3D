using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour
{

    [SyncVar]
    private bool isDead = false;
    public bool isDeadpub
    {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    public float maxHealth = 100f;

    //[SyncVar]
    //private float currentHealth;

    [SyncVar]
    public float currentHealth;

    //[SyncVar]
    //public string userName;
    public string userName = "Loading...";

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SerializeField]
    private GameObject[] disableExoSuit;
    private Rigidbody rb;

    public Animator anime;
    private bool animeFlag = false;

    [SerializeField]
    private GameObject spawnEff;

    private bool isFirstSetup = true;

    public void PlayerSetup()
    {
        //currentHealth = 100f;
        //maxHealth = 100f;
        rb = GetComponent<Rigidbody>();

        //for lobby
        if (isFirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            CmdHealth();
            isFirstSetup = false;
            //currentHealth = 100f;
        }

            SetDefaults();

        //doing the above using cmd broadcast
        if (isLocalPlayer)
        {
            //Switch cam
            GameManager.instance.SetSceneCamActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        //CmdBroadCastNewPlayerSetup();
    }

    [Command]
    void CmdHealth()
    {
        currentHealth = maxHealth;
        RpcHealth();
    }

    [ClientRpc]
    void RpcHealth()
    {
        currentHealth = maxHealth;
    }

    [Command]
    void CmdBroadCastNewPlayerSetup()
    {
        RpcBroadCastNewPlayerSetup();
    }

    [ClientRpc]
    void RpcBroadCastNewPlayerSetup()
    {
        if (isFirstSetup)
        {
            rb = GetComponent<Rigidbody>();
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            Debug.Log("set up");
            isFirstSetup = false;
        }

        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount, string sourceID)
    {
        rb = GetComponent<Rigidbody>();
        if (isDead)
            return;

        currentHealth -= amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health");

        if(currentHealth < 80f && currentHealth > 30f && !animeFlag)
        {
            anime.Play("DAMAGED00", -1, 0f);
            animeFlag = true;
        }
        if (currentHealth < 30f && animeFlag)
        {
            anime.Play("DAMAGED00", -1, 0f);
            animeFlag = false;
        }

        if (currentHealth <= 0)
        {
            Die(sourceID);
            anime.SetFloat("FVelo", 0f);
            anime.SetFloat("SVelo", 0f);
            anime.SetBool("Jump", false);
            anime.SetBool("Run", false);
            anime.Play("DAMAGED01", -1, 0f);
        }
    }

    private void Die(string sourceID)
    {
        rb.isKinematic = true;
        isDead = true;

        PlayerManager sourcePlayer = GameManager.GetPlayer(sourceID);
        if(sourcePlayer != null && sourcePlayer != this)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(userName, sourcePlayer.userName);
        }
        if(sourcePlayer == this)
        {
            GameManager.instance.onPlayerKilledCallback.Invoke(userName, sourcePlayer.userName);
        }

        deaths++;

        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //Disable exo
        for (int i = 0; i < disableExoSuit.Length; i++)
        {
            disableExoSuit[i].SetActive(false);
        }
        //Disable colliders //yup its needed
        SetAllCollidersStatus(false);

        //Omitting bcz unneeded
        //Collider col = GetComponent<Collider>(); 
        //if (col != null)
        //    col.enabled = false;

        Debug.Log(transform.name + " is dead!");

        PlayerShoot ps = GetComponent<PlayerShoot>();
        ps.CancelInvoke("Shoot");
        ps.CancelInvoke("Shoot2");


        PlayerShootJoy ps2 = GetComponent<PlayerShootJoy>();
        ps2.CancelInvoke("Shoot");
        ps2.CancelInvoke("Shoot2");
        ps2.fire1 = false;
        ps2.fire2 = false;

        //Switch cam
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCamActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        //Call respawn
        StartCoroutine("Respawn");
    }

    private IEnumerator Respawn()
    {

        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        //SetDefaults();
        anime.Play("WAIT04", -1, 0f);
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;

        yield return new WaitForSeconds(0.1f);

        //SetDefaults();
        PlayerSetup();

        Debug.Log(transform.name + " respawned");
    }

    public void SetDefaults()
    {
        rb.isKinematic = false;
        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        for (int i = 0; i < disableExoSuit.Length; i++)
        {
            disableExoSuit[i].SetActive(true);
        }
        SetAllCollidersStatus(true);
        //Collider component is kinda a special case behavior
        //Collider col = GetComponent<Collider>();
        //if (col != null)
        //    col.enabled = true;

        //if (isLocalPlayer)
        //{
        //    GameManager.instance.SetSceneCamActive(false);
        //    GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        //}

        //SpawnEff
        GameObject gfx = (GameObject)Instantiate(spawnEff, transform.position, Quaternion.identity);
        Destroy(gfx, 5f);
    }

    void SetAllCollidersStatus(bool active)
    {
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = active;
        }
    }

    //for testing:
    void Update()
    {
        if (!isLocalPlayer)
            return;
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    RpcTakeDamage(9f);
        //}

        if(anime.GetFloat("FVelo") > 0.2f || anime.GetFloat("FVelo") < -0.2f || anime.GetFloat("SVelo") > 0.2f || anime.GetFloat("SVelo") < -0.2f || anime.GetBool("Jump"))
        {
            anime.Play("WALK");
        }

    }

}
