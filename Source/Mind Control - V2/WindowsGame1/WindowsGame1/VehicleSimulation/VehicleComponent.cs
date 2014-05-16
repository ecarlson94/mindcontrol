using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Components;
using WindowsGame1.Managers;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;

namespace WindowsGame1.VehicleSimulation
{
    public class VehicleComponent : GraphicsBaseComponent
    {
        private readonly CognitivVehicle _cognitivVehicle;
        private readonly VehicleCamera _vehicleCamera;
        //scene goes here...

        protected VehicleComponent(Game game, EmoEngineManager emoEngine)
            : base(game, emoEngine)
        {
            //Add basic force effects
            Simulation.ForceEffects.Add(new Gravity());
            Simulation.ForceEffects.Add(new Damping());


        }
    }
}
