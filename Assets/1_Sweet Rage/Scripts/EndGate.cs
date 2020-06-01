﻿namespace SweetRage {
    using Deirin.EB;
    using Deirin.Utilities;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent( typeof( Rigidbody ) )]
    public class EndGate : MonoBehaviour {
        [Min(0)] public float maxLife;
        public UnityEvent OnLifeDepleated;
        public UnityEvent_Float OnLifeChangedPercent;

        private float currentLife;
        private DamageReceiver damageReceiver;

        private void Awake () {
            currentLife = maxLife;
        }

        private void OnTriggerEnter ( Collider other ) {
            Enemy e = other.GetComponentInParent<Enemy>();
            if ( e ) {
                SetLife( currentLife - e.endGateDamage );
                if ( e.TryGetBehaviour( out damageReceiver ) ) {
                    damageReceiver.Damage( damageReceiver.maxLife );
                }
            }
        }

        public void SetLife ( float value ) {
            if ( value != currentLife ) {
                currentLife = Mathf.Max( 0, value );
                OnLifeChangedPercent.Invoke( Mathf.Clamp01( currentLife / maxLife ) );
                if ( currentLife <= 0 ) {
                    currentLife = 0;
                    OnLifeDepleated.Invoke();
                }
            }
        }
    }
}