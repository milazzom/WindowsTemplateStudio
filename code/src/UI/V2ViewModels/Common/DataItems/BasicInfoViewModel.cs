﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Mvvm;
using Microsoft.Templates.UI.V2Services;
using Microsoft.Templates.UI.V2Views.NewProject;

namespace Microsoft.Templates.UI.V2ViewModels.Common
{
    public class BasicInfoViewModel : Observable
    {
        private string _title;
        private string _description;
        private string _author;
        private string _icon;
        private int _order;
        private RelayCommand _detailsCommand;
        private RelayCommand _goBackCommand;

        public string Name { get; protected set; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public RelayCommand DetailsCommand => _detailsCommand ?? (_detailsCommand = new RelayCommand(OnDetails));

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(OnGoBack));

        private void OnDetails()
        {
            NavigationService.NavigateMainFrame(new TemplateInfoPage(this));
        }

        private void OnGoBack()
        {
            NavigationService.GoBackMainFrame();
        }
    }
}
