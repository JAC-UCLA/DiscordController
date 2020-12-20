using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DiscordController {
    public class Controller {
        private Config.ControllerConfig Config;
        private string SelectedProfile;
        
        private BlockingCollection<string> InputQueue;
        private Thread InputThread;
        
        public Controller(Config.ControllerConfig config) {
            Config = config;
            if (!Config.Profiles.ContainsKey(Config.DefaultProfile)) {
                throw new Exception("Default profile \"" + Config.DefaultProfile + "\" does not exist.");
            }
            
            SelectedProfile = Config.DefaultProfile;
            
            InputQueue = new BlockingCollection<string>();
            InputThread = new Thread(InputQueueThread);
            InputThread.Start();
            
            Console.WriteLine("Started input queue.");
        }

        public void Dispose() {
            InputQueue.CompleteAdding();
            InputQueue = null;
            while (InputThread.IsAlive) {
                Thread.Sleep(50);
            }
            
            InputThread = null;
            
            Console.WriteLine("Stopped input queue.");
        }
        
        public void SetProfile(string name) {
            foreach (var entry in Config.Profiles) {
                if (entry.Key.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    SelectedProfile = entry.Key;
                    Console.WriteLine("Switched to profile \"" + entry.Key + "\".");
                    return;
                }
            }
            
            Console.WriteLine("Unknown profile \"" + name + "\".");
        }

        public void QueueInput(string input) {
            InputQueue.Add(input);
        }

        private void InputQueueThread() {
            ViGEmClient client = new ViGEmClient();
            IXbox360Controller controller = client.CreateXbox360Controller();
            controller.Connect();

            foreach (var input in InputQueue.GetConsumingEnumerable()) {
                if (InputQueue.Count > Config.MaxInputQueueSize) {
                    Console.WriteLine("Trimming \"" + input + "\" from input queue.");
                }
                
                Console.WriteLine("Processing input \"" + input + "\".");
                ParseInput(out var inputs, out var seconds, input);
                Press(controller, inputs, seconds);
            }

            controller.Disconnect();
            client.Dispose();
        }

        private void ParseInput(out string[] inputs, out double seconds, string inputString) {
            seconds = 0.1;
            if (inputString.Contains(' ')) {
                int index = inputString.IndexOf(' ');
                string secondsStr = inputString.Substring(index + 1);
                try {
                    seconds = double.Parse(secondsStr);
                } catch (FormatException e) {
                    Console.WriteLine("Ignoring invalid number of seconds: \"" + secondsStr + "\".");
                }

                inputString = inputString.Substring(0, index);
            }

            if (inputString.Contains('+')) {
                inputs = inputString.Split('+');
            } else {
                inputs = new[] { inputString };
            }
        }
        
        private void Press(IXbox360Controller controller, string[] inputs, double sec = 0.1) {
            foreach (var input in inputs) {
                SetInput(controller, input, true);
            }

            Thread.Sleep((int) (Math.Clamp(sec, Config.MinInputDuration, Config.MaxInputDuration) * 1000));

            foreach (string input in inputs) {
                SetInput(controller, input, false);
            }
        }

        private void SetInput(IXbox360Controller controller, string input, bool pressed) {
            string button = GetIgnoreCase(Config.Profiles[SelectedProfile].Buttons, input);
            if (button != null) {
                controller.SetButtonState(FindStaticField<Xbox360Button>(button),
                    pressed);
            }

            string slider = GetIgnoreCase(Config.Profiles[SelectedProfile].Sliders, input);
            if (slider != null) {
                controller.SetSliderValue(FindStaticField<Xbox360Slider>(slider),
                    pressed ? byte.MaxValue : (byte) 0);
            }

            Config.Profile.Xbox360Stick stick = GetIgnoreCase(Config.Profiles[SelectedProfile].Sticks, input);
            if (stick != null) {
                controller.SetAxisValue(FindStaticField<Xbox360Axis>(stick.Axis),
                    pressed ? (short) (stick.Direction * short.MaxValue) : (short) 0);
            }
        }

        private T GetIgnoreCase<T>(Dictionary<string, T> dictionary, string key) {
            foreach (var entry in dictionary) {
                if (entry.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) {
                    return entry.Value;
                }
            }

            return default;
        }

        private T FindStaticField<T>(string name) {
            foreach (var field in typeof(T).GetFields()) {
                if (field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    return (T) field.GetValue(null);
                }
            }

            return default;
        }
    }
}