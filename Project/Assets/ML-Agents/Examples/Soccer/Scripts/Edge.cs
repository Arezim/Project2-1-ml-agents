using System;
using UnityEngine;

namespace ML_Agents.Examples.Soccer.Scripts
{
    public class Edge : ICloneable
    {
        private GameObject source;
        private GameObject target;

        private Vector3 sourcePosition;
        private Vector3 targetPosition;

        private Vector3 directionVector;
        private float directionAngleNormalized;
        private float distance;

        public Edge(GameObject source, GameObject target, Vector3 sourcePosition, Vector3 targetPosition)
        {
            this.source = source;
            this.target = target;
            this.sourcePosition = sourcePosition;
            this.targetPosition = targetPosition;
            calculate();
        }

        // Properties with getters and setters
        public GameObject Source
        {
            get => source;
            set
            {
                source = value;
                calculate(); // Recalculate when source changes
            }
        }

        public GameObject Target
        {
            get => target;
            set
            {
                target = value;
                calculate(); // Recalculate when target changes
            }
        }

        public Vector3 SourcePosition
        {
            get => sourcePosition;
            set
            {
                sourcePosition = value;
                calculate(); // Recalculate when source position changes
            }
        }

        public Vector3 TargetPosition
        {
            get => targetPosition;
            set
            {
                targetPosition = value;
                calculate(); // Recalculate when target position changes
            }
        }

        public Edge getReversed()
        {
            Edge tempEdge = (Edge) this.Clone();

            var temp = tempEdge.source;
            tempEdge.source = tempEdge.target;
            tempEdge.target = temp;

            var tempPos = tempEdge.sourcePosition;
            tempEdge.sourcePosition = tempEdge.targetPosition;
            tempEdge.targetPosition = tempPos;

            tempEdge.directionVector = tempEdge.directionVector * -1;
            tempEdge.directionAngleNormalized = (tempEdge.directionAngleNormalized + 0.5f) % 1.0f;

            return tempEdge;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        private void calculate()
        {
            directionVector = this.targetPosition - this.sourcePosition;
            float angleRadians = Mathf.Atan2(this.directionVector.x, this.directionVector.z);
            float normalizedDirection = (angleRadians + Mathf.PI) / (2 * Mathf.PI);
            normalizedDirection %= 1.0f;

            this.directionAngleNormalized = normalizedDirection;
            this.distance = directionVector.magnitude;
        }

        public string toString()
        {
            return
                $"Source: {source.name}, Target: {target.name}, Distance: {distance}, Direction Normalised: {directionAngleNormalized}";
        }

    }
}
