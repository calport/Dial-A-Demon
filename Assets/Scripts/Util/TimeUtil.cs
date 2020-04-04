using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimeUtil
{
        public class DownCounter
        {
            public bool isToggled{ get; private set; }
            public float countTime { get; private set; }
            private Coroutine _coroutine;

            public DownCounter(float timeSpan)
            {
                countTime = timeSpan;
                isToggled = false;
            }

            public DownCounter Start()
            {
                isToggled = true;
                _coroutine = CoroutineManager.DoDelayCertainSeconds(delegate
                {
                    isToggled = false;
                    Services.eventManager.Fire(new DownCounterStop(this));
                }, countTime);
                Services.eventManager.Fire(new DownCounterStart(this));
                return this;
            }

            public DownCounter Break()
            {
                if (_coroutine == null) return this;
                
                isToggled = false;
                CoroutineManager.StopCoroutine(_coroutine);
                Services.eventManager.Fire(new DownCounterBreak(this));
                return this;
            }
        }

        /// <summary>
        /// This util should be called by an update
        /// Use if(slowupdate.detect){} for the function that you want to be called once per several frame
        /// To lower the calculation
        /// </summary>
        public class SlowUpdate
        {
            public int slowTime { get; private set; } = 1;
            public SlowUpdate(int slowTime)
            {
                this.slowTime = slowTime;
            }

            private int _updateCount = 0;

            public bool detect
            {
                get
                {
                    _updateCount++;
                    
                    if (_updateCount >= slowTime)
                    {
                        _updateCount = 0;
                        return true;
                    }

                    return false;
                }
            }
        }
} 
