using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{
    public class CharacterSpriteLayer
    {
        private CharacterManager characterManager => CharacterManager.instance;

        private const float DEFAULT_TRANSITION_SPEED = 3f;
        private float transitionSpeedMultiplier = 1;
        public int layer { get; private set; } = 0;
        public Image renderer { get; private set; } = null; // aktuális sprite megjelenitésére
        public CanvasGroup renderCG => renderer.GetComponent<CanvasGroup>();

        private List<CanvasGroup> oldRenders = new List<CanvasGroup>(); // előző renderer-ek listája (az átmenetek kezelésére)

        // animációs coroutine és azok animációi
        private Coroutine co_transitioningLayer = null;
        private Coroutine co_levelingAlpha = null;
        private Coroutine co_changingColor = null;
        private Coroutine co_flipping = null;
        private bool isFacingLeft = Character.DEFAULT_ORIENTATION_IS_FACING_LEFT;
        public bool isTransitioningLayer => co_transitioningLayer != null;
        public bool isLevelingAlpha => co_levelingAlpha != null;
        public bool isChangingColor => co_changingColor != null; 
        public bool isFlipping => co_flipping != null;

        // inicializálás alapértelmezett értékekkel
        public CharacterSpriteLayer(Image defaultRenderer, int layer = 0)
        {
            renderer = defaultRenderer;
            this.layer = layer;
        }

        // sprite hozzárendellése
        public void SetSprite(Sprite sprite)
        {
            renderer.sprite = sprite;
        }

        // átmenet inditása animációként két sprite váltása között
        public Coroutine TransitionSprite(Sprite sprite, float speed = 1)
        {
            Debug.Log($"Transparition Sprite name: {sprite.name}");
            
            if (sprite == renderer.sprite)
                return null;

            if (isTransitioningLayer)
                characterManager.StopCoroutine(co_transitioningLayer);

            co_transitioningLayer = characterManager.StartCoroutine(TransitioningSprite(sprite, speed));
            return co_transitioningLayer;
        }

        public IEnumerator TransitioningSprite(Sprite sprite, float speedMultiplier)
        {
            transitionSpeedMultiplier = speedMultiplier;
            
            Image newRenderer = CreateRenderer(renderer.transform.parent);
            newRenderer.sprite = sprite;

            yield return TryStartLevelingAlphas();

            co_transitioningLayer = null;
        }

        // új renderer létrehozása, régiekhez hozzáadni a CanvasGroup elemet
        private Image CreateRenderer(Transform parent)
        {
            Image newRenderer = Object.Instantiate(renderer, parent);
            oldRenders.Add(renderCG);

            newRenderer.name = renderer.name;
            renderer = newRenderer;
            renderer.gameObject.SetActive(true);
            renderCG.alpha = 0;

            return newRenderer;
        }

        private Coroutine TryStartLevelingAlphas()
        {
            if (isLevelingAlpha)
                characterManager.StopCoroutine(co_levelingAlpha);
                //return co_levelingAlpha;

            co_levelingAlpha = characterManager.StartCoroutine(RunAlphaLeveling());

            return co_levelingAlpha;
        }

        // új sprite fokozatos megjelenése, előző fokozatos eltűnése
        private IEnumerator RunAlphaLeveling()
        {
            while(renderCG.alpha < 1 || oldRenders.Any(oldCG => oldCG.alpha > 0)) // alfa érték határozza meg, mikor kell váltani
            {
                float speed = DEFAULT_TRANSITION_SPEED * transitionSpeedMultiplier * Time.deltaTime; // alfa érték váltásának sebessége


                renderCG.alpha = Mathf.MoveTowards(renderCG.alpha, 1, speed); // fokozatosan nő az alfa érték

                for (int i = oldRenders.Count - 1; i >= 0; i--) // régieknek fokozatosan csökken az alfa 0-ig
                {
                    CanvasGroup oldCG = oldRenders[i];
                    oldCG.alpha = Mathf.MoveTowards(oldCG.alpha, 0, speed);

                    if (oldCG.alpha <= 0) // amikor már eltávolitható a sprite a listából
                    {
                        oldRenders.RemoveAt(i);
                        Object.Destroy(oldCG.gameObject);
                    }
                }

                yield return null;
            }

            co_transitioningLayer = null;
        }

        // az új és a régi renderer szinének a megváltoztatása
        public void SetColor(Color color)
        {
            renderer.color = color;

            foreach(CanvasGroup oldCG in oldRenders)
            {
                oldCG.GetComponent<Image>().color = color;
            }
        }

        // szinátmenet kezelése
        public Coroutine TransitionColor(Color color, float speed)
        {
            if (isChangingColor)
                characterManager.StopCoroutine(co_changingColor);

            co_changingColor = characterManager.StartCoroutine(ChangingColor(color, speed));

            return co_changingColor;
        }

        // szinátmenet leállitása
        public void StopChangingColor()
        {
            if (!isChangingColor)
                return;

            characterManager.StopCoroutine(co_changingColor);

            co_changingColor = null;
        }

        private IEnumerator ChangingColor(Color color, float speedMultiplier)
        {
            Color oldColor = renderer.color;
            List<Image> oldImages = new List<Image>();

            foreach (var oldCG in oldRenders)
            {
                oldImages.Add(oldCG.GetComponent<Image>());
            }

            float colorPercent = 0;
            while(colorPercent < 1)
            {
                colorPercent += DEFAULT_TRANSITION_SPEED * speedMultiplier * Time.deltaTime;

                renderer.color = Color.Lerp(oldColor, color, colorPercent);

                /*foreach(Image oldImage in oldImages)
                {
                    oldImage.color = renderer.color;
                }*/

                for (int i = oldImages.Count - 1; i >= 0; i--)
                {
                    Image image = oldImages[i];
                    if (image != null)
                        image.color = renderer.color;
                    else
                        oldImages.RemoveAt(i);
                }

                yield return null;
            }
            co_changingColor = null;
        }

        // arc balra animéció
        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            isFacingLeft = true;
            co_flipping = characterManager.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));
        
            return co_flipping;
        }

        // karakter forgatása ellenkező irányba
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (isFacingLeft)
                return FaceRight(speed, immediate);
            else
                return FaceLeft(speed, immediate);
        }

        // arc jobbra animáció
        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            isFacingLeft = false;
            co_flipping = characterManager.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));

            return co_flipping;
        }

        private IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            float xScale = faceLeft ? 1 : -1;
            Vector3 newScale = new Vector3(xScale, 1, 1);

            if (!immediate)
            {
                Image newRenderer = CreateRenderer(renderer.transform.parent);

                newRenderer.transform.localScale = newScale;

                transitionSpeedMultiplier = speedMultiplier;
                TryStartLevelingAlphas();

                while(isLevelingAlpha)
                    yield return null;
            }
            else
            {
                renderer.transform.localScale = newScale;
            }

            co_flipping = null;
        }
    }
}
