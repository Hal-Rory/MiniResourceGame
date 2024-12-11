using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common.Utility
{
    public class CameraControl : MonoBehaviour
    {
        public float Sens = 1f;
        public float Speed = 3f;
        public float Threshold = .1f;
        public float DefaultFOV => 60;
        public float ZoomIn = 30;
        public float ZoomOut = 90;
        public Camera Cam;
        public Vector3 OriginalPosition;
        private Coroutine _zoomCO;
        private Coroutine _lerpCO;
        private Coroutine _wiggleCO;
        [SerializeField] private LayerMask _originalCullingMask;
        [SerializeField] private Transform _container;
        public bool IsLerping => _lerpCO != null;

        public void FollowTransform(Transform trans, Vector3 distance, float? speed)
        {
            CameraMoveToPosition(trans.TransformPoint(distance), Time.deltaTime * (speed ?? Speed), true);
        }
        public void MoveToTransform(Transform trans, Vector3 distance)
        {
            CameraMoveToPosition(trans.TransformPoint(distance), null);
        }
        
        private void CameraMoveToPosition(Vector3 position, float? speed, bool smooth = false)
        { 
            _container.position = smooth ? Vector3.Lerp(_container.position, position, speed ?? Speed) : position;
        }

        public void LookAt(Vector3 position, Vector3 axis, float? speed, bool smooth = false)
        {
            if (!smooth)
            {
                _container.LookAt(position);
                Vector3 lookAxis = _container.eulerAngles;
                lookAxis.x *= axis.x;
                lookAxis.y *= axis.y;
                lookAxis.z *= axis.z;
                _container.eulerAngles = lookAxis;
            }
            else
            {
                //TODO: this looks a little choppy
                _container.rotation = Quaternion.Lerp(_container.rotation,
                    Quaternion.LookRotation(position - _container.transform.position), Time.deltaTime * (speed ?? Speed));
            }
        }

        public void LerpToPosition(Vector3 position, float? speed, bool revert = false)
        {
            if (_lerpCO != null)
            {
                StopCoroutine(_lerpCO);
            }
            _lerpCO = StartCoroutine(CameraMoveCO(revert ? OriginalPosition : position, speed));
        }
        public void Zoom(bool zoomIn, bool revert = false)
        {
            if (_zoomCO != null)
            {
                StopCoroutine(_zoomCO);
            }
            _zoomCO = StartCoroutine(CameraZoom(zoomIn ? ZoomIn : revert ? DefaultFOV : ZoomOut));
        }

        private IEnumerator CameraZoom(float amount)
        {
            while (Mathf.Abs(Cam.fieldOfView - amount) > Threshold)
            {
                SetCameraZoom(amount, true, Time.deltaTime * Sens);
                yield return null;
            }
            _zoomCO = null;
        }

        private void SetCameraZoom(float amount, bool smooth = false, float sensitivity = 1)
        {
            Cam.fieldOfView = smooth ? Mathf.Lerp(Cam.fieldOfView, amount, sensitivity) : amount;
        }

        public void RevertView()
        {
            Cam.fieldOfView = DefaultFOV;
        }

        public void WiggleCamera(bool vertical, bool horizontal, float time, bool fade = true, float decreaseSpeed = 1,
            float intensity = .01f)
        {
            if (_wiggleCO != null) return;
            _wiggleCO = StartCoroutine(CameraWiggle(vertical, horizontal, time, fade, decreaseSpeed, intensity));
        }

        private IEnumerator CameraWiggle(bool vertical, bool horizontal, float time, bool fade = true, float decreaseSpeed = 1, float intensity = .01f)
        {
            float remaining = time;
            while ((remaining = Mathf.MoveTowards(remaining, 0, Time.deltaTime * decreaseSpeed)) != 0)
            {
                Vector3 shake = Random.insideUnitSphere * intensity;
                if (fade)
                {
                    shake *= (remaining / time);
                }
                shake.x = horizontal ? shake.x : 0;
                shake.y = vertical ? shake.y : 0;
                Cam.transform.localPosition = shake.FlattenZ();
                yield return null;
            }

            while (transform.localPosition != Vector3.zero)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 1);
            }

            _wiggleCO = null;
        }

        private IEnumerator CameraMoveCO(Vector3 position, float? speed)
        {
            while (Vector3.Distance(_container.position, position) > Threshold)
            {
                CameraMoveToPosition(position, Time.deltaTime * (speed ?? Speed), true);
                yield return null;
            }

            _lerpCO = null;
        }

        public void RevertPosition()
        {
            if(_lerpCO != null) StopCoroutine(_lerpCO);
            _container.position = OriginalPosition;
        }
        
        public void SetLastPosition()
        {
            OriginalPosition = _container.position;
        }

        public void RevertEffects()
        {
            if (_lerpCO != null)
            {
                StopCoroutine(_lerpCO);
                _lerpCO = null;
            }
            if (_wiggleCO != null)
            {
                StopCoroutine(_wiggleCO);
                _wiggleCO = null;
                transform.localPosition = Vector3.zero;
            }
            if (_zoomCO != null)
            {
                StopCoroutine(_zoomCO);
                _zoomCO = null;
            }
        }
        
        public void FullRevert()
        {
            RevertPosition();
            RevertView();
            RevertCulling();
        }

        public void ChangeCulling(LayerMask layers)
        {
            Cam.cullingMask = layers;
        }
        
        public void RevertCulling()
        {
            Cam.cullingMask = _originalCullingMask;
        }
    }
}
