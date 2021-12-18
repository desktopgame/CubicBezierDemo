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
    public class Curve : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] GameObject m_edgePrefab;
        [SerializeField] LineRenderer m_lineRenderer;
#pragma warning restore 649
        public IReadOnlyList<Edge> EdgeList => m_edgeList;
        private List<Edge> m_edgeList = new List<Edge>();
        private Node m_selectedEdge;

        void Start()
        {
            Observable.EveryUpdate()
                      .Where(_ => Input.GetMouseButtonDown(0))
                      .Subscribe(_ =>
                      {
                          if(!Environment.Instance.Raycast(out var hit))
                          {
                            var inst = Instantiate(m_edgePrefab, Environment.Instance.GetCursorWorldPosition(), Quaternion.identity);
                            var edge = inst.GetComponent<Edge>();
                            m_edgeList.Add(edge);
                            SetEvent(edge);
                            SetLine();
                          }
                      })
                      .AddTo(this);
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKey(KeyCode.D))
                      .Where(_ => m_selectedEdge)
                      .Subscribe(_ =>
                      {
                          m_edgeList.Remove(m_selectedEdge.GetComponent<Edge>());
                          Destroy(m_selectedEdge.gameObject);
                          this.m_selectedEdge = null;
                          SetLine();
                      })
                      .AddTo(this);
        }

        private void SetEvent(Edge edge)
        {
            var node = edge.GetComponent<Node>();
            node.OnFocus.Subscribe(_ =>
            {
                if(m_selectedEdge)
                {
                    m_selectedEdge.Unselect();
                }
                node.Select();
                this.m_selectedEdge = node;
            }).AddTo(this);
            edge.transform.ObserveEveryValueChanged(t => t.position)
                          .Subscribe(_ => SetLine())
                          .AddTo(this);
        }

        private void SetLine()
        {
            m_lineRenderer.positionCount = m_edgeList.Count;
            m_lineRenderer.SetPositions(m_edgeList.Select(edge => edge.transform.position).ToArray());
        }
    }
}