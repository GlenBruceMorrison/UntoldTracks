using UnityEngine;

    public class Indicator
    {
        public IndicationDirection Direction { get; set; }
        public BoxCollider Collider { get; set; }
        public GameObject GameObject { get; set; }

        public Vector3 DirectionVector()
        {
            switch (Direction)
            {
                case IndicationDirection.Left:
                    return Vector3.left;
                case IndicationDirection.Right:
                    return Vector3.right;
                case IndicationDirection.Forward:
                    return Vector3.forward;
                case IndicationDirection.Backward:
                    return Vector3.back;
            }

            return Vector3.zero;
        }
    }