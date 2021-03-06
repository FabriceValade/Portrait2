﻿using FVJson;
using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface
{
    public class MonitoredValueViewModel : Screen 
    {
        public MonitoredValue MonitoredValue { get; set; }
        private List<IEventBinding> binding = new List<IEventBinding>();
        public MonitoredValueViewModel(MonitoredValue monitoredValue)
        {
            MonitoredValue = monitoredValue;
            if (MonitoredValue != null)
                binding.Add(MonitoredValue.Bind(x => x.Content, (sender, arg) =>
            {
                NotifyOfPropertyChange(nameof(Value));
                NotifyOfPropertyChange(nameof(ValueWarning));
            }));
        }
        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            base.OnClose();
        }
        public string Value
        {
            get { return MonitoredValue?.Content?.ToString(); }
            set
            {
                switch (MonitoredValue?.GoalType)
                {
                    case JsonToken.TokenType.Double:
                    case JsonToken.TokenType.Integer:
                        
                        try
                        {
                            if (MonitoredValue?.Content?.Content != null)
                                if (Convert.ToDouble(value) == (double)MonitoredValue?.Content?.Content)
                                    return;
                            MonitoredValue?.Modify(MonitoredValueModification.GetReplaceModification(new JsonValue(Convert.ToDouble(value))));
                        }catch(FormatException)
                        {
                            NotifyOfPropertyChange(nameof(Value));
                        }
                        break;
                    case JsonToken.TokenType.Reference:
                        MonitoredValue?.Modify(MonitoredValueModification.GetReplaceModification(new JsonValue(value,JsonToken.TokenType.Reference)));
                        break;
                    case JsonToken.TokenType.String:
                        MonitoredValue?.Modify(MonitoredValueModification.GetReplaceModification(new JsonValue(value)));
                        break;
                    case JsonToken.TokenType.Boolean:
                        throw new NotImplementedException("Havent done boolean yet");
                    default:
                        throw new InvalidOperationException("Value type is improperly set");
                }

            }
        }
        public void Reset()
        {
            MonitoredValue?.Reset();
        }
        public string ValueWarning { get => MonitoredValue?.HasMultipleSourceFile ?? false ? "Has multiple source" : null; }
    }
}
