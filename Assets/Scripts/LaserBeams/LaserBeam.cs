using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaserBeam : MonoBehaviourPunCallbacks
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
        if(photonView.IsMine)
        {
            StartCoroutine(ExplosionCo());
        }
    }

    public IEnumerator ExplosionCo()
    {
        GameObject effect = PhotonNetwork.Instantiate(impactPrefab.name, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(3.0f);

        PhotonNetwork.Destroy(effect);
        PhotonNetwork.Destroy(gameObject);
    }
}
