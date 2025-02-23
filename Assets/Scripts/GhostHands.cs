using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GhostHands : MonoBehaviour
{
  //! [Input] Ghost hand prefab
  [Tooltip("The ghost hand prefab")]
  public GameObject ghost;

  public Transform parent;

  [HideInInspector] public GameObject sceneGhost;

  private Vector3 initTempGhost;
  private bool spawnedHands = false;

  void Start()
  {
    if (parent == null) {
      parent = this.transform.parent;
    }
  }

  //! Create ghost hands offset from the controllers
  public void CreateGhostHands() {
    // If hands have already been instantiated, destroy them 
    if (sceneGhost != null) {
      Destroy(sceneGhost);
    }

    if (Globals.vis[0] == 1 && Globals.ghostOffset != 0) {
      // Create ghost hands directly in front of where the controllers are facing
      sceneGhost = Instantiate(ghost, new Vector3(0,0,Globals.ghostOffset) + transform.position, transform.rotation, parent);
    }
  }

  public void DestroyGhostHands() {
    Destroy(sceneGhost);
  }

  void Update()
  {
    if (Globals.vis[0] == 1 && Globals.start) {
      // Make ghost hands follow hand rotation/position
      sceneGhost.transform.rotation = transform.rotation;
      sceneGhost.transform.position = transform.position + new Vector3(0,0,Globals.ghostOffset);
    }
  }
}
