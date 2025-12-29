using System;
using UnityEngine;

public class ComebackSceneController : MonoBehaviour
{
    public static event Action OnFinished;

    private enum Phase
    {
        Intro, //실장님 컨셉 설명
        Prepare, //메인메뉴 화면 + 10분 타이머
        Practice, //연습 화면 _ 타이머는 이어짐. 
        Stage // 무대
    }

    private Phase phase;

    private const float PRACTICE_DURATION = 600f;
    private float timer;
    private bool isClicked = false;

    //이번 컴백 송 데이터
    private ComebackSongData currentSong;

    //HUD
    private CombackNoticeHUD noticeHUD;
    private MainMenuHUD mainMenuHUD;
    private TimerHUD timerHUD;
    private PracticeHUD practiceHUD;
    private StageHUD stageHUD;

    private void Start()
    {
        currentSong = GameManager.Instance.CurrentComebackSong;
        if (currentSong == null)
        {
            Debug.LogError("[ComebackScene] CurrentComebackSong is null. ComeBack.Enter에서 SetCurrentComebackSong 했는지 확인");
            return;
        }
        EnterIntro();
    }

    //Intro
    private void EnterIntro()
    {
        phase = Phase.Intro;
        isClicked = false;
        Debug.Log("[컴백씬] 인트로");

        noticeHUD = UIManager.Instance.ShowHUDUI<CombackNoticeHUD>();
        noticeHUD.Init(
    $"Sprites/AlbumCovers/{currentSong.albumCover.name}",
    currentSong.title);

        noticeHUD.OnCombackPrepareStart += OnPrepareSignal;
        //TODO : 실장님 대화 표시  + 앨범 코드 하드코딩 바꾸기ㅣ,,,, 
    }

    private void OnPrepareSignal()
    {
        if (isClicked)
            return;

        isClicked = true;

        // 🔴 UIManager 건드리지 말고
        if (noticeHUD != null)
        {
            noticeHUD.OnCombackPrepareStart -= OnPrepareSignal;
            Destroy(noticeHUD.gameObject);   // ← 직접 파괴
            noticeHUD = null;
        }

        EnterPrepare();
    }


    //Prepare 컴백 준비 : 이제부터 타이머를 띄움 + 여기서 메인메뉴 화면을 다시 보여줌. 
    private void EnterPrepare()
    {
        if (phase == Phase.Prepare)
            return;

        phase = Phase.Prepare;
        Debug.Log("[컴백씬] 컴백준비단계 돌입");

        timer = 0f;

        // 타이머 HUD
        timerHUD = UIManager.Instance.ShowHUDUI<TimerHUD>();

        // 메인 메뉴 HUD
        mainMenuHUD = UIManager.Instance.ShowHUDUI<MainMenuHUD>();

        // 컴백 준비 상태 UI 세팅
        mainMenuHUD.CompackPrepareStarted();

        // 버튼 이벤트 연결
        mainMenuHUD.ClickedPracticeButton += EnterPractice;
        mainMenuHUD.ClickedComebackButton += EnterStage;
    }

    private void Update()
    {
        if (phase == Phase.Prepare || phase == Phase.Practice)
        {
            timer += Time.deltaTime;

            if (timerHUD != null)
                timerHUD.Init(FormatTime());

            if (timer >= PRACTICE_DURATION)
            {
                EnterStage();
            }
        }
    }

    //practice 컴백 연습. 
    private void EnterPractice()
    {
        if (phase != Phase.Prepare)
            return;

        phase = Phase.Practice;
        Debug.Log("[컴백씬] 연습하기");

        practiceHUD = UIManager.Instance.ShowHUDUI<PracticeHUD>();
        practiceHUD.Init(
    currentSong.title,
    $"Sprites/AlbumCovers/{currentSong.albumCover.name}");

        //TODO : 지금은 테스트용 더미 데이터.. comeback 쪽에서 이번 컴백 곡 정보 추가해서 controller가 참조해서 들고 와야 함!! 샤갈 할 거 존니 많긔
        practiceHUD.onClickedPracticeButton += StartPractice;

        stageHUD = UIManager.Instance.ShowHUDUI<StageHUD>();
    }

    private void StartPractice()
    {
        Debug.Log("[ComebackScene] Practice Started");
        // 실제 연습 로직은 이후
    }

    //stage 무대
    private void EnterStage()
    {
        if (phase == Phase.Stage)
            return;

        phase = Phase.Stage;
        Debug.Log("[컴백씬] 무대");

        UIManager.Instance.CloseAllHUD();

        // TODO: 무대 연출 시작
        // 무대 끝나면 FinishStage() 호출
    }

    public void FinishStage()
    {
        Debug.Log("[ComebackScene] Finish");
        OnFinished?.Invoke();
    }

    private string FormatTime()
    {
        int remain = Mathf.Max(0, (int)(PRACTICE_DURATION - timer));
        return $"{remain / 60:D2}:{remain % 60:D2}";
    }

    private void OnDestroy()
    {
        if (noticeHUD != null)
            noticeHUD.OnCombackPrepareStart -= OnPrepareSignal;

        if (mainMenuHUD != null)
        {
            mainMenuHUD.ClickedPracticeButton -= EnterPractice;
            mainMenuHUD.ClickedComebackButton -= EnterStage;
        }

        if (practiceHUD != null)
            practiceHUD.onClickedPracticeButton -= StartPractice;
    }
}


// using System;
// using UnityEngine;

// public class ComebackSceneController : MonoBehaviour
// {
//     public static event Action OnFinished;

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space)) // 임시 종료 트리거
//         {
//             Debug.Log("컴백 씬 종료 → 상태 종료 요청");
//             OnFinished?.Invoke();
//         }
//     }
// }
