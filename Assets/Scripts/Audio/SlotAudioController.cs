using UnityEngine;

/// <summary>
/// Central audio playback controller for the slot machine.
/// Holds references to all audio clips and exposes simple methods for
/// gameplay scripts to trigger sounds. No singleton — wired via Inspector/BuildScene.
/// </summary>
public class SlotAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip slotMachineClip;
    [SerializeField] private AudioClip leverClip;
    [SerializeField] private AudioClip winJackpotClip;

    private AudioSource audioSource;

    private void Awake()
    {
        // The AudioSource is configured by BuildScene with playOnAwake = false
        // and loop = false. We grab the reference here for runtime use.
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays the lever pull sound once. Does not interrupt itself if called
    /// multiple times in rapid succession — PlayOneShot queues overlapping sounds.
    /// </summary>
    public void PlayLeverSound()
    {
        if (leverClip != null)
            audioSource.PlayOneShot(leverClip);
    }

    /// <summary>
    /// Starts the looping reel-spin sound. Guards against starting a second
    /// copy if Spin() is somehow called while already playing.
    /// </summary>
    public void StartSpinSound()
    {
        if (slotMachineClip == null)
            return;

        if (audioSource.isPlaying && audioSource.clip == slotMachineClip)
            return;

        audioSource.clip = slotMachineClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// Stops the looping reel-spin sound immediately.
    /// </summary>
    public void StopSpinSound()
    {
        if (audioSource.clip == slotMachineClip && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    /// <summary>
    /// Plays the win/jackpot sound once. Stops any currently-playing spin sound
    /// first so the two do not overlap.
    /// </summary>
    public void PlayWinSound()
    {
        StopSpinSound();

        if (winJackpotClip != null)
            audioSource.PlayOneShot(winJackpotClip);
    }

    /// <summary>
    /// Returns true while the win/jackpot sound is still playing.
    /// After StopSpinSound clears the clip, only PlayOneShot sounds remain —
    /// so audioSource.isPlaying reliably indicates win sound playback.
    /// </summary>
    public bool IsWinSoundPlaying()
    {
        return audioSource.isPlaying;
    }
}
