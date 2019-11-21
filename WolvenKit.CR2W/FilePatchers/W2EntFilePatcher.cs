using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.BatchProcessors;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2EntFilePatcher : W2XFilePatcher
    {
        private const string TypeCActionPoint = "CActionPoint";
        private const string TypeCEntity = "CEntity";
        private const string TypeCEntityTemplate = "CEntityTemplate";
        private const string TypeEngineTransform = "EngineTransform";
        private const string TypeCFXDefinition = "CFXDefinition";
        private const string TypeCFXTrackItemParticles = "CFXTrackItemParticles";
        private const string TypeCGameplayEntity = "CGameplayEntity";
        private const string TypeCMesh = "CMesh";
        private const string TypeCMeshComponent = "CMeshComponent";
        private const string TypeCParticleComponent = "CParticleComponent";
        private const string TypeCParticleSystem = "CParticleSystem";
        private const string TypeCPointLightComponent = "CPointLightComponent";
        private const string TypeCRigidMeshComponent = "CRigidMeshComponent";
        private const string TypeCSpotLightComponent = "CSpotLightComponent";
        private const string TypeCStaticMeshComponent = "CStaticMeshComponent";
        private const string TypeSharedDataBuffer = "SharedDataBuffer";
        private const string TypeW3AnimationInteractionEntity = "W3AnimationInteractionEntity";
        private const string TypeW3Campfire = "W3Campfire";
        private const string TypeW3FireSource = "W3FireSource";
        private const string TypeW3FireSourceLifeRegen = "W3FireSourceLifeRegen";
        private const string TypeW3LightEntityDamaging = "W3LightEntityDamaging";
        private const string TypeW3LightSource = "W3LightSource";
        private const string TypeW3MonsterClue = "W3MonsterClue";

        private const string VariableNameBrightness = "brightness";
        private const string VariableNameBuffer = "buffer";
        private const string VariableNameCookedEffects = "cookedEffects";
        private const string VariableNameFlatCompiledData = "flatCompiledData";
        private const string VariableNameName = "name";
        private const string VariableNameParticleSystem = "particleSystem";
        private const string VariableNameRadius = "radius";
        private const string VariableNameShowDistance = "showDistance";
        private const string VariableNameStreamingDataBuffer = "streamingDataBuffer";
        private const string VariableNameStreamingDistance = "streamingDistance";
        private const string VariableNameTransform = "transform";

        private const string SuffixILODCollision = "_ilod_collision";

        public W2EntFilePatcher(ILocalizedStringSource localizedStringSource) : base(localizedStringSource)
        {
        }

        public List<string> PatchForIncreasedDrawDistance(string filePath, Dictionary<string, string> relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap,
            Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap, W2EntSettings w2EntSettings)
        {
            List<string> relativeCollisionMeshFilePaths = new List<string>();

            CR2WFile w2EntFile = ReadW2EntFile(filePath, localizedStringSource);

            if (w2EntFile == null)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' could not be loaded.");
            }

            (List<CByteArrayContainer> sharedDataBuffersForFires, CByteArrayContainer flatCompiledData) = ReadSharedDataBuffersAndFlatCompiledDataForFires(w2EntFile);

            if (sharedDataBuffersForFires != null)
            {
                foreach (CByteArrayContainer sharedDataBufferForFire in sharedDataBuffersForFires)
                {
                    PatchFireShowDistance(filePath, sharedDataBufferForFire, w2EntSettings);
                    PatchW2PFilePath(filePath, sharedDataBufferForFire, flatCompiledData, relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap);

                    WriteCByteArrayContainer(sharedDataBufferForFire);
                    WriteCByteArrayContainer(flatCompiledData);
                }
            }
            else
            {
                PatchW2PFilePath(filePath, null, flatCompiledData, relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap);
                WriteCByteArrayContainer(flatCompiledData);
            }

            if (PatchGlowAutoHideDistance(filePath, flatCompiledData, w2EntSettings))
            {
                WriteCByteArrayContainer(flatCompiledData);
            }

            CByteArrayContainer streamingDataBufferForFires = ReadStreamingDataBufferForFires(w2EntFile);

            if (streamingDataBufferForFires != null)
            {
                PatchMeshStreamingDistance(filePath, w2EntFile, w2EntSettings);
                relativeCollisionMeshFilePaths.AddRange(PatchW2MeshFilePath(filePath, streamingDataBufferForFires.Content.chunks, relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap));

                WriteCByteArrayContainer(streamingDataBufferForFires);
            }

            relativeCollisionMeshFilePaths.AddRange(PatchW2MeshFilePath(filePath, w2EntFile.chunks, relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap));

            WriteCR2WFile(w2EntFile);

            return relativeCollisionMeshFilePaths;
        }

        internal static CR2WFile ReadW2EntFile(string filePath, ILocalizedStringSource localizedStringSource)
        {
            return ReadCR2WFile(filePath, localizedStringSource);
        }

        internal static (List<CByteArrayContainer> sharedDataBuffersForFires, CByteArrayContainer flatCompiledData) ReadSharedDataBuffersAndFlatCompiledDataForFires(CR2WFile w2EntFile)
        {
            List<CByteArrayContainer> sharedDataBuffersForFires = null;
            CByteArrayContainer flatCompiledData = null;

            string filePath = w2EntFile.FileName;
            bool cEntityTemplateFound = false;

            foreach (CR2WChunk chunk in w2EntFile.chunks)
            {
                if (!IsCEntityTemplate(chunk))
                {
                    continue;
                }
                else if (cEntityTemplateFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + TypeCEntityTemplate + "'.");
                }

                cEntityTemplateFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + TypeCEntityTemplate + "'.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsCookedEffects(variable))
                    {
                        if (sharedDataBuffersForFires != null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one cooked effects variable with shared data buffers.");
                        }

                        sharedDataBuffersForFires = GetSharedDataBuffersForFiresFromCookedEffectsVariable(variable, filePath, w2EntFile.LocalizedStringSource);
                    }
                    else if (IsFlatCompiledData(variable))
                    {
                        if (flatCompiledData != null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one flat compiled data.");
                        }

                        CR2WFile flatCompiledDataContent = ReadCByteArrayContainerContent((CByteArray)variable, w2EntFile.LocalizedStringSource);

                        if (flatCompiledDataContent == null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains flat compiled data which could not be read.");
                        }

                        flatCompiledData = new CByteArrayContainer(flatCompiledDataContent, (CByteArray)variable);
                    }
                }
            }

            return (sharedDataBuffersForFires, flatCompiledData);
        }

        private static List<CByteArrayContainer> GetSharedDataBuffersForFiresFromCookedEffectsVariable(CVariable variableCookedEffects, string filePath, ILocalizedStringSource localizedStringSource)
        {
            List<CByteArrayContainer> sharedDataBuffersForFires = new List<CByteArrayContainer>();

            foreach (CVariable cookedEffectsEntry in ((CArray)variableCookedEffects).array)
            {
                if (cookedEffectsEntry is CVector)
                {
                    List<CVariable> cookedEffectsVariables = ((CVector)cookedEffectsEntry).variables;

                    if (cookedEffectsVariables.Count < 2)
                    {
                        throw new System.InvalidOperationException("File '" + filePath + "' contains a '" + VariableNameCookedEffects + "' entry with less than 2 variables.");
                    }

                    if (IsFire(cookedEffectsVariables[0]) && IsSharedDataBuffer(cookedEffectsVariables[1]))
                    {
                        CR2WFile sharedDataBufferContent = ReadCByteArrayContainerContent((CByteArray)cookedEffectsVariables[1], localizedStringSource);

                        if (sharedDataBufferContent == null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains a shared data buffer which could not be read.");
                        }

                        sharedDataBuffersForFires.Add(new CByteArrayContainer(sharedDataBufferContent, (CByteArray)cookedEffectsVariables[1]));
                    }
                }
            }

            return sharedDataBuffersForFires;
        }

        private static void PatchFireShowDistance(string filePath, CByteArrayContainer sharedDataBufferForFire, W2EntSettings w2EntSettings)
        {
            bool cFXDefinitionFound = false;

            foreach (CR2WChunk chunk in sharedDataBufferForFire.Content.chunks)
            {
                if (!IsCFXDefinition(chunk))
                {
                    continue;
                }
                else if (cFXDefinitionFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + TypeCFXDefinition + "'.");
                }

                cFXDefinitionFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + TypeCFXDefinition + "'.");
                }

                CVector chunkData = (CVector)chunk.data;
                bool showDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsShowDistance(variable))
                    {
                        if (showDistanceFound)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one attribute '" + VariableNameShowDistance + "'.");
                        }

                        PatchShowDistance(variable, w2EntSettings);

                        showDistanceFound = true;
                    }
                }

                if (!showDistanceFound)
                {
                    AddShowDistance(sharedDataBufferForFire.Content, chunkData, w2EntSettings);
                }
            }

            if (!cFXDefinitionFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + TypeCFXDefinition + "'.");
            }
        }

        private static void PatchShowDistance(CVariable variableShowDistance, W2EntSettings w2EntSettings)
        {
            ((CFloat)variableShowDistance).SetValue(w2EntSettings.FireShowDistance);
        }

        private static void AddShowDistance(CR2WFile file, CVector chunkData, W2EntSettings w2EntSettings)
        {
            CFloat showDistanceVariable = new CFloat(file);

            showDistanceVariable.Type = CVariableTypeFloat;
            showDistanceVariable.Name = VariableNameShowDistance;
            showDistanceVariable.SetValue(w2EntSettings.FireShowDistance);

            chunkData.variables.Add(showDistanceVariable);
        }

        private static void PatchW2PFilePath(string w2EntFilePath, CByteArrayContainer sharedDataBuffer, CByteArrayContainer flatCompiledData, Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap)
        {
            bool cFXTrackItemParticlesFound = false;

            if (sharedDataBuffer != null)
            {
                foreach (CR2WChunk chunk in sharedDataBuffer.Content.chunks)
                {
                    if (!IsCFXTrackItemParticles(chunk))
                    {
                        continue;
                    }

                    cFXTrackItemParticlesFound = true;

                    if (chunk.data == null || !(chunk.data is CVector))
                    {
                        throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCFXTrackItemParticles + "'.");
                    }

                    CVector chunkData = (CVector)chunk.data;

                    foreach (CVariable variable in chunkData.variables)
                    {
                        if (IsCSoftParticleSystem(variable))
                        {
                            PatchW2PFilePath(relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap, variable);
                        }
                    }
                }
            }

            // Some w2ent files have different structures and have the w2p references in CParticleComponents in flatCompiledData
            if (!cFXTrackItemParticlesFound)
            {
                foreach (CR2WChunk chunk in flatCompiledData.Content.chunks)
                {
                    if (!IsCParticleComponent(chunk))
                    {
                        continue;
                    }

                    if (chunk.data == null || !(chunk.data is CVector))
                    {
                        throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCParticleComponent + "'.");
                    }

                    CVector chunkData = (CVector)chunk.data;

                    foreach (CVariable variable in chunkData.variables)
                    {
                        if (IsCHandleParticleSystem(variable))
                        {
                            PatchW2PFilePath(relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap, variable);
                        }
                    }
                }
            }

            // TODO maybe enable a check again for some final testing but it should be removed in the release version
            //if (!cFXTrackItemParticlesFound && !cParticleComponentFound)
            //{
            //    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains no chunk of type '" + TypeCFXTrackItemParticles + "' nor '" + TypeCParticleComponent + "'.");
            //}
        }

        private static void PatchW2PFilePath(Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap, CVariable variableParticleSystem)
        {
            string relativeW2PFilePath = variableParticleSystem is CSoft ? ((CSoft)variableParticleSystem).Handle : ((CHandle)variableParticleSystem).Handle;
            string relativeRenamedW2PFilePath;

            if (relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap.TryGetValue(relativeW2PFilePath, out relativeRenamedW2PFilePath))
            {
                if (variableParticleSystem is CSoft)
                {
                    ((CSoft)variableParticleSystem).Handle = relativeRenamedW2PFilePath;
                }
                else
                {
                    ((CHandle)variableParticleSystem).Handle = relativeRenamedW2PFilePath;
                }
            }
        }

        private bool PatchGlowAutoHideDistance(string w2EntFilePath, CByteArrayContainer flatCompiledData, W2EntSettings w2EntSettings)
        {
            List<CR2WChunk> pointAndSpotLightComponents = new List<CR2WChunk>();
            float maximumPointLightComponentBrightness = 0;
            float maximumPointLightComponentRadius = 0;
            float maximumSpotLightComponentRadius = 0;

            foreach (CR2WChunk chunk in flatCompiledData.Content.chunks)
            {
                if (!IsCPointLightComponent(chunk) && !IsCSpotLightComponent(chunk))
                {
                    continue;
                }

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCPointLightComponent + "'.");
                }

                pointAndSpotLightComponents.Add(chunk);

                foreach (CVariable variable in ((CVector)chunk.data).variables)
                {
                    if (IsBrightness(variable) && IsCPointLightComponent(chunk))
                    {
                        maximumPointLightComponentBrightness = Math.Max(maximumPointLightComponentBrightness, ((CFloat)variable).val);
                    }
                    else if (IsRadius(variable))
                    {
                        if (IsCPointLightComponent(chunk))
                        {
                            maximumPointLightComponentRadius = Math.Max(maximumPointLightComponentRadius, ((CFloat)variable).val);
                        }
                        else
                        {
                            maximumSpotLightComponentRadius = Math.Max(maximumSpotLightComponentRadius, ((CFloat)variable).val);
                        }
                    }
                }
            }

            foreach (CR2WChunk chunk in pointAndSpotLightComponents)
            {
                CVector chunkData = (CVector)chunk.data;
                bool autoHideDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsAutoHideDistance(variable))
                    {
                        if (autoHideDistanceFound)
                        {
                            throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains more than one attribute '" + VariableNameAutoHideDistance + "'.");
                        }

                        PatchAutoHideDistance(variable, w2EntSettings.GlowAutoHideDistance);

                        autoHideDistanceFound = true;
                    }
                    else if (IsTransform(variable))
                    {
                        // For some reason changing some coordinates slightly makes the glow not clip anymore at a certain distance for some of the light sources...
                        PatchMinimumTransformY((CEngineTransform)variable, w2EntSettings);
                    }
                    else if (IsCSpotLightComponent(chunk) && IsBrightness(variable) && maximumPointLightComponentBrightness > 0)
                    {
                        PatchBrightness(variable, maximumPointLightComponentBrightness);
                    }
                    else if (IsRadius(variable) && maximumPointLightComponentRadius > 0 && maximumSpotLightComponentRadius > 0 && w2EntSettings.UnifyGlowRadius)
                    {
                        PatchRadius(variable, maximumPointLightComponentRadius, maximumSpotLightComponentRadius);
                    }
                }

                if (!autoHideDistanceFound)
                {
                    AddAutoHideDistance(flatCompiledData.Content, chunkData, w2EntSettings.GlowAutoHideDistance);
                }
            }

            return pointAndSpotLightComponents.Any();
        }

        private static void PatchBrightness(CVariable variableBrightness, float brightness)
        {
            ((CFloat)variableBrightness).SetValue(brightness);
        }

        private static void PatchMinimumTransformY(CEngineTransform variableTransform, W2EntSettings w2EntSettings)
        {
            if (w2EntSettings.MinimumGlowTransformY != null)
            {
                variableTransform.y.SetValue(Math.Max(w2EntSettings.MinimumGlowTransformY.Value, variableTransform.y.val));
            }
        }

        private static void PatchRadius(CVariable variableRadius, float radius1, float radius2)
        {
            ((CFloat)variableRadius).SetValue(Math.Min(radius1, radius2) + (Math.Abs(radius1 - radius2) / 2));
        }

        private static void PatchMeshStreamingDistance(string filePath, CR2WFile w2EntFile, W2EntSettings w2EntSettings)
        {
            bool streamingDistanceFound = false;

            foreach (CR2WChunk chunk in w2EntFile.chunks)
            {
                if (!IsTypeWithStreamingDataBuffer(chunk))
                {
                    continue;
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsStreamingDistance(variable))
                    {
                        if (streamingDistanceFound)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one attribute '" + VariableNameStreamingDistance + "'.");
                        }

                        PatchMinimumStreamingDistance((CUInt8)variable, w2EntSettings);

                        streamingDistanceFound = true;
                    }
                }
            }

            if (!streamingDistanceFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' does not contain attribute '" + VariableNameStreamingDistance + "'.");
            }
        }

        private static void PatchMinimumStreamingDistance(CUInt8 variableStreamingDistance, W2EntSettings w2EntSettings)
        {
            if (w2EntSettings.MeshStreamingDistance != null)
            {
                variableStreamingDistance.SetValue(w2EntSettings.MeshStreamingDistance.Value);
            }
        }

        private static List<string> PatchW2MeshFilePath(string w2EntFilePath, List<CR2WChunk> chunks, Dictionary<string, string> relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap)
        {
            List<string> relativeCollisionMeshFilePaths = new List<string>();

            List<CR2WChunk> meshComponentsToCopyAndRename = new List<CR2WChunk>();
            HashSet<CR2WChunk> meshComponentsWithAttachments = new HashSet<CR2WChunk>();

            string renamedFileNameSuffix = W2XFileHandler.FileNameSuffixILOD + W2XFileHandler.FileExtensionW2Mesh;

            foreach (CR2WChunk chunk in chunks)
            {
                if (!IsMeshComponent(chunk))
                {
                    continue;
                }

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for a mesh type.");
                }

                CVector chunkData = (CVector)chunk.data;
                bool meshFilePathFound = false;
                bool ignoreMesh = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsCStringName(variable) && ((CString)variable).val.EndsWith(SuffixILODCollision))
                    {
                        ignoreMesh = true;
                        break;
                    }
                    else if (IsCHandleMesh(variable))
                    {
                        string relativeW2MeshFilePath = ((CHandle)variable).Handle;

                        if (relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap.ContainsKey(relativeW2MeshFilePath) && !relativeW2MeshFilePath.EndsWith(renamedFileNameSuffix))
                        {
                            meshFilePathFound = true;
                        }
                    }
                    else if (variable is CPtr && ((CPtr)variable).PtrTargetType.Contains("Attachment"))
                    {
                        meshComponentsWithAttachments.Add(chunk);
                    }
                }

                if (!ignoreMesh && meshFilePathFound)
                {
                    meshComponentsToCopyAndRename.Add(chunk);
                }
            }

            foreach (CR2WChunk meshComponentToCopyAndRename in meshComponentsToCopyAndRename)
            {
                foreach (CVariable variable in ((CVector)meshComponentToCopyAndRename.data).variables)
                {
                    if (IsCHandleMesh(variable))
                    {
                        CHandle variableCHandleMesh = (CHandle)variable;

                        string relativeW2MeshFilePath = variableCHandleMesh.Handle;
                        string w2MeshFileName = relativeW2MeshFilePath.Substring(relativeW2MeshFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        // The chunks for these meshes get copied to have a dummy mesh for collision since renamed meshes don't support collisions anymore
                        if (!meshComponentsWithAttachments.Contains(meshComponentToCopyAndRename)
                            && (w2MeshFileName.Contains("braziers_floor") || w2MeshFileName.Contains("braziers_wall") || w2MeshFileName.Contains("pile_of_bodies")
                            || (w2MeshFileName.Contains("shrine_of_ethernal_fire_altar") && !w2MeshFileName.Contains("small"))
                            || w2MeshFileName.Contains("shipyard_pole_support") || w2MeshFileName.Contains("bonfire_large")
                            || w2MeshFileName.Contains("torch_wall") || w2MeshFileName.Contains("lantern_red_table.w2mesh")))
                        {
                            CR2WChunk copiedMeshComponent = CR2WCopyAction.CopyChunk(meshComponentToCopyAndRename, meshComponentToCopyAndRename.CR2WOwner);

                            patchNameWithILODCollisionSuffix(copiedMeshComponent);

                            // For the meshes specified here, the attributes of the original mesh will not be touched
                            if (!w2MeshFileName.Contains("bonfire_large") && !w2MeshFileName.Contains("pile_of_bodies"))
                            {
                                relativeCollisionMeshFilePaths.Add(relativeW2MeshFilePath);
                            }
                        }

                        string relativeRenamedW2MeshFilePath;

                        if (relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap.TryGetValue(relativeW2MeshFilePath, out relativeRenamedW2MeshFilePath))
                        {
                            PatchW2MeshFilePath(variableCHandleMesh, relativeRenamedW2MeshFilePath);
                        }
                    }
                }
            }

            return relativeCollisionMeshFilePaths;
        }

        private static void patchNameWithILODCollisionSuffix(CR2WChunk meshComponent)
        {
            foreach (CVariable variable in ((CVector)meshComponent.data).variables)
            {
                if (IsCStringName(variable))
                {
                    ((CString)variable).SetValue(((CString)variable).val + SuffixILODCollision);

                    return;
                }
            }
        }

        private static void PatchW2MeshFilePath(CHandle variableCHandleMesh, string relativeRenamedW2MeshFilePath)
        {
            variableCHandleMesh.Handle = relativeRenamedW2MeshFilePath;
        }

        internal static CByteArrayContainer ReadStreamingDataBufferForFires(CR2WFile w2EntFile)
        {
            CByteArrayContainer streamingDataBufferForFires = null;

            string filePath = w2EntFile.FileName;
            bool typeWithStreamingDataBufferFound = false;

            foreach (CR2WChunk chunk in w2EntFile.chunks)
            {
                if (!IsTypeWithStreamingDataBuffer(chunk))
                {
                    continue;
                }
                else if (typeWithStreamingDataBufferFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk with a streaming data buffer.");
                }

                typeWithStreamingDataBufferFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data with a streaming data buffer.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsStreamingDataBuffer(variable))
                    {
                        if (streamingDataBufferForFires != null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one cooked effects variable with streaming data buffers.");
                        }

                        CR2WFile streamingDataBufferContent = ReadCByteArrayContainerContent((CByteArray)variable, w2EntFile.LocalizedStringSource);

                        if (streamingDataBufferContent == null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains a streaming data buffer which could not be read.");
                        }

                        streamingDataBufferForFires = new CByteArrayContainer(streamingDataBufferContent, (CByteArray)variable);
                    }
                }
            }

            return streamingDataBufferForFires;
        }

        internal static List<string> GetW2PFilePathsForFires(CByteArrayContainer sharedDataBuffer, CByteArrayContainer flatCompiledData, string w2EntFilePath, string modDirectory, string dlcDirectory)
        {
            List<string> w2PFilePathsForFires = new List<string>();

            bool cFXTrackItemParticlesFound = false;

            if (sharedDataBuffer != null)
            {
                foreach (CR2WChunk chunk in sharedDataBuffer.Content.chunks)
                {
                    if (!IsCFXTrackItemParticles(chunk))
                    {
                        continue;
                    }

                    if (chunk.data == null || !(chunk.data is CVector))
                    {
                        throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCFXTrackItemParticles + "'.");
                    }

                    cFXTrackItemParticlesFound = true;

                    CVector chunkData = (CVector)chunk.data;

                    foreach (CVariable variable in chunkData.variables)
                    {
                        if (IsCSoftParticleSystem(variable))
                        {
                            AddW2PFilePath(w2PFilePathsForFires, ((CSoft)variable).Handle, modDirectory, dlcDirectory);
                        }
                    }
                }
            }

            // Some w2ent files have different structures and have the w2p references in CParticleComponents in flatCompiledData
            if (!cFXTrackItemParticlesFound)
            {
                foreach (CR2WChunk chunk in flatCompiledData.Content.chunks)
                {
                    if (!IsCParticleComponent(chunk))
                    {
                        continue;
                    }

                    if (chunk.data == null || !(chunk.data is CVector))
                    {
                        throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCParticleComponent + "'.");
                    }

                    CVector chunkData = (CVector)chunk.data;

                    foreach (CVariable variable in chunkData.variables)
                    {
                        if (IsCHandleParticleSystem(variable))
                        {
                            AddW2PFilePath(w2PFilePathsForFires, ((CHandle)variable).Handle, modDirectory, dlcDirectory);
                        }
                    }
                }
            }

            return w2PFilePathsForFires;
        }

        private static void AddW2PFilePath(List<string> w2PFilePathsForFires, String relativeW2PFilePath, string modDirectory, string dlcDirectory)
        {
            string initialPath = relativeW2PFilePath.StartsWith(W2XFileHandler.PathDLC) ? dlcDirectory : modDirectory;

            string absoluteW2PFilePath = initialPath + Path.DirectorySeparatorChar + W2XFileHandler.PathBundle + Path.DirectorySeparatorChar + relativeW2PFilePath;
            string w2pFileName = relativeW2PFilePath.Substring(relativeW2PFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            if ((w2pFileName.Contains("fire") || w2pFileName.Contains("flame") || (w2pFileName.Contains("candle") && !w2pFileName.Contains("wraith") && !w2pFileName.Contains("smoke") && !w2pFileName.Contains("spark"))
                || w2pFileName.Contains("_brazier") || w2pFileName.Contains("torch") || w2pFileName.Contains("chandelier") || (w2pFileName.Contains("coal") && !w2pFileName.Contains("smoke"))) && !w2pFileName.Contains("geralt")
                && !relativeW2PFilePath.Contains("arson") && !relativeW2PFilePath.Contains("arachas") && !relativeW2PFilePath.Contains("weapons") && !relativeW2PFilePath.Contains("beehive")
                && !relativeW2PFilePath.Contains("monsters") && !relativeW2PFilePath.Contains("characters") && !relativeW2PFilePath.Contains("environment") && !relativeW2PFilePath.Contains("work")
                && !relativeW2PFilePath.Contains("igni") && !relativeW2PFilePath.Contains("oil_barrel") && !relativeW2PFilePath.Contains("monster_nest"))
            {
                w2PFilePathsForFires.Add(absoluteW2PFilePath);
            }
        }

        internal static List<string> GetW2MeshFilePathsForFires(List<CR2WChunk> chunks, string w2EntFilePath, string modDirectory, string dlcDirectory)
        {
            List<string> w2MeshFilePathsForFires = new List<string>();

            foreach (CR2WChunk chunk in chunks)
            {
                if (!IsMeshComponent(chunk))
                {
                    continue;
                }

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for a mesh type.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsCHandleMesh(variable))
                    {
                        AddW2MeshFilePath(w2MeshFilePathsForFires, ((CHandle)variable).Handle, modDirectory, dlcDirectory);
                    }
                }
            }

            return w2MeshFilePathsForFires;
        }

        private static void AddW2MeshFilePath(List<string> w2MeshFilePathsForFires, string relativeW2MeshFilePath, string modDirectory, string dlcDirectory)
        {
            string initialPath = relativeW2MeshFilePath.StartsWith(W2XFileHandler.PathDLC) ? dlcDirectory : modDirectory;

            string absoluteW2MeshFilePath = initialPath + Path.DirectorySeparatorChar + W2XFileHandler.PathBundle + Path.DirectorySeparatorChar + relativeW2MeshFilePath;
            string w2MeshFileName = relativeW2MeshFilePath.Substring(relativeW2MeshFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            if ((w2MeshFileName.Contains("torch") || w2MeshFileName.Contains("fire") || w2MeshFileName.Contains("pillar") || w2MeshFileName.Contains("chain")
                || w2MeshFileName.Contains("pole") || w2MeshFileName.Contains("brazier") || w2MeshFileName.Contains("corp") || w2MeshFileName.Contains("burn") || w2MeshFileName.Contains("candelabra")
                || w2MeshFileName.Contains("pile") || w2MeshFileName.Contains("cauldron") || w2MeshFileName.Contains("roast") || w2MeshFileName.Contains("candle") || w2MeshFileName.Contains("forge")
                || w2MeshFileName.Contains("chandelier") || w2MeshFileName.Contains("coal") || w2MeshFileName.Contains("lamp") || w2MeshFileName.Contains("pot")
                || w2MeshFileName.Contains("stove") || w2MeshFileName.Contains("shrine") || w2MeshFileName.Contains("pyre") || w2MeshFileName.Contains("lantern"))
                && !w2MeshFileName.Contains("proxy") && !w2MeshFileName.Contains("castle") && !w2MeshFileName.Contains("sword") && !w2MeshFileName.Contains("stew") && !w2MeshFileName.Contains("plane")
                && !w2MeshFileName.Contains("tree") && !w2MeshFileName.Contains("fence") && !relativeW2MeshFilePath.Contains("near_water"))
            {
                w2MeshFilePathsForFires.Add(absoluteW2MeshFilePath);
            }
        }

        private static bool IsBrightness(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(VariableNameBrightness);
        }

        private static bool IsCEntityTemplate(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCEntityTemplate);
        }

        private static bool IsCFXDefinition(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCFXDefinition);
        }

        private static bool IsCFXTrackItemParticles(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCFXTrackItemParticles);
        }

        private static bool IsCHandleMesh(CVariable variable)
        {
            return variable is CHandle && ((CHandle)variable).FileType.Equals(TypeCMesh);
        }

        private static bool IsCHandleParticleSystem(CVariable variable)
        {
            return variable is CHandle && ((CHandle)variable).FileType.Equals(TypeCParticleSystem) && variable.Name.Equals(VariableNameParticleSystem);
        }

        private static bool IsCookedEffects(CVariable variable)
        {
            return variable is CArray && variable.Name.Equals(VariableNameCookedEffects);
        }

        private static bool IsCParticleComponent(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCParticleComponent);
        }

        private static bool IsCPointLightComponent(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCPointLightComponent);
        }

        private static bool IsCSoftParticleSystem(CVariable variable)
        {
            return variable is CSoft && ((CSoft)variable).FileType.Equals(TypeCParticleSystem);
        }

        private static bool IsCSpotLightComponent(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCSpotLightComponent);
        }

        private static bool IsCStringName(CVariable variable)
        {
            return variable is CString && variable.Name.Equals(VariableNameName);
        }

        private static bool IsFire(CVariable variable)
        {
            if (!(variable is CName))
            {
                return false;
            }

            string variableValue = ((CName)variable).Value;

            return variableValue.Contains("fire") || variableValue.Contains("torch") || variableValue.Contains("destroy") || variableValue.Contains("light_on")
                || variableValue.Equals("effects") || variableValue.Equals("active") || variableValue.Equals("candle") || variableValue.Equals("smoke") || variableValue.Equals("burn") || variableValue.Equals("active_effect");
        }

        private static bool IsFlatCompiledData(CVariable variable)
        {
            return variable is CByteArray && variable.Name.Equals(VariableNameFlatCompiledData);
        }
        private static bool IsMeshComponent(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCMeshComponent) || chunk.Type.Equals(TypeCRigidMeshComponent) || chunk.Type.Equals(TypeCStaticMeshComponent);
        }

        private static bool IsRadius(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(VariableNameRadius);
        }

        private static bool IsSharedDataBuffer(CVariable variable)
        {
            return variable is CByteArray && variable.Name.Equals(VariableNameBuffer) && variable.Type.Equals(TypeSharedDataBuffer);
        }

        private static bool IsStreamingDataBuffer(CVariable variable)
        {
            return variable is CByteArray && variable.Name.Equals(VariableNameStreamingDataBuffer) && variable.Type.Equals(TypeSharedDataBuffer);
        }

        private static bool IsStreamingDistance(CVariable variable)
        {
            return variable is CUInt8 && variable.Name.Equals(VariableNameStreamingDistance);
        }

        private static bool IsShowDistance(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(VariableNameShowDistance);
        }
        private static bool IsTransform(CVariable variable)
        {
            return variable is CEngineTransform && variable.Name.Equals(VariableNameTransform) && variable.Type.Equals(TypeEngineTransform);
        }

        private static bool IsTypeWithStreamingDataBuffer(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCActionPoint) || chunk.Type.Equals(TypeCEntity) || chunk.Type.Equals(TypeCGameplayEntity) || chunk.Type.Equals(TypeW3AnimationInteractionEntity) || chunk.Type.Equals(TypeW3Campfire)
                || chunk.Type.Equals(TypeW3FireSource) || chunk.Type.Equals(TypeW3FireSourceLifeRegen) || chunk.Type.Equals(TypeW3LightEntityDamaging) || chunk.Type.Equals(TypeW3LightSource) || chunk.Type.Equals(TypeW3MonsterClue);
        }
    }
}
