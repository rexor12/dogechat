using System;
using Xamarin.Forms;

namespace DogeChat.ViewModels
{
    /// <summary>
    /// Abstract base class for view-models.
    /// </summary>
    public abstract class ViewModelBase : BindableObject, IDisposable
    {
        /// <summary>
        /// Notifies the subscribers of a change in the property with the given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        public void RaisePropertyChanged(string propertyName) =>
            OnPropertyChanged(propertyName);

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public virtual void Dispose()
        {
        }
    }
}