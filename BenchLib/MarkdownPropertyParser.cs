﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class MarkdownPropertyParser
    {
        #region Static Members

        private static readonly string YamlHeaderStart = "---";
        private static readonly Regex YamlHeaderEndExp = new Regex("^---|...$");
        private static readonly Regex CodeBlockExp = new Regex("^(?<preamble>```+|~~~+)");
        private static readonly string HtmlCommentStart = "<!--";
        private static readonly string HtmlCommentEnd = "-->";
        private static readonly Regex MdListExp = new Regex("^[\\*\\+-]\\s+(?<key>[a-zA-Z][a-zA-Z0-9]*?)\\s*:\\s*(?<value>.*?)\\s*$");
        private static readonly string MdListFormat = "* {0}: {1}";

        private static readonly Regex BooleanValueExp = new Regex("^true|false$", RegexOptions.IgnoreCase);
        private static readonly Regex TrueValueExp = new Regex("^true$", RegexOptions.IgnoreCase);
        private static readonly Regex ListValueExp = new Regex("^`.*?`(?:\\s*,\\s*`.*?`)+$");
        private static readonly char[] ListSeparator = { ',' };
        private static readonly string ListSeparatorStr = ", ";
        private static readonly Regex ListStartExp = new Regex("^[\\*\\+-]\\s+(?<key>[a-zA-Z][a-zA-Z0-9]*?)\\s*:\\s*$");
        private static readonly Regex ListElementPairExp = new Regex("^(?:\t|  +)[\\*\\+-]\\s+(?<key>.*?)\\s*:\\s*(?<value>.*?)\\s*$");
        private static readonly string ListElementPairFormat = "`{0}: {1}`";
        private static readonly Regex ListElementExp = new Regex("^(?:\t|  +)[\\*\\+-]\\s+(?<value>.*?)\\s*$");
        private static readonly string ListElementFormat = "`{0}`";

        private static readonly Regex DefaultGroupBeginCue = new Regex("^###\\s+(?<category>.+?)\\s*(?:\\{.*?\\})?\\s*(?:###)?$");
        private static readonly Regex DefaultGroupEndCue = new Regex("^\\s*$");

        private static readonly Regex DefaultCategoryCue = new Regex("^##\\s+(?<category>.+?)\\s*(?:\\{.*?\\})?\\s*(?:##)?$");

        private static bool IsYamlHeaderStart(int lineNo, string line)
        {
            if (lineNo > 0) return false;
            return line == YamlHeaderStart;
        }

        private static bool IsYamlHeaderEnd(int lineNo, string line)
        {
            if (lineNo == 0) return false;
            return YamlHeaderEndExp.IsMatch(line);
        }

        private static bool IsHtmlCommentStart(string line, int pos = 0)
        {
            var startP = line.LastIndexOf(HtmlCommentStart, pos);
            if (startP < 0) return false;
            return line.IndexOf(HtmlCommentEnd, startP) < 0;
        }

        private static bool IsHtmlCommentEnd(string line)
        {
            var endP = line.IndexOf(HtmlCommentEnd);
            if (endP < 0) return false;
            return !IsHtmlCommentStart(line, endP);
        }

        private static bool IsCodeBlockStart(string line, ref string preamble)
        {
            var m = CodeBlockExp.Match(line);
            if (m.Success)
            {
                preamble = m.Groups["preamble"].Value;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsCodeBlockEnd(string line, ref string preamble)
        {
            if (line.Trim() == preamble)
            {
                preamble = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsListStart(string line, ref string key)
        {
            var m = ListStartExp.Match(line);
            if (m.Success)
            {
                key = m.Groups["key"].Value;
                return true;
            }
            return false;
        }

        private static bool IsListElement(string line, IList<string> elements)
        {
            var m = ListElementPairExp.Match(line);
            if (m.Success)
            {
                var key = RemoveQuotes(m.Groups["key"].Value);
                var value = RemoveQuotes(m.Groups["value"].Value);
                elements.Add(string.Format(ListElementPairFormat, key, value));
                return true;
            }
            m = ListElementExp.Match(line);
            if (m.Success)
            {
                var value = RemoveQuotes(m.Groups["value"].Value);
                elements.Add(string.Format(ListElementFormat, value));
                return true;
            }
            return false;
        }

        private static string RemoveQuotes(string value)
        {
            value = value.Trim();
            if (value.StartsWith("`") && value.EndsWith("`") ||
                value.StartsWith("<") && value.EndsWith(">"))
            {
                return value.Substring(1, value.Length - 2);
            }
            else
            {
                return value;
            }
        }

        #endregion

        public Regex ActivationCue { get; set; }

        public Regex DeactivationCue { get; set; }

        /// <remarks>
        /// The regular expression needs a named group with name <c>category</c>.
        /// </remarks>
        public Regex CategoryCue { get; set; }

        /// <remarks>
        /// The regular expression needs a named group with name <c>group</c>.
        /// </remarks>
        public Regex GroupBeginCue { get; set; }

        public Regex GroupEndCue { get; set; }

        public IGroupedPropertyTarget Target { get; set; }

        public MarkdownPropertyParser()
        {
            ActivationCue = null;
            DeactivationCue = null;
            CategoryCue = DefaultCategoryCue;
            GroupBeginCue = DefaultGroupBeginCue;
            GroupEndCue = DefaultGroupEndCue;
        }

        public MarkdownPropertyParser(IGroupedPropertyTarget target)
            : this()
        {
            Target = target;
        }

        private void OnPropertyValue(string name, object value)
        {
            var target = Target;
            if (target != null)
            {
                target.SetValue(CurrentGroup, name, value);
            }
        }

        private int LineNo;
        private string CodePreamble;
        private string CurrentCategory;
        private string CurrentGroup;
        private string ListKey;
        private List<string> ListItems = new List<string>();

        private MdContext Context;

        public void Parse(Stream source)
        {
            Parse(new StreamReader(source, Encoding.UTF8));
        }

        public void Parse(TextReader source)
        {
            LineNo = 0;
            CodePreamble = null;
            CurrentGroup = null;
            Context = ActivationCue == null ? MdContext.Text : MdContext.Inactive;
            string line = null;
            while ((line = source.ReadLine()) != null)
            {
                ProcessLine(line);
                LineNo++;
            }
        }

        private void ProcessLine(string line)
        {
            switch (Context)
            {
                case MdContext.Inactive:
                    if (IsYamlHeaderStart(LineNo, line))
                        Context = MdContext.YamlHeader;
                    else if (IsHtmlCommentStart(line))
                        Context = MdContext.HtmlComment;
                    else if (IsCodeBlockStart(line, ref CodePreamble))
                        Context = MdContext.CodeBlock;
                    else if (IsActivationCue(line))
                        Context = MdContext.Text;
                    break;
                case MdContext.YamlHeader:
                    if (IsYamlHeaderEnd(LineNo, line))
                        Context = MdContext.Text;
                    break;
                case MdContext.HtmlComment:
                    if (IsHtmlCommentEnd(line))
                        Context = MdContext.Text;
                    break;
                case MdContext.CodeBlock:
                    if (IsCodeBlockEnd(line, ref CodePreamble))
                        Context = MdContext.Text;
                    break;
                case MdContext.Text:
                    if (IsYamlHeaderStart(LineNo, line))
                        Context = MdContext.YamlHeader;
                    else if (IsHtmlCommentStart(line))
                        Context = MdContext.HtmlComment;
                    else if (IsCodeBlockStart(line, ref CodePreamble))
                        Context = MdContext.CodeBlock;
                    else if (IsDeactivationCue(line))
                        Context = MdContext.Inactive;
                    else if (IsListStart(line, ref ListKey))
                        Context = MdContext.List;
                    else
                        ProcessTextLine(line);
                    break;
                case MdContext.List:
                    if (!IsListElement(line, ListItems))
                    {
                        var listValue = string.Join(ListSeparatorStr, ListItems.ToArray());
                        ProcessTextLine(string.Format(MdListFormat, ListKey, listValue));
                        ListKey = null;
                        ListItems.Clear();
                        Context = MdContext.Text;
                        ProcessLine(line);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsActivationCue(string line)
        {
            return ActivationCue != null && ActivationCue.IsMatch(line);
        }

        private bool IsDeactivationCue(string line)
        {
            return DeactivationCue != null && DeactivationCue.IsMatch(line);
        }

        private void ProcessTextLine(string line)
        {
            if (CategoryCue != null)
            {
                var cm = CategoryCue.Match(line);
                if (cm.Success)
                {
                    CurrentCategory = cm.Groups["category"].Value;
                    return;
                }
            }
            if (GroupBeginCue != null)
            {
                var gm = GroupBeginCue.Match(line);
                if (gm.Success)
                {
                    CurrentGroup = gm.Groups["group"].Value;
                    if (CurrentCategory != null)
                    {
                        Target.SetGroupCategory(CurrentGroup, CurrentCategory);
                    }
                    return;
                }
            }
            if (GroupEndCue != null)
            {
                var gm = GroupEndCue.Match(line);
                if (gm.Success)
                {
                    CurrentGroup = null;
                    return;
                }
            }
            var m = MdListExp.Match(line);
            if (m.Success)
            {
                var key = m.Groups["key"].Value;
                var value = m.Groups["value"].Value;
                if (ListValueExp.IsMatch(value))
                {
                    var values = value.Split(ListSeparator, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = RemoveQuotes(values[i]);
                    }
                    OnPropertyValue(key, values);
                }
                else
                {
                    value = RemoveQuotes(value);
                    if (BooleanValueExp.IsMatch(value))
                    {
                        OnPropertyValue(key, TrueValueExp.IsMatch(value));
                    }
                    else
                    {
                        OnPropertyValue(key, value);
                    }
                }
            }
        }

        private enum MdContext
        {
            Inactive,
            YamlHeader,
            Text,
            CodeBlock,
            HtmlComment,
            List
        }
    }
}
