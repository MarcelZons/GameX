using UnityEngine;
using UnityEngine.UI;
using WG.GameX.Enemy;
using WG.GameX.Managers;
using WG.GameX.Util;

namespace WG.GameX.Player
{
    public class ObjectLocatorArrow : MonoBehaviour
    {
        [SerializeField] private float _heightThreashold = .2f;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _enemy;
        
        private const int _rotationDown = 270;
        private const int _rotationUp = 90;
        private const int _rotationRight = 0;
        private const int _rotationLeft = 180;
        
        public RectTransform _canvas;
        private RectTransform _rectTransform;
        private Image _image;
        public bool ShowLocator { get; set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();
            _image = GetComponent<Image>();

        }

        private void Start()
        {
            _player = DependencyMediator.Instance.PlayerShipController.transform;
            _enemy = GetComponentInParent<EnemyShipController>().transform;
        }

        private void Update()
        {
            if (ShowLocator == false)
            {
                _image.enabled = false;
                return;
            }

            _image.enabled = true;
                
            
            var inv = _player.InverseTransformPoint(_enemy.position).normalized;
            var pos = Vector2.zero;
            var rotation = _rectTransform.eulerAngles;

            if (Mathf.Abs(inv.y) < _heightThreashold) // if player ship and enemy at the same y level- show Left and Right Indicator
            {
                if (inv.x < 0)
                {
                    pos.x = 0;
                    rotation.z = _rotationLeft;
                }
                else
                {
                    pos.x = 2;
                    rotation.z = _rotationRight;
                }
                
                pos.y = inv.y.Remap(-1, 1, .1f, 1.9f);
            }
            else
            {
                if (inv.y < 0)
                {
                    rotation.z = _rotationDown;
                    pos.y = 0;
                }
                else
                {
                    rotation.z = _rotationUp;
                    pos.y = 2;
                }

                pos.x = inv.x.Remap(0, 2, .1f, 1.9f);
            }
            
            _rectTransform.position = pos * _canvas.sizeDelta * _canvas.pivot;
            _rectTransform.eulerAngles = rotation;
        }
    }
}