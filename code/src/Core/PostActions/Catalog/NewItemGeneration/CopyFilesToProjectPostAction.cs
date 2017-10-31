﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Templates.Core.Gen;

namespace Microsoft.Templates.Core.PostActions.Catalog
{
    public class CopyFilesToProjectPostAction : PostAction<TempGenerationResult>
    {
        public CopyFilesToProjectPostAction(TempGenerationResult config)
            : base(config)
        {
        }

        internal override void ExecuteInternal()
        {
            foreach (var file in Config.ModifiedFiles)
            {
                var sourceFile = Path.Combine(GenContext.Current.OutputPath, file);
                var destFilePath = Path.Combine(GenContext.Current.SolutionPath, file);

                var destDirectory = Path.GetDirectoryName(destFilePath);
                Fs.SafeCopyFile(sourceFile, destDirectory, true);
                if (Path.GetExtension(file).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    Gen.GenContext.ToolBox.Shell.RefreshProject();
                    GenContext.ToolBox.Shell.SaveSolution();
                    GenContext.ToolBox.Shell.CleanSolution();
                }
            }

            foreach (var file in Config.NewFiles)
            {
                var sourceFile = Path.Combine(GenContext.Current.OutputPath, file);
                var destFilePath = Path.Combine(GenContext.Current.SolutionPath, file);

                var destDirectory = Path.GetDirectoryName(destFilePath);
                Fs.SafeCopyFile(sourceFile, destDirectory, true);

                // Add to projectItems to add to project later
                GenContext.Current.ProjectItems.Add(destFilePath);
                GenContext.Current.FilesToOpen.Add(destFilePath);
            }
        }
    }
}
