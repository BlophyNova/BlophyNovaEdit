using UnityEngine;
namespace Controller
{
    public class HitSoundController : MonoBehaviour
    {
        public AudioSource hitSound;
        public HitSoundController SetClip(AudioClip audioClip)
        {
            hitSound.clip = audioClip;
            return this;
        }
        public HitSoundController Play()
        {
            hitSound.Play();
            return this;
        }
    }
}
