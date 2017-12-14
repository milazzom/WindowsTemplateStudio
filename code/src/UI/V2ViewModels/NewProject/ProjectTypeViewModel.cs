﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Templates.Core.Mvvm;
using Microsoft.Templates.UI.V2Services;
using Microsoft.Templates.UI.V2ViewModels.Common;
using System;

namespace Microsoft.Templates.UI.V2ViewModels.NewProject
{
    public class ProjectTypeViewModel : Observable
    {
        private BasicInfoViewModel _selected;

        public BasicInfoViewModel Selected
        {
            get => _selected;
            set
            {
                SetProperty(ref _selected, value);
                ProjectTypeChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<BasicInfoViewModel> ProjectTypeChanged;

        public ObservableCollection<BasicInfoViewModel> Items { get; } = new ObservableCollection<BasicInfoViewModel>();

        public ProjectTypeViewModel()
        {
        }

        public void LoadData()
        {
            if (DataService.LoadProjectTypes(Items))
            {
                Selected = Items.First();
            }
        }
    }
}
