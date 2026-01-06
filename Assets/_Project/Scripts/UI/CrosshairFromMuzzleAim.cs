using UnityEngine;
using UnityEngine.UI;

namespace Cubergy.Player
{
    public sealed class CrosshairFromMuzzleAim : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;

        [Header("Muzzles")]
        [SerializeField] private Transform _form1Muzzle;
        [SerializeField] private Transform _form2Muzzle;

        [Header("Camera")]
        [SerializeField] private Camera _uiCamera; // для Screen Space - Camera. Для Overlay можно оставить null

        [Header("Aim")]
        [SerializeField] private LayerMask _aimMask = ~0;
        [SerializeField] private float _range = 70f;
        [SerializeField] private float _form1Range = 70f;
        [SerializeField] private float _form2Range = 70f;
        [SerializeField] private Transform _form2AimSource;
        [Header("Visual")]
        [SerializeField] private Image _image;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _targetColor = Color.red;

        [Header("Ignore Self")]
        [SerializeField] private Transform _selfRoot;

        private RectTransform _rect;
        private Canvas _canvas;
        private RectTransform _canvasRect;
        private readonly RaycastHit[] _hits = new RaycastHit[32];

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _canvas = GetComponentInParent<Canvas>();
            _canvasRect = _canvas != null ? (RectTransform)_canvas.transform : null;

            if (_selfRoot == null)
                _selfRoot = _forms != null ? _forms.transform : null;
            if (_image == null)
                _image = GetComponent<Image>();

            if (_image != null)
                _image.color = _defaultColor;
        }

        private void LateUpdate()
        {
            float range = GetRange();
            
            Transform muzzle = GetMuzzle();
            if (muzzle == null)
                return;

            Vector3 origin = muzzle.position;
            Vector3 dir = (_forms != null && _forms.CurrentForm == PlayerForm.Form2 && _form2AimSource != null)
                ? _form2AimSource.forward
                : muzzle.forward;

            Vector3 aimPoint;
            bool isTarget;

            if (RaycastIgnoreSelf(origin, dir, range, out RaycastHit hit))
            {
                aimPoint = hit.point;
                isTarget = hit.collider != null &&
                           hit.collider.GetComponentInParent<Cubergy.Combat.TargetHealth>() != null;
            }
            else
            {
                aimPoint = origin + dir * range;
                isTarget = false;
            }

            UpdateColor(isTarget);

            Camera camForWorldToScreen = _uiCamera != null ? _uiCamera : Camera.main;
            if (camForWorldToScreen == null)
                return;

            Vector3 screen = camForWorldToScreen.WorldToScreenPoint(aimPoint);
            if (screen.z <= 0.01f)
                return;

            if (_canvas == null || _canvasRect == null)
            {
                _rect.position = screen;
                return;
            }

            Camera eventCam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camForWorldToScreen;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screen, eventCam, out Vector2 local))
                _rect.anchoredPosition = local;
        }

        private void UpdateColor(bool isTarget)
        {
            if (_image == null)
                return;

            _image.color = isTarget ? _targetColor : _defaultColor;
        }

        private Transform GetMuzzle()
        {
            if (_forms == null)
                return _form1Muzzle;

            return _forms.CurrentForm == PlayerForm.Form1 ? _form1Muzzle : _form2Muzzle;
        }
        private float GetRange()
        {
            if (_forms != null && _forms.CurrentForm == PlayerForm.Form1)
                return _form1Range > 0f ? _form1Range : _range;

            return _form2Range > 0f ? _form2Range : _range;
        }

        private bool IsSelf(Collider col)
        {
            return col != null && _selfRoot != null && col.transform.IsChildOf(_selfRoot);
        }

        private bool RaycastIgnoreSelf(Vector3 origin, Vector3 dir, float maxDistance, out RaycastHit bestHit)
        {
            int count = Physics.RaycastNonAlloc(new Ray(origin, dir), _hits, maxDistance, _aimMask, QueryTriggerInteraction.Ignore);

            int bestIndex = -1;
            float bestDist = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                RaycastHit h = _hits[i];
                if (IsSelf(h.collider))
                    continue;

                if (h.distance < bestDist)
                {
                    bestDist = h.distance;
                    bestIndex = i;
                }
            }

            if (bestIndex >= 0)
            {
                bestHit = _hits[bestIndex];
                return true;
            }

            bestHit = default;
            return false;
        }
    }
}
