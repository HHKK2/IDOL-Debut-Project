using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class SoundManager : AdolpSingleton<SoundManager>
{
    [SerializeField] private AudioMixer mixer;
    [Header("each bg source have to be same as scene name")]
    [SerializeField]private AudioSource[] bgList;
    [SerializeField]private AudioSource bgSound;

    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < bgList.Length; i++)
        {
            if (scene.name == bgList[i].clip.name)
            {
                bgSound = bgList[i];
                BgSoundPlay(bgList[i].clip);
            }   
        }
    }
    
    /// <summary>
    /// 앨범이 바뀔때마다 앨범곡 재생하는 함수. 앨범을 바꿀 때마다 다른 clip삽입하여 호출하기
    /// </summary>
    /// <remarks>다른 곡 재생할 때마다 끄고 킬 필요x(하나의오브젝트에서 재생하는 클립을 갈아끼우는 형태로 구현돼있음)</remarks>
    /// <param name="clip"></param>
    public void AlbumPlay(AudioClip clip)
    {
        GameObject albumGO = GameObject.Find("albumSoundGO");
        if (albumGO == null)
        {
            albumGO = new GameObject("albumSoundGO"); 
        }

        AudioSource audioSource = GameObjectUtils.GetOrAddComponent<AudioSource>(albumGO);
        
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();
    }
    
    /// <summary>
    /// 앨범곡 재생 끄는 함수
    /// </summary>
    /// <remarks>앨범재생 오브젝트를 파괴하고 싶을 떄 호출하기</remarks>
    /// <param name="clip"></param>
    public void AlbumStop()
    {
        GameObject albumGO = GameObject.Find("albumSoundGO");
        if (albumGO != null)
        {
            Destroy(albumGO);
        }
    }
    
    
    /// <summary>
    /// 효과음이 달린 오브젝트에서, 해당 함수를 호출해야함.
    /// </summary>
    /// <param name="albumName"></param>
    /// <param name="clip"></param>
    public void SFXPlay(string albumName, AudioClip clip)
    {
        GameObject go = new GameObject(albumName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length);
    }

    /// <summary>
    /// 환경설정에서 브금 사운드 조절용
    /// 슬라이드 UI의 onValueChanged에 해당 함수를 추가하세요
    /// </summary>
    /// <param name="val"></param>
    public void BGSoundVolume(float val)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(val)*20);
    }
    
    /// <summary>
    /// 환경설정에서 효과음 사운드 조절용
    /// 슬라이드 UI의 onValueChanged에 해당 함수를 추가하세요
    /// </summary>
    /// <param name="val"></param>
    public void SFXSoundVolume(float val)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(val)*20);
    }

    /// <summary>
    /// 브금이 달린 오브젝트(this)에서, 해당 함수를 호출해야함.
    /// </summary>
    /// <param name="clip"></param>
    private void BgSoundPlay(AudioClip clip)
    {
        foreach (var sound in bgList)
        {
            sound.Stop();
        }
        
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.1f;
        bgSound.Play();     
    }

    /// <summary>
    /// BGM 정지 (컴백 씬 등에서 사용)
    /// </summary>
    public void StopBGM()
    {
        if (bgSound != null && bgSound.isPlaying)
        {
            bgSound.Stop();
        }
        
        foreach (var sound in bgList)
        {
            if (sound != null && sound.isPlaying)
            {
                sound.Stop();
            }
        }
    }
}
