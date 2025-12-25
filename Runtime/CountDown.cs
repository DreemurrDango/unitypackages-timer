using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DreemurrStudio.CountDownTimer
{

    /// <summary>
    /// 通用倒计时组件
    /// </summary>
    public class CountDown : MonoBehaviour
    {
        [Header("倒计时设置")]
        [SerializeField]
        [Tooltip("是否使用时间缩放")]
        private bool useTimeScale = true;
        [SerializeField]
        [Tooltip("是否在按下任意键时重置倒计时")]
        private bool resetOnAnyInput = true;
        [Tooltip("倒计时结束时事件，输出倒计时运行的时间")]
        public UnityEvent<float> onCountDownCompleted;

        [Header("文本显示")]
        [SerializeField]
        [Tooltip("显示倒计时的文本组件")]
        private TMP_Text timeValueText;
        [SerializeField]
        [Tooltip("显示倒计时的文本内容模式，可添加的参数包括{0:剩余时间}{1:完整时间}{2:已经过时间}{3:剩余时间百分比}")]
        private string contentFormat = "{0}";

        [Header("进度条显示")]
        [SerializeField]
        [Tooltip("显示倒计时进度的滑动条")]
        private Slider timeValueSlider;

        [Header("调试")]
        [SerializeField]
        [Tooltip("倒计时的完整时间")]
        private float countDownFullTime;
        [SerializeField]
        [Tooltip("剩余倒计时时间")]
        private float t_CountDown;

        /// <summary>
        /// 当前是否正处于倒计时中
        /// </summary>
        public bool InCountDown => LeftTime > 0f;
        /// <summary>
        /// 剩余倒计时时间
        /// </summary>
        public float LeftTime => t_CountDown;
        /// <summary>
        /// 经过的时间
        /// </summary>
        public float PassedTime => countDownFullTime - t_CountDown;
        /// <summary>
        /// 剩余倒计时时间
        /// </summary>
        public float CountDownProgress => t_CountDown / countDownFullTime;

        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time">倒计时时间</param>
        /// <param name="start">是否立即开始倒计时</param>
        public void Set(float time, bool start)
        {
            countDownFullTime = time;
            t_CountDown = time;
            enabled = start;
            if (start) Begin();
        }
        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time">倒计时时间</param>
        public void Set(float time) => Set(time, true);

        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void Begin() => enabled = true;

        /// <summary>
        /// 暂停倒计时
        /// </summary>
        public void Pause() => enabled = false;

        /// <summary>
        /// 取消暂停，继续倒计时
        /// </summary>
        public void UnPause() => enabled = true;

        /// <summary>
        /// 重设计时器为原最大时间值，并沿用原运行状态倒计时
        /// </summary>
        public void Reset() => Reset(InCountDown);
        /// <summary>
        /// 重设计时器为原最大时间值
        /// </summary>
        /// <param name="start">是否开始倒计时</param>
        public void Reset(bool start)
        {
            t_CountDown = countDownFullTime;
            enabled = start;
            UpdateShow();
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        /// <param name="updateShow">是否将剩余时间重设为0并更新显示，默认为是</param>
        public void Stop(bool updateShow = true)
        {
            if (updateShow)
            {
                t_CountDown = 0f;
                UpdateShow();
            }
            Pause();
        }

        private void UpdateShow()
        {
            if (timeValueText && timeValueText.isActiveAndEnabled)
                //更新文本显示
                timeValueText.text = string.Format(contentFormat, t_CountDown, countDownFullTime, LeftTime, CountDownProgress);
            if (timeValueSlider) timeValueSlider.value = CountDownProgress;
        }

        private void Update()
        {
            if (!(t_CountDown > 0f)) return;
            //按下任意键时重置倒计时
            if (resetOnAnyInput)
            {
#if ENABLE_INPUT_SYSTEM
                //使用新输入系统
                if (UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame)
#else
            //使用旧输入系统
                if (Input.anyKeyDown)
#endif
                {
                    Reset(); 
                    return;
                }
            }
            //倒计时数据更新，显示更新
            t_CountDown -= useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
            UpdateShow();
            //倒计时结束
            if (t_CountDown > 0) return;
            onCountDownCompleted?.Invoke(countDownFullTime);
        }
    }
}