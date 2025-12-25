using System;

namespace DreemurrStudio.CountDownTimer
{
    /// <summary>
    /// 封装的计时器类，提供通用的计时功能
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// 每秒的计时器刻度数
        /// </summary>
        public const int TICK_PERSECOND = 1000;

        /// <summary>
        /// 剩余时间
        /// </summary>
        private float leftTime;
        /// <summary>
        /// 完整计时器时间
        /// </summary>
        private float fullTime;
        /// <summary>
        /// 计时器步长，单位为秒
        /// </summary>
        private float step;
        /// <summary>
        /// 所使用的时间缩放，不使用时默认为1
        /// </summary>
        private float timeScale = 1f;
        /// <summary>
        /// 是否处于计时中
        /// </summary>
        private bool inCountDown = false;
        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool doInit = false;

        /// <summary>
        /// 计时器开始时事件，输出计时器要运行的时间
        /// </summary>
        public event Action<float> OnTimerBegin;
        /// <summary>
        /// 计时器结束时事件，输出计时器运行的时间
        /// </summary>
        public event Action<float> OnTimerEnd;
        /// <summary>
        /// 计时器每次步进时事件，输出当前剩余时间
        /// </summary>
        public event Action<float> OnTimerStep;

        /// <summary>
        /// 实现计时功能的定时器
        /// </summary>
        private System.Timers.Timer timer;
        /// <summary>
        /// 当前计时器是否在倒计时中
        /// </summary>
        public bool InCountDown => inCountDown;
        /// <summary>
        /// 获取当前计时器的实际间隔时间
        /// </summary>
        public float Interval => step * TICK_PERSECOND / timeScale;
        /// <summary>
        /// 获取当前计时器的剩余时间
        /// </summary>
        public float LeftTime => leftTime;
        /// <summary>
        /// 时间缩放，默认为1
        /// </summary>
        public float TimeScale => timeScale;

        /// <summary>
        /// 设置计时器
        /// </summary>
        /// <param name="time">倒计时要到达的完整时间，若要设为</param>
        /// <param name="step">每次步进</param>
        /// <param name="timeScale"></param>
        /// <param name="start"></param>
        public void Set(float time, float step = 1f, float timeScale = 1f, bool start = true)
        {
            leftTime = time;
            fullTime = time;
            this.step = step;
            this.timeScale = timeScale;
            doInit = false;

            timer = new(step * TICK_PERSECOND / timeScale);
            timer.AutoReset = true;
            timer.Elapsed += (sender, e) =>
            {
                if (LeftTime <= 0f)
                {
                    timer.Stop();
                    OnTimerEnd?.Invoke(fullTime);
                    return;
                }
                leftTime -= step * timeScale;
                OnTimerStep?.Invoke(LeftTime);
            };
            if (start) Start();
        }

        /// <summary>
        /// 重设计时器为原最大时间值
        /// </summary>
        /// <param name="start">是否立即开始计时，可设为空，为空时不变更当前运行状态</param>
        public void Reset(bool? start = null) =>
            Set(fullTime, step, timeScale, start.HasValue ? start.Value : InCountDown);

        /// <summary>
        /// 开始计时器
        /// </summary>
        public void Start()
        {
            Resume();
            if (!doInit) OnTimerBegin?.Invoke(fullTime);
            doInit = true;
        }

        /// <summary>
        /// 继续计时器
        /// </summary>
        public void Resume()
        {
            inCountDown = true;
            timer.Start();
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        public void Pause()
        {
            inCountDown = false;
            timer.Stop();
        }

        /// <summary>
        /// 修改计时器的时间缩放
        /// </summary>
        /// <param name="scale">要修改的缩放值，不指定时默认为1，即正常值</param>
        public void SetTimeScale(float scale = 1f)
        {
            if (scale < 0f) throw new ArgumentOutOfRangeException(nameof(scale), "时间缩放不能小于0");
            timer.Interval = step * TICK_PERSECOND / scale;
            timeScale = scale;
        }
    }
}