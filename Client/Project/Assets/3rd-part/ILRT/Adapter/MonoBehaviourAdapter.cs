using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;

public class MonoBehaviourAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(MonoBehaviour);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }
    //为了完整实现MonoBehaviour的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

        IMethod mAwakeMethod;
        bool mAwakeMethodGot;
        public void Awake()
        {
            if (instance == null)
                return;
            if (!mAwakeMethodGot)
            {
                mAwakeMethod = instance.Type.GetMethod("Awake", 0);
                mAwakeMethodGot = true;
            }

            if (mAwakeMethod != null)
            {
                appdomain.Invoke(mAwakeMethod, instance, null);
            }
        }

        IMethod mStartMethod;
        bool mStartMethodGot;
        void Start()
        {
            if (instance == null)
                return;
            if (!mStartMethodGot)
            {
                mStartMethod = instance.Type.GetMethod("Start", 0);
                mStartMethodGot = true;
            }

            if (mStartMethod != null)
            {
                appdomain.Invoke(mStartMethod, instance, null);
            }
        }

        IMethod mEnableMethod;
        bool mEnableMethodGot;
        void OnEnable()
        {
            if (instance == null)
                return;
            if (!mEnableMethodGot)
            {
                mEnableMethod = instance.Type.GetMethod("OnEnable", 0);
                mEnableMethodGot = true;
            }

            if (mEnableMethod != null)
            {
                appdomain.Invoke(mEnableMethod, instance, null);
            }
        }

        IMethod mDisableMethod;
        bool mDisableMethodGot;
        void OnDisable()
        {
            if (!mDisableMethodGot)
            {
                mDisableMethod = instance.Type.GetMethod("OnDisable", 0);
                mDisableMethodGot = true;
            }

            if (mDisableMethod != null)
            {
                appdomain.Invoke(mDisableMethod, instance, null);
            }
        }

        IMethod mUpdateMethod;
        bool mUpdateMethodGot;
        void Update()
        {
            if (instance == null)
                return;
            if (!mUpdateMethodGot)
            {
                mUpdateMethod = instance.Type.GetMethod("Update", 0);
                mUpdateMethodGot = true;
            }

            if (mStartMethod != null)
            {
                appdomain.Invoke(mUpdateMethod, instance, null);
            }
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }
}

