using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WG.GameX.Managers;

namespace WG.GameX.Player
{
    public class EnemyShipUiTargeter : MonoBehaviour
    {
        [Header("Settings Input -------------")]
        [SerializeField] private float _appearDistance;
        [SerializeField] private float _hidingDistance;
        
        [Header("Target Object")]
        [SerializeField] private Transform _target;
        
        [Header("Canvas Transform")]
        [SerializeField] private RectTransform _canvasRectTransform;

        [Header("Distant Transform and Images")]
        [SerializeField] private RectTransform _distanceTargeterRectTransform;
        [SerializeField] private TextMeshProUGUI _distanceText;
        [SerializeField] private Image _targeterHealthBarImageFg;
        
        private Image _distanceTargeterImage;
        private Animator _distanceTargeterAnimator;
        private Transform _playerTransform;
        private Camera _mainCamera;
        private static readonly int Appear = Animator.StringToHash("Appear");
        private ObjectLocatorArrow _objectLocatorArrow;
        
        private bool _showTopUi;
        private TopUiDisplayEvent topUiDisplayEvent;

        public TopUiDisplayEvent TopUiDisplayEvent => topUiDisplayEvent;


        private void Awake()
        {
            _objectLocatorArrow = transform.parent.GetComponentInChildren<ObjectLocatorArrow>();
            _distanceTargeterImage = _distanceTargeterRectTransform.GetComponent<Image>();
            _distanceTargeterAnimator = _distanceTargeterRectTransform.GetComponent<Animator>();
            topUiDisplayEvent = new TopUiDisplayEvent();
        }

        private void Start()
        {
            _playerTransform = DependencyMediator.Instance.PlayerShipController.transform;
            _mainCamera = DependencyMediator.Instance.MainCamera;
        }

        private void Update()
        { 
            var distance = Vector3.Distance(_playerTransform.position, _target.position);
            ShowTopUi(distance);
            if (distance > _hidingDistance && distance < _appearDistance)
            {
                if (_playerTransform.InverseTransformPoint(_target.position).z < 0)
                {
                    SetImageVisibilityState(false);
                }
                else
                {
                    SetImageVisibilityState(true);
                    _distanceText.text = $"{(int)distance}m";
                    _distanceTargeterRectTransform.anchoredPosition = WorldToCanvasPosition(_canvasRectTransform, _mainCamera, _target.position);
                }
            }
            else
            {
                SetImageVisibilityState(false);
            }
        }

        private void ShowTopUi(float distance)
        {
            if (distance < _hidingDistance)
            {
                if (_showTopUi == false)
                {
                    topUiDisplayEvent.Invoke(true);
                    _showTopUi = true;
                }
            }
            else
            {
                if (_showTopUi == true)
                {
                    topUiDisplayEvent.Invoke(false);
                    _showTopUi = false;
                }
            }
            
        }

        private void SetImageVisibilityState(bool state)
        {
            if (_distanceTargeterAnimator != null)
            {
                _distanceTargeterAnimator.SetBool(Appear, state);
            }
            else
            {
                _distanceTargeterImage.enabled = state;
            }

            _distanceText.text = state ? _distanceText.text : ""; 
            _objectLocatorArrow.ShowLocator = !state;
        }

        private Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
        {
            Vector2 temp = camera.WorldToViewportPoint(position);
            temp.x *= canvas.sizeDelta.x;
            temp.y *= canvas.sizeDelta.y;

            temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
            temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

            return temp;
        }
    }
}