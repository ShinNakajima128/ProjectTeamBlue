using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class PlayerController : MonoBehaviour
    {
        #region property
        public float Speed = 5.0f;
        #endregion

        #region serialize
        #endregion

        #region private
        private CharacterController controller;
        #endregion

        #region Constant
        #endregion

        #region Event
        #endregion

        #region unity methods
        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
            controller.Move(moveDirection * Time.deltaTime * Speed);
        }
        #endregion

        #region public method
        #endregion

        #region private method
        #endregion
    }
}
