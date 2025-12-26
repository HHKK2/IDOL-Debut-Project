using System;
using System.Runtime.CompilerServices;
using UnityEditor.Overlays;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    // 플레이어의 모든 스탯을 관리함 -> 스탯 읽기, 쓰기, 범위 제한, 변경 시 이벤트 발행 관리
    // GameState UI 민심은 직접 Player 에게 접근하지 않음
    // 스텟 변경시 이벤트를 통해 UI 및 민심 시스템에 알림

    // TODO
    // (완료) 스탯 최대 최소 값 정하기
    // 밸런싱 수치 조정 < 무슨 말인지 이해 못 함.
    // 세이브/로드 연동
    
    public static PlayerStatController Instance { get; private set; }

    private Player _player;

    // 플레이어 스탯 변경 이벤트 (UI, 민심 시스템 등이 구독)
    public event Action<int> OnReputationChanged;
    public event Action<int> OnFanNumberChanged;
    public event Action<int> OnMentalHealthChanged;



    // 플레이어 스탯 상수 정의
    private const int MIN_REPUTATION = -100;
    private const int MAX_REPUTATION = 100;
    private const int MIN_MENTAL_HEALTH = 0;
    private const int MAX_MENTAL_HEALTH = 100;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _player = new Player();
        InitializeStats();
    }

    private void InitializeStats()
    {
        _player.Reputation = 0;
        _player.FanNumber = 0;
        _player.MentalHealth = 100;
    }


    // Getter for player stats
    public int Reputation => _player.Reputation;
    public int FanNumber => _player.FanNumber;
    public int MentalHealth => _player.MentalHealth;
    public string PlayerName => _player.Name;
    public Gender PlayerGender => _player.Gender;



    // 플레이어 스탯 변경 메서드
    public void ModifyReputation(int amount)
    {
        int before = _player.Reputation;
        _player.Reputation = Mathf.Clamp(_player.Reputation + amount, MIN_REPUTATION, MAX_REPUTATION);
        if (before != _player.Reputation)
            OnReputationChanged?.Invoke(_player.Reputation);
    }

    public void ModifyFanNumber(int amount)
    {
        int before = _player.FanNumber;
        _player.FanNumber = Mathf.Max(0, _player.FanNumber + amount);
        if (before != _player.FanNumber)
            OnFanNumberChanged?.Invoke(_player.FanNumber);
    }

    public void ModifyMentalHealth(int amount)
    {
        int before = _player.MentalHealth;
        _player.MentalHealth = Mathf.Clamp(_player.MentalHealth + amount, MIN_MENTAL_HEALTH, MAX_MENTAL_HEALTH);
        if (before != _player.MentalHealth)
            OnMentalHealthChanged?.Invoke(_player.MentalHealth);
    }

    public void SetPlayerInfo(string name, Gender gender)
    {
        _player.Name = name;
        _player.Gender = gender;
    }

    public void ResetStats()
    {
        InitializeStats();
        OnReputationChanged?.Invoke(_player.Reputation);
        OnFanNumberChanged?.Invoke(_player.FanNumber);
        OnMentalHealthChanged?.Invoke(_player.MentalHealth);
    }
}
