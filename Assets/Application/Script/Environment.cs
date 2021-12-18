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
    public class Environment : SingletonMonoBehaviour<Environment>
    {
#pragma warning disable 649
        [SerializeField] float m_depth = 10;
#pragma warning restore 649
        public float Depth => m_depth;

        public Vector3 GetCursorMousePosition()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = m_depth;
            return mousePosition;
        }

        public Vector3 GetCursorWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(GetCursorMousePosition());
        }

        public bool Raycast(out RaycastHit hit)
        {
            return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
        }
    }
}