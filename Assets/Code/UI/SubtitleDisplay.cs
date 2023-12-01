using System;
using System.Collections;
using System.IO;
using BeauUtil;
using BeauUtil.Tags;
using Leaf;
using Leaf.Defaults;
using TMPro;
using UnityEngine;

namespace WeatherStation {
    public class SubtitleDisplay : MonoBehaviour, ITextDisplayer {
        public TMP_Text CharacterLabel;
        public TMP_Text Text;
		
		[NonSerialized] public float ClipDisplayLength = 0f;
		
        private void Start() {
            gameObject.SetActive(false);
            CharacterLabel.gameObject.SetActive(false);
        }
		
        public IEnumerator CompleteLine() {
            yield return 1;
            gameObject.SetActive(false);
			CharacterLabel.gameObject.SetActive(false);
        }

        public TagStringEventHandler PrepareLine(TagString inString, TagStringEventHandler inBaseHandler) {
            Text.SetText(inString.RichText);
            return null;
        }

        public IEnumerator TypeLine(TagString inSourceString, TagTextData inType) {
            gameObject.SetActive(true);
			CharacterLabel.gameObject.SetActive(true);
			if (inSourceString.TryFindEvent(LeafUtils.Events.Character, out TagEventData charData)) {
                StringHash32 charId = charData.GetStringHash();
				CharacterLabel.SetText(charId.ToDebugString() + ":");
			}
			
			if(ClipDisplayLength == 0f) {
				yield return inType.VisibleCharacterCount * 0.095f;
			} else {
				yield return ClipDisplayLength;
			}
        }
		
		public IEnumerator TypeLineString(string inHeader, string inText) {
            gameObject.SetActive(true);
			Text.SetText(inText);
			CharacterLabel.gameObject.SetActive(true);
			CharacterLabel.SetText(inHeader + ":");
			if(ClipDisplayLength == 0f) {
				yield return inText.Length * 0.095f;
			} else {
				yield return ClipDisplayLength;
			}
        }
    }
}