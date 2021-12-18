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
    public class Node : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] GameObject m_hitArea;
        [SerializeField] MeshRenderer m_meshRenderer;
        [SerializeField] Material m_selectedMaterial;
#pragma warning restore 649
        public bool IsSelected { private set; get; }
        private Material m_defaultMaterial;

        public IObservable<Unit> OnFocus => m_onFocusSubject;
        private Subject<Unit> m_onFocusSubject = new Subject<Unit>();

        public IObservable<Unit> OnBlur => m_onBlurSubject;
        private Subject<Unit> m_onBlurSubject = new Subject<Unit>();

        private bool m_touch;

        // Start is called before the first frame update
        void Start()
        {
            if(!m_hitArea)
            {
                this.m_hitArea = gameObject;
            }
            this.m_defaultMaterial = m_meshRenderer.material;
            Observable.EveryUpdate()
                      .Where(_ => Input.GetMouseButtonDown(0))
                      .Subscribe(_ =>
                      {
                            if(Environment.Instance.Raycast(out var hit) &&
                               hit.collider.gameObject == m_hitArea)
                            {
                                m_onFocusSubject.OnNext(Unit.Default);
                            }
                            else
                            {
                                m_onBlurSubject.OnNext(Unit.Default);
                            }
                      })
                      .AddTo(this);
                Observable.EveryUpdate()
                          .Where(_ => m_touch && Input.GetMouseButton(0))
                          .Subscribe(_ => transform.position = Environment.Instance.GetCursorWorldPosition())
                          .AddTo(this);
                Observable.EveryUpdate()
                          .Where(_ => Input.GetMouseButtonUp(0))
                          .Subscribe(_ => m_touch = false)
                          .AddTo(this);
        }

        public void Select()
        {
            m_meshRenderer.material = m_selectedMaterial;
            this.IsSelected = true;
            this.m_touch = true;
        }

        public void Unselect()
        {
            m_meshRenderer.material = m_defaultMaterial;
            this.IsSelected = false;
        }
    }
}