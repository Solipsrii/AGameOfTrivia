using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class textproFloater : MonoBehaviour
{
    private TextMeshProUGUI text;
    public bool stopAnimation;
    public float amplitude, frequency;
    // Start is called before the first frame update
    void Start()
    {
        
        stopAnimation = false;
        if (amplitude <= 0)
            amplitude = 10f;
        if (frequency < 2)
            frequency = 2f;
        text = this.GetComponent<TextMeshProUGUI>();
    }

    public void startAnimation(float alpha, float ampAmount){
            StartCoroutine(fadeIn(alpha, ampAmount));
        StartCoroutine(floatText());
    }

    private IEnumerator floatText(){
        TMP_TextInfo textInfo = text.textInfo;
        while(!stopAnimation){
            text.ForceMeshUpdate();

            //this entire loop goes over the text's meshes, and edits its "draft set".
            for (int i = 0; i < textInfo.characterCount; i++){
                //checking for a white-space. Skipping entire i-step in case it is.
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                var charInfo = textInfo.characterInfo[i];
                //look for total array of all verts in all characters...I don't get this shit.
                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                int vertexIndex = charInfo.vertexIndex;
                
                for (int j=0; j < 4; j++){
                    var orig = verts[vertexIndex + j];
                    verts[vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*frequency + orig.x*0.01f) * amplitude, 0);
                }
            }

            //Updating the working set of the mesh. why tho.
            for(int i=0; i < textInfo.meshInfo.Length; i++){
                //meshinfo.mesh.vertices == working set
                //mesh.vertices = draft set, the editable one. why is that important?
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                //this seems so wasteful and stupid.
                text.UpdateGeometry(meshInfo.mesh, i);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator fadeIn(float alpha, float ampAmount){
        var parent = GetComponentInParent<CanvasGroup>();
        parent.DOFade(alpha, 1.3f);
        
        //spike amp and then lower it slowly
        float tmpLitude = amplitude;
        amplitude = ampAmount;

        while(amplitude >= tmpLitude){
            amplitude = (amplitude - Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }
        amplitude = tmpLitude;
        yield break;
    }
}
