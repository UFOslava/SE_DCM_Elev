﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public List<IMyTextSurface> outputLcdList;
        public string outputLcdListPattern = "[LiftLCD]";
        public List<IMyLandingGear> bottomLandingGearsList;
        public List<IMyLandingGear> topLandingGearsList;
        public IMyShipConnector bottomConnector = null;
        public string bottomConnectorPattern = "[TopConn]";
        public IMyShipConnector topConnector = null;
        public IMyProgrammableBlock cruiseControlProgrammableBlock = null;
        public static List<IMyTerminalBlock> TerminalBlockList = new List<IMyTerminalBlock>();//declare an empty list of TerminalBlocks for later use in searches.
        public static List<IMyTerminalBlock> TerminalBlockListCurrentGrid = new List<IMyTerminalBlock>();// T:

        public List<IMyTerminalBlock> GetBlocksByPattern(string Pattern) {
            List<IMyTerminalBlock> ReturnList = new List<IMyTerminalBlock>();
            
            if (Pattern.StartsWith("T:")) {//Return current grid Blocks only, by name.
                Pattern = Pattern.Substring(2);//Update pattern with T: removed.
                foreach (IMyTerminalBlock Block in TerminalBlockListCurrentGrid) {
                    if (Block.CustomName.Contains(Pattern)) ReturnList.Add(Block);
                }
                return ReturnList;
            }

            if (Pattern.StartsWith("G:")) {//Return all group Blocks
                GridTerminalSystem.GetBlockGroupWithName(Pattern.Substring(2)).GetBlocks(ReturnList);
                return ReturnList;
            }
            foreach (IMyTerminalBlock Block in TerminalBlockListCurrentGrid)
            {
                if (Block.CustomName.Contains(Pattern)) ReturnList.Add(Block);
            }
            return ReturnList;
        }

        //parse Custom Data
        public static void ParseCustomData(IMyTerminalBlock block, Dictionary<string, string> cfg, bool clr = true) {
            if (clr) {
                cfg.Clear();
            }

            string[] CustomDataLines = block.CustomData.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < CustomDataLines.Length; i++) {
                string line = CustomDataLines[i];
                string value;

                int pos = line.IndexOf('=');
                if (pos > -1) {
                    value = line.Substring(pos + 1);
                    line = line.Substring(0, pos);
                } else {
                    value = "";
                }

                cfg[line.Trim()] = line.Trim();
            }
        }

        bool filterThis(IMyTerminalBlock block)
        {
            return block.CubeGrid == Me.CubeGrid;
        }

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            //update block records

            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(TerminalBlockList);//Acquire all "Smart" blocks
            foreach (IMyTerminalBlock Block in TerminalBlockList) {
                if (filterThis(Block)) TerminalBlockListCurrentGrid.Add(Block);//Get Blocks of current Grid.
            }


            for (int i = 0; i < TerminalBlockList.Count; i++) {
                if (TerminalBlockList[i].CustomName.Contains(LCD_Name))
                {
                    if (TerminalBlockList[i].BlockDefinition.ToString().Contains("ProgrammableBlock"))
                    {
                        IMyProgrammableBlock block = (IMyProgrammableBlock)TerminalBlockList[i];
                        outputLcdList.Add(block.GetSurface(0));
                    }
                    else
                        outputLcdList.Add((IMyTextSurface)TerminalBlockList[i]);

                    if (TerminalBlockList[i].CustomName.Contains(my_Cockpit_name))
                        my_Cockpit = (IMyCockpit)TerminalBlockList[i];
                }
            }
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.
        }
    }
}