//
// Copyright (c) 2021, Aaron Shumate
// All rights reserved.
//
// This source code is licensed under the BSD-style license found in the
// LICENSE.txt file in the root directory of this source tree. 
//
// Dyson Sphere Program is developed by Youthcat Studio and published by Gamera Game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
namespace DSPSaveGameSorter
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("DSPGAME.exe")]
    public class DSPSaveGameSorter : BaseUnityPlugin
    {
        public const string pluginGuid = "greyhak.dysonsphereprogram.savegamesorter";
        public const string pluginName = "DSP Save Game Sorter";
        public const string pluginVersion = "1.0.1";
        new internal static ManualLogSource Logger;
        Harmony harmony;

        public static bool configSortLoadScreen = true;
        public static bool configSortSaveScreen = true;

        public void Awake()
        {
            Logger = base.Logger;  // "C:\Program Files (x86)\Steam\steamapps\common\Dyson Sphere Program\BepInEx\LogOutput.log"
            configSortLoadScreen = Config.Bind<bool>("Config", "SortLoadScreen", configSortLoadScreen, "Sort load-game screen list.").Value;
            configSortSaveScreen = Config.Bind<bool>("Config", "SortSaveScreen", configSortSaveScreen, "Sort save-game screen list.").Value;

            harmony = new Harmony(pluginGuid);
            harmony.PatchAll(typeof(DSPSaveGameSorter));
        }

        public class GameSaveEntryReverseSorter : IComparer<UIGameSaveEntry>
        {
            public int Compare(UIGameSaveEntry x, UIGameSaveEntry y)
            {
                if (x.fileDate < y.fileDate)
                    return 1;
                if (x.fileDate > y.fileDate)
                    return -1;
                return 0;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UILoadGameWindow), "RefreshList")]
        public static void UILoadGameWindow_RefreshList_Postfix(ref UILoadGameWindow __instance)
        {
            if (configSortLoadScreen)
            {
                //List<UIGameSaveEntry> sorted = __instance.entries.OrderByDescending(e => e.fileDate).ToList();

                __instance.entries.Sort(new GameSaveEntryReverseSorter());

                for (int i = 0; i < __instance.entries.Count;)
                {
                    UIGameSaveEntry entry = __instance.entries[i++];
                    entry.SetEntry(i, entry.fileInfo);
                }

                __instance.OnSelectedChange();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UISaveGameWindow), "RefreshList")]
        public static void UISaveGameWindow_RefreshList_Postfix(ref UISaveGameWindow __instance)
        {
            if (configSortSaveScreen)
            {
                __instance.entries.Sort(new GameSaveEntryReverseSorter());

                for (int i = 0; i < __instance.entries.Count;)
                {
                    UIGameSaveEntry entry = __instance.entries[i++];
                    entry.SetEntry(i, entry.fileInfo);
                }

                __instance.OnSelectedChange();
            }
        }
    }
}
