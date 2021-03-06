﻿using FVJson;

using SSEditor.FileHandling;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredArray: MonitoredField 
    {
        public ObservableCollection<JsonToken> ContentArray { get; } = new ObservableCollection<JsonToken>();
        public override bool Modified { get => this.IsModified(); }
        public ObservableCollection<JsonToken> GetOriginalContent()
        {
            ObservableCollection<JsonToken> result = new ObservableCollection<JsonToken>();
            if (FieldPath != null)
            {
                ResolveBase(result);
            }
            return result;
        }
        public List<MonitoredArrayModification> GetAddedMod()
        {
            var list = from MonitoredArrayModification m in ModificationCollection
                       where m.ModType == MonitoredArrayModification.ModificationType.Add
                       select m;
            return list.ToList();
        }
        private ObservableCollection<MonitoredArrayModification> ModificationCollection {get;} = new ObservableCollection<MonitoredArrayModification>();

        public void Modify(MonitoredArrayModification mod)
        {
            ContentArray.ApplyModification(mod);
            ModificationCollection.AddArrayMod(mod);
            NotifyOfPropertyChange(nameof(Modified));
        }

        public void ResetModification()
        {
            ModificationCollection.Clear();
            Resolve();
            NotifyOfPropertyChange(nameof(Modified));
        }

        private void ResolveBase(ObservableCollection<JsonToken> modified)
        {
            if (FieldPath != null)
            {
                var fileArrayPair = from f in Files
                                    where f.Fields.ContainsKey(FieldPath) == true
                                    select new { value = f.Fields[FieldPath], file = f };
                modified.Clear();
                foreach (var pair in fileArrayPair)
                {
                    switch (pair.value)
                    {
                        case JsonArray jArray:
                            foreach (JsonToken data in jArray.Values)
                            {
                                modified.Add(data);
                            }
                            break;
                        case JsonValue jValue:
                            modified.Add(jValue);
                            break;
                        default:
                            throw new ArgumentException("Path leads to non array token");
                    }
                }
            }
        }
        public override void Resolve()
        {
            if (FieldPath != null)
            {
                ResolveBase(ContentArray);
                foreach (var mod in ModificationCollection)
                    ContentArray.ApplyModification(mod);
            }
            
        }
        public override JsonToken GetJsonEquivalent()
        {
            JsonArray jArray = new JsonArray();
            foreach (JsonToken data in ContentArray)
            {
                jArray.Values.Add(data);
            }
            return jArray; 
        }
        public override JsonToken GetJsonEquivalentNoOverwrite()
        {
            ObservableCollection<JsonToken> NoOverwriteContentArray = new ObservableCollection<JsonToken>();
            var AddMod = from MonitoredArrayModification m in ModificationCollection
                         where m.ModType == MonitoredArrayModification.ModificationType.Add
                         select m;
            foreach (MonitoredArrayModification m in AddMod)
                NoOverwriteContentArray.ApplyModification(m);
            JsonArray jArray = new JsonArray();
            foreach (JsonToken data in NoOverwriteContentArray)
            {
                jArray.Values.Add(data);
            }
            if (jArray.Values.Count == 0)
                return null;
            return jArray;
        }

        public override bool IsModified()
        {
            return ModificationCollection.Count() > 0 ? true : false;
        }

        public override bool RequiresOverwrite()
        {
            var test = (from MonitoredArrayModification m in ModificationCollection
                        where m.ModType != MonitoredArrayModification.ModificationType.Add
                        select m).FirstOrDefault();
            return test == null ? false : true;
        }

        protected override void ResolveAdd(ISSJson file)
        {
            Resolve();
        }

        protected override void ResolveRemove(ISSJson file)
        {
            Resolve();
        }
        public override string ToString()
        {
            return base.FieldPath + " Array: (" + this.ContentArray.Count.ToString() + ")value, first one: " + (this.ContentArray.FirstOrDefault()?.ToString() ?? "none") ;
        }

        public override IEnumerable<GroupModification> GetModification()
        {
            if (Modified)
            {
                List<GroupModification> result = new List<GroupModification>();
                foreach (MonitoredArrayModification mod in ModificationCollection)
                {
                    GroupModification GMod = new GroupModification()
                    {
                        Modification = mod,
                        FieldPath = this.FieldPath
                    };
                    result.Add(GMod);
                }
                return result;
            }
            else
                return new List<GroupModification>();
        }


        public override Dictionary<string, MonitoredField> GetPathedChildrens()
        {
            return new Dictionary<String, MonitoredField>() { { "", this } };
        }
    }
}
