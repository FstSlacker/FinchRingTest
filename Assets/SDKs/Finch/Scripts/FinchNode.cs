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

using System;
using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Provides information about Finch device: rotation, position, device's elements states, battery level. Allows to control device's haptic feedback (vibration).
    /// </summary>
    public abstract class FinchNode
    {
        /// <summary>
        /// Action that takes place on Finch device's connection.
        /// </summary>
        public Action OnConnected;

        /// <summary>
        /// Action that takes place on Finch device's disconnection.
        /// </summary>
        public Action OnDisconnected;

        /// <summary>
        /// Finch device's chirality (left or right).
        /// </summary>
        protected Chirality NodeChirality;

        /// <summary>
        /// Controller type.
        /// </summary>
        public ControllerType ControllerType { get { return FinchNodeManager.ControllerType; } }

        protected NodeType Node;

        #region SensorInput
        /// <summary>
        /// Device's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get { return FinchInput.GetRotation(Node); }
        }

        /// <summary>
        /// Device's position.
        /// </summary>
        public Vector3 Position
        {
            get { return FinchInput.GetPosition(Node); }
        }

        /// <summary>
        /// Returns position of the device.
        /// </summary>
        /// <param name="nodeType">Type of Finch device.</param>
        /// <returns>Position coordinates.</returns>
        public static Vector3 GetPosition(NodeType nodeType)
        {
            return FinchInput.GetPosition(nodeType);
        }

        /// <summary>
        /// Returns rotation of the device.
        /// </summary>
        /// <param name="nodeType">Type of FinchNode.</param>
        /// <returns>Node's rotation quaternion.</returns>
        public static Quaternion GetRotation(NodeType nodeType)
        {
            return FinchInput.GetRotation(nodeType);
        }
        #endregion

        #region ControllerElementInput
        protected static bool GetPress(Chirality chirality, RingElement element, bool isUpperArm)
        {
            return Internal.FinchNode.GetPress(chirality, (int)element, isUpperArm);
        }

        protected static bool GetPressDown(Chirality chirality, RingElement element, bool isUpperArm)
        {
            return Internal.FinchNode.GetPressDown(chirality, (int)element, isUpperArm);
        }

        protected static bool GetPressUp(Chirality chirality, RingElement element, bool isUpperArm)
        {
            return Internal.FinchNode.GetPressUp(chirality, (int)element, isUpperArm);
        }

        protected static float GetPressTime(Chirality chirality, RingElement element, bool isUpperArm)
        {
            return Internal.FinchNode.GetPressTime(chirality, (int)element, isUpperArm);
        }

        /// <summary>
        /// Returns true if the controller's element is pressed at the moment.
        /// </summary>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller element is pressed.</returns>
        public bool GetPress(RingElement element)
        {
            return FinchInput.GetPress(Node, element);
        }

        /// <summary>
        /// Returns true if the controller's element was pressed down.
        /// </summary>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller element was pressed down.</returns>
        public bool GetPressDown(RingElement element)
        {
            return FinchInput.GetPressDown(Node, element);
        }

        /// <summary>
        /// Returns true if the controller's element was released.
        /// </summary>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller's element was released.</returns>
        public bool GetPressUp(RingElement element)
        {
            return FinchInput.GetPressUp(Node, element);
        }

        /// <summary>
        /// Returns time in milliseconds how long controller's element was pressed.
        /// </summary>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>Time in milliseconds how long controller's element was pressed.</returns>
        public float GetPressTime(RingElement element)
        {
            return FinchInput.GetPressTime(Node, element);
        }

        /// <summary>
        /// True if the Home button is pressed at the moment.
        /// </summary>
        public bool HomeButton
        {
            get { return FinchInput.GetPress(Node, RingElement.HomeButton); }
        }

        /// <summary>
        /// True if the Home button was pressed down.
        /// </summary>
        public bool HomeButtonDown
        {
            get { return FinchInput.GetPressDown(Node, RingElement.HomeButton); }
        }

        /// <summary>
        /// True if the Home button was released.
        /// </summary>
        public bool HomeButtonUp
        {
            get { return FinchInput.GetPressUp(Node, RingElement.HomeButton); }
        }

        /// <summary>
        /// True if the Touchpad is touched at the moment.
        /// </summary>
        public bool IsTouching
        {
            get { return FinchInput.GetPress(Node, RingElement.Touch); }
        }

        /// <summary>
        /// True if the Touchpad was touched down.
        /// </summary>
        public bool TouchDown
        {
            get { return FinchInput.GetPressDown(Node, RingElement.Touch); }
        }

        /// <summary>
        /// True if the Touchpad was released.
        /// </summary>
        public bool TouchUp
        {
            get { return FinchInput.GetPressUp(Node, RingElement.Touch); }
        }

        /// <summary>
        /// Touchpad touch point coordinates.
        /// </summary>
        public Vector2 TouchAxes
        {
            get { return FinchInput.GetTouchAxes(Node); }
        }

        /// <summary>
        /// Trigger value.
        /// </summary>
        public float Analog
        {
            get { return FinchInput.GetAnalog(NodeChirality); }
        }

        /// <summary>
        /// True if the Trigger was pressed down.
        /// </summary>
        public bool AnalogDown
        {
            get { return FinchInput.GetPressDown(Node, RingElement.Analog); }
        }

        /// <summary>
        /// True if the Trigger was released.
        /// </summary>
        public bool AnalogUp
        {
            get { return FinchInput.GetPressUp(Node, RingElement.Analog); }
        }
        #endregion

        #region Haptic
        /// <summary>
        /// Defines controller vibration time in milliseconds. 
        /// </summary>
        /// <param name="node">Controller's chirality: left, right or both.</param>
        /// <param name="pattern">Vibration pattern.</param>
        public static void HapticPulse(NodeType node, FinchHapticPattern pattern)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Internal.FinchNodeManager.HapticPulse(node, pattern);
#endif
        }
        #endregion

        #region Touchpad
        /// <summary>
        /// True if user has swiped from left to right.
        /// </summary>
        public bool SwipeRight
        {
            get
            {
                return FinchInput.GetTouchpadEvent(Node, (Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight);
            }
        }

        /// <summary>
        /// True if user has swiped from right to left.
        /// </summary>
        public bool SwipeLeft
        {
            get
            {
                return FinchInput.GetTouchpadEvent(Node, (Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft);
            }
        }

        /// <summary>
        /// True if user has swiped from bottom to top.
        /// </summary>
        public bool SwipeTop
        {
            get
            {
                return FinchInput.GetTouchpadEvent(Node, (Internal.TouchpadEvents)RingTouchpadEvents.SwipeUp);
            }
        }

        /// <summary>
        /// True if user has swiped from top to bottom.
        /// </summary>
        public bool SwipeBottom
        {
            get
            {
                return FinchInput.GetTouchpadEvent(Node, (Internal.TouchpadEvents)RingTouchpadEvents.SwipeDown);
            }
        }

        /// <summary>
        /// True if user has tapped.
        /// </summary>
        public bool Tap
        {
            get
            {
                return FinchInput.GetTouchpadEvent(Node, (Internal.TouchpadEvents)RingTouchpadEvents.Tap);
            }
        }
        #endregion

        /// <summary>
        /// True if node is connected.
        /// </summary>
        public bool IsConnected
        {
            get { return FinchNodeManager.IsConnected(Node); }
        }

        /// <summary>
        /// Battery charge in percent.
        /// </summary>
        public ushort BatteryCharge
        {
            get { return FinchInput.GetBatteryCharge(Node); }
        }
    }

    /// <summary>
    /// Provides information about Finch controller: rotation, position, controller's elements states, battery level. Allows to control controller's haptic feedback (vibration).
    /// </summary>
    public class FinchController : FinchNode
    {
        /// <summary>
        /// Instance of the right Finch controller.
        /// </summary>
        public static readonly FinchController RightController = new FinchController(Chirality.Right, NodeType.RightHand);

        /// <summary>
        /// Instance of the left Finch controller.
        /// </summary>
        public static readonly FinchController LeftController = new FinchController(Chirality.Left, NodeType.LeftHand);

        /// <summary>
        /// Controller's chirality (left or right).
        /// </summary>
        public readonly Chirality Chirality;

        private FinchController(Chirality chirality, NodeType node)
        {
            NodeChirality = chirality;
            Chirality = chirality;
            Node = node;
        }

        /// <summary>
        /// Returns controller instance according to its chirality.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <returns>Finch Controller of specified chirality.</returns>
        public static FinchController GetController(Chirality chirality)
        {
            switch (chirality)
            {
                case Chirality.Left:
                    return LeftController;

                case Chirality.Right:
                    return RightController;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns position of the controller with specified chirality.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <returns>Position coordinates.</returns>
        public static Vector3 GetPosition(Chirality chirality)
        {
            return GetController(chirality).Position;
        }

        /// <summary>
        /// Returns rotation of the controller with specified chirality.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <returns>Controller's rotation quaternion.</returns>
        public static Quaternion GetRotation(Chirality chirality)
        {
            return GetController(chirality).Rotation;
        }

        /// <summary>
        /// Returns true if the controller's element is pressed at the moment.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller element is pressed.</returns>
        public static bool GetPress(Chirality chirality, RingElement element)
        {
            return GetPress(chirality, element, isUpperArm: false);
        }

        /// <summary>
        /// Returns true if the controller's element was pressed down.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller element was pressed down.</returns>
        public static bool GetPressDown(Chirality chirality, RingElement element)
        {
            return GetPressDown(chirality, element, isUpperArm: false);
        }

        /// <summary>
        /// Returns true if the controller's element was released.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>True if the controller's element was released.</returns>
        public static bool GetPressUp(Chirality chirality, RingElement element)
        {
            return GetPressUp(chirality, element, isUpperArm: false);
        }

        /// <summary>
        /// Returns time in milliseconds how long controller's element was pressed.
        /// </summary>
        /// <param name="chirality">Controller's chirality: left or right.</param>
        /// <param name="element">Element of the controller (button, touchpad, etc).</param>
        /// <returns>Time in milliseconds how long controller's element was pressed.</returns>
        public static float GetPressTime(Chirality chirality, RingElement element)
        {
            return GetPressTime(chirality, element, isUpperArm: false);
        }
    }

    /// <summary>
    /// Provides information about Finch Tracker: rotation, position, Tracker's elements states, battery level. Allows to control Tracker's haptic feedback (vibration).
    /// </summary>
    public class FinchTracker : FinchNode
    {
        /// <summary>
        /// Instance of the Right Finch tracker.
        /// </summary>
        public static readonly FinchTracker RightTracker = new FinchTracker(Chirality.Right, NodeType.RightUpperArm);

        /// <summary>
        /// Instance of the Left Finch tracker.
        /// </summary>
        public static readonly FinchTracker LeftTracker = new FinchTracker(Chirality.Left, NodeType.LeftUpperArm);

        public readonly Chirality Chirality;

        private FinchTracker(Chirality chirality, NodeType node)
        {
            NodeChirality = chirality;
            Chirality = chirality;
            Node = node;
        }

        /// <summary>
        /// Returns tracker instance according to its chirality.
        /// </summary>
        /// <param name="chirality">Tracker's chirality: left or right.</param>
        /// <returns>Finch Tracker of specified chirality.</returns>
        public static FinchTracker GetTracker(Chirality chirality)
        {
            switch (chirality)
            {
                case Chirality.Left:
                    return LeftTracker;

                case Chirality.Right:
                    return RightTracker;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns position of the tracker with specified chirality.
        /// </summary>
        /// <param name="chirality">Tracker's chirality: left or right.</param>
        /// <returns>Position coordinates.</returns>
        public static Vector3 GetPosition(Chirality chirality)
        {
            return GetTracker(chirality).Position;
        }

        /// <summary>
        /// Returns rotation of the tracker with specified chirality.
        /// </summary>
        /// <param name="chirality">Tracker's chirality: left or right.</param>
        /// <returns>Controller's rotation quaternion.</returns>
        public static Quaternion GetRotation(Chirality chirality)
        {
            return GetTracker(chirality).Rotation;
        }

        /// <summary>
        /// Returns true if the tracker's element is pressed at the moment.
        /// </summary>
        /// <param name="chirality">Tracker's chirality: left or right.</param>
        /// <param name="element">Element of the tracker (button, touchpad, etc).</param>
        /// <returns>True if the tracker element is pressed.</returns>
        public static bool GetPress(Chirality chirality, RingElement element)
        {
            return GetPress(chirality, element, isUpperArm: true);
        }

        /// <summary>
        /// Returns true if the tracker's element was pressed down.
        /// </summary>
        /// <param name="chirality">Tracker's chirality: left or right.</param>
        /// <param name="element">Element of the tracker (button, touchpad, etc).</param>
        /// <returns>True if the tracker element was pressed down.</returns>
        public static bool GetPressDown(Chirality chirality, RingElement element)
        {
            return GetPressDown(chirality, element, isUpperArm: true);
        }

        /// <summary>
        /// Returns true if the tracker's element was released.
        /// </summary>
        /// <param name="chirality">Tracker's chirality: left or right.</param>
        /// <param name="element">Element of the tracker (button, touchpad, etc).</param>
        /// <returns>True if the tracker's element was released.</returns>
        public static bool GetPressUp(Chirality chirality, RingElement element)
        {
            return GetPressUp(chirality, element, isUpperArm: true);
        }

        /// <summary>
        /// Returns time in milliseconds how long tracker's element was pressed.
        /// </summary>
        /// <param name="chirality">Tracker's chirality: left or right.</param>
        /// <param name="element">Element of the tracker (button, touchpad, etc).</param>
        /// <returns>Time in milliseconds how long tracker's element was pressed.</returns>
        public static float GetPressTime(Chirality chirality, RingElement element)
        {
            return GetPressTime(chirality, element, isUpperArm: true);
        }
    }
}
