#region using

using System.ComponentModel;
using HBD.Framework.Core;
using HBD.Mef.Shell.Core;

#endregion

namespace HBD.Mef.Shell.Navigation
{
    public interface IMenuInfo : IParentable<IMenuInfoCollection>, INotifyPropertyChanged
    {
        MenuAlignment Alignment { get; set; }
        int DisplayIndex { get; set; }
        IPermissionValidationInfo PermissionValidation { get; set; }
    }
}