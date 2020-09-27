using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualOcclusion : MonoBehaviour
{

    [Header("Controls")]

    [SerializeField] float activationDistance;
    [SerializeField] [Range(0.1f, 100)] float checkRate;

    [Header("Read Only")]

    [SerializeField] bool active;

    [SerializeField] List<Transform> children = new List<Transform>();

    // Start is called before the first frame update
    void Awake() {
        StartCoroutine(Setup());
    }

    IEnumerator Setup() {

        PopulateChildList();

        // Wait until the PTUtilities script is defined
        yield return new WaitUntil(() => PTUtilities.instance != null);

        StartCoroutine(CheckingDistance());        
    }
        
    void PopulateChildList() {
        for (int i = 0; i < transform.childCount; i++) {
            // Add the children if they are not already in the list
            if (!children.Contains(transform.GetChild(i))) {
                children.Add(transform.GetChild(i));
            }
        }
    }


    IEnumerator CheckingDistance() {
        yield return new WaitForSeconds(Random.Range(0, checkRate));

        while (true) {
            yield return new WaitForSeconds(checkRate);

            active = (PTUtilities.instance.HeadsetPosition() - transform.position).magnitude < activationDistance;
            
            if (children[0].gameObject.activeSelf != active) {
                foreach (Transform child in children) {
                    child.gameObject.SetActive(active);
                }
            }            
        }

    }

}
