using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket {
    public class GhostTransparency : MonoBehaviour
    {

        public SkinnedMeshRenderer ghostMesh;
        public List<SkinnedMeshRenderer> chainMeshes = new List<SkinnedMeshRenderer>();

        [SerializeField] Material ghostMaterial;
        [SerializeField] List<Material> chainMaterials = new List<Material>();

        private GhostPerception ghostPerception;

        [Header("Controls")]
        [SerializeField] float fadeDuration;

        Color ghostMatColor;
        Color ghostMatTransparent;
        Color chainMatColor;
        Color chainMatTransparent;

        Coroutine fadingGhost;
        Coroutine fadingChain;

        void Awake() {

            ghostMaterial = ghostMesh.material;
            for (int i = 0; i < chainMeshes.Count; i++) {
                chainMaterials.Add(chainMeshes[i].material);
            }

            ghostMatColor = ghostMaterial.color;
            ghostMatTransparent = new Color(ghostMatColor.r, ghostMatColor.g, ghostMatColor.b, 0.1f);

            chainMatColor = chainMaterials[0].color;
            chainMatTransparent = new Color(chainMatColor.r, chainMatColor.g, chainMatColor.b, 0.1f);

            //StartCoroutine(TestingTransparency());
            // Grab the ghost movement reference
            ghostPerception = ghostPerception ?? GetComponentInParent<GhostPerception>();
            if (!ghostPerception) {
                Debug.LogError("[GhostAnimController] ERROR -> No Ghost Perception component found on parent object! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }

        }

        void OnEnable() {
            ghostPerception.onSeePlayer += SetTransparencyOff;
            ghostPerception.onForgottenPlayer += SetTransparencyOn;

            ToggleGhostTransparency(true);
            ToggleChainTransparency(true);
        }

        void OnDisable() {
            ghostPerception.onSeePlayer -= SetTransparencyOff;
            ghostPerception.onForgottenPlayer -= SetTransparencyOn;
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

        public void SetTransparencyOn() {
            ToggleGhostTransparency(true);
            ToggleChainTransparency(true);
        }

        public void SetTransparencyOff() {
            ToggleGhostTransparency(false);
            ToggleChainTransparency(false);
        }

        public void ToggleGhostTransparency( bool transparent ) {
            if (fadingGhost != null) {
                StopCoroutine(fadingGhost);
            }
            fadingGhost = StartCoroutine(FadingColor(new List<Material> { ghostMaterial }, transparent ? ghostMatTransparent : ghostMatColor, fadeDuration));
        }

        public void ToggleChainTransparency( bool transparent ) {
            if (fadingChain != null) {
                StopCoroutine(fadingChain);
            }
            fadingChain = StartCoroutine(FadingColor(chainMaterials, transparent ? chainMatTransparent : chainMatColor, fadeDuration));
        }


        IEnumerator FadingColor( List<Material> materials, Color color, float fadeTime ) {

            Color initialColor = materials[0].GetColor("_Color");

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime) {
                foreach (Material mat in materials) {
                    Color newColor = Color.Lerp(initialColor, color, t);
                    mat.color = newColor;
                    //mat.SetColor("_Color", newColor);
                }
                yield return null;
            }

            foreach (Material mat in materials) {
                //mat.SetColor("_Color", color);
                mat.color = color;
            }

        }


    }

}
