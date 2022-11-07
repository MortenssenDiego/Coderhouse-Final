using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaserBeam : MonoBehaviour
{
    public GameObject impactPrefab;
    private Vector3 movement = new();
    public float moveSpeed = 40f;
    // Update is called once per frame
    void Update()
    {
        movement = transform.forward * moveSpeed;
        transform.position += movement * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ExplosionCo());
    }

    public IEnumerator ExplosionCo()
    {
        GameObject effect = PhotonNetwork.Instantiate(impactPrefab.name, transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject);
 
        yield return new WaitForSeconds(5.0f);

        PhotonNetwork.Destroy(effect);
    }
}
