using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(AudioSource))]
public class TriggerZone : MonoBehaviour
{
    public Volume triggerVolume;
    public AudioClip breathingSoundEffect;
    public int loopCount = 1;
    private AudioSource audioSource;
    public AudioSource tunnelAudioSource;
    private RigidbodyFirstPersonController player;
    private float backwardSpeed;
    private float forwardSpeed;
    private float strafeSpeed;
    private float runMultiplier;
    private float jumpForce;
    public float vignetteFirstStep = 0.7f;
    public float vignetteSecondStep = 1.0f;
    public int numberOfCycles = 3;
    public float interpolateInterval = 0.2f;
    public float secondsToComplete = 1.5f; 
    private void Awake()
    {
        player = GetComponent<RigidbodyFirstPersonController>();
        audioSource = GetComponent<AudioSource>();
        CacheOriginalValues();
    }


    private void SlowDownPlayer()
    {
        player.movementSettings.BackwardSpeed = 2f;
        player.movementSettings.ForwardSpeed = 2f;
        player.movementSettings.StrafeSpeed = 1f;
        player.movementSettings.RunMultiplier = 0.1f;
        player.movementSettings.JumpForce = 0.5f;
        audioSource.volume = 0.01f;
        audioSource.Play();
        tunnelAudioSource.volume = 0.01f;
        tunnelAudioSource.Play();
      UnityEngine.Rendering.Universal.Vignette vignette;
      
        if (triggerVolume.profile.TryGet<UnityEngine.Rendering.Universal.Vignette>(out vignette))
        {
            StartCoroutine(ManageVignette(vignette, numberOfCycles));
        }
    }

    private IEnumerator ManageVignette(UnityEngine.Rendering.Universal.Vignette vig, int cycle)
    {
       
      var timer = 0.0f;
        
        for (int i = 0; i < 120; i++)
        {

            timer += vignetteFirstStep / 120.0f;
            vig.intensity.Interp(0, vignetteFirstStep,timer);
            yield return new WaitForSeconds(vignetteFirstStep / 120.0f);
        }

        for (int i = 0; i < cycle; i++)
        {
            timer = vignetteFirstStep;
            for (int j = 0; j < 120; j++)
            {
                
                timer += (vignetteSecondStep - vignetteFirstStep) / 120.0f;
                vig.intensity.value = timer;
                yield return new WaitForSeconds((vignetteSecondStep - vignetteFirstStep) / 120.0f);
                
            }
            timer = vignetteSecondStep;
            for (int k = 0; k < 120; k++)
            {
                
                timer -= (vignetteSecondStep - vignetteFirstStep) / 120f;
                
                vig.intensity.value = timer;
                yield return new WaitForSeconds((vignetteSecondStep - vignetteFirstStep) / 120.0f);
            }
        }

        vig.intensity.value = 0.0f;

    }

    private void CacheOriginalValues()
    {
      backwardSpeed =  player.movementSettings.BackwardSpeed;
     forwardSpeed =   player.movementSettings.ForwardSpeed;
       strafeSpeed = player.movementSettings.StrafeSpeed;
       runMultiplier = player.movementSettings.RunMultiplier;
       jumpForce = player.movementSettings.JumpForce;
        
    }
    private void SetOriginalValues()
    {
        player.movementSettings.BackwardSpeed = backwardSpeed;
        player.movementSettings.ForwardSpeed = forwardSpeed;
        player.movementSettings.StrafeSpeed = strafeSpeed;
        player.movementSettings.RunMultiplier = runMultiplier;
                    player.movementSettings.JumpForce = jumpForce;
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zone"))
        {
            audioSource.clip = breathingSoundEffect;
            StartCoroutine(WaitUntilClipIsOver(loopCount));
        }
        
       
    }
    IEnumerator WaitUntilClipIsOver(int loop)
    {
        var timer = breathingSoundEffect.length;
        for (int i = 0; i < loop; i++)
        {
            
            SlowDownPlayer();
            yield return new WaitForSeconds(timer);
        }

        SetOriginalValues();
    }

}
