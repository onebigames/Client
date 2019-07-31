﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structures.Containers
{
    public class Locker : MonoBehaviour, IInteractable
    {
        [SerializeField] private Container container = null;
        [SerializeField] private Transform door = null;
        [SerializeField] private Transform dropPosition = null;
        [SerializeField] private float maxAngle = 100;

        [SerializeField] private float openTime = 1f;
        [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool open;
        private Coroutine currentCoroutine;
        

        private void OnEnable()
        {
            container.OnContained += OnPlacedInLocked;
        }

        private void OnDisable()
        {
            container.OnContained -= OnPlacedInLocked;
        }

        // TODO: Add logic for permissions
        public bool IsInteractable(GameObject source)
        {
            return true;
        }

        public void Interact(GameObject source)
        {
            if (!IsInteractable(source))
            {
                return;
            }

            var inHand = source.GetComponent<CharacterInventory>().GetInHand();
            if (inHand && open)
            {
                // Place item inside if we can
                inHand.Container = container;
            }
            else
            {
                // Otherwise toggle the door
                open = !open;
                container.Restricted = !open;

                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }

                currentCoroutine = StartCoroutine(Move());
            }
        }

        private IEnumerator Move()
        {
            var from = (door.transform.localRotation.eulerAngles.z + 360) % 360;
            var to = open ? maxAngle : 0;
            for (float t = 0; t < openTime; t += Time.deltaTime)
            {
                var p = openCurve.Evaluate(t / openTime);
                var angle = Mathf.LerpUnclamped(from, to, p);
                door.transform.localRotation = Quaternion.Euler(0, 0, angle);
                yield return null;
            }

            door.transform.localRotation = Quaternion.Euler(0, 0, to);
        }

        private void OnPlacedInLocked(Containable containable)
        {
            containable.transform.position = dropPosition.transform.position;
        }
    }
}