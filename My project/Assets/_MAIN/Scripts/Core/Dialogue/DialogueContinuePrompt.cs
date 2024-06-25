using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueContinuePrompt : MonoBehaviour
    {
        private RectTransform root;

        [SerializeField] private Animator anim;
        [SerializeField] private TextMeshProUGUI tmpro;

        public bool isShowing => anim.gameObject.activeSelf;

        // Start meghívása az első keretfrissítés előtt történik
        void Start()
        {
            root = GetComponent<RectTransform>();
        }

        public void Show()
        {
            if (tmpro.text == string.Empty) // ha a szöveg üres, akkor ne jelenítse meg a prompt-ot
            {
                if (isShowing)
                    Hide();

                return;
            }

            tmpro.ForceMeshUpdate(); // A szöveg frissitése

            // prompt megjelenitése
            anim.gameObject.SetActive(true);
            root.transform.SetParent(tmpro.transform);

            // utolsó karakter poziciójának meghatározása
            TMP_CharacterInfo finalCharacter = tmpro.textInfo.characterInfo[tmpro.textInfo.characterCount - 1];
            Vector3 targetPos = finalCharacter.bottomRight;
            float characterWidth = finalCharacter.pointSize * 0.5f;
            targetPos = new Vector3(targetPos.x + characterWidth, -0.05f * targetPos.y, 0);

            // prompt poziciójának beállitása a szöveg alapján
            root.localPosition = targetPos;
        }

        public void Hide() // prompt elrejtése
        {
            anim.gameObject.SetActive(false);
        }
    }
}
