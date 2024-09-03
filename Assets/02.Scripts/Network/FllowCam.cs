using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class FllowCam : MonoBehaviourPun
{
    private CinemachineVirtualCamera virtualCamera;
    private PhotonView pv = null;

    private Vector3 receivePos;
    private Quaternion receiveRot;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
        pv.ObservedComponents[0] = this;
        if (pv.IsMine)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
    }
}
