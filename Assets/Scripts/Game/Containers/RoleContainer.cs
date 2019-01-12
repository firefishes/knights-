﻿using ShipDock.Framework.AppointerIOC.IOC;
using UnityEngine;
using ShipDock.Framework.Interfaces;
using ShipDock.Framework.ObjectPool;
using ShipDock.Framework.Cores.Notices;
using ShipDock.Framewrok.Managers;
using ShipDock.Framework.Finess.ECS;
using ShipDock.Framework.Applications.RPG.Components;
using ShipDock.Framework.Finess.ECS.Containers;
using System;
using System.Collections.Generic;

namespace FF.Game
{
    public class RoleContainer : ContainerIOC, IEntitasSystem
    {
        Transform mMainRoleTF;
        Transform mRoleAgentTF;
        MainRoleComponent mMainRoleComponent;
        RoleAgentComponent mMainRoleAgentComponent;
        private int mMainRoleComponentKey;
        private int mMainRoleAgentComponentKey;

        private RoleComponent mRole;
        private RoleAgentComponent mRoleAgent;
        private List<IEntitasComponent> mRoles;
        private List<IEntitasComponent> mRoleAgents;

        public override void Start()
        {
            base.Start();

            Register<ParamNotice<int>, IValueHolder<int>>("GetMainCharacterControllerNotice", Pooling<ParamNotice<int>>.Instance);
            Register<ParamNotice<int>, IValueHolder<int>>("GetMainRoleAgentNotice", Pooling<ParamNotice<int>>.Instance);
        }

        public override void Finish()
        {
            base.Finish();

            //SetAppoint<IValueHolder<int>>(this, SetMainCharacterController, "GetMainCharacterControllerNotice");
            //SetAppoint<IValueHolder<int>>(this, SetMainRoleAgentComponent, "GetMainRoleAgentNotice");
            //SetAppoint<IValueHolder<Vector3>>(this, MainRoleWalk);
            //SetAppoint<IValueHolder<Vector3>>(this, MainRoleRun);

            IsActive = true;
            FinessECS.AddSystem(this);
        }

        //private void MainRoleRun<I>(ref I target)
        //{
        //    Vector3 v = (target as IValueHolder<Vector3>).GetValue();
        //    mMainRoleAgentComponent.faceToMovement = v;
        //    mMainRoleComponent.characterController.SimpleMove(v * mMainRoleAgentComponent.speedRun);

        //    if (!mMainRoleAgentComponent.isNeedCheckRoleFaceTo)
        //    {
        //        mMainRoleAgentComponent.isNeedCheckRoleFaceTo = true;
        //    }
        //}

        private void SetMainRoleAgentComponent<I>(ref I target)
        {
            mMainRoleAgentComponentKey = (target as IValueHolder<int>).GetValue();
        }

        //private void MainRoleWalk<I>(ref I target)
        //{
        //    Vector3 v = (target as IValueHolder<Vector3>).GetValue();
        //    mMainRoleAgentComponent.faceToMovement = v;
        //    mMainRoleComponent.characterController.SimpleMove(v * mMainRoleAgentComponent.speedWalk);

        //    if (!mMainRoleAgentComponent.isNeedCheckRoleFaceTo)
        //    {
        //        mMainRoleAgentComponent.isNeedCheckRoleFaceTo = true;
        //    }
        //}

        private void RoleMove(ref RoleAgentComponent agentComp, ref RoleComponent roleComp, Vector3 v)
        {
            roleComp.characterController.SimpleMove(v);

            if (!agentComp.isNeedCheckRoleFaceTo)
            {
                agentComp.isNeedCheckRoleFaceTo = true;
            }
        }

        //private void OnMainRoleFaceingMovement(int time)
        //{
        //    if (!mMainRoleAgentComponent.isNeedCheckRoleFaceTo)
        //    {
        //        return;
        //    }
        //    mMainRoleTF.LookAt(mMainRoleTF.position + mMainRoleAgentComponent.faceToMovement);
        //    if (Mathf.Abs(mMainRoleTF.rotation.y - mRoleAgentTF.rotation.y) <= 1)
        //    {
        //        mMainRoleAgentComponent.isNeedCheckRoleFaceTo = false;
        //    }
        //}

        private void UpdateRoleFacing(ref RoleAgentComponent roleAgent, ref Transform roleTF)
        {
            if (!roleAgent.isNeedCheckRoleFaceTo)
            {
                return;
            }
            
            roleTF.LookAt(roleTF.position + roleAgent.faceToMovement);

            if (Mathf.Abs(roleTF.rotation.y) <= 1)
            {
                roleAgent.currentSpeed = 0;
                roleAgent.isNeedCheckRoleFaceTo = false;
            }
        }

        private void SetMainCharacterController<I>(ref I target)
        {
            mMainRoleComponentKey = (target as IValueHolder<int>).GetValue();
            mMainRoleComponent = GetEntitasComponent<MainRoleComponent>(mMainRoleComponentKey);

            mMainRoleAgentComponent = GetEntitasComponent<RoleAgentComponent>(mMainRoleAgentComponentKey);
            //mMainRoleComponent.characterController = mMainRoleAgentComponent.characterController;

            if (mMainRoleTF == null)
            {
                mMainRoleTF = mMainRoleComponent.cachedTF;
            }
            if (mRoleAgentTF == null)
            {
                mRoleAgentTF = mMainRoleAgentComponent.cachedTF;
            }

            FruitsMainRoleFSM fsm = new FruitsMainRoleFSM(mMainRoleComponent, mMainRoleAgentComponent);
            fsm.Run(null, FruitMainRoleStateName.STATE_IDLE);

            //GameUpdaterManager.Instance.Add(OnMainRoleFaceingMovement);
        }

        public T GetEntitasComponent<T>(int key) where T : IEntitasComponent
        {
            return FinessECS.GetComponet<T>(key);
        }

        public void OnExecute(ref IEntitasSystem system, int time)
        {
            if(IsActive)
            {
                if(IsChanged)
                {
                    mRoles = FinessECS.GetComponents("RoleComponent");

                    
                }
                if (mRoles != null)
                {
                    int max = mRoles.Count;
                    for (int i = 0; i < max; i++)
                    {
                        mRole = mRoles[i] as RoleComponent;
                        mRoleAgent = mRole.roleAgent;
                        if(mRoleAgent.currentSpeed != 0)
                        {
                            RoleMove(ref mRoleAgent, ref mRole, mRoleAgent.faceToMovement * mRoleAgent.currentSpeed);
                        }
                        UpdateRoleFacing(ref mRoleAgent, ref mRole.cachedTF);
                    }
                }
            }
        }

        public string EntitasSystemName
        {
            get
            {
                return "Roles";
            }
        }

        public bool IsActive { get; set; }

        public bool IsChanged { get; set; }
    }

}