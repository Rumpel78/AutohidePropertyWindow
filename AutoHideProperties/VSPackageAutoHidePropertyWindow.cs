using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AutoHideProperties
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(VSPackageAutoHidePropertyWindow.PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class VSPackageAutoHidePropertyWindow : Package
    {
        /// <summary>
        /// VSPackageAutoHidePropertyWindow GUID string.
        /// </summary>
        public const string PackageGuidString = "c45eb422-e040-4a48-96a5-0e4cfcf7bdd1";

        /// <summary>
        /// Initializes a new instance of the <see cref="VSPackageAutoHidePropertyWindow"/> class.
        /// </summary>
        public VSPackageAutoHidePropertyWindow()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        private DTE dte;
        private WindowEvents events;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            this.dte = GetService(typeof(SDTE)) as DTE;
            if (this.dte != null)
            {
                this.events = this.dte.Events.WindowEvents;
                this.events.WindowActivated += Events_WindowActivated;
            }
        }

        private void Events_WindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.Document == null)
                return;

            var pwin = this.dte.Windows.Item(EnvDTE.Constants.vsWindowKindProperties);
            pwin.AutoHides = !GotFocus.Caption.EndsWith(" [Design]") && !GotFocus.Caption.EndsWith(" [Entwurf]");
        }

        protected override void Dispose(bool disposing)
        {
            this.events.WindowActivated -= Events_WindowActivated;
            base.Dispose(disposing);
        }

        #endregion
    }
}
