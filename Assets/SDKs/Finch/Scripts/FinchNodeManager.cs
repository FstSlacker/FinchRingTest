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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Finch
{
    /// <summary>
    /// Provides functions to manage nodes.
    /// </summary>
    public static class FinchNodeManager
    {
        /// <summary>
        /// Current controller connection type.
        /// </summary>
        public static ControllerType ControllerType { get { return (ControllerType)Internal.FinchCore.ControllerType; } }

        /// <summary>
        /// Is scanner scanning right now.
        /// </summary>
        public static bool IsScanning { get { return Internal.FinchNodeManager.IsScanning; } }

        public static Internal.FinchCore.Finch_ScannerType ScannerType
        {
            get
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return Internal.FinchCore.Finch_ScannerType.Bonded;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS
                return Internal.FinchCore.Finch_ScannerType.BA;
#elif UNITY_ANDROID
                return Internal.FinchCore.Finch_ScannerType.BA;
#endif
            }
        }

        /// <summary>
        /// Start scanner to connect nodes.
        /// </summary>
        /// <returns>True if scanner start was succesful.</returns>
        public static bool StartScan()
        {
            return Internal.FinchNodeManager.StartScan(ScannerType);
        }

        /// <summary>
        /// Stop the scanner.
        /// </summary>
        public static void StopScan()
        {
            Internal.FinchNodeManager.StopScan();
        }

        /// <summary>
        /// Updates information about connection state of Finch controller nodes.
        /// </summary>
        public static void UpdateConnectionStates()
        {
            Internal.FinchNodeManager.UpdateConnectionStates();
        }

        /// <summary>
        /// Returns true if node is disconected by user.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>True if node was disconnected by user.</returns>
        public static bool IsExternalDisconnect(NodeType node)
        {
            return Internal.FinchNodeManager.IsExternalDisconnect(node);
        }

        #region NodesConnectionData
        /// <summary>
        /// Returns connected nodes count.
        /// </summary>
        /// <returns>The number of nodes connected.</returns>
        public static int GetNodesCount()
        {
            return Internal.FinchNodeManager.GetNodesCount();
        }

        /// <summary>
        /// Returns connected UpperArms count.
        /// </summary>
        /// <returns>The number of UpperArms connected.</returns>
        public static int GetUpperArmCount()
        {
            return Internal.FinchNodeManager.GetTrackersCount();
        }

        /// <summary>
        /// Returns connected controllers count.
        /// </summary>
        /// <returns>The number of controllers connected.</returns>
        public static int GetControllersCount()
        {
            return Internal.FinchNodeManager.GetControllersCount();
        }

        /// <summary>
        /// Returns connected controller chirality.
        /// </summary>
        /// <returns>Chirality of the controller connected.</returns>
        public static Chirality GetControllerConectionChirality()
        {
            return Internal.FinchNodeManager.GetControllerConectionChirality();
        }

        /// <summary>
        /// Returns connected UpperArm chirality.
        /// </summary>
        /// <returns>What chirality of UpperArm was connected.</returns>
        public static Chirality GetUpperArmConectionChirality()
        {
            return Internal.FinchNodeManager.GetUpperArmConectionChirality();
        }
        #endregion

        #region LedState
        private const int ledBit = 12;
        private const int ledChirality = 10;

        /// <summary>
        /// Returns true if LEDs are turned on.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>True if LEDs are turned on</returns>
        public static bool GetLedState(NodeType node)
        {
            return Internal.FinchNodeManager.GetLedState(node);
        }

        /// <summary>
        /// Changes state of LEDs.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <param name="on">LEDs state.</param>
        /// <returns>True if LEDs state change was succesful.</returns>
        public static bool SetLedState(NodeType node, bool on)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Internal.FinchNodeManager.SetLedState(node, on);
#else
            return true;
#endif
        }

        /// <summary>
        /// Returns LEDs chirality.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>Chirality of LEDs.</returns>
        public static Chirality GetLedChirality(NodeType node)
        {
            return Internal.FinchNodeManager.GetLedChirality(node);
        }

        /// <summary>
        /// Changes LEDs chirality state.
        /// </summary>
        /// <param name="node">Certain controller cirality.</param>
        /// <param name="chirality">LEDs chirality.</param>
        /// <returns>True if LEDs chirality change was succesful.</returns>
        public static bool SetLedChirality(NodeType node, Chirality chirality)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Internal.FinchNodeManager.SetLedChirality(node, chirality);
#else
            return true;
#endif
        }
        #endregion

        #region SwapNodes
        /// <summary>
        /// Swaps nodes.
        /// </summary>
        /// <param name="firstNode">First node to swap.</param>
        /// <param name="secondNode">Second node to swap.</param>
        public static void SwapNodes(NodeType firstNode, NodeType secondNode)
        {
            Internal.FinchNodeManager.SwapNodes(firstNode, secondNode);
        }
        #endregion

        #region NodeState
        /// <summary>
        /// Checks if node is connected.
        /// </summary>
        /// <param name="node">Certain node.</param>
        /// <returns>True if certain node is connected.</returns>
        public static bool IsConnected(NodeType node)
        {
            return Internal.FinchNodeManager.IsConnected(node);
        }

        /// <summary>
        /// Sends command to the node to power it off.
        /// </summary>
        /// <param name="node">Certain node.</param>
        public static void PowerOff(NodeType node)
        {
            Internal.FinchNodeManager.PowerOff(node);
        }

        /// <summary>
        /// Disconnects node.
        /// </summary>
        /// <param name="node">Certain node.</param>
        public static void DisconnectNode(NodeType node)
        {
            Internal.FinchNodeManager.DisconnectNode(node);
        }
        #endregion
    }
}
