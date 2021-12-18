// 2021-12-18
// Create Dansaka Koya
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;
using DG.Tweening;

namespace Company.Product
{
    public class CubicBezierDemo : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] Curve m_curve;
        [SerializeField] GameObject m_particlePrefab;
        [SerializeField] int m_emitCount;
        [SerializeField] int m_emitDelay;
#pragma warning restore 649
        // Start is called before the first frame update
        void Start()
        {
            var cts = new CancellationTokenSource();
            Observable.Merge(
                m_curve.ObserveEveryValueChanged(c => c.EdgeList.Count)
                       .AsUnitObservable(),
                Observable.Merge(m_curve.EdgeList.Select(edge =>
                    Observable.Merge(
                        edge.ObserveEveryValueChanged(t => t.transform.position),
                        edge.Node1.ObserveEveryValueChanged(t => t.transform.position),
                        edge.Node2.ObserveEveryValueChanged(t => t.transform.position)
                    )
                )).AsUnitObservable()
            )
            .Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                cts.Cancel();
                cts.Dispose();
                cts = new CancellationTokenSource();
                Play(cts.Token).Forget();
            }).AddTo(this);
        }

        private async UniTask Play(CancellationToken token)
        {
            while(true)
            {
                var edges = new List<Edge>(m_curve.EdgeList);
                var objs = new List<GameObject>();
                foreach(int _ in Enumerable.Range(0, m_emitCount))
                {
                    await UniTask.WaitWhile(() => edges.Count == 0, cancellationToken: token);
                    var inst = Instantiate(m_particlePrefab, edges.First().Core.transform.position, Quaternion.identity);
                    var path = new List<Vector3>();
                    objs.Add(inst);
                    ComputePath(edges, path);
                    DOPathAndDestroy(inst, path, token).Forget();
                    await UniTask.Delay(TimeSpan.FromMilliseconds(m_emitDelay), cancellationToken: token);
                }
                await UniTask.WaitWhile(() => objs.All(obj => obj), cancellationToken: token);
            }
        }

        private static async UniTask DOPathAndDestroy(GameObject inst, List<Vector3> path, CancellationToken token)
        {
            try
            {
                await inst.transform.DOPath(path.ToArray(), 1.0f, PathType.CubicBezier).ToUniTask(TweenCancelBehaviour.Complete, token);
            }
            finally
            {
                Destroy(inst);
            }
        }

        private void ComputePath(List<Edge> edges, List<Vector3> outList)
        {
            outList.Add(edges[1].Core.transform.position);
            outList.Add(edges[0].Node1.transform.position);
            outList.Add(edges[1].Node1.transform.position);
            int index = 1;
            while(index + 1 < edges.Count)
            {
                outList.Add(edges[index + 1].Core.transform.position);
                outList.Add(edges[index].Node2.transform.position);
                outList.Add(edges[index + 1].Node1.transform.position);
                index++;
            }
        }
    }
}