using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    [RequireComponent(typeof(Rigidbody))]
    public class RBFastDepenetrate : BatchedComponent {
        public RingBuffer<ContactPoint> Contacts = new RingBuffer<ContactPoint>(64, RingBufferMode.Expand);
        [NonSerialized] public Rigidbody CachedRB;

        static private readonly List<ContactPoint> s_ContactBuffer = new List<ContactPoint>(16);

        private void Awake() {
            this.CacheComponent(ref CachedRB);
        }

        private void OnCollisionStay(Collision collision) {
            collision.GetContacts(s_ContactBuffer);
            foreach(var contact in s_ContactBuffer) {
                Contacts.PushBack(contact);
            }
        }
    }
}
