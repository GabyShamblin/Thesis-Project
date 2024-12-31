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

  //! [Input] Ghost hand parent
  [Tooltip("The ghost hand parent")]
  public Transform console;

  [Tooltip("The temp ghost hand to show offset amount (optional)")]
  public Transform tempGhost;

  public GameObject realPosition;

  [SerializeField] private GameObject placeHands;

  [HideInInspector] public GameObject sceneGhost;

  //! Parent rotation
  private Transform parent;
  private Vector3 initTempGhost;
  private Quaternion startRotation;
  private Vector3 startPosition;
  private bool spawnedHands = false;

  void Start()
  {
    if (tempGhost != null) {
      initTempGhost = tempGhost.position;
      MoveHand();
    }

    if (console == null) {
      Debug.LogError("Console has not been found. It should be the grandparent of this object. Please check the hierarchy.");
      console = this.transform.parent;
    }

    parent = this.transform.parent;
    if (parent == null) {
      Debug.LogError("Parent has not been found. It should be the grandparent of this object. Please check the hierarchy.");
      // parent = this.transform.parent;
    }
  }

  //! Adjust temp hand to display offset amount and save offset value
  public void MoveHand() {
    Slider slide = GameObject.Find("Slider").GetComponent<Slider>();
    float offset = (float)Math.Round((double)slide.value, 2);

    tempGhost.localPosition = new Vector3(
      initTempGhost.x, 
      initTempGhost.y, 
      initTempGhost.z + offset
    );

    Globals.ghostOffset = offset;
  }

  //! Create ghost hands offset from the controllers
  public void CreateGhostHands() {
    if (placeHands != null) {
      placeHands.SetActive(false);
    }

    // If hands have already been instantiated, destroy them 
    if (sceneGhost != null) {
      Destroy(sceneGhost);
    }

    if (Globals.ghostOffset != 0) {
      // Save the controller rotation and position for future movement caluclations
      startRotation = transform.rotation;
      startPosition = transform.forward * Globals.ghostOffset;
      
      // Create ghost hands directly in front of where the controllers are facing
      sceneGhost = Instantiate(ghost, startPosition + transform.position, startRotation, console);
    }
  }

  void Update()
  {
    if (Globals.start) {
      if (!spawnedHands) {
        // If exists, turn off example hand
        if (tempGhost != null) {
          tempGhost.gameObject.SetActive(false);
        }
        if (realPosition != null) {
          realPosition.SetActive(false);
        }
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
