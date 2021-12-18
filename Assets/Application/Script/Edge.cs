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
    public class Edge : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] GameObject m_core;
        [SerializeField] Node m_node1;
        [SerializeField] Node m_node2;
#pragma warning restore 649
        void Start()
        {
            Observable.Merge(
                m_node1.OnFocus.Select(_ => 1),
                m_node2.OnFocus.Select(_ => 2)
            )
            .Subscribe(i =>
            {
                m_node1.Unselect();
                m_node2.Unselect();
                if(i == 1) m_node1.Select();
                if(i == 2) m_node2.Select();
            })
            .AddTo(this);
        }
    }
}