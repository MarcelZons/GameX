using UnityEngine;
using UnityEngine.UI;
using WG.GameX.Enemy;
using WG.GameX.Managers;

namespace WG.GameX.Player
{
    public class EnemyWeakPointUiTargeter : MonoBehaviour
    {
        [SerializeField] private bool _useDefault = true;
        
        [SerializeField] private Transform _target;
        [SerializeField] private float _appearDistance;
        private RectTransform _rectTransform;
        private RectTransform _canvas;
        private Transform _playerTransform;
        private Image _image;
        private Camera _mainCamera;
        private Animator _animator;
        private static readonly int Appear = Animator.StringToHash("Appear");

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = transform.parent.GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            if (GetComponent<Animator>() != null)
                _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _playerTransform = DependencyMediator.Instance.PlayerShipController.transform;
            _mainCamera = DependencyMediator.Instance.MainCamera;
            
            if (_target!=null && _target.GetComponent<EnemyTurret>() != null)
                _appearDistance = _target.GetComponent<EnemyTurret>().ActivationDistance;
            else
            {
                _appearDistance = 200;
            }
        }

        private void Update()
        {
            if (Vector3.Distance(_playerTransform.position, _target.position) < _appearDistance)
            {
                if (_playerTransform.InverseTransformPoint(_target.position).z < 0)
                {
                    SetImageVisibilityState(false);
                }
                else
                {
                    SetImageVisibilityState(true);
                    _rectTransform.anchoredPosition = WorldToCanvasPosition(_canvas,_mainCamera, _target.position);                
                }                
            }
            else
            {
                SetImageVisibilityState(false);
            }
        }

        private void SetImageVisibilityState(bool state)
        {
            if (_animator != null)
            {
                _animator.SetBool(Appear, state);
            }
            else
            {
                _image.enabled = state;
            }
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

        private void OnValidate()
        {
            if (_useDefault)
            {
                if(_target== null)
                    _target = GetComponentInParent<EnemyWeakPoint>().transform;
                
                if (_target!=null && _target.GetComponent<EnemyTurret>()!= null)
                {
                    _appearDistance = _target.GetComponent<EnemyTurret>().ActivationDistance;
                }

                else
                {
                    _appearDistance = 200;
                }
            }
        }
    }
}