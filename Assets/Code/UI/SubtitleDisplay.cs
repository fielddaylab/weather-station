using System;
using System.Collections;
using System.IO;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Tags;
using Leaf;
using Leaf.Defaults;
using TMPro;
using UnityEngine;

namespace WeatherStation {
    public class SubtitleDisplay : MonoBehaviour, ITextDisplayer {
        public bool SubtitlesOn = true;
		public TMP_Text CharacterLabel;
        public TMP_Text Text;
		
		[NonSerialized] public bool ForceExit = false;
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
		
		public void SetOn(bool on) {
			SubtitlesOn = on;
			gameObject.SetActive(SubtitlesOn);
			CharacterLabel.gameObject.SetActive(SubtitlesOn);
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
			
			//Debug.Log(Text.text);
			
			if(ForceExit) {
				ForceExit = false;
				yield return null;
			}
			
			if(ClipDisplayLength == 0f) {
				yield return inType.VisibleCharacterCount * 0.095f;
			} else {
				yield return Routine.Inline(TruncateLine(ClipDisplayLength / (float)Text.text.Length));//ClipDisplayLength;
			}
        }
		
		public IEnumerator TruncateLine(float freq) {
			
			yield return new WaitForSeconds(1.5f);
			
			while(Text.text.Length > 0) {
				yield return new WaitForSeconds(freq);
				string s = Text.text;
				if(s.Length > 0) {
					s = s.Remove(0,1);
				}
				Text.SetText(s);
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
				yield return Routine.Inline(TruncateLine(ClipDisplayLength / (float)Text.text.Length));//ClipDisplayLength;
			}
        }
    }
}