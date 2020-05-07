﻿
using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredValue<T>: MonitoredField<T> where T:SSJson 
    {
        public JsonValue Content { get; private set; }
        public override bool Modified { get => this.IsModified(); }

        private JsonValue _Modification;
        public JsonValue Modification 
        { 
            get => _Modification;
            set 
            {
                SetAndNotify(ref _Modification, value);
                Content.SetContent(value.Content);
                NotifyOfPropertyChange(nameof(Content));
                NotifyOfPropertyChange(nameof(Modified));
            }
                
        }

        public bool HasMultipleSourceFile { get; private set; } = false;
        public MonitoredValue() : base()
        {
            Content = new JsonValue();
        }
        public MonitoredValue(JsonValue content)
        {
            Content = content;
        }
        public void Reset()
        {
            Modification = null;
            Resolve();
        }
        override public void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.Fields.ContainsKey(FieldPath) == true
                                   select new { modName = f.SourceMod.ModName , value = f.Fields[FieldPath], file = f};
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file } ;
                JsonToken TokenResult = Ordered.FirstOrDefault()?.value;
                if (Ordered.Count() >1)
                {
                    HasMultipleSourceFile = true;
                    NotifyOfPropertyChange(nameof(HasMultipleSourceFile));
                }
                if (Modification != null)
                    TokenResult = Modification;
                if (TokenResult is JsonValue value)
                    Content.SetContent(value.Content);
                else if (TokenResult == null)
                    Content.SetContent(null);
                else
                    throw new ArgumentException("Path leads to wrong type of token");
                NotifyOfPropertyChange(nameof(Content));
                T FileResult = Ordered.FirstOrDefault()?.file;
            }
        }
        public override JsonToken GetJsonEquivalent()
        {
            return Content;
        }
        public override JsonToken GetJsonEquivalentNoOverwrite()
        {
            return Modification;
        }

        public override bool IsModified()
        {
            return Modification != null;
        }
        public override bool RequiresOverwrite()
        {
            if (Modification != null && HasMultipleSourceFile)
                return true;
            else
                return false;
        }

        protected override void ResolveAdd(T file)
        {
            Resolve();
        }

        protected override void ResolveRemove(T file)
        {
            Resolve();
        }

        public override string ToString()
        {
            return base.FieldPath+": "+ Content.ToString();
        }

        public override Dictionary<string, MonitoredField<T>> GetPathedChildrens()
        {
            return new Dictionary<String, MonitoredField<T>>() { { "", this } };
        }
    }
}
