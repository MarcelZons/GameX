using KoganeUnityLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WG.GameX.Managers
{
    public enum MessageType
    {
        Positive, Negative, Neutral
    }
    public class MessageBoxController: MonoBehaviour
    {
        [SerializeField] private Image _messageBoxBg;
        [SerializeField] private Sprite _positive, _negative, _neutral;
        [SerializeField] private Color _positiveColor, _negativeColor, _neutralColor;
        [SerializeField] private Animator _messageBoxAnimator;
        [SerializeField] private TextMeshProUGUI _messageBoxText;

        private TMP_Typewriter _inforamtionTextTypewriter;
        private static readonly int Appear = Animator.StringToHash("Appear");
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

        private void Awake()
        {
            _inforamtionTextTypewriter = _messageBoxText.gameObject.GetComponent<TMP_Typewriter>();
            _messageBoxText.text = "";
        }

        public void SetMessage(string message,MessageType messageType)
        {
            if (messageType == MessageType.Neutral)
            {
                _messageBoxBg.sprite = _neutral;
                _messageBoxText.faceColor = _neutralColor;
                _messageBoxText.fontSharedMaterial.SetColor(GlowColor, _neutralColor);
            }
            else if (messageType == MessageType.Positive)
            {
                _messageBoxBg.sprite = _positive;
                _messageBoxText.faceColor = _positiveColor;
                _messageBoxText.fontSharedMaterial.SetColor(GlowColor, _positiveColor);
            }
            else if (messageType == MessageType.Negative)
            {
                _messageBoxBg.sprite = _negative;
                _messageBoxText.faceColor = _negativeColor;
                _messageBoxText.fontSharedMaterial.SetColor(GlowColor, _negativeColor);
            }
            
            CancelInvoke(nameof(HideInformationText));
            _messageBoxAnimator.SetBool(Appear,true);
            _inforamtionTextTypewriter.Play(message.ToUpper(), 50, null);
            Invoke(nameof(HideInformationText), 3f);
        }
        
        

        private void HideInformationText()
        {
            _messageBoxText.text = "";
            _messageBoxAnimator.SetBool(Appear,false);
        }
    }
}