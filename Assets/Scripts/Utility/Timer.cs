using System;

/// <summary>
/// タイマー機能の基底クラス
/// カウントダウンとストップウォッチの共通機能を提供
/// </summary>
public abstract class Timer
{
    /// <summary>
    /// タイマーの初期設定時間
    /// </summary>
    protected float _initialTime;

    /// <summary>
    /// 現在のタイマー値
    /// </summary>
    public float Time { get; set; }

    /// <summary>
    /// タイマーが実行中かどうかを示すフラグ
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// タイマーの進行度（0.0から1.0の範囲）
    /// </summary>
    public float Progress => ReturnProgress();

    /// <summary>
    /// タイマー開始時に実行されるイベント
    /// </summary>
    public Action OnTimerStart = delegate { };

    /// <summary>
    /// タイマー停止時に実行されるイベント
    /// </summary>
    public Action OnTimerStop = delegate { };

    /// <summary>
    /// タイマーの初期化
    /// </summary>
    /// <param name="time">設定する時間（秒）</param>
    protected Timer(float time)
    {
        _initialTime = time;
        IsRunning = false;
    }

    /// <summary>
    /// タイマーを開始し、初期時間にリセット
    /// </summary>
    public void Start()
    {
        Time = _initialTime;
        if (!IsRunning)
        {
            IsRunning = true;
            OnTimerStart.Invoke();
        }
    }

    /// <summary>
    /// タイマーを停止し、停止イベントを発火
    /// </summary>
    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    /// <summary>
    /// 一時停止したタイマーを再開
    /// </summary>
    public void Resume() => IsRunning = true;

    /// <summary>
    /// タイマーを一時停止
    /// </summary>
    public void Pause() => IsRunning = false;

    /// <summary>
    /// タイマーの更新処理
    /// </summary>
    /// <param name="deltaTime">前フレームからの経過時間</param>
    public abstract void Tick(float deltaTime);

    /// <summary>
    /// カウントダウンの進行度を取得
    /// </summary>
    /// <returns></returns>
    private float ReturnProgress()
    {
        if (_initialTime != 0)
        {
            return Time / _initialTime;
        }

        return Time;
    }
}

/// <summary>
/// カウントダウン機能を提供するタイマークラス
/// 指定時間から0までカウントダウン
/// </summary>
public class CountdownTimer : Timer
{
    /// <summary>
    /// カウントダウンタイマーの初期化
    /// </summary>
    /// <param name="time">カウントダウンする時間（秒）</param>
    public CountdownTimer(float time) : base(time) { }

    /// <summary>
    /// カウントダウンの更新処理
    /// </summary>
    /// <param name="deltaTime">前フレームからの経過時間</param>
    public override void Tick(float deltaTime)
    {
        if (IsRunning && Time > 0)
        {
            Time -= deltaTime;
        }

        if (IsRunning && Time <= 0)
        {
            Stop();
        }
    }

    /// <summary>
    /// カウントダウンが完了したかどうか
    /// </summary>
    public bool IsFinished()
    {
        if (Time <= 0)
        {
            return true;
        }
        return false;
    }
    

    /// <summary>
    /// タイマーを初期時間にリセット
    /// </summary>
    public void Reset()
    {
        Time = _initialTime;
    }

    /// <summary>
    /// 新しい時間を設定してリセット
    /// </summary>
    /// <param name="newTime">新しく設定する時間（秒）</param>
    public void Reset(float newTime)
    {
        _initialTime = newTime;
        Reset();
    }
}

/// <summary>
/// ストップウォッチ機能を提供するタイマークラス
/// 0から時間を計測
/// </summary>
public class StopwatchTimer : Timer
{
    /// <summary>
    /// ストップウォッチタイマーの初期化
    /// </summary>
    public StopwatchTimer() : base(0) { }

    /// <summary>
    /// ストップウォッチの更新処理
    /// </summary>
    /// <param name="deltaTime">前フレームからの経過時間</param>
    public override void Tick(float deltaTime)
    {
        if (IsRunning)
        {
            Time += deltaTime;
        }
    }

    /// <summary>
    /// ストップウォッチをゼロにリセット
    /// </summary>
    public void Reset() => Time = 0;

    /// <summary>
    /// 現在の計測時間を取得
    /// </summary>
    /// <returns>経過時間（秒）</returns>
    public float GetTime() => Time;
}

