using Godot;

public partial class FlyCam : CharacterBody3D {
  [Export] public float MaxSpeed = 50.0f;
  [Export] public float JumpSpeed = 18.0f;
  [Export] public float Accel = 10f;
  [Export] public float Deaccel = 16.0f;
  [Export] public float MouseSensitivity = 0.05f;

  // private readonly float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

#pragma warning disable CS8618
  private Camera3D _camera;
  private Node3D _rotationHelper;
#pragma warning restore CS8618

  public override void _PhysicsProcess(float delta) {
    var camXform = _camera.GlobalTransform;
    var inputMovementVector = Input.GetVector("ui_left", "ui_right", "ui_down", "ui_up");
    var dir = new Vector3();
    dir += -camXform.basis.z * inputMovementVector.y;
    dir += camXform.basis.x * inputMovementVector.x;
    dir = dir.Normalized();

    var vel = Velocity;

    if (IsOnFloor() && Input.IsActionJustPressed("movement_jump")) {
      vel.y = JumpSpeed;
    }
    // vel.y += delta * _gravity;

    if (Input.IsActionJustPressed("ui_cancel")) {
      Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible
          ? Input.MouseModeEnum.Captured
          : Input.MouseModeEnum.Visible;
    }

    var target = dir * MaxSpeed;
    var accel = dir.Dot(vel) > 0 ? Accel : Deaccel;
    vel = vel.Lerp(target, accel * delta);

    Velocity = vel;
    MoveAndSlide();
  }

  public override void _Ready() {
    _camera = GetNode<Camera3D>("RotationHelper/Camera");
    _rotationHelper = GetNode<Node3D>("RotationHelper");

    Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseMotion mouseEvent &&
        Input.MouseMode == Input.MouseModeEnum.Captured) {
      _rotationHelper.RotateX(Mathf.Deg2Rad(mouseEvent.Relative.y * MouseSensitivity));
      RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));

      // Vector3 cameraRot = _rotationHelper.RotationDegrees;
      // cameraRot.x = Mathf.Clamp(cameraRot.x, -70, 70);
      // _rotationHelper.RotationDegrees = cameraRot;
    }
  }
}
