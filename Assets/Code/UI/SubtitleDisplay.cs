using System.Collections;
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
            yield return inType.VisibleCharacterCount * 0.095f;	//todo - should be based on length of vo clip...
        }
    }
}