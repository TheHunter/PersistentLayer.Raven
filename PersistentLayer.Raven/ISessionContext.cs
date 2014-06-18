﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISessionContext
        : ISessionProvider
    {
        /// <summary>
        /// 
        /// </summary>
        bool HasSessionBinded { get; }
    }
}
