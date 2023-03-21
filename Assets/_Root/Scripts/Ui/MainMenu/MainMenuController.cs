using Tool;
using Profile;
using Services;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Tables;
using TMPro;

namespace Ui
{
    internal class MainMenuController : BaseController
    {
        private readonly ResourcePath _resourcePath = new ResourcePath("Prefabs/Ui/MainMenu");
        private readonly ProfilePlayer _profilePlayer;
        private readonly MainMenuView _view;

        private int _languageIndex = 0;
        private const int _enIndex = 0;
        private const int _ruIndex = 2;

        public MainMenuController(Transform placeForUi, ProfilePlayer profilePlayer)
        {
            _profilePlayer = profilePlayer;
            _view = LoadView(placeForUi);
            _view.Init(StartGame, OpenSettings, OpenShed, PlayRewardedAds, 
                BuyProduct, OpenDailyReward, ExitGame, ChangeLanguage);

            SubscribeAds();
            SubscribeIAP();
        }

        protected override void OnDispose()
        {
            UnsubscribeAds();
            UnsubscribeIAP();
        }


        private MainMenuView LoadView(Transform placeForUi)
        {
            GameObject prefab = ResourcesLoader.LoadPrefab(_resourcePath);
            GameObject objectView = Object.Instantiate(prefab, placeForUi, false);
            AddGameObject(objectView);

            return objectView.GetComponent<MainMenuView>();
        }

        private void StartGame() =>
            _profilePlayer.CurrentState.Value = GameState.Game;

        private void OpenSettings() =>
            _profilePlayer.CurrentState.Value = GameState.Settings;

        private void OpenShed() =>
            _profilePlayer.CurrentState.Value = GameState.Shed;

        private void PlayRewardedAds() =>
            ServiceRoster.AdsService.RewardedPlayer.Play();

        private void BuyProduct(string productId) =>
            ServiceRoster.IAPService.Buy(productId);

        private void OpenDailyReward() =>
            _profilePlayer.CurrentState.Value = GameState.DailyReward;

        private void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        
        private void ChangeLanguage()
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_languageIndex];
            _languageIndex = _languageIndex == _enIndex ? _ruIndex : _enIndex;
            foreach (var locale in _view.LocolizedTextsDictionary)
                UpdateTextAsync(locale.Key, locale.Value);
        }

        private void UpdateTextAsync(string tag, TMP_Text changeText) =>
            LocalizationSettings.StringDatabase.GetTableAsync(_view.TableName).Completed +=
                handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        StringTable table = handle.Result;
                        changeText.text = table.GetEntry(tag)?.GetLocalizedString();
                    }
                    else
                    {
                        string errorMessage = $"[{GetType().Name}] Could not load String Table: {handle.OperationException}";
                        Debug.LogError(errorMessage);
                    }
                };

        private void SubscribeAds()
        {
            ServiceRoster.AdsService.RewardedPlayer.Finished += OnAdsFinished;
            ServiceRoster.AdsService.RewardedPlayer.Failed += OnAdsCancelled;
            ServiceRoster.AdsService.RewardedPlayer.Skipped += OnAdsCancelled;
        }

        private void UnsubscribeAds()
        {
            ServiceRoster.AdsService.RewardedPlayer.Finished -= OnAdsFinished;
            ServiceRoster.AdsService.RewardedPlayer.Failed -= OnAdsCancelled;
            ServiceRoster.AdsService.RewardedPlayer.Skipped -= OnAdsCancelled;
        }

        private void SubscribeIAP()
        {
            ServiceRoster.IAPService.PurchaseSucceed.AddListener(OnIAPSucceed);
            ServiceRoster.IAPService.PurchaseFailed.AddListener(OnIAPFailed);
        }

        private void UnsubscribeIAP()
        {
            ServiceRoster.IAPService.PurchaseSucceed.RemoveListener(OnIAPSucceed);
            ServiceRoster.IAPService.PurchaseFailed.RemoveListener(OnIAPFailed);
        }

        private void OnAdsFinished() => Log("You've received a reward for ads!");
        private void OnAdsCancelled() => Log("Receiving a reward for ads has been interrupted!");

        private void OnIAPSucceed() => Log("Purchase succeed");
        private void OnIAPFailed() => Log("Purchase failed");
    }
}
