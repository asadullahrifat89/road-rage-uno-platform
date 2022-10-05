﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RoadRage
{
    public static class Constants
    {
        public const string PLAYER_TAG = "player";

        public const string CAR_TAG = "car";        

        public const string POWERUP_TAG = "powerup";
        public const string HEALTH_TAG = "health";

        public const string ROADMARK_TAG = "roadmark";

        public const double CarWidth = 60;
        public const double CarHeight = 120;

        public const double PlayerWidth = 70;
        public const double PlayerHeight = 130;

        public const double PowerUpWidth = 30;
        public const double PowerUpHeight = 30;

        public const double RoadMarkWidth = 30;
        public const double RoadMarkHeight = 80;

        public static Uri[] CAR_TEMPLATES = new Uri[]
        {
            new Uri("ms-appx:///Assets/Images/car1.png"),
            new Uri("ms-appx:///Assets/Images/car2.png"),
            new Uri("ms-appx:///Assets/Images/car3.png"),
            new Uri("ms-appx:///Assets/Images/car4.png"),
            new Uri("ms-appx:///Assets/Images/car5.png"),            
        };

        public static Uri POWERUP_TEMPLATE = new Uri("ms-appx:///Assets/Images/powerup.png");
        public static Uri HEALTH_TEMPLATE = new Uri("ms-appx:///Assets/Images/health.png");

        public static Uri PLAYER_TEMPLATE = new Uri("ms-appx:///Assets/Images/player.png");
    }
}
