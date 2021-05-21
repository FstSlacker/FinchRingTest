// Copyright 2018 - 2020 Finch Technologies Ltd. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Finch.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Elements of the FinchRing and FinchTracker controllers - buttons, touchpad and etc.
    /// </summary>
    public enum RingElement : ushort
    {
        /// <summary>
        /// Button under the touchpad of the FinchRing or FinchTracker.
        /// </summary>
        HomeButton = 7,

        /// <summary>
        /// Touchpad of the FinchRing or FinchTracker.
        /// </summary>
        Touch = 15,

        /// <summary>
        /// Sensor that detects that FinchRing is on the user's finger.
        /// </summary>
        CapacitySensor = 8,

        /// <summary>
        /// State of the FinchCradle charging case.
        /// </summary>
        ChargeCaseState = 9,

        /// <summary>
        /// Force sensor of the FinchRing or FinchTracker.
        /// </summary>
        Analog = 4,
    }

    /// <summary>
    /// Events of the Touchpad element of the FinchRing or FinchTracker controller.
    /// </summary>
    public enum RingTouchpadEvents
    {
        /// <summary>
        /// This event appears if there was any change of Touchpad's state.    
        /// </summary>
        SwipeCounter = 6,

        /// <summary>
        /// Tap on the FinchRing or FinchTracker touchpad.
        /// </summary>
        Tap = 5,

        /// <summary>
        /// Swipe up on the FinchRing or FinchTracker touchpad.
        /// </summary>
        SwipeUp = 0,

        /// <summary>
        /// Swipe down on the FinchRing or FinchTracker touchpad.
        /// </summary>
        SwipeDown = 1,

        /// <summary>
        /// Swipe to the left on the FinchRing or FinchTracker touchpad.
        /// </summary>
        SwipeLeft = 2,

        /// <summary>
        /// Swipe to the right on the FinchRing or FinchTracker touchpad.
        /// </summary>
        SwipeRight = 3,
    }

    /// <summary>
    /// Manages position and rotation changes of the controllers and manages states of controller's elements.
    /// </summary>
    public static class FinchInput
    {
        #region TransformInput
        /// <summary>
        /// Returns bone rotation quaternion value.
        /// </summary>
        /// <param name="bone">Certain bone.</param>
        /// <param name="fPose">Use fPose rotation.</param>
        /// <returns>Bone rotation quaternion value.</returns>
        public static Quaternion GetRotation(Bone bone, bool fPose = true)
        {
            return Internal.FinchInput.GetRotation(bone, fPose).ToUnity();
        }

        /// <summary>
        /// Returns node rotation quaternion value.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <param name="fPose">Use fPose rotation.</param>
        /// <returns>Node rotation quaternion value.</returns>
        public static Quaternion GetRotation(NodeType node, bool fPose = true)
        {
            return Internal.FinchInput.GetRotation(node, fPose).ToUnity();
        }

        /// <summary>
        /// Returns position coordinates of bone.
        /// </summary>
        /// <param name="bone">Certain bone.</param>
        /// <returns>Position coordinates of bone.</returns>
        public static Vector3 GetPosition(Bone bone)
        {
            return Internal.FinchInput.GetPosition(bone).ToUnity();
        }

        /// <summary>
        /// Returns position coordinates of node.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>Position coordinates of node.</returns>
        public static Vector3 GetPosition(NodeType node)
        {
            return Internal.FinchInput.GetPosition(node).ToUnity();
        }
        #endregion

        #region SensorInput
        /// <summary>
        /// Returns node's linear acceleration in meters per second squared.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>Linear acceleration in meters per second squared.</returns>
        public static Vector3 GetLinearAcceleration(NodeType node)
        {
            return Internal.FinchInput.GetLinearAcceleration(node).ToUnity();
        }

        /// <summary>
        /// Returns node angular speed in radians per second.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>Node angular speed in radians per second.</returns>
        public static Vector3 GetAngularVelocity(NodeType node)
        {
            return Internal.FinchInput.GetAngularVelocity(node).ToUnity();
        }

        /// <summary>
        /// Returns battery charge in percent.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>Battery charge in percent.</returns>
        public static ushort GetBatteryCharge(NodeType node)
        {
            return Internal.FinchInput.GetBatteryCharge(node);
        }
        #endregion

        #region ElementInput
        /// <summary>
        /// Returns true if the node's element is pressed at the moment.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller's element is pressed.</returns>
        public static bool GetPress(NodeType node, RingElement element)
        {
            return Internal.FinchInput.GetPress(node, (int)element);
        }

        /// <summary>
        /// Returns true if the controller's element was pressed down.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller's element was pressed down.</returns>
        public static bool GetPressDown(NodeType node, RingElement element)
        {
            return Internal.FinchInput.GetPressDown(node, (int)element);
        }

        /// <summary>
        /// Returns true if the controller's element was released.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller's element was released.</returns>
        public static bool GetPressUp(NodeType node, RingElement element)
        {
            return Internal.FinchInput.GetPressUp(node, (int)element);
        }

        /// <summary>
        /// Returns time in milliseconds how long controller's element was pressed.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>Time in milliseconds how long controller's element was pressed.</returns>
        public static float GetPressTime(NodeType node, RingElement element)
        {
            return Internal.FinchInput.GetPressTime(node, (int)element);
        }

        /// <summary>
        /// Returns true if the node's touchpad event is occured at the moment.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <param name="element">Touchpad event (swipe or tap).</param>
        /// <returns>True if the node's touchpad event is occured.</returns>
        public static bool GetTouchpadEvent(NodeType node, TouchpadEvents touchpadEvent)
        {
            return Internal.FinchInput.GetTouchpadEvent(node, (int)touchpadEvent);
        }

        /// <summary>
        /// Returns touchpad touch point coordinates.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <returns>Touchpad touch point coordinates.</returns>
        public static Vector2 GetTouchAxes(NodeType node)
        {
            return Internal.FinchInput.GetTouchAxes(node).ToUnity();
        }

        /// <summary>
        /// Returns Trigger or Force sensor value.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <returns>Trigger value.</returns>
        public static float GetAnalog(Chirality chirality)
        {
            return Internal.FinchInput.GetAnalog(chirality);
        }

        /// <summary>
        /// Returns swipe direction.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <returns>Swipe direction.</returns>
        public static Vector2 GetSwipe(NodeType node)
        {
            return Internal.FinchInput.GetSwipe(node).ToUnity();
        }

        /// <summary>
        /// Returns swipe duration.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <returns>Swipe duration.</returns>
        public static float GetSwipeTime(NodeType node)
        {
            return Internal.FinchInput.GetSwipeTime(node);
        }
        #endregion
    }
}
