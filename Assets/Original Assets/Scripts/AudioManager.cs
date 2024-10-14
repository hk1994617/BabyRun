using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip collisionSound;
    private AudioSource audioSource;

    public string[] targetTags; // Массив тегов, с которыми должно происходить воспроизведение звука

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, есть ли у объекта, с которым происходит столкновение, подходящий тег
        foreach (string targetTag in targetTags)
        {
            if (collision.gameObject.CompareTag(targetTag))
            {
                if (collisionSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(collisionSound);
                }
                break; // Выходим из цикла, если нашли подходящий тег
            }
        }
    }
}
