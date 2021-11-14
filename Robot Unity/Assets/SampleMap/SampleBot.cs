using RobotController;
using UnityEngine;

public class SampleBot : MonoBehaviour, IRobot
{
    public void Start()
    {

    }

    public void Update()
    {
        
    }

    public bool CanMove(Direction d)
    {
        throw new System.NotImplementedException();
    }

    public void MoveForward()
    {
        throw new System.NotImplementedException();
    }

    public void RotateLeft()
    {
        this.transform.Rotate(0, 90, 0);
    }

    public void RotateRight()
    {
        this.transform.Rotate(0, -90, 0);
    }
}