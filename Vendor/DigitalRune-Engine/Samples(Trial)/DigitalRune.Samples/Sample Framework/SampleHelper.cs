using System;
using System.Linq;
using DigitalRune.Geometry;
using DigitalRune.Graphics.Effects;
using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics.Constraints;
using DigitalRune.Physics.Specialized;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples
{
  /// <summary>
  /// Provides useful helper and extension methods.
  /// </summary>
  public static class SampleHelper
  {
    /// <summary>
    /// Enables the per-pixel lighting for all contained meshes.
    /// </summary>
    /// <param name="node">The scene node.</param>
    public static void EnablePerPixelLighting(SceneNode node)
    {
      var effectBindings = node.GetSubtree()
                               .OfType<MeshNode>()
                               .SelectMany(meshNode => meshNode.Mesh.Materials)
                               .SelectMany(material => material.EffectBindings);

      foreach (var effectBinding in effectBindings)
      {
        if (effectBinding is BasicEffectBinding)
          ((BasicEffectBinding)effectBinding).PreferPerPixelLighting = true;
        else if (effectBinding is SkinnedEffectBinding)
          ((SkinnedEffectBinding)effectBinding).PreferPerPixelLighting = true;
      }
    }


    /// <summary>
    /// Visualizes the constraints of the ragdoll (for debugging).
    /// </summary>
    /// <param name="debugRenderer">The debug renderer.</param>
    /// <param name="ragdoll">The ragdoll.</param>
    /// <param name="scale">
    /// A scale factor that determines the size of the drawn elements.
    /// </param>
    /// <param name="drawOverScene">
    /// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
    /// disabled).
    /// </param>
    /// <remarks>
    /// Currently, only <see cref="TwistSwingLimit" />s and <see cref="AngularLimit" />s are
    /// supported.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="debugRenderer" /> or <paramref name="ragdoll" /> is <see langword="null" />.
    /// </exception>
    public static void DrawConstraints(this DebugRenderer debugRenderer, Ragdoll ragdoll, float scale, bool drawOverScene)
    {
      if (debugRenderer == null)
        throw new ArgumentNullException("debugRenderer");
      if (ragdoll == null)
        throw new ArgumentNullException("ragdoll");

      // Render information for each limit.
      foreach (Constraint limit in ragdoll.Limits)
      {
        // Get the ball joint constraint that connects the two bodies of the limit.
        BallJoint joint = null;
        foreach (Constraint constraint in ragdoll.Joints)
        {
          if (constraint.BodyA == limit.BodyA && constraint.BodyB == limit.BodyB
              || constraint.BodyA == limit.BodyB && constraint.BodyB == limit.BodyA)
          {
            joint = constraint as BallJoint;
            break;
          }
        }

        // Skip this limit if no joint was found.
        if (joint == null)
          continue;

        TwistSwingLimit twistSwingLimit = limit as TwistSwingLimit;
        if (twistSwingLimit != null)
        {
          DrawTwistSwingLimit(debugRenderer, joint, twistSwingLimit, scale, drawOverScene);
          continue;
        }

        AngularLimit angularLimit = limit as AngularLimit;
        if (angularLimit != null)
        {
          DrawAngularLimit(debugRenderer, joint, angularLimit, scale, drawOverScene);
          continue;
        }
      }
    }


    /// <summary>
    /// Visualizes the <see cref="TwistSwingLimit"/> of a <see cref="BallJoint"/>.
    /// </summary>
    /// <param name="debugRenderer">The debug renderer.</param>
    /// <param name="joint">The joint.</param>
    /// <param name="limit">The limit.</param>
    /// <param name="scale">
    /// A scale factor that determines the size of the drawn elements.
    /// </param>
    /// <param name="drawOverScene">
    /// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
    /// disabled).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="debugRenderer" />, <paramref name="joint" />,  or <paramref name="limit" />
    /// is <see langword="null" />.
    /// </exception>
    public static void DrawTwistSwingLimit(this DebugRenderer debugRenderer, BallJoint joint, TwistSwingLimit limit, float scale, bool drawOverScene)
    {
      if (debugRenderer == null)
        throw new ArgumentNullException("debugRenderer");
      if (joint == null)
        throw new ArgumentNullException("joint");
      if (limit == null)
        throw new ArgumentNullException("limit");

      // ----- Draw swing cone.
      // The tip of the swing cone:
      Vector3F coneTip = joint.BodyA.Pose.ToWorldPosition(joint.AnchorPositionALocal);

      // The first point on the swing cone:
      var previousConePoint = limit.GetPointOnCone(0, coneTip, scale);

      // Draw swing cone.
      const int numberOfSegments = 24;
      const float segmentAngle = ConstantsF.TwoPi / numberOfSegments;
      Color color = Color.Violet;
      for (int i = 0; i < numberOfSegments; i++)
      {
        var conePoint = limit.GetPointOnCone((i + 1) * segmentAngle, coneTip, scale);

        // Line from cone tip to cone base.
        debugRenderer.DrawLine(coneTip, conePoint, color, drawOverScene);
        
        // Line on the cone base.
        debugRenderer.DrawLine(previousConePoint, conePoint, color, drawOverScene);

        previousConePoint = conePoint;
      }

      // ----- Draw twist axis.      
      // The x-axis is the twist direction. 
      Vector3F twistAxis = Vector3F.UnitX;
      // The twist axis relative to body B.
      Vector3F twistAxisDirectionBLocal = limit.AnchorOrientationBLocal * twistAxis;
      // The twist axis relative to world space.
      Vector3F twistAxisDirection = limit.BodyB.Pose.ToWorldDirection(twistAxisDirectionBLocal);
      // (A similar computation is used in DrawArc() below.)

      // Line in twist direction.
      debugRenderer.DrawLine(coneTip, coneTip + twistAxisDirection * scale, Color.Red, drawOverScene);

      // A transformation that converts from constraint anchor space to world space.
      Pose constraintToWorld = limit.BodyA.Pose * new Pose(limit.AnchorOrientationALocal);

      // Draw an arc that visualizes the twist limits.
      DrawArc(debugRenderer, constraintToWorld, coneTip, Vector3F.UnitX, Vector3F.UnitY, limit.Minimum.X, limit.Maximum.X, scale, Color.Red, drawOverScene);
    }


    /// <summary>
    /// Visualizes the <see cref="AngularLimit"/> of a <see cref="BallJoint"/>.
    /// </summary>
    /// <param name="debugRenderer">The debug renderer.</param>
    /// <param name="joint">The joint.</param>
    /// <param name="limit">The limit.</param>
    /// <param name="scale">
    /// A scale factor that determines the size of the drawn elements.
    /// </param>
    /// <param name="drawOverScene">
    /// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
    /// disabled).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="debugRenderer" />, <paramref name="joint" />,  or <paramref name="limit" />
    /// is <see langword="null" />.
    /// </exception>
    public static void DrawAngularLimit(this DebugRenderer debugRenderer, BallJoint joint, AngularLimit limit, float scale, bool drawOverScene)
    {
      if (debugRenderer == null)
        throw new ArgumentNullException("debugRenderer");
      if (joint == null)
        throw new ArgumentNullException("joint");
      if (limit == null)
        throw new ArgumentNullException("limit");

      Vector3F jointPosition = joint.BodyA.Pose.ToWorldPosition(joint.AnchorPositionALocal);

      // A transformation that converts from constraint anchor space to world space.
      Pose constraintToWorld = limit.BodyA.Pose * new Pose(limit.AnchorOrientationALocal);

      // Draw an arc for each rotation axis. 
      DrawArc(debugRenderer, constraintToWorld, jointPosition, Vector3F.UnitX, Vector3F.UnitY, limit.Minimum.X, limit.Maximum.X, scale, Color.Red, drawOverScene);
      DrawArc(debugRenderer, constraintToWorld, jointPosition, Vector3F.UnitY, Vector3F.UnitX, limit.Minimum.Y, limit.Maximum.Y, scale, Color.Green, drawOverScene);
      DrawArc(debugRenderer, constraintToWorld, jointPosition, Vector3F.UnitZ, Vector3F.UnitX, limit.Minimum.Z, limit.Maximum.Z, scale, Color.Blue, drawOverScene);
    }


    /// <summary>
    /// Draws an arc to visualize a rotation limit about an axis.
    /// </summary>
    /// <param name="debugRenderer">The debug renderer.</param>
    /// <param name="constraintToWorld">
    /// A transformation that transforms from constraint anchor space to world space.
    /// </param>
    /// <param name="center">The center of the circle.</param>
    /// <param name="axis">The rotation axis.</param>
    /// <param name="direction">A direction vector (e.g. the direction of a bone).</param>
    /// <param name="minimum">The minimum angle.</param>
    /// <param name="maximum">The maximum angle.</param>
    /// <param name="scale">The scale.</param>
    /// <param name="color">The color.</param>
    /// <param name="drawOverScene">
    /// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
    /// disabled).
    /// </param>
    private static void DrawArc(this DebugRenderer debugRenderer, Pose constraintToWorld, Vector3F center, Vector3F axis, Vector3F direction, float minimum, float maximum, float scale, Color color, bool drawOverScene)
    {
      if (minimum == 0 && maximum == 0)
        return;

      // Line from circle center to start of arc.
      Vector3F previousArcPoint = center + scale * constraintToWorld.ToWorldDirection(QuaternionF.CreateRotation(axis, minimum).Rotate(direction));
      debugRenderer.DrawLine(center, previousArcPoint, color, drawOverScene);

      // Draw arc.
      int numberOfSegments = (int)Math.Max((maximum - minimum) / (ConstantsF.Pi / 24), 1);
      float segmentAngle = (maximum - minimum) / numberOfSegments;
      for (int i = 0; i < numberOfSegments; i++)
      {
        Vector3F arcPoint = center + scale * constraintToWorld.ToWorldDirection(QuaternionF.CreateRotation(axis, minimum + (i + 1) * segmentAngle).Rotate(direction));
        debugRenderer.DrawLine(previousArcPoint, arcPoint, color, drawOverScene);
        previousArcPoint = arcPoint;
      }

      // Line from end of arc to circle center.
      debugRenderer.DrawLine(previousArcPoint, center, color, drawOverScene);
    }
  }
}
