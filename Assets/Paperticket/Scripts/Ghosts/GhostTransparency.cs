using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket {
    public class GhostTransparency : MonoBehaviour
    {

        public List<SkinnedMeshRenderer> ghostMeshes = new List<SkinnedMeshRenderer>();
        public List<SkinnedMeshRenderer> chainMeshes = new List<SkinnedMeshRenderer>();

        [SerializeField] List<Material> ghostMaterials = new List<Material>();
        [SerializeField] List<Material> chainMaterials = new List<Material>();

        private GhostPerception ghostPerception;

        [Header("Controls")]
        [SerializeField] float defaultFadeDuration = 2;
        [SerializeField] float fadedGhostAlpha = 0.4f;
        [SerializeField] float fadedChainAlpha = 0.3f;
        [SerializeField] bool debugging;

        [Header("Read Only")]
        [SerializeField] Color ghostMatColor;
        //Color ghostMatTransparent;
        [SerializeField] Color chainMatColor;
        //Color chainMatTransparent;

        Coroutine fadingGhost;
        Coroutine fadingChain;

        void Awake() {

            for (int i = 0; i < ghostMeshes.Count; i++) {
                ghostMaterials.Add(ghostMeshes[i].material);
            }
            for (int i = 0; i < chainMeshes.Count; i++) {
                chainMaterials.Add(chainMeshes[i].material);
            }

            ghostMatColor = ghostMaterials[0].GetColor("_BaseColor");
            chainMatColor = chainMaterials[0].GetColor("_BaseColor");

            //ghostMatTransparent = new Color(ghostMatColor.r, ghostMatColor.g, ghostMatColor.b, defaultFadeAlpha);
            //chainMatTransparent = new Color(chainMatColor.r, chainMatColor.g, chainMatColor.b, defaultFadeAlpha);

            // Grab the ghost movement reference
            ghostPerception = ghostPerception ?? GetComponentInParent<GhostPerception>();
            if (!ghostPerception) {
                Debug.LogError("[GhostAnimController] ERROR -> No Ghost Perception component found on parent object! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }

        }

        void OnEnable() {
            ghostPerception.onSeePlayer += FadeIn;
            ghostPerception.onForgottenPlayer += FadeOut;

            FadeOut(0.1f);
        }

        void OnDisable() {
            ghostPerception.onSeePlayer -= FadeIn;
            ghostPerception.onForgottenPlayer -= FadeOut;
        }

        //IEnumerator TestingTransparency() {
        //    while (true) {

        //        yield return new WaitForSeconds(Random.Range(10f, 18f));

        //        ToggleGhostTransparency(true);
        //        ToggleChainTransparency(true);
        //        //if (Random.Range(0f, 10f) > 0.5f) {
        //        //    ToggleChainTransparency(true);
        //        //}

        //        yield return new WaitForSeconds(Random.Range(5f, 16f));

        //        ToggleGhostTransparency(false);
        //        ToggleChainTransparency(false);
        //        //if (chainMaterials[0].color == chainMatTransparent) {
        //        //    ToggleChainTransparency(false);
        //        //}

        //    }
        //}

        public void FadeOut() {
            if (debugging) Debug.Log("[GhostTransparency] Fading out!");
            SetGhostTransparency(fadedGhostAlpha, defaultFadeDuration);
            SetChainTransparency(fadedChainAlpha, defaultFadeDuration);
        }
        public void FadeOut( float fadeTime ) {
            if (debugging) Debug.Log("[GhostTransparency] Fading out!*");
            SetGhostTransparency(fadedGhostAlpha, fadeTime);
            SetChainTransparency(fadedChainAlpha, fadeTime);
        }

        public void FadeOutTotal() {
            if (debugging) Debug.Log("[GhostTransparency] Fading out completely!");
            SetGhostTransparency(0f, defaultFadeDuration);
            SetChainTransparency(0f, defaultFadeDuration);
        }
        public void FadeOutTotal( float fadeTime ) {
            if (debugging) Debug.Log("[GhostTransparency] Fading out completely!*");
            SetGhostTransparency(0f, fadeTime);
            SetChainTransparency(0f, fadeTime);
        }


        public void FadeIn() {
            if (debugging) Debug.Log("[GhostTransparency] Fading in!");
            SetGhostTransparency(ghostMatColor.a, defaultFadeDuration);
            SetChainTransparency(ghostMatColor.a, defaultFadeDuration);
        }
        public void FadeIn( float fadeTime ) {
            if (debugging) Debug.Log("[GhostTransparency] Fading in!*");
            SetGhostTransparency(ghostMatColor.a, fadeTime);
            SetChainTransparency(ghostMatColor.a, fadeTime);
        }

        public void FadeInTotal() {
            if (debugging) Debug.Log("[GhostTransparency] Fading in completely!");
            SetGhostTransparency(1f, defaultFadeDuration);
            SetChainTransparency(1f, defaultFadeDuration);
        }
        public void FadeInTotal( float fadeTime ) {
            if (debugging) Debug.Log("[GhostTransparency] Fading in completely!*");
            SetGhostTransparency(1f, fadeTime);
            SetChainTransparency(1f, fadeTime);
        }



        void SetGhostTransparency( float alpha, float fadeDuration ) {
            if (fadingGhost != null) {
                if (debugging) Debug.Log("[GhostTransparency] Stopping existing fade...");
                StopCoroutine(fadingGhost);
            }
            Color newColor = new Color(ghostMatColor.r, ghostMatColor.g, ghostMatColor.b, alpha);
            fadingGhost = StartCoroutine(FadingColor(ghostMaterials, newColor, fadeDuration));
        }

        void SetChainTransparency( float alpha, float fadeDuration ) {
            if (fadingChain != null) {
                if (debugging) Debug.Log("[GhostTransparency] Stopping existing fade...");
                StopCoroutine(fadingChain);
            }
            Color newColor = new Color(chainMatColor.r, chainMatColor.g, chainMatColor.b, alpha);
            fadingChain = StartCoroutine(FadingColor(chainMaterials, newColor, fadeDuration));
        }




        IEnumerator FadingColor( List<Material> materials, Color color, float fadeTime ) {
            if (debugging) Debug.Log("[GhostTransparency] Fading to Color(" + color + ") over " + fadeTime +" seconds");

            Color initialColor = materials[0].GetColor("_BaseColor");

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime) {
                foreach (Material mat in materials) {
                    Color newColor = Color.Lerp(initialColor, color, t);
                    mat.SetColor("_BaseColor", newColor);
                    //mat.SetColor("_Color", newColor);
                }
                yield return null;
            }

            foreach (Material mat in materials) {
                mat.SetColor("_BaseColor", color);
               // mat.color = color;
            }

            if (debugging) Debug.Log("[GhostTransparency] Finished fading!");

        }


    }

}
