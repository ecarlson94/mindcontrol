using DigitalRune.Physics.ForceEffects;


namespace DigitalRune.Samples.Physics.Specialized
{
  [Sample(SampleCategory.PhysicsSpecialized,
    @"This sample shows how to implement vehicle physics.",
    @"A controllable car is created using a ray-car method where each wheel is implemented
by a short ray that senses the ground. The car supports suspension with damping, wheel
friction and sliding, etc.",
    50)]
  public class VehicleSample : PhysicsSpecializedSample
  {
    private readonly VehicleLevelObject _vehicleLevelObject;
    private readonly VehicleObject _vehicleObject;
    private readonly VehicleCameraObject _vehicleCameraObject;


    public VehicleSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Add basic force effects.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a game object which loads the test obstacles.
      _vehicleLevelObject = new VehicleLevelObject(Services);
      GameObjectService.Objects.Add(_vehicleLevelObject);

      // Add a game object which controls a vehicle.
      _vehicleObject = new VehicleObject(Services);
      GameObjectService.Objects.Add(_vehicleObject);

      // Add a camera that is attached to chassis of the vehicle.
      _vehicleCameraObject = new VehicleCameraObject(_vehicleObject.Vehicle.Chassis, Services);
      GameObjectService.Objects.Add(_vehicleCameraObject);
      GraphicsScreen.CameraNode = _vehicleCameraObject.CameraNode;
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        GameObjectService.Objects.Remove(_vehicleLevelObject);
        GameObjectService.Objects.Remove(_vehicleObject);
        GameObjectService.Objects.Remove(_vehicleCameraObject);
        GraphicsScreen.CameraNode = null;
      }

      base.Dispose(disposing);
    }
  }
}
