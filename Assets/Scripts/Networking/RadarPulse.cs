using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPulse : MonoBehaviour
{
    private Transform pulseTransform;
    [SerializeField]
    private Transform radarMaskTransform;
    [SerializeField]
    private Transform playerTransform;
    private float range;
    private List<Collider> alreadyPingedColliderList = new();
    public GameObject detectedPrefab;
    public float maxRange = 1900.0f;
    public float radiusSpeed = 150.0f;

    private void Awake()
    {
        pulseTransform = transform.Find("Pulse");
    }

    private void Start()
    {
        radarMaskTransform = GameObject.FindGameObjectWithTag("RadarMask").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        range += radiusSpeed * Time.deltaTime;
        if(range > maxRange)
        {
            range = 0.0f;
            alreadyPingedColliderList.Clear();
            GameObject[] detectedGO = GameObject.FindGameObjectsWithTag("Detected");
            foreach (GameObject go in detectedGO)
            {
                Destroy(go);
            }
        }
        pulseTransform.localScale = new Vector3(range, range, range);

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.parent.position, range / 2.0f, transform.forward);

        foreach (RaycastHit hit in raycastHits)
        {            
            if (hit.collider != null)
            {
                if(hit.collider.CompareTag("Player"))
                {
                    if (!alreadyPingedColliderList.Contains(hit.collider))
                    {
                        alreadyPingedColliderList.Add(hit.collider);
                        GameObject detectedGO = Instantiate(detectedPrefab, new Vector3(hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.z, 0.0f), Quaternion.identity, radarMaskTransform);
                        Vector2 offset = new Vector2(pulseTransform.localPosition.x, pulseTransform.localPosition.z) / 2.0f;
                        detectedGO.GetComponent<RectTransform>().localPosition = new Vector2(hit.collider.transform.localPosition.x / 2.0f, hit.collider.transform.localPosition.z / 2.0f) + offset;
                        Debug.Log(hit.collider.name);
                    }
                }
            }
        }  
    }
}
