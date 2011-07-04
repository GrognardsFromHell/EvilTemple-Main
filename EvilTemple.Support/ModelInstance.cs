using System;
using EvilTemple.Rules;
using EvilTemple.Runtime;
using Rules;

namespace EvilTemple.Support
{
    /// <summary>
    /// Wraps a C++ model instance and links it to a Critter. The model instance will be automatically updated with the critter's
    /// equipment and statistics, when they change.
    /// </summary>
    public class ModelInstance : IDisposable
    {
        private dynamic _modelInstance;
        
        private Critter _critter;

        public ModelInstance(dynamic modelInstance, Critter critter)
        {
            _modelInstance = modelInstance;
            _critter = critter;
            _critter.OnPropertyChanged += (sender, e) => UpdateModel();
            UpdateModel();
        }

        private void UpdateModel()
        {
            var model = Services.Models.Load(_critter.Model);
            _modelInstance.model = model;
        }

        public void Dispose()
        {
            _critter = null;
            _modelInstance = null;
            GC.SuppressFinalize(this);
        }
    }
}
