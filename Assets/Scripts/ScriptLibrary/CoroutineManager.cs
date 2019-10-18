using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialADemon.Library
{
    internal class CoroutineManagerMonoBehaviour : MonoBehaviour{}

    public class CoroutineManager
    {
        public delegate void DelayDelegate();
        private static CoroutineManagerMonoBehaviour _CoroutineManagerMonoBehaviour;

        static CoroutineManager()
        {
            Init();
        }

        public static void DoCoroutine(IEnumerator routine)
        {
            _CoroutineManagerMonoBehaviour.StartCoroutine(routine);
        }

        public static void DoOneFrameDelay(DelayDelegate a)
        {
            _CoroutineManagerMonoBehaviour.StartCoroutine(OneFrameDelay(a));
        }
        
        public static void DoDelayCertainSeconds(DelayDelegate a, float delayTime)
        {
            _CoroutineManagerMonoBehaviour.StartCoroutine(SetTimeDelay(a,delayTime));
        }
        
        public static void DoYieldCoroutine(IEnumerator routine)
        {
            _CoroutineManagerMonoBehaviour.StartCoroutine(yieldRoutine(routine));
        }

        private static IEnumerator yieldRoutine(IEnumerator routine)
        {
            yield return _CoroutineManagerMonoBehaviour.StartCoroutine(routine);
        }
        private static void Init()
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
}

