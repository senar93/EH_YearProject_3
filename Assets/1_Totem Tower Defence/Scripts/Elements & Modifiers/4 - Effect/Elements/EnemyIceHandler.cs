﻿namespace SweetRage {
    using Deirin.EB;
    using UnityEngine;

    public class EnemyIceHandler : AbsEnemyElementHandler {
        [Header("Ice Params"), Tooltip("Percentuale di riduzione del movimento"), Range(0, 1), Space]
        public float[] slowPercents;

        private PathPatroller pathPatroller;

        private void OnDisable () {
            OnCurrentChargeIncreases.RemoveListener( ChargeChangeHandler );
            OnCurrentChargeDecreases.RemoveListener( ChargeChangeHandler );
        }

        protected override void CustomSetup () {
            base.CustomSetup();

            Entity.TryGetBehaviour( out pathPatroller );

            if ( pathPatroller ) {
                OnCurrentChargeIncreases.AddListener( ChargeChangeHandler );
                OnCurrentChargeDecreases.AddListener( ChargeChangeHandler );
            }
#if UNITY_EDITOR
            else {
                print( name + " could not find path patroller" );
            }
#endif
        }

        private void ChargeChangeHandler ( int currentCharge ) {
            int count = slowPercents.Length;
            if ( pathPatroller && count > 0 && count >= currentCharge ) {
                pathPatroller.ResetSpeed();
                pathPatroller.SetSpeed( pathPatroller.Speed - pathPatroller.Speed * slowPercents[currentCharge - 1] );
            }
        }
    }
}