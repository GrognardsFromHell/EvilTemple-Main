using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using EvilTemple.Rules.Utilities;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenTK;

namespace EvilTemple.Rules
{

    /// <summary>
    /// Represents the state that is necesarry to display a Map on screen.
    /// </summary>
    public class MapRenderState
    {
        private readonly Dictionary<BaseObject, IModelInstance> _objectRenderState;

        public static MapRenderState Current { get; private set; }

        public GlobalLightingController LightingController { get; private set; }

        public Map Map { get; private set; }

        public Campaign Campaign { get; private set; }

        public MapRenderState(Campaign campaign, Map map)
        {
            _objectRenderState = new Dictionary<BaseObject, IModelInstance>();
            Map = map;
            Campaign = campaign;
        }

        public void Activate()
        {
            if (Current != null)
                throw new InvalidOperationException("Cannot only activate one map render state at once.");

            LightingController = new GlobalLightingController();

            var scene = Services.Scene;
            scene.Clear();

            var backgroundMap = Services.RenderableFactory.CreateBackgroundMap();
            backgroundMap.Directory = Map.MapInfo.DayBackground;
            LightingController.BackgroundMap = backgroundMap;

            CreateLighting(scene);

            var node = scene.CreateNode();
            node.Attach(backgroundMap);

            // Load static objects
            if (Map.MapInfo.StaticObjectsFiles != null)
                LoadObjectsFromFile(scene, Map.MapInfo.StaticObjectsFiles);

            // Load dynamic objects
            if (Map.MapInfo.DynamicObjectsFile != null)
                LoadObjectsFromFile(scene, Map.MapInfo.DynamicObjectsFile);
        }

        private void LoadObjectsFromFile(IScene scene, string filename)
        {
            var data = Services.ResourceManager.ReadResource(filename);
            var reader = new JsonTextReader(new StringReader(Encoding.UTF8.GetString(data)));
            var objects = BaseObjectSerializer.Serializer.Deserialize<List<BaseObject>>(reader);

            foreach (var obj in objects)
                CreateSceneNode(scene, obj);
        }

        public void Deactivate()
        {
            if (Current != this)
                throw new InvalidOperationException("Can only deactivate the current map render state.");

            _objectRenderState.Clear();
            Services.Scene.Clear();
            // TODO Implementation
        }

        private void CreateLighting(IScene scene)
        {
            // Create global lighting in form of an infinite-range directional light
            var globalLight = Services.RenderableFactory.CreateDynamicLight();
            globalLight.Range = 10000000000; // The light should be visible anywhere on the Map
            globalLight.Type = DynamicLightType.Directional;
            globalLight.Color = Map.MapInfo.Lighting.Color.ToVector4(); // The color is actually used
            globalLight.Direction = new Vector4(-0.6324094f, -0.7746344f, 0, 0); // the direction is fixed for *all* maps, regardless of lighting.
            
            var sceneNode = scene.CreateNode();

            sceneNode.Position = new Vector3(480 * 28, 0, 480 * 28);
            sceneNode.Attach(globalLight);

            // Map.renderGlobalLight = globalLight;
            LightingController.Settings = Map.MapInfo.Lighting;
            LightingController.DynamicLight = globalLight;
            LightingController.ChangeTime(Campaign.Time);

            if (Map.MapInfo.DynamicLightsFile == null)
                return;

            var lightsData = Services.ResourceManager.ReadResource(Map.MapInfo.DynamicLightsFile);
            var lightsReader = new JsonTextReader(new StreamReader(new MemoryStream(lightsData)));
            var serializer = new JsonSerializer
                                 {
                                     ContractResolver = new CamelCasePropertyNamesContractResolver()
                                 };
            serializer.Converters.Add(new Vector2Converter());
            serializer.Converters.Add(new Vector3Converter());
            var lights = serializer.Deserialize<List<LightInfo>>(lightsReader);
            
            Trace.TraceInformation("Creating {0} lights.", lights.Count);

            foreach (var light in lights) {
                sceneNode = scene.CreateNode();
                sceneNode.Interactive = false;
                sceneNode.Position = light.Position;
                
                var lightRenderable = Services.RenderableFactory.CreateDynamicLight();
                switch (light.Type)
                {
                    case 1:
                        lightRenderable.Type = DynamicLightType.Point;
                        break;
                    case 2:
                        lightRenderable.Type = DynamicLightType.Spot;
                        break;
                    case 3:
                        lightRenderable.Type = DynamicLightType.Directional;
                        break;
                }

                lightRenderable.Range = light.Range;
                lightRenderable.Attenuation = 4; // obj.range * obj.range
                lightRenderable.Color = light.Color;
                // Enable this to see the range of lights
                // light.debugging = true;

                sceneNode.Attach(lightRenderable);
            }
        }

        private void CreateSceneNode(IScene scene, BaseObject baseObject)
        {
            var node = scene.CreateNode();
            node.Position = baseObject.Position;
            node.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(baseObject.Rotation));
            node.Scale = new Vector3(baseObject.Scale / 100.0f, baseObject.Scale / 100.0f, baseObject.Scale / 100.0f);
            node.Interactive = baseObject.Interactive;

            var baseModel = Services.Models.Load(baseObject.Model);
            var modelInstance = Services.RenderableFactory.CreateModelInstance();
            modelInstance.Model = baseModel;

            // Add equipment?
            var critter = baseObject as Critter;
            if (critter != null)
            {
                var equipmentBuilder = new CritterEquipmentBuilder(Services.Get<EquipmentStyles>());
                equipmentBuilder.BuildEquipment(critter);

                modelInstance.ClearOverrideMaterials();
                // TODO: Clear Add Meshes

                foreach (var entry in equipmentBuilder.OverrideMaterials)
                {
                    var material = Services.Materials.Load(entry.Value);
                    modelInstance.OverrideMaterial(entry.Key, material);
                }

                foreach (var entry in equipmentBuilder.AddMeshes)
                {
                    Trace.TraceInformation("Loading addmesh {0}", entry);
                    var model = Services.Models.Load(entry);
                    modelInstance.AddMesh(model);
                }

                foreach (var entry in equipmentBuilder.MaterialProperties)
                {
                    Trace.TraceInformation("Setting material property {0} to {1}", entry.Key, entry.Value);
                    if (entry.Value is Vector4)
                        modelInstance.SetMaterialProperty(entry.Key, (Vector4) entry.Value);
                    else if (entry.Value is float)
                        modelInstance.SetMaterialProperty(entry.Key, (float)entry.Value);
                    else
                        Trace.TraceError("Unknown material property type {0} for property {1}", entry.Value.GetType(), entry.Key);
                }
            }

            if (baseObject.Interactive)
            {
                var selectionCircle = Services.RenderableFactory.CreateSelectionCircle();

                if (baseObject.SelectionRadius > 0)
                    selectionCircle.Radius = baseObject.SelectionRadius;
                else
                    selectionCircle.Radius = 25;

                // TODO: Reaction Color
                selectionCircle.Color = new Vector4(1, 0, 0, 1);

                // TODO: This may be a bit slow for every interactive object.
                // if (Selection.isSelected(this))
                //    selectionCircle.selected = true;

                node.Attach(selectionCircle);

                // this.registerHandlers(sceneNode, modelInstance);
                modelInstance.OnMouseReleased += () => Console.WriteLine("Hello: " + baseObject.Rotation + " " +
                                                                         baseObject.Model);
                selectionCircle.OnMouseReleased += () => Console.WriteLine("Hello: " + baseObject.Rotation + " " +
                                                                         baseObject.Model);
                selectionCircle.OnMouseEnter += () =>
                                                    {
                                                        selectionCircle.Hovering = true;
                                                    };
                selectionCircle.OnMouseLeave += () =>
                                                    {
                                                        selectionCircle.Hovering = false;
                                                    };
            }

            node.Attach(modelInstance);

            _objectRenderState[baseObject] = modelInstance;
        }
        
        private void UpdateDayNight(int oldHour, GameTime now)
        {
            var newHour = now.HourOfDay;
            Trace.TraceInformation("New Hour: " + newHour + " Old Hour: " + oldHour);

            var oldHourWasNight = oldHour < 6 || oldHour >= 18;
            var newHourIsNight = newHour < 6 || newHour >= 18;
            
            // No background fade is necessary if there's no night background
            if (Map.MapInfo.NightBackground == null)
                return;

            var fadeNightToDay = oldHourWasNight && !newHourIsNight;
            var fadeDayToNight = !oldHourWasNight && newHourIsNight;

            if (!fadeNightToDay && !fadeDayToNight)
                return;

            Vector4 fading3DStartColor;
            Vector4 fading3DEndColor;

            var fadingOutMap = Services.RenderableFactory.CreateBackgroundMap();
            var fadingOutMapNode = Services.Scene.CreateNode();
            fadingOutMapNode.Attach(fadingOutMap);

            if (fadeNightToDay)
            {
                Trace.TraceInformation("TRANSITION NIGHT->DAY");
                
                fadingOutMap.Directory = Map.MapInfo.NightBackground;
                if (Map.MapInfo.Lighting.Night2dKeyframes.Count > 0)
                    fadingOutMap.Color =
                        new Vector4(
                            GlobalLightingController.InterpolateColor(newHour, Map.MapInfo.Lighting.Night2dKeyframes),
                            1);
                    

                if (Map.MapInfo.Lighting.Night3dKeyframes.Count > 0)
                    fading3DStartColor =
                        new Vector4(
                            GlobalLightingController.InterpolateColor(newHour, Map.MapInfo.Lighting.Night3dKeyframes),
                            1);
                else
                    fading3DStartColor = Map.MapInfo.Lighting.Color.ToVector4();

                if (Map.MapInfo.Lighting.Day3dKeyframes.Count > 0)
                    fading3DEndColor =
                        new Vector4(
                            GlobalLightingController.InterpolateColor(newHour, Map.MapInfo.Lighting.Day3dKeyframes),
                            1);
                else
                    fading3DEndColor = Map.MapInfo.Lighting.Color.ToVector4();

                Trace.TraceInformation("Switching background Map to: " + Map.MapInfo.DayBackground);
                LightingController.BackgroundMap.Directory = Map.MapInfo.DayBackground;
            }
            else
            {
                Trace.TraceInformation("TRANSITION DAY->NIGHT");

                Trace.TraceInformation("Switching background Map to: " + Map.MapInfo.NightBackground);

                fadingOutMap.Directory = Map.MapInfo.DayBackground;
                if (Map.MapInfo.Lighting.Day2dKeyframes.Count > 0)
                    fadingOutMap.Color = new Vector4(
                        GlobalLightingController.InterpolateColor(newHour, Map.MapInfo.Lighting.Day2dKeyframes),
                        1);

                if (Map.MapInfo.Lighting.Day3dKeyframes != null)
                    fading3DStartColor =
                        new Vector4(GlobalLightingController.InterpolateColor(newHour,
                                                                              Map.MapInfo.Lighting.Day3dKeyframes), 1);
                else
                    fading3DStartColor = Map.MapInfo.Lighting.Color.ToVector4();

                if (Map.MapInfo.Lighting.Night3dKeyframes != null)
                    fading3DEndColor = new Vector4(GlobalLightingController.InterpolateColor(newHour, Map.MapInfo.Lighting.Night3dKeyframes), 1);
                else
                    fading3DEndColor = Map.MapInfo.Lighting.Color.ToVector4();

                LightingController.BackgroundMap.Directory = Map.MapInfo.NightBackground;
            }

            LightingController.ChangeTime(Campaign.Time);

            var crossfader = new CrossFader();
            crossfader.OnProgress += factor =>
                                         {
                                             var backgroundMap = LightingController.BackgroundMap;

                                             // Fade between day/night background Map
                                             backgroundMap.Color = new Vector4(backgroundMap.Color.Xyz, factor);

                                             fadingOutMap.Color = new Vector4(fadingOutMap.Color.Xyz, 1 - factor);

                                             // Interpolate between 3d colors
                                             var diff = fading3DEndColor - fading3DStartColor;

                                             LightingController.DynamicLight.Color = fading3DStartColor + factor * diff;
                                         };

            crossfader.OnFinish += () =>
                                       {
                                           Services.Scene.Remove(fadingOutMapNode);
                                           LightingController.ChangeTime(Campaign.Time);
                                       };

            crossfader.CrossFade(1000);
        }
}
}
