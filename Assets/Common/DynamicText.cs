using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Common.Utility
{
    [Serializable]
    public class DynamicText
    {
        public string OriginalText;
        public string PrintText { get; private set; }
        public bool IsPrinting { get; private set; }
        public Action<bool> OnIsPrinting;
        /// <summary>
        /// Has the last value to print been printed?
        /// </summary>
        public bool IsLast { get; private set; }
        public float Timing;
        /// <summary>
        /// Is the timing for the last value different from the regular timing?
        /// </summary>
        [Tooltip("A pause after the last value. If not pausing, leave at 0")]
        public float LastTiming;
        public bool WholeWords;
        [Tooltip("Add the next value to the current values " +
                 "or replace the previous values with the next value")]
        public bool ReplaceWords;
        public bool Split;
        public bool KeepSpaces;

        public void SetPrint(string original)
        {
            OriginalText = original;
        }
        
        public IEnumerator StartPrint()
        {
            IsPrinting = true;
            IsLast = false;
            PrintText = string.Empty;
            float timing = Timing;
            List<string> printText = new List<string>();
            int index = 0;
            if (Split)
            {
                string regex = KeepSpaces ? WholeWords ? @"({\d})|(\s)" : @"({\d})|(\S)" :
                    WholeWords ? @"({\d})|\s" : @"({\d})|\S";
                string[] words = Regex.Split(OriginalText, regex);
                printText.AddRange(words);
            }
            else
            {
                printText.Add(OriginalText);
            }

            OnIsPrinting?.Invoke(IsPrinting);
            while (index < printText.Count && IsPrinting)
            {
                PrintText = ReplaceWords ? printText[index++] : PrintText += printText[index++];
                IsLast = index >= printText.Count;
                if (IsLast && LastTiming > 0)
                {
                    timing = LastTiming;
                }
                yield return new WaitForSeconds(timing);
            }

            if (!IsPrinting) yield break;
            IsPrinting = false;
            StopDialogue();
        }

        public void StopDialogue()
        {
            IsPrinting = false;
            OnIsPrinting?.Invoke(IsPrinting);
        }

        public bool FinishedPrinting()
        {
            return !IsPrinting;
        }

        public void Clear()
        {
            PrintText = string.Empty;
        }
    }
}
