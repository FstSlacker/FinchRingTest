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

namespace Finch
{
    /// <summary>
    /// Tracks controller's rotation and position.
    /// </summary>
    public class FinchControllerTracked : MonoBehaviour
    {
        [Header("Chirality")]
        /// <summary>
        /// Controller's chirality: left or right.
        /// </summary>
        public Chirality Chirality;

        [Header("Tracked options")]
        /// <summary>
        /// If update object's position or not.
        /// </summary>
        public bool TrackPosition = true;

        /// <summary>
        /// If update object's rotation or not.
        /// </summary>
        public bool TrackRotation = true;

        private void Update()
        {
            if (TrackPosition)
            {
                transform.position = FinchController.GetPosition(Chirality);
            }

            if (TrackRotation)
            {
                transform.rotation = FinchController.GetRotation(Chirality);
            }
        }
    }
}
