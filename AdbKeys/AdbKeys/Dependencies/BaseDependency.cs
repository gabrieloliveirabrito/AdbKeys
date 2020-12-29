using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AdbKeys.Dependencies
{
    public class BaseDependency<TDependency>
        where TDependency : BaseDependency<TDependency>
    {
        public static TDependency Dependency
        {
            get => DependencyService.Get<TDependency>();
        }
    }
}
