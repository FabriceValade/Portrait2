﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FVJson
{
    public class JsonValue : JsonToken, IEquatable<JsonValue>
    {
        public object Content { get; set; } = null;
        public void SetContent(bool value)
        {
            if (Content == null)
            {
                base.Type = TokenType.Boolean;
                Content = value;
                return;
            }
            if (base.Type != TokenType.Boolean)
                throw new ArgumentException("Worng type of JsonValue");
            Content = value;
        }
        public void SetContent(string value)
        {
            if (Content == null)
            {
                base.Type = TokenType.String;
                Content = value;
                return;
            }
            if (base.Type != TokenType.String)
                throw new ArgumentException("Worng type of JsonValue");
            Content = value;
        }
        public void SetContent(double value)
        {
            if (Content == null)
            {
                base.Type = TokenType.Double;
                Content = value;
                return;
            }
            if (base.Type != TokenType.Double)
                throw new ArgumentException("Worng type of JsonValue");
            Content = value;
        }
        public void SetContent(int value)
        {
            if (Content == null)
            {
                base.Type = TokenType.Integer;
                Content = value;
                return;
            }
            if (base.Type != TokenType.Integer)
                throw new ArgumentException("Worng type of JsonValue");
            Content = value;
        }
        public void SetContent(object obj)
        {
            switch (obj)
            {

                case string str:
                    this.SetContent(str);
                    break;
                case bool b:
                    this.SetContent(b);
                    break;
                case double d:
                    this.SetContent(d);
                    break;
                case int i:
                    this.SetContent(i);
                    break;
                default:
                    throw new ArgumentException("wrong type of content for this JsonValue");
            }
        }


        public JsonValue()
        {
        }
        public JsonValue(bool value)
        {
            this.Type = TokenType.Boolean;
            Content = value;
        }
        public JsonValue(string value)
        {
            this.Type = TokenType.String;
            Content = value;
        }
        public JsonValue(double value)
        {
            this.Type = TokenType.Double;
            Content = value;
        }
        public JsonValue(int value)
        {
            this.Type = TokenType.Integer;
            Content = value;
        }
        
        public JsonValue(string str, TokenType t)
        {
            Type = t;
            Content = str;
        }
        public JsonValue(Queue<TextToken> textQueue)
        {
            TextToken current = textQueue.Dequeue();
            switch (current.type)
            {
                case TextToken.TextTokenType.Double:
                    Type = TokenType.Double;
                    Content = Convert.ToDouble(current.content);
                    break;
                case TextToken.TextTokenType.False:
                    Type = TokenType.Boolean;
                    Content = false;
                    break;
                case TextToken.TextTokenType.Integer:
                    Type = TokenType.Integer;
                    Content = Convert.ToInt32(current.content);
                    break;
                case TextToken.TextTokenType.Reference:
                    Type = TokenType.Reference;
                    Content = current.content;
                    break;
                case TextToken.TextTokenType.String:
                    Type = TokenType.String;
                    Content = current.content;
                    break;
                case TextToken.TextTokenType.True:
                    Type = TokenType.Boolean;
                    Content = true;
                    break;
                default:
                    throw new FormatException("TextToken cannot be a value");
            }
        }

        public override string ToString()
        {
            if (Content == null)
                return null;
            else
                return Content.ToString();
        }
        public override string ToJsonString()
        {
            switch (Type)
            {
                case TokenType.Double:
                    return Content.ToString();
                case TokenType.Boolean:
                    return (bool)Content ? "true" : "false";
                case TokenType.Integer:
                    return Content.ToString();
                case TokenType.Reference:
                    return (string)Content;
                case TokenType.String:
                    return "\""+(string)Content + "\"";
                default:
                    throw new InvalidOperationException();
            }
        }
        public override string ToJsonString(int tab)
        {
            return this.ToJsonString();
        }
        public override JsonToken SelectFromQueuePath(Queue<string> path)
        {
            if (path.Count == 0)
                return this;
            else
                throw new ArgumentOutOfRangeException();
        }
        public override Dictionary<string, JsonToken> GetPathedChildrens()
        {
            return new Dictionary<String, JsonToken>() { { "", this } };
        }



        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return this.Equals(obj as JsonValue);
            }
        }
        public override int GetHashCode()
        {
            int hash = Content.ToString().GetHashCode() + Type.GetHashCode();
            return hash;
        }
        public bool Equals(JsonValue other)
        {
            bool test = (this.Type.Equals(other.Type) && this.Content.Equals(other.Content));
            return test;
        }
    }
}
