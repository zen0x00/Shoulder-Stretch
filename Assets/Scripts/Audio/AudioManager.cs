using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]private AudioSource audioSource;
    [SerializeField]private AudioClip GunShot;
    [SerializeField]private AudioClip ButtonClick;
    [SerializeField]private AudioClip ZombieDead;
    [SerializeField]private AudioClip WavesSound;
    [SerializeField] private AudioClip playerHurtSound;
    public void PlayGunShot() { if (audioSource && GunShot) audioSource.PlayOneShot(GunShot); }
    public void PlayButtonClick() { if (audioSource && ButtonClick) audioSource.PlayOneShot(ButtonClick); }
    public void PlayZombieDead() { if (audioSource && ZombieDead) audioSource.PlayOneShot(ZombieDead); }
    public void PlayWavesSound() { if (audioSource && WavesSound) audioSource.PlayOneShot(WavesSound); }
    public void PlayPlayerDamageTakenSound() { if (audioSource && playerHurtSound) audioSource.PlayOneShot(playerHurtSound); }

}
