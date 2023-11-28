using System.Collections;
using BeauUtil.Tags;
using Leaf.Defaults;
using TMPro;
using UnityEngine;

namespace WeatherStation {
    public class SubtitleDisplay : MonoBehaviour, ITextDisplayer {
        public TMP_Text CharacterLabel;
        public TMP_Text Text;

        private void Start() {
            gameObject.SetActive(false);
            CharacterLabel.gameObject.SetActive(false);
        }

        public IEnumerator CompleteLine() {
            yield return 1;
            gameObject.SetActive(false);
        }

        public TagStringEventHandler PrepareLine(TagString inString, TagStringEventHandler inBaseHandler) {
            Text.SetText(inString.RichText);
            return null;
        }

        public IEnumerator TypeLine(TagString inSourceString, TagTextData inType) {
            gameObject.SetActive(true);
            yield return inType.VisibleCharacterCount * 0.01f;
        }
    }
}