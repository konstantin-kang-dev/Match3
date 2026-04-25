using Game.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Utils;

namespace Game
{
    public class PlayerLevelUIView: MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _levelTMP;
        [SerializeField] TextMeshProUGUI _expTMP;
        [SerializeField] TextMeshProUGUI _requiredExpTMP;
        [SerializeField] ProgressBarUI _progressBar;

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
