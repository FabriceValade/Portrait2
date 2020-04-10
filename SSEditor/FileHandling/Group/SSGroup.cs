﻿using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public abstract class SSGroup<T> : ISSGroup where T: ISSMergable
    {
        public ReadOnlyObservableCollection<T> CommonFilesReadOnly { get; private set; }
        protected ObservableCollection<T> CommonFiles { get;} = new ObservableCollection<T>();

        public SSRelativeUrl CommonRelativeUrl { get; private set; }

        public bool MustOverwrite { get; set; } = true;

        public SSGroup()
        {
            CommonFilesReadOnly = new ReadOnlyObservableCollection<T>(CommonFiles);
        }

        public void Add(T file)
        {
            if (file.LinkRelativeUrl == null || file.LinkRelativeUrl.Link == null || file.LinkRelativeUrl.Relative == null)
                throw new ArgumentException("Cannot add file with no path to group");
            if (CommonFiles.Count() == 0)
            {
                CommonRelativeUrl = file.LinkRelativeUrl.GetRelative();
                CommonFiles.Add(file);
            } else
            {
                if (!file.LinkRelativeUrl.GetRelative().Equals(CommonRelativeUrl))
                    throw new ArgumentException("Cannot add file with unrelated path to group");
                CommonFiles.Add(file);
            }
        }

        public void Remove(T file)
        {
            CommonFiles.Remove(file);
            if (CommonFiles.Count() == 0)
                CommonRelativeUrl = null;
        }

        public abstract void WriteMergeTo(SSBaseLinkUrl newPath);

        public override string ToString()
        {
            return "Group of (" + CommonFilesReadOnly.Count.ToString()+ ") " + (CommonRelativeUrl?.ToString() ?? "no file");
        }
    }

    
}