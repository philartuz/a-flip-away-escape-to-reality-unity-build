using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("---- Audio Sources ----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource footstepsSource;
    [SerializeField] AudioSource pushSource;
    [SerializeField] AudioSource pullSource;
    [SerializeField] AudioSource windSource;
    [SerializeField] AudioSource dialogueSource;

    [Header("---- Actual Audio: General  ----")]
    public AudioClip mainMenu;
    public AudioClip background;
    public AudioClip goal;
    public AudioClip death;

    [Header("---- Actual Audio: UI  ----")]
    public AudioClip mouseClick;
    public AudioClip mouseBack;
    public AudioClip mouseHover;

    [Header("---- Actual Audio: Player Actions  ----")]
    public AudioClip footsteps;
    public AudioClip jump;
    public AudioClip landing; 
    public AudioClip pushing;
    public AudioClip pulling;

    [Header("---- Actual Audio: Environment  ----")]
    public AudioClip breakable;
    public AudioClip wind;
    public AudioClip water;
    public AudioClip dialogue;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "World5_0_A" || SceneManager.GetActiveScene().name == "World5_1_A") {

            PlaySFX(water, 0.05f);
        
        }

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            musicSource.clip = mainMenu;
            musicSource.loop = true;
            musicSource.volume = 0.08f;
            musicSource.Play();
        }

        else {
            musicSource.clip = background;
            musicSource.loop = true;
            musicSource.volume = 0.02f;
            musicSource.Play();
        }
        
    }

    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        sfxSource.PlayOneShot(clip, volume);

        if (clip == water) {
            sfxSource.loop = true; 
        }

    }

    #region UI Sound Methods
    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(mouseClick, 0.5f);
    }

    public void PlayButtonHover()
    {
        sfxSource.PlayOneShot(mouseHover, 0.1f);

    }

    public void PlayButtonBack()
    {
        sfxSource.PlayOneShot(mouseBack, 0.8f);

    }

    #endregion

    #region Movement Methods that Loop
    public void StartFootsteps()
    {
        if (!footstepsSource.isPlaying)
        {
            footstepsSource.clip = footsteps;
            footstepsSource.loop = true;
            footstepsSource.Play();
        }
    }

    public void StopFootsteps()
    {
        if (footstepsSource.isPlaying)
        {
            footstepsSource.Stop();
        }
    }

    public void StartPushing()
    {
        if (!pushSource.isPlaying)
        {
            pushSource.clip = pushing;
            pushSource.loop = true;
            pushSource.Play();
        }
    }

    public void StopPushing()
    {
        if (pushSource.isPlaying)
        {
            pushSource.Stop();
        }
    }

    public void StartPulling()
    {
        if (!pullSource.isPlaying)
        {
            pullSource.clip = pulling;
            pullSource.loop = true;
            pullSource.Play();
        }
    }

    public void StopPulling()
    {
        if (pullSource.isPlaying)
        {
            pullSource.Stop();
        }
    }
    #endregion

    public void StartWind()
    {
        if (!windSource.isPlaying)
        {
            windSource.clip = wind;
            windSource.loop = true;
            windSource.Play();
        }
    }

    public void StopWind()
    {
        if (windSource.isPlaying)
        {
            windSource.Stop();
        }
    }

    public void StartDialogue()
    {
        if (!dialogueSource.isPlaying)
        {
            dialogueSource.clip = dialogue;
            dialogueSource.loop = true;
            dialogueSource.Play();
        }
    }

    public void StopDialogue()
    {
        if (dialogueSource.isPlaying)
        {
            dialogueSource.Stop();
        }
    }





}
