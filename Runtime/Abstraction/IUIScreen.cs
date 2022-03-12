using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public interface IUIScreen
    {
        bool HasBlackScreen { get; }
        string ScreenName { get; }
        bool HasLogEvent { get; }
    }
}
