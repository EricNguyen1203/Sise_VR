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
        private GameObject _deskTable;
        [SerializeField]
        private GameObject _deskSurface;

        [SerializeField]
        private GameObject _feedbackView;
        [SerializeField]
        private GameObject _feedbackButton;

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
        #region Degree Rotation
        [SerializeField]
        private TMP_Text _rotationDegreeText;

        [SerializeField]
        private float _rotationLerpSpeed = 1f;

        [SerializeField]
        private float _rotationChangeIncrement;
        #endregion

        #region Elevation
        [SerializeField]
        private TMP_Text _elevationText;

        [SerializeField]
        private float _elevationChangeIncrement;

        [SerializeField]
        private float _elevationChangeLerpSpeed = 1f;
        #endregion

        private bool _screenEnabled;
        private bool _feedbackEnabled;

        private Vector3 _targetPosition;
        private Vector3 _targetRotation;


        private void Start()
        {
            _screenEnabled = false;
            _feedbackEnabled = false;

            _targetPosition = _deskTable.transform.position;
            _targetRotation = _deskSurface.transform.rotation.eulerAngles;
            IncrementElevation(true);
            IncrementElevation(false);
            IncrementRotationDegree(true);
            IncrementRotationDegree(false);
        }

        private void Update()
        {
            _deskTable.transform.position = Vector3.Lerp(_deskTable.transform.position, _targetPosition, _elevationChangeLerpSpeed * Time.deltaTime);
            _deskSurface.transform.rotation = Quaternion.Lerp(_deskSurface.transform.rotation, Quaternion.Euler(_targetRotation), _rotationLerpSpeed * Time.deltaTime);
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
            _feedbackButton.SetActive(_feedbackEnabled);
            _imageFocusView.SetActive(!_feedbackEnabled);
            if (_feedbackEnabled)
            {
                SetLayerRecursively(_feedbackView, LayerMask.NameToLayer("Default"));
            }
            else
            {
                SetLayerRecursively(_feedbackView, LayerMask.NameToLayer("UI"));
            }
        }

        void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
        public void IncrementElevation(bool up)
        {
            Debug.Log("Incrementing elevation: " + (up ? "up" : "down"));
            float increment = _elevationChangeIncrement;
            if (!up)
            {
                increment *= -1f;
            }
            _targetPosition = new Vector3(_targetPosition.x, Mathf.Clamp(_targetPosition.y + increment, 0.1f, 1f), _targetPosition.z);
            _elevationText.text = "Elevation: " + _targetPosition.y.ToString("0.00");
        }
        
        public void IncrementRotationDegree(bool clockwise)
        {
            Debug.Log("Incrementing rotation degree: " + (clockwise ? "clockwise" : "counter-clockwise"));
            float increment = _rotationChangeIncrement;
            if (!clockwise)
            {
                increment *= -1f;
            }
            float currentRotation = _targetRotation.x;
            Debug.Log("Current rotation: " + currentRotation);
            currentRotation = Mathf.Clamp(currentRotation + increment, 0f, 360f);
            _targetRotation = new Vector3(currentRotation, _targetRotation.y,  _targetRotation.z);
            _rotationDegreeText.text =  "Degree: " + (_targetRotation.x - 360f).ToString("0.0");
    }
}
}