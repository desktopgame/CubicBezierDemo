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
        [SerializeField] LineRenderer m_lineRenderer1;
        [SerializeField] LineRenderer m_lineRenderer2;
        [SerializeField] Node m_node1;
        [SerializeField] Node m_node2;
#pragma warning restore 649
        public GameObject Core => m_core;
        public Node Node1 => m_node1;
        public Node Node2 => m_node2;

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
            // Core -> Node2への線
            SetLine1();
            Observable.Merge(
                transform.ObserveEveryValueChanged(t => t.position),
                m_node1.transform.ObserveEveryValueChanged(t => t.position)
            ).Subscribe(_ => SetLine1())
             .AddTo(this);
            // Core -> Node2への線
            SetLine2();
            Observable.Merge(
                transform.ObserveEveryValueChanged(t => t.position),
                m_node2.transform.ObserveEveryValueChanged(t => t.position)
            ).Subscribe(_ => SetLine2())
             .AddTo(this);
        }

        private void SetLine1()
        {
            m_lineRenderer1.positionCount = 2;
            m_lineRenderer1.SetPositions(new []{transform.position, m_node1.transform.position});
        }

        private void SetLine2()
        {
            m_lineRenderer2.positionCount = 2;
            m_lineRenderer2.SetPositions(new []{transform.position, m_node2.transform.position});
        }
    }
}