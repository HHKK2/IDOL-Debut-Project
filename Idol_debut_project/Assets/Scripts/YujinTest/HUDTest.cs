using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDTest : UIHUD
{
     
    enum Buttons
    {
        NextSongButton,
        StopSongButton
    }
    
    [SerializeField] private AudioSource[] albumAudioSources;
    private int songIndex = 0;

    private void Start()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
      
        Button NextSongButton = Get<Button>((int)Buttons.NextSongButton);
        BindEvent(NextSongButton.gameObject,OnClickedNextSongButton,GameEvents.UIEvent.Click);
        Button StopSongButton = Get<Button>((int)Buttons.StopSongButton);
        BindEvent(StopSongButton.gameObject,OnClickedStopSongButton,GameEvents.UIEvent.Click);
    }

    private void OnClickedNextSongButton(PointerEventData eventData)
    {
        SoundManager.Instance.AlbumPlay(albumAudioSources[songIndex++].clip);
        if (songIndex > albumAudioSources.Length - 1)
        {
            songIndex = 0;
        }
    }
    
    private void OnClickedStopSongButton(PointerEventData eventData)
    {
        SoundManager.Instance.AlbumStop();
    }
}
