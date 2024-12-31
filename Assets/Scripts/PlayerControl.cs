using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! Custom player controls
/*!
  Custom player turning because the default meta one is bad. Also allows passthrough to be toggled with adjust mode to more easily adjust the console.
*/
public class PlayerControl : MonoBehaviour
{
  [Tooltip("How much the player rotates")]
  //! [Input] How much the player rotates
  public int rotationAmount = 20;
  //! How much to wait before allowing user to rotate again
  private float wait = 0.5f;
  //! The timer for wait
  private float timer = 0;

  void FixedUpdate()
  {
    timer += Time.deltaTime;

    if (timer >= wait) {
      if (OVRInput.Get(OVRInput.RawButton.RThumbstickLeft)) {
        transform.Rotate(0, -rotationAmount, 0);
        timer = 0;
      }
      if (OVRInput.Get(OVRInput.RawButton.RThumbstickRight)) {
        transform.Rotate(0, rotationAmount, 0);
        timer = 0;
      }
    }
  }
}
