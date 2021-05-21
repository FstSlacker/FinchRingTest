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

using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Finch
{
    /// <summary>
    /// Describes the type of the controller.
    /// </summary>
    public enum ControllerType : byte
    {
        /// <summary>3DoF and 6DoF FinchRing (hands-free) controller.</summary>
        Ring = Internal.FinchCore.Finch_ControllerType.Ring
    }

    /// <summary>
    /// Data update type, according to HMD device type.
    /// </summary>
    public enum UpdateType : byte
    {
        /// <summary>
        /// Only rotation is used for tracking.
        /// </summary>
        HmdRotation = Internal.FinchCore.Finch_UpdateOption.Internal | Internal.FinchCore.Finch_UpdateOption.HmdOrientation,

        /// <summary>
        /// Both position and rotation are used for tracking. Use this option if you have headset that allows head tracking (for example, HTC Vive).
        /// </summary>
        HmdTransform = Internal.FinchCore.Finch_UpdateOption.Internal | Internal.FinchCore.Finch_UpdateOption.HmdOrientation | Internal.FinchCore.Finch_UpdateOption.HmdPosition
    }

    /// <summary>
    /// Defines parameters for FinchCore. Type of controller tracking depends on these settings.
    /// </summary>
    [Serializable]
    public class FinchInitialSettings
    {
        /// <summary>
        /// Defines Finch controller type according to Finch device that you are going to use.
        /// </summary>
        public ControllerType ControllerType = ControllerType.Ring;

        /// <summary>
        /// Defines Finch update type according to HMD type you are going to use.
        /// </summary>
        public UpdateType UpdateType = UpdateType.HmdTransform;
    }

    internal static class Converter
    {
        internal static Internal.FinchCore.Finch_Vector2 ToFinch(UnityEngine.Vector2 uv)
        {
            return new Internal.FinchCore.Finch_Vector2(uv.x, uv.y);
        }

        internal static UnityEngine.Vector2 ToUnity(this Internal.FinchCore.Finch_Vector2 fv)
        {
            return new UnityEngine.Vector2((float)fv.X, (float)fv.Y);
        }

        internal static Internal.FinchCore.Finch_Vector3 ToFinch(UnityEngine.Vector3 uv)
        {
            return new Internal.FinchCore.Finch_Vector3(uv.x, uv.y, uv.z);
        }

        internal static UnityEngine.Vector3 ToUnity(this Internal.FinchCore.Finch_Vector3 fv)
        {
            return new UnityEngine.Vector3((float)fv.X, (float)fv.Y, (float)fv.Z);
        }

        internal static Internal.FinchCore.Finch_Quaternion ToFinch(UnityEngine.Quaternion uv)
        {
            return new Internal.FinchCore.Finch_Quaternion(uv.x, uv.y, uv.z, uv.w);
        }

        internal static UnityEngine.Quaternion ToUnity(this Internal.FinchCore.Finch_Quaternion fq)
        {
            return new UnityEngine.Quaternion((float)fq.X, (float)fq.Y, (float)fq.Z, (float)fq.W);
        }
    }

    /// <summary>
    /// Main Finch class. Provides Finch data updates - provides data for FinchCore and gets data from FinchCore according to FinchSettings.
    /// </summary>
    public class Finch : MonoBehaviour
    {
        /// <summary>
        /// Provides versioning information for the Finch SDK for Unity.
        /// </summary>
        public static readonly string FinchSDKVersion = "1.0.2";

        /// <summary>
        /// Character's head position and rotation quaternions.
        /// </summary>
        [Tooltip("Leave this field empty if you want to use data of Camera.main")]
        public Transform Head;

        /// <summary>
        /// Current Finch settings. Defines used controllers type, head position update type, character's root rotation type and controller's rotation recenter type.
        /// </summary>
        public FinchInitialSettings Settings = new FinchInitialSettings();

        public bool InstantiateControllers = true;

        public GameObject RightControllerInstance;
        public GameObject LeftControllerInstance;

        private void Awake()
        {
            if (Settings != null && !Internal.Settings.ReplyManager.IsHmdRoot((Internal.FinchCore.Finch_UpdateOption)Settings.UpdateType))
            {
                UnityEngine.XR.InputTracking.disablePositionalTracking = true;
            }

            FindHead();

            if (InstantiateControllers)
            {
                ControllersInstantiation();
            }

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Internal.FinchBase.Init((Internal.FinchCore.Finch_ControllerType)Settings.ControllerType, (Internal.FinchCore.Finch_UpdateOption)Settings.UpdateType);
        }

        private void OnApplicationQuit()
        {
            Internal.FinchBase.Exit();
        }

        private void LateUpdate()
        {
            FindHead();
            Internal.FinchBase.Update(Converter.ToFinch(Head.position), Converter.ToFinch(Head.rotation), Time.time);

            UpdateHmdPosition();
        }

        private void UpdateHmdPosition()
        {
            if ((Head != null) && !Internal.Settings.ReplyManager.IsHmdRoot(Internal.FinchCore.UpdateType))
            {
                Head.transform.position = Internal.FinchInput.GetPosition(Bone.HeadBase).ToUnity();
            }
        }

        private void FindHead()
        {
            if (Head == null)
            {
                Head = Camera.main.transform;
            }
        }

        /// <summary>
        /// Instantiate Finch Controllers prefab
        /// </summary>
        public virtual void ControllersInstantiation()
        {
            if (RightControllerInstance)
            {
                Instantiate(RightControllerInstance);
            }

            if (LeftControllerInstance)
            {
                Instantiate(LeftControllerInstance);
            }
        }
    }
}
