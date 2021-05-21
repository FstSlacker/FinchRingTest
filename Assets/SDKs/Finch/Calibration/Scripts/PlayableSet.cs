using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Describes the configuration of controllers used in application.
    /// </summary>
    public enum PlayableSet
    {
        /// <summary>
        /// Any available set from the list below.
        /// </summary>
        Any = 0,
        /// <summary>
        /// One arm 3DoF mode (one FinchRing)
        /// </summary>
        OneThreeDof = 1,
        /// <summary>
        /// Two arms 3DoF mode (two FinchRings)
        /// </summary>
        TwoThreeDof = 2,
        /// <summary>
        /// One arm 6DoF mode (one FinchTracker and one FinchRing)
        /// </summary>
        OneSixDof = 11,
        /// <summary>
        /// Two arms 6DoF mode (two FinchTrackers and two FinchRings)
        /// </summary>
        TwoSixDof = 22
    }
}
