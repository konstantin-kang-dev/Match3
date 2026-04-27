using Game.UI;
using TMPro;
using UnityEngine;
using Game.Utils;

namespace Game
{
    public class PlayerLevelUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelTMP;
        [SerializeField] private TextMeshProUGUI _expTMP;
        [SerializeField] private TextMeshProUGUI _requiredExpTMP;
        [SerializeField] private ProgressBarUI _progressBar;

        public void SetLevel(int level)
        {
            _levelTMP.text = level.ToString();
        }

        public void SetExp(float current, float required)
        {
            _expTMP.text = ProjectUtils.FormatNumber(current);
            _requiredExpTMP.text = ProjectUtils.FormatNumber(required);
            _progressBar.SetValue(current / required);
        }
    }
}