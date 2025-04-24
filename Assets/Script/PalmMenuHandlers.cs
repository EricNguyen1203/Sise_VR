/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using TMPro;
using UnityEngine;

namespace Oculus.Interaction.Samples.PalmMenu
{
    /// <summary>
    /// Example of a bespoke behavior created to react to a particular palm menu. This controls the state
    /// of the object that responds to the menu, but also parts of the menu itself, specifically those
    /// which depend on the state of the controlled object (swappable icons, various text boxes, etc.).
    /// </summary>
    public class PalmMenuHandlers : MonoBehaviour
    {
        [SerializeField]
        private GameObject _screenView;

        [SerializeField]
        private GameObject _feedbackView;

        [SerializeField]
        private GameObject _imageFocusView;

        [SerializeField]
        private GameObject _screenEnabledIcon;

        [SerializeField]
        private GameObject _screenDisabledIcon;

        [SerializeField]
        private GameObject _feedbackEnabledIcon;

        [SerializeField]
        private GameObject _feedbackDisabledIcon;

        [SerializeField]
        private GameObject[] _rotationDirectionIcons;

        private bool _screenEnabled;
        private bool _feedbackEnabled;

        private Vector3 _targetPosition;


        private void Start()
        {
            _screenEnabled = false;
            _feedbackEnabled = false;
        }

        private void Update()
        {

        }

        public void ToggleScreenEnabled()
        {
            _screenEnabled = !_screenEnabled;
            _screenEnabledIcon.SetActive(_screenEnabled);
            _screenDisabledIcon.SetActive(!_screenEnabled);
            _screenView.SetActive(_screenEnabled);
        }

        public void ToggleFeedbackEnabled()
        {
            _feedbackEnabled = !_feedbackEnabled;
            _feedbackEnabledIcon.SetActive(_feedbackEnabled);
            _feedbackDisabledIcon.SetActive(!_feedbackEnabled);
            _feedbackView.SetActive(_feedbackEnabled);
            _imageFocusView.SetActive(!_feedbackEnabled);
        }
    }
}