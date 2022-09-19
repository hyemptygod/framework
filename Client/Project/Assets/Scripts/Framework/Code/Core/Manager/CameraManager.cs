using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class CameraManager : Manager<CameraManager>
    {
        public Camera MainCamera { get; private set; }
        public Camera UICamera { get; private set; }

        protected override void Init()
        {
            base.Init();

            MainCamera = Camera.main;

            foreach (var c in Camera.allCameras)
            {
                if (c.gameObject.CompareTag("UICamera"))
                    UICamera = c;
            }
        }

    }
}
