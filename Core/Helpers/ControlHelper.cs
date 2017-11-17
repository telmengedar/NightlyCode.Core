﻿using System.Reflection;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// extensions for controls
    /// </summary>
    public static class ControlHelper {

            /// <summary>
            /// Extension method to return if the control is in design mode
            /// </summary>
            /// <param name="control">Control to examine</param>
            /// <returns>True if in design mode, otherwise false</returns>
            public static bool IsInDesignMode(this System.Windows.Forms.Control control) {
                return ResolveDesignMode(control);
            }


            /// <summary>
            /// Method to test if the control or it's parent is in design mode
            /// </summary>
            /// <param name="control">Control to examine</param>
            /// <returns>True if in design mode, otherwise false</returns>
            private static bool ResolveDesignMode(System.Windows.Forms.Control control) {
                if(System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv")
                    return true;

                // Get the protected property
                PropertyInfo designModeProperty = control.GetType().GetProperty(
                    "DesignMode",
                    BindingFlags.Instance
                    | BindingFlags.NonPublic);


                // Get the controls DesignMode value
                bool designMode = (bool)designModeProperty.GetValue(control, null);


                // Test the parent if it exists
                if(control.Parent != null) {
                    designMode |= ResolveDesignMode(control.Parent);
                }


                return designMode;
            }
        } 
}