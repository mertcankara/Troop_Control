public class RunnerUnit : Unit
{
    protected override void Start()
    {
        base.Start();

        SetDefaults(DefaultMovementSpeed * 10f, DefaultAcceleration * 30f, DefaultRadius / 5f); 
    }
}