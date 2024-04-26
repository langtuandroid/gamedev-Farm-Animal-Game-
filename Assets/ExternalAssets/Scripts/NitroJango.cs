using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroJango : MonoBehaviour
{

    private Rigidbody rigid;
    private float topspeedCache;
    public bool usingNitro;
    
    //Nitro
    [Range(0, 10)]
    public float nitroStrength = 5f;
    [Range(0, 50)]
    public float addedNitroSpeed = 25;
    public float nitroRegenerationRate = 0.1f;
    public float nitroDepletionRate = 0.25f;
    public float nitroCapacity = 1.0f;
    public float nitroAddedSpeed = 20.0f;
    [SerializeField] private float nitroTopSpeed;
    public GameObject nitroGroup;
    
    
    //Sounds
    public AudioSource nitroAudioSource;
    public AudioClip nitroSound;
    
    // Start is called before the first frame update
    void Start()
    {
        nitroAudioSource.clip = nitroSound;
        rigid = GetComponent<Rigidbody>();
        topspeedCache = GetComponent<RCC_CarMainControllerV3>().maxspeed;
        if (nitroGroup)
        {
            nitroGroup.SetActive(true);

            foreach(Transform p in nitroGroup.transform)
            {
                if (p.GetComponent<ParticleSystem>())
                {
                    var em = p.GetComponent<ParticleSystem>().emission;
                    em.enabled = false;
                }                       
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Nitro();
    }

    void Nitro()
    {
        if (usingNitro && nitroCapacity > 0f && GetComponent<RCC_CarMainControllerV3>().speed > 0)
        {

            //increase top speed
            GetComponent<RCC_CarMainControllerV3>().maxspeed = nitroTopSpeed;

            //deplete nitro
            nitroCapacity = Mathf.MoveTowards(nitroCapacity, 0, nitroDepletionRate * Time.deltaTime);

            //add nitro boost
            rigid.AddForce(transform.forward * nitroStrength,ForceMode.Acceleration);

            //handle sound
            if (nitroAudioSource)
            {
                if (!nitroAudioSource.isPlaying)
                {
                    nitroAudioSource.Play();
                }

                nitroAudioSource.volume = Mathf.Lerp(nitroAudioSource.volume, 1.0f, Time.deltaTime * 2f);
                nitroAudioSource.pitch = Mathf.Lerp(nitroAudioSource.pitch, 1.5f, Time.deltaTime * 2f);
            }

            //activate nitro
            if (nitroGroup)
            {
                foreach (Transform p in nitroGroup.transform)
                {
                    if (p.GetComponent<ParticleSystem>())
                    {
                        p.GetComponent<ParticleSystem>().Emit(1);
                    }
                }
            }
        }
        else
        {
            //handle sound
            if (nitroAudioSource)
            {
                nitroAudioSource.volume = Mathf.Lerp(nitroAudioSource.volume, 0.0f, Time.deltaTime * 2f);
                nitroAudioSource.pitch = Mathf.Lerp(nitroAudioSource.pitch, 1.0f, Time.deltaTime * 2f);
            }

            //reset top speed
            GetComponent<RCC_CarMainControllerV3>().maxspeed = topspeedCache;

            //recharge nitro
            if (!usingNitro && nitroRegenerationRate > 0)
                nitroCapacity = Mathf.MoveTowards(nitroCapacity, 1, nitroRegenerationRate * Time.deltaTime);
        }
    }
}
