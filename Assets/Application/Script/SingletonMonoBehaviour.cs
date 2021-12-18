// 2021-12-18
// Create Dansaka Koya
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

namespace Company.Product
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
#pragma warning disable 649
#pragma warning restore 649
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if(s_instance == null)
                {
                    var t = typeof(T);
                    s_instance = (T)FindObjectOfType(t);
                    if(s_instance == null)
                    {
                        Debug.LogError($"{t} is not found.");
                    }
                }
                return s_instance;
            }
        }

        private void Awake()
        {
            if(this != Instance)
            {
                Destroy(this);
            }
        }
    }
}