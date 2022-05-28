using Godot;

public partial class FlyCam : CharacterBody3D {
  [Export] public float Gravity = -24.8f;
  [Export] public float MaxSpeed = 50.0f;
  [Export] public float JumpSpeed = 18.0f;
  [Export] public float Accel = 10f;
  [Export] public float Deaccel = 16.0f;
  [Export] public float MaxSlopeAngle = 40.0f;
  [Export] public float MouseSensitivity = 0.05f;

  private Vector3 _vel;
  private Vector3 _dir;

  private Camera3D _camera;
  private Node3D _rotationHelper;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    _camera = GetNode<Camera3D>("RotationHelper/Camera");
    _rotationHelper = GetNode<Node3D>("RotationHelper");

    Input.SetMouseMode(Input.MouseMode.Captured);
  }

  public override void _PhysicsProcess(float delta) {
    ProcessInput(delta);
    ProcessMovement(delta);
  }

  private void ProcessInput(float delta) {
    //  -------------------------------------------------------------------
    //  Walking
    _dir = new Vector3();
    var camXform = _camera.GlobalTransform;

    var inputMovementVector = Input.GetVector("ui_left", "ui_right", "ui_down", "ui_up");

    // Basis vectors are already normalized.
    _dir += -camXform.basis.z * inputMovementVector.y;
    _dir += camXform.basis.x * inputMovementVector.x;
    //  -------------------------------------------------------------------

    //  -------------------------------------------------------------------
    //  Jumping
    if (IsOnFloor()) {
      if (Input.IsActionJustPressed("movement_jump"))
        _vel.y = JumpSpeed;
    }
    //  -------------------------------------------------------------------

    //  -------------------------------------------------------------------
    //  Capturing/Freeing the cursor
    if (Input.IsActionJustPressed("ui_cancel")) {
      Input.SetMouseMode(Input.GetMouseMode() == Input.MouseMode.Visible
                             ? Input.MouseMode.Captured
                             : Input.MouseMode.Visible);
    }
    //  -------------------------------------------------------------------
  }

  private void ProcessMovement(float delta) {
    _dir.y = 0;
    _dir = _dir.Normalized();

    // _vel.y += delta * Gravity;

    var hvel = _vel;
    hvel.y = 0;

    var target = _dir;

    target *= MaxSpeed;

    var accel = _dir.Dot(hvel) > 0 ? Accel : Deaccel;

    hvel = hvel.Lerp(target, accel * delta);
    _vel.x = hvel.x;
    _vel.z = hvel.z;
    Velocity = _vel;
    MoveAndSlide();
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseMotion mouseEvent &&
        Input.GetMouseMode() == Input.MouseMode.Captured) {
      _rotationHelper.RotateX(Mathf.Deg2Rad(mouseEvent.Relative.y * MouseSensitivity));
      RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));
      
      // Vector3 cameraRot = _rotationHelper.RotationDegrees;
      // cameraRot.x = Mathf.Clamp(cameraRot.x, -70, 70);
      // _rotationHelper.RotationDegrees = cameraRot;
    }
  }
}
