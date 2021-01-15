using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{


    public Slider slider;

    float soundCooldown = 0;
    AudioSource loopSound;
    AudioSource BuildSound;
    AudioSource birdSound;

    float overall_volume = 0.5f;
    float volume = 0.5f; 


    // Start is called before the first frame update
    void Start()
    {
        loopSound = gameObject.AddComponent<AudioSource>();
        loopSound.clip = Resources.Load<AudioClip>("Sounds/Footsteps");
        loopSound.loop = true;
        loopSound.pitch = 0.8f;
        BuildSound = gameObject.AddComponent<AudioSource>();
        BuildSound.clip = Resources.Load<AudioClip>("Sounds/Build_Effect");
        birdSound = gameObject.AddComponent<AudioSource>();
        birdSound.loop = true;
        birdSound.clip = Resources.Load<AudioClip>("Sounds/Bird");
        birdSound.Play();
        WorldController.Instance.world.RegisterTileChanged(OnTileChanged);
        WorldController.Instance.world.RegisterInstalledObjectCreated(OnInstalledObjectCreated);
        
    }

    // Update is called once per frame
    void Update()
    {

        if(volume != overall_volume)
        {
            overall_volume = volume;
            loopSound.volume = overall_volume;
            BuildSound.volume = overall_volume;
            birdSound.volume = overall_volume;
        }

        soundCooldown -= Time.deltaTime;

        if (WorldController.Instance.world.ReachtheEnd)
        {
            loopSound.Pause();
        }
        if (WorldController.Instance.world.enemies[0].slowcooldown != 0)
        {
            loopSound.pitch = 0.5f;
        }
        else
        {
            loopSound.pitch = 0.8f;
        }
    }

    public void OnTileChanged(Tile tile_data)
    {
        if (soundCooldown > 0)
        {
            return;
        }
        BuildSound.Play();
        soundCooldown = 0.1f;
    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        if (soundCooldown > 0)
        {
            return;
        }
        BuildSound.Play();
        soundCooldown = 0.1f;
    }

    public void OnEnemyWalk()
    {
        loopSound.Play(); 
    }

    public void SetVolume()
    {
        volume = slider.value / 10;
    }

}
