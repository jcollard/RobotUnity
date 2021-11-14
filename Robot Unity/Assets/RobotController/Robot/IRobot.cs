namespace RobotController
{
    public interface IRobot
    {
        public void MoveForward();
        public void RotateLeft();
        public void RotateRight();
        public bool CanMove(Direction d);
    }

    public enum Direction
    {
        Left,
        Right,
        Forward,
        Backward

    }

}