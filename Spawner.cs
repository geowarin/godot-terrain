using Godot;

public partial class Spawner : Node3D
{
#pragma warning disable CS8618
  [Export]
  public PackedScene Scene;
#pragma warning restore CS8618
  
  public override void _Ready() {
    GD.Randomize();
  }

  public override void _PhysicsProcess(float delta) {
    if (Input.IsKeyPressed(Key.Space)) {
      var ball = (RigidDynamicBody3D) Scene.Instantiate();

      ball.Transform = Transform;
      ball.LinearVelocity = new Vector3(GD.Randf(), GD.Randf(), GD.Randf());

      AddChild(ball);
    }
  }
}
