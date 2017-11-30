﻿using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace Param_ItemNamespace.Models
{
    public class SharedDataStorageItemsModel : SharedDataModelBase
    {
        public List<ImageSource> Images { get; } = new List<ImageSource>();

        public SharedDataStorageItemsModel()
            : base()
        {
        }
    }
}
