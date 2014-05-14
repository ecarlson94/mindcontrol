using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using Microsoft.Practices.ServiceLocation;

namespace WindowsGame1.VehicleSimulation
{
    public class VehicleCamera : GameObject
    {
        //----------------------------------------------------------------------
        #region Fields

        private readonly IServiceLocator _services;
        private readonly IInputService _inputService;
        private readonly IGeometricObject _vehicle;
        //x-axis pivot
        private float _yaw;
        //y-axis pivot
        private float _pitch;

        #endregion

        //----------------------------------------------------------------------
        #region Properties



        #endregion

        //----------------------------------------------------------------------
        #region Creation and Cleanup



        #endregion

        //----------------------------------------------------------------------
        #region Overridden Methods



        #endregion
    }
}
