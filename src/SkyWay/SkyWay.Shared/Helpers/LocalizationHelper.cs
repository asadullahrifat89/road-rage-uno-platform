﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Windows.Storage;

namespace SkyWay
{
    public static class LocalizationHelper
    {
        #region Fields

        private static LocalizationKey[] LOCALIZATION_KEYS;
        private static string _localizationJson;

        #endregion

        #region Properties
        public static string CurrentCulture { get; set; }

        #endregion

        #region Methods

        public static async void LoadLocalizationKeys()
        {
            if (_localizationJson.IsNullOrBlank())
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/localization.json"));
                _localizationJson = await FileIO.ReadTextAsync(file);
                LOCALIZATION_KEYS = JsonConvert.DeserializeObject<LocalizationKey[]>(_localizationJson);
#if DEBUG
                Console.WriteLine("Localization Keys Count:" + LOCALIZATION_KEYS.Length);
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["LOCALIZATION_KEYS"] = _localizationJson;
#endif
            }
        }

        public static string GetLocalizedResource(string resourceKey)
        {
            var localizationTemplate = LOCALIZATION_KEYS.FirstOrDefault(x => x.Key == resourceKey);
            return localizationTemplate?.CultureValues.FirstOrDefault(x => x.Culture == CurrentCulture).Value;
        }

        public static void SetLocalizedResource(UIElement uIElement)
        {
            var localizationTemplate = LOCALIZATION_KEYS.FirstOrDefault(x => x.Key == uIElement.Name);

            if (localizationTemplate is not null)
            {
                var value = localizationTemplate?.CultureValues.FirstOrDefault(x => x.Culture == CurrentCulture).Value;

                if (uIElement is TextBlock textBlock)
                    textBlock.Text = value;
                else if (uIElement is TextBox textBox)
                    textBox.Header = value;
                else if (uIElement is PasswordBox passwordBox)
                    passwordBox.Header = value;
                else if (uIElement is Button button)
                    button.Content = value;
                else if (uIElement is ToggleButton toggleButton)
                    toggleButton.Content = value;
                else if (uIElement is HyperlinkButton hyperlinkButton)
                    hyperlinkButton.Content = value;
                else if (uIElement is CheckBox checkBox)
                    checkBox.Content = value;
            }
        }

        public static void CheckLocalizationCache()
        {
            if (CacheHelper.GetCachedValue(Constants.CACHE_LANGUAGE_KEY) is string language)
                CurrentCulture = language;
        }

        public static void SaveLocalizationCache(string tag)
        {
            if (CacheHelper.GetCachedValue(Constants.COOKIE_KEY) is string cookie && cookie == "Accepted")
                CacheHelper.SetCachedValue(Constants.CACHE_LANGUAGE_KEY, tag);
        }

        #endregion        
    }
}
