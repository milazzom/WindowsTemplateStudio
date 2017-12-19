﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Templates.UI.V2Services;
using Microsoft.Templates.UI.V2ViewModels.Common;

namespace Microsoft.Templates.UI.V2ViewModels.NewProject
{
    public enum TemplateOrigin
    {
        Layout,
        UserSelection
    }

    public enum InitializationResult
    {
        Success,
        SelectionResetRequested
    }

    public class UserSelection
    {
        private bool _hasItemsAddedByUser;
        private bool _isInitialized;

        public ObservableCollection<SavedTemplateViewModel> Pages { get; } = new ObservableCollection<SavedTemplateViewModel>();

        public ObservableCollection<SavedTemplateViewModel> Features { get; } = new ObservableCollection<SavedTemplateViewModel>();

        public UserSelection()
        {
        }

        public InitializationResult Initialize(string projectTypeName, string frameworkName)
        {
            if (_isInitialized && !_hasItemsAddedByUser)
            {
                Pages.Clear();
                Features.Clear();
            }
            else if (_isInitialized && _hasItemsAddedByUser)
            {
                // TODO mvegaca Temporary reset;
                _hasItemsAddedByUser = false;
                return Initialize(projectTypeName, frameworkName);
                //return InitializationResult.SelectionResetRequested;
            }

            var layout = GenComposer.GetLayoutTemplates(projectTypeName, frameworkName);
            foreach (var item in layout)
            {
                if (item.Template != null)
                {
                    var templateInfo = new TemplateInfoViewModel(item.Template, GenComposer.GetAllDependencies(item.Template, frameworkName));
                    Add(TemplateOrigin.Layout, templateInfo, item.Layout.Name);
                }
            }

            _isInitialized = true;
            return InitializationResult.Success;
        }

        public IEnumerable<string> GetNames() => Pages.Select(t => t.Name)
                                                    .Concat(Features.Select(f => f.Name));

        public SavedTemplateViewModel Add(TemplateOrigin templateOrigin, TemplateInfoViewModel template, string name = null)
        {
            var items = template.TemplateType == Core.TemplateType.Page ? Pages : Features;

            var newTemplate = new SavedTemplateViewModel()
            {
                Icon = template.Icon,
            };

            if (!string.IsNullOrEmpty(name))
            {
                newTemplate.Name = name;
            }
            else
            {
                newTemplate.Name = ValidationService.InferTemplateName(template.Name, template.ItemNameEditable, true);
            }

            items.Add(newTemplate);
            if (templateOrigin == TemplateOrigin.UserSelection)
            {
                _hasItemsAddedByUser = true;
            }

            return newTemplate;
        }
    }
}
