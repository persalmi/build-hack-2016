﻿using System;
using GalaSoft.MvvmLight;
using SnapFeud.WebApi.Models;

namespace SnapFeud.UWP
{
    public class SnapFeudContext : ViewModelBase
    {
#if DEBUG
        public readonly Uri WebApiBaseUri = new Uri("http://localhost:4699/");

        public readonly Uri LeaderBoardUri = new Uri("http://localhost:4699");
#else
        public readonly Uri WebApiBaseUri = new Uri("http://snapfeudwebapi.azurewebsites.net/");

        public readonly Uri LeaderBoardUri = new Uri("http://snapfeudwebapi.azurewebsites.net/");
#endif
        public string Title => "Snap Feud";

        public string UserName { get; set; }

        public string ResultText { get; set; }

        private Game _currentGame;
        public Game CurrentGame
        {
            get { return _currentGame; }
            set { Set(ref _currentGame, value); }
        }
    }
}
