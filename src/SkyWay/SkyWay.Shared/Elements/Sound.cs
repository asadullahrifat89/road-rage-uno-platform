﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyWay
{
    public class Sound
    {
        #region Fields
        
        private readonly AudioPlayer _audioPlayer;
        private readonly Random random; 

        #endregion

        public Sound(SoundType soundType)
        {
            this.SoundType = soundType;

            random = new Random();
            var baseUrl = App.GetBaseUrl();

            switch (soundType)
            {
                case SoundType.INTRO:
                    {
                        var tracks = Constants.SOUND_TEMPLATES.Where(x => x.Key == soundType).ToArray();
                        var trackNum = random.Next(0, tracks.Length);
                        var track = tracks[trackNum];

                        var source = string.Concat(baseUrl, "/", track);

                        _audioPlayer = new AudioPlayer(
                            source: source,
                            volume: 0.5,
                            loop: true);
                    }
                    break;
                case SoundType.MENU_SELECT:
                    {

                    }
                    break;
                case SoundType.BACKGROUND:
                    {
                        var tracks = Constants.SOUND_TEMPLATES.Where(x => x.Key == soundType).ToArray();
                        var trackNum = random.Next(0, tracks.Length);
                        var track = tracks[trackNum];

                        var source = string.Concat(baseUrl, "/", track);

                        _audioPlayer = new AudioPlayer(
                            source: source,
                            volume: 0.4,
                            loop: true);
                    }
                    break;
                default:
                    {
                        var track = Constants.SOUND_TEMPLATES.FirstOrDefault(x => x.Key == soundType).Value;
                        var source = string.Concat(baseUrl, "/", track);
                        _audioPlayer = new AudioPlayer(source: source);
                    }
                    break;
            }
        }

        #region Properties

        public SoundType SoundType { get; set; }

        #endregion

        #region Methods

        public void Play()
        {
            _audioPlayer.Play();
        }

        public void Stop()
        {
            _audioPlayer.Stop();
        }

        public void Pause()
        {
            _audioPlayer.Pause();
        }

        public void Resume()
        {
            _audioPlayer.Resume();
        } 

        #endregion
    }

    public enum SoundType
    {
        MENU_SELECT,
        INTRO,
        BACKGROUND,
        POWER_UP,
        POWER_DOWN,
        HEALTH_GAIN,
        HEALTH_LOSS,
        COLLECTIBLE_COLLECTED,
        GAME_START,
        GAME_OVER,
    }
}
