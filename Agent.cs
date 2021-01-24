using System;
using System.Numerics;

namespace qt_benchmark
{
    public class Agent
    {
        public int id;
        public float radius;
        public Vector2 position;
        public Vector2 move;
        public float speed;
        public void Move() => position += move * speed;
        public override bool Equals(object obj) => obj is Agent agent && id == agent.id;
        public override int GetHashCode() => id.GetHashCode();
        public static bool operator ==(Agent a, Agent b) => a.id == b.id;
        public static bool operator !=(Agent a, Agent b) => !(a == b);


        // ignore in basic - a must for v2
        public string nodeId;
        public Action<Agent> OnMove;

        public void SetPath(string newPath) => nodeId = newPath;
    }
}