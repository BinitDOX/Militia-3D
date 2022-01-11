using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shell : NetworkBehaviour
{
    public LayerMask m_RemotePlayerMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public float m_MaxDamage = 30f;
    public float m_ExplosionForce = 500f;
    public float m_MaxLifeTime = 5f;
    public float m_ExplosionRadius = 5f;

    private Rigidbody rb;

    public string launchedBy;

    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
        rb = GetComponent<Rigidbody>();
        //rb.centerOfMass = new Vector3(0f, 0f, 0.35f);
        //rb.SetDensity(1);
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (!isLocalPlayer)
        //{
        //    return;
        //}

        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_RemotePlayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidBody)
                continue;

            targetRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth>();
            //if (!targetHealth)
            //    continue;
            PlayerManager pm = targetRigidBody.GetComponent<PlayerManager>();

            float damage = CalculateDamage(targetRigidBody.position);

            pm.RpcTakeDamage(damage, launchedBy);
            //Explode(targetRigidBody.name, damage);

            
        }

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }

    //[Client]
    //void Explode(string name, float dm)
    //{
    //    gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    //    CmdPlayerExplo(name, dm);
    //}

    //[Command]
    //void CmdPlayerExplo(string playerID, float damage)
    //{
    //    Debug.LogError(playerID + " has been exploded!");

    //    PlayerManager player = GameManager.GetPlayer(playerID);
    //    player.RpcTakeDamage(damage);
    //}

    void Update()
    {
        transform.LookAt(transform.position + rb.velocity);
    }
}
