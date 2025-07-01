using System;
using Il2CppCapuchinPlayFab;
using UnityEngine;
using MelonLoader;
using JetBrains.Annotations;
using HarmonyLib;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MelonLoader.Utils;

[HarmonyPatch(typeof(Il2CppLocomotion.Player), "Awake")]
public class PlayerAwakePatch
{
    public static void LoadPluginViews(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(IMenuView).IsAssignableFrom(type) && !type.IsAbstract)
            {
                MenuHandler.instance.RegisterView((IMenuView)Activator.CreateInstance(type));
                MelonLogger.Msg($"Registered view {type.Name} from {assembly.GetName().Name}");
            }
        }
    }

    public static void LoadMelonLoaderPlugins()
    {
        foreach (var plugin in MelonMod.RegisteredMelons)
        {
            Assembly pluginAssembly = plugin.MelonAssembly.Assembly;
            if (pluginAssembly == Assembly.GetExecutingAssembly()) continue;
            LoadPluginViews(pluginAssembly);
        }
    }

    public static void LoadExternalPlugins()
    {
        // Heh... Universal Plugin loading
        string path = $"{MelonEnvironment.MelonLoaderDirectory}/CapuPuterPlugins";
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string[] dllFiles = Directory.GetFiles(path, "*.dll");

        foreach (string file in dllFiles)
        {
            Assembly assembly = Assembly.LoadFile(file);
            LoadPluginViews(assembly);
            MelonLogger.Msg($"Loaded Plugin: {Path.GetFileName(file)}");
        }
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
        if (MenuHandler.instance == null)
        {
            new MenuHandler();
            MenuHandler.instance.Initialize();
            
            LoadPluginViews(Assembly.GetExecutingAssembly());
            LoadMelonLoaderPlugins();
            LoadExternalPlugins();
        }
    }
}

[HarmonyPatch(typeof(Il2CppLocomotion.Player), "LateUpdate")]
public class PlayerLateUpdatePatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (MenuHandler.instance != null)
        {
            MenuHandler.instance.Run(Il2CppLocomotion.Player.Instance.leftIkTarget);
        }
    }
}