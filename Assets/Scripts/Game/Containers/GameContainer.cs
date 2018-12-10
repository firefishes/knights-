﻿using ShipDock.Framework.AppointerIOC.IOC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FF.Game
{
    public class GameContainer : ContainerIOC
    {

        public GameContainer()
        {
            IOCManager.Add(new MainRoleContainer());
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Finish()
        {
            base.Finish();
        }
    }

}