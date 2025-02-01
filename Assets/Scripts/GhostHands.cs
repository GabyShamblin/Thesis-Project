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

  [HideInInspector] public GameObject sceneGhost;

  //! Parent rotation
  private Transform parent;
  private Vector3 initTempGhost;
  private Quaternion startRotation;
  private Vector3 startPosition;
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
      // Save the controller rotation and position for future movement caluclations
      startRotation = transform.rotation;
      startPosition = transform.forward * Globals.ghostOffset;
      
      // Create ghost hands directly in front of where the controllers are facing
      sceneGhost = Instantiate(ghost, startPosition + transform.position, startRotation, parent);
    }
  }

  void Update()
  {
    if (Globals.vis[0] == 1 && Globals.start) {
      if (!spawnedHands) {
        // Figure out how to stabilize hand 
        // this.transform.rotation = Quaternion.identity
        parent.rotation = Quaternion.Euler(0, parent.rotation.eulerAngles.y, 0);
        CreateGhostHands();
        spawnedHands = true;
      } else {
        // Make ghost hands follow hand rotation/position
        sceneGhost.transform.rotation = transform.rotation;
        sceneGhost.transform.position = startPosition + transform.position;
      }
    }
  }
}
