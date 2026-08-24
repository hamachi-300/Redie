using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [Header("BGM Settings")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool loop = true;

    private AudioSource audioSource;

    void Start()
    {
        if (bgmClip == null)
        {
            Debug.LogWarning("SceneBGM: ยังไม่ได้ใส่ไฟล์เพลง BGM ในช่อง Inspector!");
            return;
        }

        // ค้นหา AudioSource บน Camera หากไม่มีจะสร้างให้อัตโนมัติ
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ตั้งค่าการเล่นเพลง
        audioSource.clip = bgmClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.spatialBlend = 0f; // ตั้งค่าเป็น 2D Sound เสียงดังเท่ากันทั่วทั้งฉาก
        audioSource.Play();
    }
}