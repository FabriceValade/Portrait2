﻿using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using Stylet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class FactionEditorViewModel : Screen
    {
        private FactionEditorFactory FactionEditorFactory { get; set; }
        public FactionEditor FactionEditor { get; private set; }
        public List<SSFactionGroup> Factions { get => FactionEditor.Factions; }
        public FactionEditorViewModel(FactionEditorFactory factionEditorFactory)
        {
            FactionEditorFactory = factionEditorFactory;
        }

        protected override void OnActivate() 
        {
            FactionEditor = FactionEditorFactory.CreateFactionEditor();
            //FactionEditor.GetFaction();
            //NotifyOfPropertyChange(nameof(Factions));
        }

        public FactionGroupViewModel SelectedFactionViewModel { get => new FactionGroupViewModel(SelectedFaction, FactionEditor.PortraitsRessource); }
        SSFactionGroup _SelectedFaction;
        public SSFactionGroup SelectedFaction
        {
            get => this._SelectedFaction;
            set
            {
                this._SelectedFaction = value;
                NotifyOfPropertyChange(nameof(SelectedFactionViewModel));
            }
        }

    }
    [ValueConversion(typeof(SSFactionGroup), typeof(FactionGroupTokenViewModel))]
    public class FactionToFactionVMConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FactionGroupTokenViewModel((SSFactionGroup)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}