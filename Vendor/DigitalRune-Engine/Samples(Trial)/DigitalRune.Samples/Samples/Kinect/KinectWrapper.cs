using System;
using DigitalRune.Animation.Character;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

// Type Skeleton exists in DigitalRune Animation and in the Kinect SDK.
using DRSkeleton = DigitalRune.Animation.Character.Skeleton;
using KinectSkeleton = Microsoft.Kinect.Skeleton;

// Type MathHelper exists in DigitalRune Mathematics and in XNA.
using MathHelper = DigitalRune.Mathematics.MathHelper;


namespace DigitalRune.Samples.Kinect
{
  // This game component manages the Kinect sensor. Kinect is initialized. Two 
  // player skeletons are tracked. 
  // The skeleton data is stored in SkeletonPose instances (SkeletonPoseA and SkeletonPoseB). 
  // These SkeletonPose instances are different from usual SkeletonPose instance: Normally, most 
  // bone transformations contain only rotations and no translations. But the Kinect skeleton 
  // data consists only of translations (= joint positions) but no rotations! That means,
  // SkeletonPoseA and SkeletonPoseB correctly describe joint positions but contain no 
  // information about bone rotations.
  public class KinectWrapper : GameComponent
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    
    // The Kinect device.
    private KinectSensor _kinect;

    // A buffer for Kinect skeleton data (see OnSkeletonFrameReady());
    private KinectSkeleton[] _kinectSkeletons;

    // The Kinect tracking ID for the two players. If a player is not tracked, the ID is 0.
    private int _trackingIdA;
    private int _trackingIdB;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    // Is the Kinect sensor running?
    public bool IsRunning
    {
      get { return _kinect != null; }
    }


    // Is the first player currently being tracked?
    public bool IsTrackedA
    {
      get { return _trackingIdA > 0; }
    }


    // Is the second player currently being tracked?
    public bool IsTrackedB
    {
      get { return _trackingIdB > 0; }
    }


    // The SkeletonPose that stores Kinect skeleton data for the first player.
    public SkeletonPose SkeletonPoseA { get; private set; }


    // The SkeletonPose that stores Kinect skeleton data for the second player.
    public SkeletonPose SkeletonPoseB { get; private set; }


    // An offset that is applied to the Kinect joint positions.
    public Vector3F Offset { get; set; }


    // A scale factor that is applied to the Kinect joint positions.
    public Vector3F Scale { get; set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    public KinectWrapper(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      Offset = new Vector3F(0, 0, 0);
      Scale = new Vector3F(1, 1, 1);
      InitializeSkeletonPoses();
    }


    private void InitializeSkeletonPoses()
    {
      // Create a list of the bone/joint names of a Kinect skeleton.
      int numberOfJoints = Enum.GetNames(typeof(JointType)).Length;
      var boneNames = new string[numberOfJoints];
      for (int i = 0; i < numberOfJoints; i++)
        boneNames[i] = ((JointType)i).ToString();

      // Create list with one entry per bone. Each entry is the index of the parent bone.
      var boneParents = new[]
      {
        -1,
        (int)JointType.HipCenter,
        (int)JointType.Spine,
        (int)JointType.ShoulderCenter,
        (int)JointType.ShoulderCenter,
        (int)JointType.ShoulderLeft,
        (int)JointType.ElbowLeft,
        (int)JointType.WristLeft,
        (int)JointType.ShoulderCenter,
        (int)JointType.ShoulderRight,
        (int)JointType.ElbowRight,
        (int)JointType.WristRight,
        (int)JointType.HipCenter,
        (int)JointType.HipLeft,
        (int)JointType.KneeLeft,
        (int)JointType.AnkleLeft,
        (int)JointType.HipCenter,
        (int)JointType.HipRight,
        (int)JointType.KneeRight,
        (int)JointType.AnkleRight,
      };

      // Create a list of the bind pose transformations of all bones. Since we do not 
      // get such information from Kinect, we position all bones in the local origin.
      var boneBindPoses = new SrtTransform[numberOfJoints];
      for (int i = 0; i < numberOfJoints; i++)
        boneBindPoses[i] = SrtTransform.Identity;

      // Create a skeleton that defines the bone hierarchy and rest pose.
      var skeleton = new DRSkeleton(boneParents, boneNames, boneBindPoses);

      // Create a SkeletonPose for each player. 
      SkeletonPoseA = SkeletonPose.Create(skeleton);
      SkeletonPoseB = SkeletonPose.Create(skeleton);
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    public override void Update(GameTime gameTime)
    {
      // Kinect was not found yet. (Re-)Try initialization.
      if (_kinect == null)
        InitializeKinect();

      base.Update(gameTime);
    }


    private void InitializeKinect()
    {
      // Wait until a Kinect sensor is connected and ready.
      if (KinectSensor.KinectSensors.Count == 0 || KinectSensor.KinectSensors[0].Status != KinectStatus.Connected)
        return;

      // Start Kinect.
      _kinect = KinectSensor.KinectSensors[0];

#if USE_KINECT_SMOOTHING
      _kinect.SkeletonStream.Enable(new TransformSmoothParameters
      {
        Correction = 0.5f,
        JitterRadius = 0.05f,
        MaxDeviationRadius = 0.04f,
        Prediction = 0.5f,
        Smoothing = 0.9f,
      });
#else
      _kinect.SkeletonStream.Enable();
#endif

      _kinect.Start();
      _kinect.SkeletonFrameReady += OnSkeletonFrameReady;
    }


    // Called when Kinect has new skeleton data.
    private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs eventArgs)
    {
      // Get the new skeleton data from Kinect.
      using (var skeletonFrame = eventArgs.OpenSkeletonFrame())
      {
        if (skeletonFrame == null)
          return;

        if (_kinectSkeletons == null || _kinectSkeletons.Length != skeletonFrame.SkeletonArrayLength)
          _kinectSkeletons = new KinectSkeleton[skeletonFrame.SkeletonArrayLength];

        skeletonFrame.CopySkeletonDataTo(_kinectSkeletons);
      }

      // Get the two tracked skeletons.
      KinectSkeleton skeletonDataA = null;
      KinectSkeleton skeletonDataB = null;
      foreach (var skeleton in _kinectSkeletons)
      {
        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
        {
          if (skeletonDataA == null)
            skeletonDataA = skeleton;
          else
            skeletonDataB = skeleton;
        }
      }

      // Make sure that each player uses the same skeleton as last time. 
      // Swap skeleton data if necessary.
      if (skeletonDataA != null && skeletonDataA.TrackingId == _trackingIdB
          || skeletonDataB != null && skeletonDataB.TrackingId == _trackingIdA)
      {
        MathHelper.Swap(ref skeletonDataA, ref skeletonDataB);
      }

      // Update tracking IDs. 
      _trackingIdA = (skeletonDataA != null) ? skeletonDataA.TrackingId : 0;
      _trackingIdB = (skeletonDataB != null) ? skeletonDataB.TrackingId : 0;

      // Update the SkeletonPose from the Kinect skeleton data.
      UpdateKinectSkeletonPose(skeletonDataA, SkeletonPoseA);
      UpdateKinectSkeletonPose(skeletonDataB, SkeletonPoseB);
    }


    private void UpdateKinectSkeletonPose(KinectSkeleton skeletonData, SkeletonPose skeletonPose)
    {
      if (skeletonData == null)
        return;

      // Update the skeleton pose using the data from Kinect. 
      for (int i = 0; i < skeletonPose.Skeleton.NumberOfBones; i++)
      {
        var joint = (JointType)i;
        if (skeletonData.Joints[joint].TrackingState != JointTrackingState.NotTracked)
        {
          // The joint position in "Kinect space".
          SkeletonPoint kinectPosition = skeletonData.Joints[joint].Position;

          // Convert Kinect joint position to a Vector3F.
          // z is negated because in XNA the camera forward vectors is -z, but the Kinect
          // forward vector is +z. 
          Vector3F position = new Vector3F(kinectPosition.X, kinectPosition.Y, -kinectPosition.Z);

          // Apply scale and offset.
          position = position * Scale + Offset;

          var orientation = QuaternionF.Identity;

          // Optional:
          // The newer Kinect SDKs also compute bone orientations. We do not need these values 
          // because the current samples use only the joint positions to derive bone rotations.
          //if (joint != JointType.HipCenter)   // Motion retargeting seems to work better if the hip center bone is not rotated.
          //{
          //  orientation = GetBoneOrientation(skeletonData, skeletonPose, i);
          //}

          skeletonPose.SetBonePoseAbsolute(i, new SrtTransform(orientation, position));
        }
      }
    }


    private QuaternionF GetBoneOrientation(KinectSkeleton skeletonData, SkeletonPose skeletonPose, int boneIndex)
    {
      // Get first child bone. (Kinect stores the bone orientations in the child joints.)
      int childIndex = -1;
      if (skeletonPose.Skeleton.GetNumberOfChildren(boneIndex) > 0)
        childIndex = skeletonPose.Skeleton.GetChild(boneIndex, 0);

      if (childIndex == -1)
        return QuaternionF.Identity;

      var q = skeletonData.BoneOrientations[(JointType)childIndex].AbsoluteRotation.Quaternion;

      // Convert to DigitalRune quaternion and mirror z axis.
      return new QuaternionF(-q.W, q.X, q.Y, -q.Z);
    }


    protected override void Dispose(bool disposing)
    {
      // Clean up.
      if (_kinect != null)
      {
        _kinect.SkeletonFrameReady -= OnSkeletonFrameReady;
        _kinect.Stop();
        _kinect = null;
      }

      base.Dispose(disposing);
    }
    #endregion
  }
}