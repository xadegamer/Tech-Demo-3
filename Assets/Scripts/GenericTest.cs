
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTest: MonoBehaviour
{
    public UnityEvent ops;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }

        ops?.Invoke();
    }

    void Test()
    {
        void Yes()
        {
            Debug.Log("Yes");
        }
        
        StartCoroutine(Test2(()=> ops.AddListener (Yes), null,()=> ops.RemoveListener(Yes), 5));
    }
    
    //Coroutine test;

    //private void Start()
    //{
    //    Tester();
    //}

    //void Tester()
    //{
    //    if (test != null)
    //    {
    //        Debug.Log("test is not null");
    //        StopCoroutine(test);
    //    }
    //    else
    //    {
    //        Debug.Log("test is null");
    //    }

    //    test = StartCoroutine(Test2());
    //}

    ////public IEnumerator Test1()
    ////{
    ////    while(true)
    ////    {
    ////        Debug.Log("Test1");
    ////        yield return new WaitForSeconds(1);
    ////    }
    ////}

    ////public IEnumerator Test2()
    ////{
    ////    while (true)
    ////    {
    ////        Debug.Log("Test2");
    ////        yield return new WaitForSeconds(2);
    ////    }
    ////}

    //public IEnumerator Test2()
    //{
    //    Debug.Log("Test2");
    //    yield return new WaitForSeconds(2);

    //    test = null;
    //    Tester();
    //}

    public IEnumerator Test2(Action start, Action<float> progress, Action end, float duration)
    {
        start?.Invoke();

        float activeTime = 0;
        while (activeTime < duration)
        {
            activeTime += Time.deltaTime;
            progress?.Invoke(activeTime / duration);
            yield return null;
        }
        end?.Invoke();
    }
}

