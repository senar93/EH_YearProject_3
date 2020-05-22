﻿namespace Deirin.EB {
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    public class LookAt : BaseBehaviour {
        #region Inspector
        [Header("Refs")]
        public Transform target;
        public Transform startTarget;

        [Header("Params")]
        public bool X;
        public bool Y;
        public bool Z;
        public Vector3 targetOffset;
        public float maxTurnAngle = 360;
        public float turnSpeed;
        [Tooltip("Angle in which the target is considered in view.")] public float viewAngle = 2;

        [Header("Events")]
        [Tooltip("Called when the target enters the view angle.")] public UnityEvent OnTargetSeen;
        [Tooltip("Called when the target exits the view angle.")] public UnityEvent OnTargetLost;
        #endregion

        private bool inView = false;
        private Vector3 targetWorldLookDir;

        public override void OnUpdate () {
            if ( !target )
                return;

            RotateTowardsTarget();

            ViewAngleCheck();
        }

        private void RotateTowardsTarget () {
            Vector3 targetPos = target.position + targetOffset;
            targetPos.Set( X ? targetPos.x : transform.position.x, Y ? targetPos.y : transform.position.y, Z ? targetPos.z : transform.position.z );

            Quaternion currentLocalRotation = transform.localRotation;
            transform.localRotation = Quaternion.identity;

            targetWorldLookDir = targetPos - transform.position;
            Vector3 targetLocalLookDir = transform.InverseTransformDirection(targetWorldLookDir);

            targetLocalLookDir = Vector3.RotateTowards(
              transform.forward,
              targetLocalLookDir,
              Mathf.Deg2Rad * maxTurnAngle,
              0
            );

            Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

            transform.localRotation = Quaternion.Slerp(
              currentLocalRotation,
              targetLocalRotation,
              1 - Mathf.Exp( -turnSpeed * Time.deltaTime )
            );
        }

        float angle;
        private void ViewAngleCheck () {
            angle = Vector3.Angle( transform.forward, targetWorldLookDir );

            if ( inView == false && angle <= viewAngle ) {
                OnTargetSeen.Invoke();
                inView = true;
            }
            else if ( inView == true ) {
                OnTargetLost.Invoke();
                inView = false;
            }
        }

        private void OnDrawGizmos () {
            Handles.Label( transform.position + Vector3.up * 2, angle.ToString( "f2" ), EditorStyles.boldLabel );
        }

        #region API
        public void SetTarget ( Transform target ) {
            this.target = target;
        }

        public void ReturnToTargetRotation () {
            target = startTarget;
        }

        public void RemoveTarget () {
            target = null;
        }
        #endregion
    }
}