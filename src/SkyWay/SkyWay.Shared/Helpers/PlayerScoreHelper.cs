﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SkyWay
{
    public static class PlayerScoreHelper
    {
        public static SkyWayScore PlayerScore { get; set; }

        public static bool GameScoreSubmissionPending { get; set; }
    }
}
