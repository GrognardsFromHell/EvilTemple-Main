#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using EvilTemple.Rules.Prototypes;
using Newtonsoft.Json;
using OpenTK;

#endregion

namespace EvilTemple.Rules
{
    public class BaseObject
    {
        private List<BaseObject> _content = new List<BaseObject>();
        private uint? _damageTaken;
        private bool? _drawBehindWalls;
        private uint? _hitPoints;
        private string _individualName;
        private bool? _interactive;
        private BaseObject _parent;
        private float? _rotation;
        private uint? _scale;
        private uint? _subdualDamageTaken;
        private uint? _temporaryHitPoints;
        private string _model;
        private uint? _selectionRadius;
        private int? _selectionHeight;
        private string _internalDescription;

        public BaseObject()
        {
            Prototype = DefaultPrototype;
        }

        [JsonIgnore]
        public IList<BaseObject> Content
        {
            get { return _content.AsReadOnly(); }
        }

        [JsonProperty(PropertyName = "content")]
        internal List<BaseObject> ContentInternal
        {
            get { return _content; }
            set { _content = value; }
        }

        [JsonIgnore]
        public BaseObject Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value && _parent != null)
                    _parent._content.Remove(this);
                _parent = value;
                if (_parent != null)
                    _parent._content.Add(this);
            }
        }

        public virtual BaseObjectPrototype Prototype { get; set; }

        public uint Scale
        {
            get { return _scale ?? Prototype.Scale; }
            set { _scale = (value == Prototype.Scale) ? (uint?) null : value; }
        }

        public bool ShouldSerializeScale()
        {
            return _scale.HasValue;
        }

        public float Rotation
        {
            get { return _rotation ?? Prototype.Rotation; }
            set { _rotation = (value == Prototype.Rotation) ? (float?) null : value; }
        }

        public bool ShouldSerializeRotation()
        {
            return _rotation.HasValue;
        }

        public uint SelectionRadius
        {
            get { return _selectionRadius ?? Prototype.SelectionRadius; }
            set { _selectionRadius = (value == Prototype.SelectionRadius) ? (uint?)null : value; }
        }
        
        public bool ShouldSerializeSelectionRadius()
        {
            return _selectionRadius.HasValue;
        }

        public int SelectionHeight
        {
            get { return _selectionHeight ?? Prototype.SelectionHeight; }
            set { _selectionHeight = (value == Prototype.SelectionHeight) ? (int?)null : value; }
        }

        public bool ShouldSerializeSelectionHeight()
        {
            return _selectionHeight.HasValue;
        }

        public bool Interactive
        {
            get { return _interactive ?? Prototype.Interactive; }
            set { _interactive = (value == Prototype.Interactive) ? (bool?) null : value; }
        }

        public bool ShouldSerializeInteractive()
        {
            return _interactive.HasValue;
        }

        public bool DrawBehindWalls
        {
            get { return _drawBehindWalls ?? Prototype.DrawBehindWalls; }
            set { _drawBehindWalls = (value == Prototype.DrawBehindWalls) ? (bool?) null : value; }
        }

        public bool ShouldSerializeDrawBehindWalls()
        {
            return _drawBehindWalls.HasValue;
        }

        public uint HitPoints
        {
            get { return _hitPoints ?? Prototype.HitPoints; }
            set { _hitPoints = (value == Prototype.HitPoints) ? (uint?) null : value; }
        }

        public bool ShouldSerializeHitPoints()
        {
            return _hitPoints.HasValue;
        }

        public uint TemporaryHitPoints
        {
            get { return _temporaryHitPoints ?? Prototype.TemporaryHitPoints; }
            set { _temporaryHitPoints = (value == Prototype.TemporaryHitPoints) ? (uint?) null : value; }
        }

        public bool ShouldSerializeTemporaryHitPoints()
        {
            return _temporaryHitPoints.HasValue;
        }

        public uint DamageTaken
        {
            get { return _damageTaken ?? Prototype.DamageTaken; }
            set { _damageTaken = (value == Prototype.DamageTaken) ? (uint?) null : value; }
        }

        public bool ShouldSerializeDamageTaken()
        {
            return _damageTaken.HasValue;
        }

        public uint SubdualDamageTaken
        {
            get { return _subdualDamageTaken ?? Prototype.SubdualDamageTaken; }
            set { _subdualDamageTaken = (value == Prototype.SubdualDamageTaken) ? (uint?) null : value; }
        }

        public bool ShouldSerializeSubdualDamageTaken()
        {
            return _subdualDamageTaken.HasValue;
        }

        public string IndividualName
        {
            get { return _individualName ?? Prototype.IndividualName; }
            set { _individualName = (value == Prototype.IndividualName) ? null : value; }
        }

        public bool ShouldSerializeIndividualName()
        {
            return _individualName != null;
        }

        public string Name
        {
            get { return IndividualName; }
        }

        public virtual BaseObjectPrototype DefaultPrototype
        {
            get { return BaseObjectPrototype.Default; }
        }

        public Vector3 Position { get; set; }

        public string Id { get; set; }
        
        public string Model
        {
            get { return _model ?? Prototype.Model; }
            set { _model = (value == Prototype.Model) ? null : value; }
        }

        public bool ShouldSerializeModel()
        {
            return _model != null;
        }
        
        private string _internalId;

        /// <summary>
        ///   A legacy id used to identify certain objects in the game world by python scripts.
        ///   In new scripts, the actual GUID should be used to identify objects.
        /// </summary>
        public string InternalId
        {
            get { return _internalId ?? Prototype.InternalId; }
            set { _internalId = (value == Prototype.InternalId) ? null : value; }
        }

        public bool ShouldSerializeInternalId()
        {
            return _internalId != null;
        }

        /// <summary>
        ///   An internal description for this object. Visible only in the world editor.
        /// </summary>
        public string InternalDescription
        {
            get { return _internalDescription ?? Prototype.InternalDescription; }
            set { _internalDescription = (value == Prototype.InternalDescription) ? null : value; }
        }

        public bool ShouldSerializeInternalDescription()
        {
            return _internalDescription != null;
        }

        public void RemoveItem(BaseObject baseObject)
        {
            Trace.Assert(baseObject._parent == this);
            baseObject._parent = null;
            _content.Remove(baseObject);
        }

        public void AddItem(BaseObject child)
        {
            if (child._parent == this)
                return;

            child._parent = this;
            _content.Add(child);
        }

        public event EventHandler<PropertyChangedEventArgs> OnPropertyChanged;

        protected void InvokeOnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            var handler = OnPropertyChanged;
            if (handler != null) handler(this, e);
        }
    }
}