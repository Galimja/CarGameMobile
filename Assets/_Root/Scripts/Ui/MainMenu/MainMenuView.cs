using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

namespace Ui
{
    
    internal class MainMenuView : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string _productId;

        [Header("Buttons")]
        [SerializeField] private Button _buttonStart;
        [SerializeField] private Button _buttonSettings;
        [SerializeField] private Button _buttonShed;
        [SerializeField] private Button _buttonAdsReward;
        [SerializeField] private Button _buttonBuyProduct;
        [SerializeField] private Button _buttonDailyReward;
        [SerializeField] private Button _exitGame;
        [SerializeField] private Button _changeLanguage;

        [Header("Locolized text")]
        [SerializeField] private string _tableName;
        [SerializeField] private string[] _tags;
        [SerializeField] private TMP_Text[] _texts;

        public string TableName { get => _tableName; private set => _tableName = value; }
        public Dictionary<string, TMP_Text> LocolizedTextsDictionary { get; set; } = new Dictionary<string, TMP_Text>();

        private void Awake()
        {
            if (_tags.Length != _texts.Length)
            {
                Debug.LogError("Wrong localize data");
                return;
            }

            for (int i = 0; i < _texts.Length; i++)
            {
                LocolizedTextsDictionary.Add(_tags[i], _texts[i]);
            }
        }

        public void Init(UnityAction startGame, UnityAction openSettings,
            UnityAction openShed, UnityAction playRewardedAds, 
            UnityAction<string> buyProduct, UnityAction openDailyReward, 
            UnityAction exitGame, UnityAction changeLanguage)
        {
            _buttonStart.onClick.AddListener(startGame);
            _buttonSettings.onClick.AddListener(openSettings);
            _buttonShed.onClick.AddListener(openShed);
            _buttonAdsReward.onClick.AddListener(playRewardedAds);
            _buttonBuyProduct.onClick.AddListener(() => buyProduct(_productId));
            _buttonDailyReward.onClick.AddListener(openDailyReward);
            _exitGame.onClick.AddListener(exitGame);
            _changeLanguage.onClick.AddListener(changeLanguage);
        }

        public void OnDestroy()
        {
            _buttonStart.onClick.RemoveAllListeners();
            _buttonSettings.onClick.RemoveAllListeners();
            _buttonShed.onClick.RemoveAllListeners();
            _buttonAdsReward.onClick.RemoveAllListeners();
            _buttonBuyProduct.onClick.RemoveAllListeners();
            _buttonDailyReward.onClick.RemoveAllListeners();
            _exitGame.onClick.RemoveAllListeners();
            _changeLanguage.onClick.RemoveAllListeners();
        }
    }
}
