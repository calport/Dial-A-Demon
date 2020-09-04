using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    internal class CoroutineManagerMonoBehaviour : MonoBehaviour{}

    public class CoroutineManager
    {
        public delegate void DelayDelegate();
        private static CoroutineManagerMonoBehaviour _CoroutineManagerMonoBehaviour;

        static CoroutineManager()
        {
            _Init();
        }

        public static Coroutine DoCoroutine(IEnumerator routine)
        {
            return _CoroutineManagerMonoBehaviour.StartCoroutine(routine);
        }

        public static void StopCoroutine(Coroutine routine)
        {
            _CoroutineManagerMonoBehaviour.StopCoroutine(routine);
        }

        public static Coroutine DoOneFrameDelay(DelayDelegate a)
        {
            return _CoroutineManagerMonoBehaviour.StartCoroutine(OneFrameDelay(a));
        }
        
        public static Coroutine DoDelayCertainSeconds(DelayDelegate a, float delayTime)
        {
            return _CoroutineManagerMonoBehaviour.StartCoroutine(SetTimeDelay(a,delayTime));
        }
        
        public static void DoYieldCoroutine(IEnumerator routine)
        {
            _CoroutineManagerMonoBehaviour.StartCoroutine(yieldRoutine(routine));
        }

        private static IEnumerator yieldRoutine(IEnumerator routine)
        {
            yield return _CoroutineManagerMonoBehaviour.StartCoroutine(routine);
        }
        private static void _Init()
        {
            var go = new GameObject();
            go.name = "CoroutineManager";
            _CoroutineManagerMonoBehaviour = go.AddComponent<CoroutineManagerMonoBehaviour>();
            GameObject.DontDestroyOnLoad(go);
        }
        private static IEnumerator OneFrameDelay(DelayDelegate a)
        {
            yield return null;
            a();
        }
        
        private static IEnumerator SetTimeDelay(DelayDelegate a, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            a();
        }
    }


