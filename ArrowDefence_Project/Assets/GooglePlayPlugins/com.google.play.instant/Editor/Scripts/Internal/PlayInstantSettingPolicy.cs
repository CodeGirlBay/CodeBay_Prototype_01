// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;
using UnityEngine.Rendering;

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// Enumerates Unity setting policies that affect whether an instant app can successfully be built. Some settings
    /// are required and must be configured in a certain way, where others are simply recommended. Each policy can
    /// report whether it is configured correctly and if not, provides a delegate to change to the preferred state.
    /// </summary>
    public class PlayInstantSettingPolicy
    {
        private const string ApkSizeReductionDescription = "This setting reduces APK size.";

        private const string GraphicsApiDescription =
            "Removing additional Graphics APIs reduces APK size.";

        public delegate bool IsCorrectStateDelegate();

        public delegate bool ChangeStateDelegate();

        private PlayInstantSettingPolicy(
            string name,
            string description,
            IsCorrectStateDelegate isCorrectState,
            ChangeStateDelegate changeState)
        {
            Name = name;
            Description = description;
            IsCorrectState = isCorrectState;
            ChangeState = changeState;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IsCorrectStateDelegate IsCorrectState { get; private set; }

        public ChangeStateDelegate ChangeState { get; private set; }

        /// <summary>
        /// Returns the policies that are required to build an instant app.
        /// </summary>
        public static IEnumerable<PlayInstantSettingPolicy> GetRequiredPolicies()
        {
            return new List<PlayInstantSettingPolicy>
            {
                new PlayInstantSettingPolicy(
                    "Build Target should be Android",
                    null,
                    () => EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android,
                    () => EditorUserBuildSettings.SwitchActiveBuildTarget(
                        BuildTargetGroup.Android, BuildTarget.Android)),

                // We require Gradle for all versions, but hide this policy for 2019.1+ since Gradle is the only option.
#if !UNITY_2019_1_OR_NEWER
                new PlayInstantSettingPolicy(
                    "Android build system should be Gradle",
                    "Required for APK Signature Scheme v2.",
                    () => EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Gradle,
                    () =>
                    {
                        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                        return true;
                    }),
#endif

                new PlayInstantSettingPolicy(
                    "Split Application Binary should be disabled",
                    "Instant apps don't support APK Expansion (OBB) Files.",
                    () => !PlayerSettings.Android.useAPKExpansionFiles,
                    () =>
                    {
                        PlayerSettings.Android.useAPKExpansionFiles = false;
                        return true;
                    })
            };
        }

        /// <summary>
        /// Returns the policies that are recommended when building an instant app, e.g. to reduce APK size.
        /// </summary>
        public static IEnumerable<PlayInstantSettingPolicy> GetRecommendedPolicies()
        {
            var policies = new List<PlayInstantSettingPolicy>();

            policies.Add(new PlayInstantSettingPolicy(
                "Android minSdkVersion should be 26",
                "Lower than 26 is fine, though 26 is the minimum supported by Google Play Instant.",
                () => (int)PlayerSettings.Android.minSdkVersion >= 26,
                () =>
                {
                    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
                    return true;
                })
            );

            policies.Add(new PlayInstantSettingPolicy(
                "Use a single Graphics API, either GLES2, GLES3, or Vulkan",
                GraphicsApiDescription,
                () => !PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android) &&
                      PlayerSettings.GetGraphicsAPIs(BuildTarget.Android).Length == 1,
                () =>
                {
                    // On headless build machines we don't want to show a dialog, so just ignore.
                    if (WindowUtils.IsHeadlessMode())
                    {
                        return true;
                    }

                    // Otherwise, ask which single Graphics API to use (generally recommending GLES3).
                    var preferredGraphicsApi = GraphicsDeviceType.OpenGLES3;
                    var preferredGraphicsApiName = "GLES3";
                    if (!PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android))
                    {
                        var types = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                        if (!types.Contains(GraphicsDeviceType.OpenGLES3))
                        {
                            // If GLES3 isn't in the existing list, recommend GLES2. (Alternatively, Vulkan could be
                            // recommended here since it's available on Android SDK 24+.)
                            preferredGraphicsApi = GraphicsDeviceType.OpenGLES2;
                            preferredGraphicsApiName = "GLES2";
                        }
                    }

                    var result = EditorUtility.DisplayDialog("Remove Additional Graphics APIs",
                        GraphicsApiDescription +
                        " Set Graphics APIs to " + preferredGraphicsApiName + " Only or Cancel to ignore.",
                        preferredGraphicsApiName + " Only", "Cancel");
                    if (result)
                    {
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { preferredGraphicsApi });
                    }

                    return true;
                }));

            // For the purpose of switching to .NET 2.0 Subset, it's sufficient to check #if NET_2_0. However, it would
            // be confusing if the option disappeared after clicking the Update button, so we also check NET_2_0_SUBSET.
            // For background on size reduction benefits see https://docs.unity3d.com/Manual/dotnetProfileSupport.html
#if NET_2_0 || NET_2_0_SUBSET
            policies.Add(new PlayInstantSettingPolicy(
                "API Compatibility Level should be \".NET 2.0 Subset\"",
                ApkSizeReductionDescription,
                () => PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android) ==
                      ApiCompatibilityLevel.NET_2_0_Subset,
                () =>
                {
                    PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android,
                        ApiCompatibilityLevel.NET_2_0_Subset);
                    return true;
                }));
#endif

            // Unity 2017.1 added experimental support for .NET 4.6, however .NET Standard 2.0 is unavailable.
            // Therefore, we only provide this policy option starting with Unity 2018.1.
            // Our #if check includes NET_STANDARD_2_0 for the reason we check NET_2_0_SUBSET above.
            // https://blogs.unity3d.com/2018/03/28/updated-scripting-runtime-in-unity-2018-1-what-does-the-future-hold/
#if UNITY_2018_1_OR_NEWER && (NET_4_6 || NET_STANDARD_2_0)
            policies.Add(new PlayInstantSettingPolicy(
                "API Compatibility Level should be \".NET Standard 2.0\"",
                ApkSizeReductionDescription,
                () => PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android) ==
                      ApiCompatibilityLevel.NET_Standard_2_0,
                () =>
                {
                    PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android,
                        ApiCompatibilityLevel.NET_Standard_2_0);
                    return true;
                }));
#endif

            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == ScriptingImplementation.IL2CPP)
            {
                // This will reduce APK size, but may cause runtime issues if needed components are removed.
                // See https://docs.unity3d.com/Manual/IL2CPP-BytecodeStripping.html
                policies.Add(new PlayInstantSettingPolicy(
                    "IL2CPP builds should strip engine code",
                    "This setting reduces APK size, but may cause runtime issues.",
                    () => PlayerSettings.stripEngineCode,
                    () =>
                    {
                        PlayerSettings.stripEngineCode = true;
                        return true;
                    }));
            }

#if UNITY_2018_3_OR_NEWER
            policies.Add(new PlayInstantSettingPolicy(
                "Release builds should use managed code stripping",
                ApkSizeReductionDescription,
                () => PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android) == ManagedStrippingLevel.High,
                () =>
                {
                    PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.High);
                    return true;
                }));
#endif

            return policies;
        }
    }
}